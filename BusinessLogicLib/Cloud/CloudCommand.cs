using VeraWAF.AzureTableStorage;
using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security;
using System.Web;
using System.Web.Hosting;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using VeraWAF.CrossCuttingConcerns;
using VeraWAF.WebPages.Bll;
using VeraWAF.WebPages.Bll.Search;
using VeraWAF.WebPages.Dal;

namespace VeraWAF.WebPages.Bll.Cloud
{
    /*
    * Non-MVC interface to the ASP.NET process
    * We've used a ASP.NET interface as standard MVC does not have access to the ASP.NET process
    */
    public partial class CloudCommand : CloudCommandEx
    {
        /// <summary>
        /// Holds the result from executing a cloud command
        /// </summary>
        public struct CloudCommandResult
        {
            /// <summary>
            /// Result, true if success or false if error
            /// </summary>
            public bool success;

            /// <summary>
            /// Error message if any
            /// </summary>
            public string errorMessage;

            /// <summary>
            /// Command that was executed
            /// </summary>
            public string command;

            /// <summary>
            /// Command arguments
            /// </summary>
            public string [] args;
        }

        /// <summary>
        /// Azure table storage data source
        /// </summary>
        AzureTableStorageDataSource _dataSource;

        string _applicationName;

        public CloudCommand()
        {
            _applicationName = ConfigurationManager.AppSettings["ApplicationName"];
            _dataSource = new AzureTableStorageDataSource();
        }

        /// <summary>
        /// Clears the local web hosting environment cache
        /// </summary>
        void ClearHostingEnvirionmentCache()
        {
            foreach (DictionaryEntry val in HostingEnvironment.Cache)
                HostingEnvironment.Cache.Remove(val.Key.ToString());
        }

        /// <summary>
        /// Touches all virtual files, this causes the virtual path provider to update its
        /// cache.
        /// </summary>
        void TouchVirtualPageCacheDependencyFiles()
        {
            var storagePath = RoleEnvironment.GetLocalResource("VirtualPages").RootPath;
            var virtualCacheDependencyFiles = new DirectoryInfo(storagePath);

            foreach (var file in virtualCacheDependencyFiles.GetFiles())
                file.LastWriteTimeUtc = DateTime.UtcNow;
        }

        /// <summary>
        /// Deletes all the virtual files, this causes the virtual path provider to update its
        /// caches.
        /// </summary>
        void DeleteVirtualPageCacheDependencyFiles()
        {
            var storagePath = RoleEnvironment.GetLocalResource("VirtualPages").RootPath;
            var virtualCacheDependencyFiles = new DirectoryInfo(storagePath);

            foreach (var file in virtualCacheDependencyFiles.GetFiles())
                file.Delete();
        }

        /// <summary>
        /// Reloads the ASP.NET sitemap provider
        /// </summary>
        void ReloadSitemap()
        {
            // Make sure the menus are updated by reloading the sitemap provider
            AzureSiteMapProvider customProvider = SiteMap.Provider as AzureSiteMapProvider;
            customProvider.Refresh();
        }

        /// <summary>
        /// Clears the local page cache
        /// </summary>
        void ClearPageCache()
        {
            new PageCache().Clear();
        }

        /// <summary>
        /// Clears the local forum page cache
        /// </summary>
        void ClearForumPageCache()
        {
            new ForumPageCache().Clear();
        }

        void TouchVirtualForumPageCacheDependencyFiles()
        {
            var storagePath = RoleEnvironment.GetLocalResource("VirtualForumPages").RootPath;
            var virtualCacheDependencyFiles = new DirectoryInfo(storagePath);

            foreach (var file in virtualCacheDependencyFiles.GetFiles())
                file.LastWriteTimeUtc = DateTime.UtcNow;
        }

        /// <summary>
        /// Rebuilds the search index
        /// </summary>
        void RebuildSearchIndex()
        {
            var articles = _dataSource.GetPages(_applicationName);
            new LuceneClient().BuildIndex(articles);
        }

        /// <summary>
        /// Rebuilds the /sitemap.xml file
        /// </summary>
        void RebuildXmlSitemapFile()
        {
            var baseUrl = ConfigurationManager.AppSettings["BaseUrl"];
            new XmlSitemapGenerator().GenerateFile(baseUrl);
        }

        /// <summary>
        /// Executes a cloud command on the local node
        /// </summary>
        /// <param name="command">Command to execute</param>
        /// <param name="args">Command arguments</param>
        /// <returns>Command execution result</returns>
        public CloudCommandResult Execute(string command, string[] args = null)
        {
            // Prepare results
            var result = new CloudCommandResult();
            result.command = command;
            result.args = args;

            try
            {
                switch (command)
                {
                    case "ClearHostingEnvirionmentCache":
                        ClearHostingEnvirionmentCache();
                        break;
                    case "ClearPageCache":
                        // Frees the page cache
                        ClearPageCache();
                        break;
                    case "ClearUserCache":
                        // Frees the user cache
                        new UserCache().Clear2();
                        break;
                    case "ForumPageCRUD":
                        // Called after a forum page CRUD operators
                        TouchVirtualForumPageCacheDependencyFiles();
                        ClearForumPageCache();
                        RebuildSearchIndex();
                        RebuildXmlSitemapFile();
                        break;
                    case "FreeAllCaches":
                        // Free all caches
                        ClearHostingEnvirionmentCache();
                        TouchVirtualPageCacheDependencyFiles();
                        TouchVirtualForumPageCacheDependencyFiles();
                        ClearPageCache();
                        ReloadSitemap();
                        break;
                    case "PageCRUD":
                        // Called after a page CRUD operators
                        TouchVirtualPageCacheDependencyFiles();
                        ClearPageCache();
                        ReloadSitemap();
                        RebuildSearchIndex();
                        RebuildXmlSitemapFile();
                        break;
                    case "Ping":
                        // Do nothing
                        break;
                    case "RebuildSearchIndex":
                        RebuildSearchIndex();
                        break;
                    case "RebuildXmlSitemapFile":
                        RebuildXmlSitemapFile();
                        break;
                    case "RestartServer":
                        // Restart server
                        RoleEnvironment.RequestRecycle();
                        break;
                    case "ReloadSitemap":
                        // Reload the sitemap provider
                        ReloadSitemap();
                        break;
                    case "TouchVirtualPageCacheDependencyFiles":
                        TouchVirtualPageCacheDependencyFiles();
                        break;
                    case "DeleteVirtualPageCacheDependencyFiles":
                        DeleteVirtualPageCacheDependencyFiles();
                        break;
                    case "VoteCRUD":
                        // Called after a vote CRUD operators
                        new VoteCache().Clear();
                        break;
                    default:
                        // Call the extended execution functionality
                        base.Execute(command, args);
                        break;
                }

            }
            catch(Exception ex)
            {
                var errorMessage = String.Format("Role \"{0}\" failed to execute a \"{1}\" command from {2}: {3}",
                        RoleEnvironment.CurrentRoleInstance.Id,
                        command,
                        new ServerTools().GetClientIpAddress(),
                        ex.ToString()
                    );

                // Log error
                new LogEvent().AddEvent(ELogEventTypes.Error, errorMessage, ConfigurationManager.AppSettings["ApplicationName"]);

                // Record error message
                result.errorMessage = errorMessage;
                result.success = false;
                return result;
            }

            // Success
            result.success = true;
            return result;
        }
    }
}