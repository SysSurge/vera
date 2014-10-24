using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.IO;
using System.Linq;
using System.Web.Caching;
using System.Web.Hosting;
using VeraWAF.AzureTableStorage;
using Microsoft.WindowsAzure.ServiceRuntime;
using VeraWAF.WebPages.Dal;

namespace VeraWAF.WebPages.Bll.VirtualPathProvider
{
    /// <summary>
    /// Custom ASP.NET virtual path provider.
    /// Uses the Azure table storage for persistence
    /// </summary>
    public class CustomVirtualPathProvider : System.Web.Hosting.VirtualPathProvider {

        /// <summary>
        ///   Data set provider for the CustomVirtualDirectory and
        ///   CustomVirtualFile classes. In a production application
        ///   this method would be on a provider class that accesses
        ///   the virtual resource data source.
        /// </summary>
        /// <returns>
        ///   The System.Data.DataSet containing the virtual resources 
        ///   provided by the SamplePathProvider.
        /// </returns>
        public IEnumerable<PageEntity> GetVirtualData()
        {
            return new PageCache().GetAllPages().Where(page => page.IsPublished && String.IsNullOrWhiteSpace(page.ParentRowKey));
        }

        /// <summary>
        ///   Determines whether a specified virtual path is within
        ///   the virtual file system.
        /// </summary>
        /// <param name="virtualPath">An absolute virtual path.</param>
        /// <returns>
        ///   true if the virtual path is within the 
        ///   virtual file system; otherwise, false.
        /// </returns>
        bool IsPathVirtual(string virtualPath)
        {
            try
            {
                var virtualFiles = GetVirtualData();
                var pathExists = virtualFiles.Any(virtualFile => virtualFile.VirtualPath.StartsWith(virtualPath));
                return pathExists;
            }
            catch (DataServiceQueryException)
            {
                return false;
            }
        }

        public override VirtualFile GetFile(string virtualPath) {
            return IsPathVirtual(virtualPath) ? new CustomVirtualFile(virtualPath, this) : Previous.GetFile(virtualPath);
        }

        public override bool FileExists(string virtualPath)
        {
            if (IsPathVirtual(virtualPath)) {
                var file = (CustomVirtualFile)GetFile(virtualPath);
                return file.Exists;
            }
            return Previous.FileExists(virtualPath);
        }

        public override bool DirectoryExists(string virtualDir)
        {
            if (IsPathVirtual(virtualDir)) {
                var dir = (CustomVirtualDirectory)GetDirectory(virtualDir);
                return dir.Exists;
            }
            return Previous.DirectoryExists(virtualDir);
        }

        public override VirtualDirectory GetDirectory(string virtualDir)
        {
            return IsPathVirtual(virtualDir) ? new CustomVirtualDirectory(virtualDir, this) : Previous.GetDirectory(virtualDir);
        }

        CacheDependency GetVirtualCacheDependency(string virtualPath, DateTime utcStart)
        {
            string fileName;

            var page = GetVirtualData().FirstOrDefault(page1 => page1.VirtualPath == virtualPath);
            if (page != null)
            {
                var storagePath = RoleEnvironment.GetLocalResource("VirtualPages").RootPath;
                fileName = storagePath + page.PartitionKey;
                
                if (!File.Exists(fileName))
                {
                    var file = new StreamWriter(fileName);
                    file.Close();
                }
            }
            else return null;

            return new CacheDependency(fileName, utcStart);            
        }

        public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            return IsPathVirtual(virtualPath) 
                ? GetVirtualCacheDependency(virtualPath, utcStart) 
                : Previous.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
        }
    }
}