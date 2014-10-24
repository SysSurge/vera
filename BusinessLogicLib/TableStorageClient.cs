using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using System;
using System.Configuration;
using System.Data.Services.Client;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Security;
using VeraWAF.AzureTableStorage;
using VeraWAF.CrossCuttingConcerns;
using VeraWAF.WebPages.Bll;
using VeraWAF.WebPages.Bll.Cloud;
using VeraWAF.WebPages.Bll.Search;
using VeraWAF.WebPages.Dal;
using VeraWAF.WebPages.Dal.Interchange;

namespace VeraWAF.WebPages.Bll
{
    /// <summary>
    /// Azure table storage client logic.
    /// Contains some common functionality for accessing the Azure table storage.
    /// </summary>
    public class TableStorageClient : TableStorageClientEx
    {
        /// <summary>
        /// Updates the virtual file cache dependency file that relates to the current page entity.
        /// This causes the page cache to be flushed
        /// </summary>
        /// <param name="page">Page entity data</param>
        void UpdateVirtualFileCacheDependency(PageEntity page)
        {
            var storagePath = RoleEnvironment.GetLocalResource("VirtualPages").RootPath;
            var fileName = storagePath + page.PartitionKey;

            if (!File.Exists(fileName))
            {
                var file = new StreamWriter(fileName);
                file.Close();
            }
            else File.SetLastWriteTimeUtc(fileName, DateTime.UtcNow);
        }

        /// <summary>
        /// Updates the search index with the changes
        /// </summary>
        /// <param name="page">Page entity data</param>
        void UpdateSearchIndex(PageEntity page)
        {
            new LuceneClient().UpdateIndex(page);
        }

        /// <summary>
        /// Updates the XML sitemap file
        /// </summary>
        void UpdateXmlSitemapFile()
        {
            var baseUrl = ConfigurationManager.AppSettings["BaseUrl"];
            new XmlSitemapGenerator().GenerateFile(baseUrl);
        }

        /// <summary>
        /// Refreshes the sitemap
        /// </summary>
        void ReloadSitemap()
        {
            // Make sure the menus are updated by reloading the sitemap provider
            AzureSiteMapProvider customProvider = SiteMap.Provider as AzureSiteMapProvider;
            customProvider.Refresh();
        }

        /// <summary>
        /// Flush cloud instance page caches
        /// </summary>
        /// <param name="page">Page entity</param>
        void FlushPageCaches(PageEntity page)
        {
            new PageCache().Clear();
            UpdateVirtualFileCacheDependency(page);

            // Make sure that all the other cloud instances are updated to reflect the page changes
            new CloudCommandClient().SendCommand("PageCRUD", true);
        }

