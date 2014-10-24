using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using VeraWAF.AzureTableStorage;
using Microsoft.WindowsAzure.ServiceRuntime;
using VeraWAF.Core.Templates;
using VeraWAF.WebPages.Bll;
using VeraWAF.WebPages.Bll.Search;

namespace VeraWAF.WebPages.Edit
{
    public partial class Default : PageTemplateBase
    {
        AzureTableStorageDataSource _dataSource;
        string _applicationName;

        protected void Page_Load(object sender, EventArgs e)
        {
            _dataSource = new AzureTableStorageDataSource();
            _applicationName = ConfigurationManager.AppSettings["ApplicationName"];
        }

        void ClearHostingEnvirionmentCache() {
            foreach (DictionaryEntry val in HostingEnvironment.Cache)
                HostingEnvironment.Cache.Remove(val.Key.ToString());
        }

        void TouchVirtualPageCacheDependencyFiles()
        {
            var storagePath = RoleEnvironment.GetLocalResource("VirtualPages").RootPath;
            var virtualCacheDependencyFiles = new DirectoryInfo(storagePath);

            foreach (var file in virtualCacheDependencyFiles.GetFiles())
                file.LastWriteTimeUtc = DateTime.UtcNow;
        }

        protected void lnkClearPageCache_Click(object sender, EventArgs e)
        {
            ClearHostingEnvirionmentCache();
            TouchVirtualPageCacheDependencyFiles();
        }

        protected void lnkRebuildSearchIndex_Click(object sender, EventArgs e)
        {
            var articles = _dataSource.GetPages(_applicationName);
            new LuceneClient().BuildIndex(articles);
        }

        protected void lnkRestart_Click(object sender, EventArgs e)
        {
            RoleEnvironment.RequestRecycle();
        }

        protected void lnkRebuildXmlSitemap_Click(object sender, EventArgs e)
        {
            var baseUrl = ConfigurationManager.AppSettings["BaseUrl"];
            new XmlSitemapGenerator().GenerateFile(baseUrl);
        }
    }
}