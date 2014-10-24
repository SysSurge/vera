#if !DEBUG
using System;
using VeraWAF.WebPages.Bll;
using System.Web.Caching;
#endif
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Hosting;
using VeraWAF.AzureTableStorage;
using VeraWAF.CrossCuttingConcerns;

namespace VeraWAF.WebPages.Dal {
    public class ForumPageCache {
        public const string CacheKey = "VPFPData";

#if !DEBUG
        void AddVirtualFilesToCache(IEnumerable<PageEntity> virtualFiles)
        {
            var cacheSlidingExpiration = new RuntimeConfiguration().GetVirtualFileCacheSlidingExpiration();

            HostingEnvironment.Cache.Add(CacheKey, virtualFiles, null,
              Cache.NoAbsoluteExpiration,
              cacheSlidingExpiration,
              CacheItemPriority.Default, null);
        }
#endif
        IEnumerable<PageEntity> GetVirtualFilesFromCache() {
            return (IEnumerable<PageEntity>)HostingEnvironment.Cache.Get(CacheKey);
        }

        IEnumerable<PageEntity> GetAllVirtualFiles() {
            var datasource = new AzureTableStorageDataSource();
            var applicationName = ConfigurationManager.AppSettings["ApplicationName"];
            return datasource.GetPages(applicationName).Where(page => 
                page.Template == "ForumSection.aspx" || page.Template == "ForumPage.aspx");
        }

        public IEnumerable<PageEntity> GetAllPages() {
            var virtualFiles = GetVirtualFilesFromCache();
            if (virtualFiles == null) {
                virtualFiles = GetAllVirtualFiles();
#if !DEBUG
                AddVirtualFilesToCache(virtualFiles);
#endif
            }
            return virtualFiles;
        }

        public PageEntity GetPageByPartitionKey(string partitionKey) {
            return GetAllPages().FirstOrDefault(page => page.PartitionKey == partitionKey
                && String.IsNullOrWhiteSpace(page.ParentRowKey));
        }

        public PageEntity GetPageByVirtualPath(string virtualPath)
        {
            return GetAllPages().FirstOrDefault(page => page.VirtualPath == virtualPath
                && String.IsNullOrWhiteSpace(page.ParentRowKey));            
        }

        public IEnumerable<PageEntity> GetPagesByVirtualPath(string virtualPath) {
            return GetAllPages().Where(page => page.VirtualPath.StartsWith(virtualPath)
                && String.IsNullOrWhiteSpace(page.ParentRowKey));
        }

        public IEnumerable<PageEntity> GetPagesByAuthor(string userName) {
            return GetAllPages().Where(page => page.Author == userName);
        }

        public IEnumerable<PageEntity> GetCommentsByVirtualPath(string virtualPath) {
            return GetAllPages().Where(page => page.PartitionKey == new StringUtilities().ConvertToHex(virtualPath)
                && !String.IsNullOrWhiteSpace(page.ParentRowKey)
                && page.IsPublished);
        }

        public void Clear()
        {
            HostingEnvironment.Cache.Remove(CacheKey);         
        }

    }
}