using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;
using VeraWAF.AzureTableStorage;
using VeraWAF.CrossCuttingConcerns;
using VeraWAF.WebPages.Dal;

namespace VeraWAF.WebPages.Bll.VirtualPathProvider
{
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class CustomVirtualFile : VirtualFile {
        /// <summary>
        /// Page
        /// </summary>
        PageEntity _content;

        /// <summary>
        /// Vera ASP.NET virtual path provider
        /// </summary>
        readonly CustomVirtualPathProvider spp;

        public bool Exists { get; private set; }

        public CustomVirtualFile(string virtualPath, CustomVirtualPathProvider provider) : base(virtualPath) {
            spp = provider;
            GetData();
        }

        protected void GetData() {
            var virtualFiles = spp.GetVirtualData();

            _content = virtualFiles.Where(virtualFile => virtualFile.VirtualPath == VirtualPath).FirstOrDefault();

            Exists = _content != null;
        }

        void StorePageTemplateInCache(string cacheKey, string templateFile, string pageTemplate)
        {
            // Make pageTemplate dependent on the template file.
            var cd = new CacheDependency(templateFile);

            /*
             * Since the pages are virtual we must make sure that if their templates are changed their cache is 
             * flushed, this is acchived by adding a cache dependency and then touching a virtual file on disk
             * to trigger the flush when the virtual page's template has been altered.
             */
            HostingEnvironment.Cache.Insert(
                  cacheKey,
                  pageTemplate,
                  cd,
                  DateTime.MaxValue,
                  TimeSpan.Zero,
                  CacheItemPriority.High,
                  delegate(string key, object value, CacheItemRemovedReason reason)
                  {
                      var storagePath = RoleEnvironment.GetLocalResource("VirtualPages").RootPath;
                      var fileName = storagePath + key;

                      if (!File.Exists(fileName))
                      {
                          var file = new StreamWriter(fileName);
                          file.Close();
                      }
                      else File.SetLastWriteTimeUtc(fileName, DateTime.UtcNow);
                  }
             );
        }

        /// <summary>
        /// Read the page template file
        /// </summary>
        /// <returns>Stream with the page template</returns>
        public Stream OpenTemplate()
        {
            // Try to get the page template from the cache
            var pageTemplate = HostingEnvironment.Cache.Get(_content.PartitionKey) as string;

            if (pageTemplate == null)
            {
                // Page template was not in cache, so load it from disk
                var templateFile = String.Format("{0}Templates\\{1}", HostingEnvironment.ApplicationPhysicalPath, _content.Template);

                try
                {
                    // Get the page template.
                    using (var reader = new StreamReader(templateFile))
                        pageTemplate = reader.ReadToEnd();
                }
                catch(Exception ex)
                {
                    // Log the missing template file
                    new LogEvent().AddEvent(ELogEventTypes.Error,
                        String.Format("Could not open the \"{0}\" template file for \"{1}\". {2}", templateFile, _content.VirtualPath, ex.Message),
                        ConfigurationManager.AppSettings["ApplicationName"]);
                    throw;
                }

                StorePageTemplateInCache(_content.PartitionKey, templateFile, pageTemplate);
            }

            Stream stream = new MemoryStream();
            var writer = new StreamWriter(stream);

            writer.Write(pageTemplate);
            writer.Flush();
            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }

        /// <summary>
        /// Read the virtual file
        /// </summary>
        /// <returns></returns>
        public override Stream Open()
        {
            // Determine the content typ and act accordingly
            var suffixIdx = _content.VirtualPath.IndexOf('.');

            // Default to text file if not found
            var suffix = suffixIdx == -1 ? "txt" : _content.VirtualPath.Substring(suffixIdx+1);

            try
            {
                // Is this a file that requires a template?
                if (!String.IsNullOrEmpty(_content.Template))
                    return OpenTemplate();

                // Serve the file directly
                Stream stream = new MemoryStream();
                var writer = new StreamWriter(stream);

                writer.Write(_content.MainContent);
                writer.Flush();
                stream.Seek(0, SeekOrigin.Begin);

                return stream;
            }
            catch(Exception ex)
            {
                // Log the bad read file attempt
                new LogEvent().AddEvent(ELogEventTypes.Error,
                    String.Format("Could not read from the \"{0}\" file. {1}", _content.VirtualPath, ex.Message),
                    ConfigurationManager.AppSettings["ApplicationName"]);
                throw;
            }
        }
    }
}