        /// <summary>
        /// Create a page row key from a date
        /// </summary>
        /// <param name="createdDate">Page creation date</param>
        /// <returns></returns>
        public string GetPageRowKey(DateTime createdDate)
        {
            return createdDate.Ticks.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Checks if a page entity exists
        /// </summary>
        /// <param name="datasource">Azure table storage data source</param>
        /// <param name="partitionKey">Page entity partition key</param>
        /// <param name="rowKey">Page entity row key. Can be omitted.</param>
        /// <param name="applicationName">Application name</param>
        /// <returns>True if the page exists</returns>
        public bool PageExists(AzureTableStorageDataSource datasource, string partitionKey, string rowKey, 
            string applicationName)
        {
            return String.IsNullOrWhiteSpace(rowKey) ?
                datasource.PageExists(partitionKey, applicationName) :
                datasource.PageExists(partitionKey, rowKey, applicationName);
        }

        /// <summary>
        /// Gets a page
        /// </summary>
        /// <param name="partitionKey">Page entity partition key</param>
        /// <param name="rowKey">Page entity row key. Can be omitted.</param>
        /// <param name="applicationName">Application name</param>
        /// <returns>Page entity data or null of not found</returns>
        public PageEntity GetPage(string partitionKey, string rowKey,
            string applicationName)
        {
            var datasource = new AzureTableStorageDataSource();
            return String.IsNullOrWhiteSpace(rowKey) ?
                datasource.GetPage(partitionKey, applicationName) :
                datasource.GetPage(partitionKey, rowKey, applicationName);
        }
        
        /// <summary>
        /// Get a page entity
        /// </summary>
        /// <param name="virtualPath">Virtual path to page entity</param>
        /// <param name="rowKey">Row key to entity</param>
        /// <returns>Page entity or null if not found</returns>
        public PageEntity GetPageByVirtualPath(string virtualPath, string rowKey, string applicationName)
        {
            var datasource = new AzureTableStorageDataSource();
            var partitionKey = new StringUtilities().ConvertToHex(virtualPath);

            return String.IsNullOrWhiteSpace(rowKey) ?
                datasource.GetPage(partitionKey, applicationName) :
                datasource.GetPage(partitionKey, rowKey, applicationName);
        }           

        /// <summary>
        /// Saves a page 
        /// </summary>
        /// <param name="page">Page entity data</param>
        /// <param name="overwrite">If true then the content page will be overwritten if it already exists</param>
        /// <param name="applicationName">Application name</param>
        /// <param name="partitionKey">Partition key</param>
        /// <returns>Page entity data</returns>
        public PageEntity SavePage(PageEntity page, bool overwrite, string partitionKey, string applicationName)
        {
            var datasource = new AzureTableStorageDataSource();

            // Check if the page exists already
            bool pageExists;
            try
            {
                pageExists = PageExists(datasource, partitionKey, null, applicationName);
            }
            catch (DataServiceQueryException)
            {
                pageExists = false;
            }

            if (!overwrite && pageExists)
                throw new ApplicationException("Content page already exists");

            // Delete the old page if it existed
            if (pageExists)
            {
                var oldPage = GetPage(partitionKey, null, applicationName);
                datasource.Delete(oldPage);
            }

            datasource.Insert(page);

            ReloadSitemap();

            return page;
        }

        /// <summary>
        /// Updates a content page
        /// </summary>
        /// <param name="page">Page entity data</param>
        /// <param name="overwrite">If true then the content page will be overwritten if it already exists</param>
        /// <param name="partitionKey">Page partition key</param>
        /// <param name="applicationName">Application name</param>
        public PageEntity UpdatePage(PageEntity page, bool overwrite, string partitionKey, string applicationName)
        {
            SavePage(page, overwrite, partitionKey, applicationName);

            // Flush the cloud node caches to reflect the changes
            FlushPageCaches(page);

            // Update the search index
            UpdateSearchIndex(page);

            // Update the sitemap.xml file
            UpdateXmlSitemapFile();

            return page;
        }

        /// <summary>
        /// Updates a property value in a Azure table storage table
        /// </summary>
        /// <param name="fieldData">Azure table field data</param>
        public void UpdateGeneric<T>(Type entityType, TablePropertyInfo fieldData, string applicationName)
        {
            var datasource = new AzureTableStorageDataSource();

            // Get the existing data entity
            T entity = datasource.GetAny<T>(fieldData.TableName, fieldData.PartitionKey, applicationName);

            // Was a Azure table entity with a matching partition key found?
            if (entity == null)
                throw new ArgumentException(
                        String.Format("Azure table \"{0}\"'s entity for \"{1}\" with partition key \"{2}\" was not found",
                            fieldData.TableName, applicationName, fieldData.PartitionKey)
                    );

            // Set the property value
            var prop = entityType.GetProperty(fieldData.PropertyName);
            object propertyValue = Convert.ChangeType(fieldData.PropertyValue, prop.PropertyType);
            prop.SetValue(entity, propertyValue, null);

            datasource.Update<T>(entity, fieldData.TableName);
        }

        /// <summary>
        /// Deletes a page
        /// </summary>
        /// <param name="partitionKey">Page partition key</param>
        /// <param name="applicationName">Application name</param>
        public void DeletePage(string partitionKey, string applicationName)
        {
            var datasource = new AzureTableStorageDataSource();

            while (PageExists(datasource, partitionKey, null, applicationName))
            {
                var page = GetPage(partitionKey, null, applicationName);
                datasource.Delete(page);
                new LuceneClient().DeleteFromIndex(page);
            }
        }

    }
}
