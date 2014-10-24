using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Configuration;
using System.Data.Services.Client;
using System.Globalization;
using System.IO;
using System.Web.Security;
using System.Web.UI;
using VeraWAF.AzureTableStorage;
using VeraWAF.Core.Templates;
using VeraWAF.CrossCuttingConcerns;
using VeraWAF.WebPages.Bll;
using VeraWAF.WebPages.Bll.Cloud;
using VeraWAF.WebPages.Bll.Search;
using VeraWAF.WebPages.Bll.Security;
using VeraWAF.WebPages.Dal;

namespace VeraWAF.WebPages.Templates
{
    /// <summary>
    /// Allows theuser to create or edit a forum page
    /// </summary>
    public partial class EditForumPage : PageTemplateBase {

        /// <summary>
        /// Application name
        /// </summary>
        readonly string _applicationName;

        /// <summary>
        /// Class contructor
        /// </summary>
        public EditForumPage() {
            _applicationName = ConfigurationManager.AppSettings["ApplicationName"];
        }

        /// <summary>
        /// Fill the controls with page entity data
        /// </summary>
        /// <param name="page"></param>
        void PopulateFormFieldsFromPageEntityData(PageEntity page)
        {
            txtTitle.Text = page.Title;
            txtMainContent.Text = page.MainContent;
            chkPublish.Checked = page.IsPublished;
        }

        /// <summary>
        /// Get the forum page for a specific virtual path
        /// </summary>
        /// <param name="virtualPath">Virtual path</param>
        /// <returns>The page entity of null if not found</returns>
        PageEntity GetPage(string virtualPath) {
            var datasource = new AzureTableStorageDataSource();
            var partitionKey = new StringUtilities().ConvertToHex(virtualPath);
            return datasource.GetPage(partitionKey, _applicationName);
        }

        /// <summary>
        /// Get the parent URL level, i.e. if this path is http://example.com/a/b.aspx the returned URL is http://example.com/a/
        /// </summary>
        /// <returns>Parent url level</returns>
        string GetParentUrl()
        {
            return Request.Url.AbsolutePath.Substring(0, Request.Url.AbsolutePath.LastIndexOf('/'));
        }

        /// <summary>
        /// Get the current virtual path of the one specified in the "path" request parameter
        /// </summary>
        /// <returns>The virtual path</returns>
        string GetVirtualPath() {
            string virtualPath;

            if (!String.IsNullOrEmpty(Request["path"]))
                virtualPath = Request["path"];
            else if (!String.IsNullOrWhiteSpace(txtVirtualPath.Text))
                virtualPath = txtVirtualPath.Text;
            else virtualPath = String.Format("{0}/{1}.aspx", GetParentUrl(), DateTime.Now.Ticks);

            return virtualPath;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Membership.GetUser() == null) {
                FormsAuthentication.RedirectToLoginPage();
                Response.End();
                return;
            }

            var virtualPath = GetVirtualPath();

            var page = GetPage(virtualPath);

            if (page != null && !Page.IsPostBack && !String.IsNullOrEmpty(virtualPath)) 
                PopulateFormFieldsFromPageEntityData(page);
        }

        /// <summary>
        /// Create a new row key for the page entity data. Uses the date ticks as the basis.
        /// This ensures that the combination of the primary key and the rowkey in the VeraPages database
        /// table is unique.
        /// </summary>
        /// <param name="createdDate">Date to base the rowkey on</param>
        /// <returns>New rowkey</returns>
        string GetRowKey(DateTime createdDate) {
            return createdDate.Ticks.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Creates a new page entity for the forum page based on the data from the form's input controls.
        /// </summary>
        /// <param name="createdDate">Page creation date, essentially now</param>
        /// <param name="virtualPath">Page virtual path</param>
        /// <returns>The new page entity data for the new forum page</returns>
        PageEntity CreatePageEntityFromFormData(DateTime createdDate, string virtualPath) {
            return new PageEntity {
                PartitionKey = new StringUtilities().ConvertToHex(virtualPath),
                RowKey = GetRowKey(createdDate),
                ApplicationName = _applicationName,
                Template = ConfigurationManager.AppSettings["DefaultForumPostTemplate"],
                Title = txtTitle.Text,
                MainContent = txtMainContent.Text,
                VirtualPath = virtualPath,
                IsPublished = chkPublish.Checked,
                MenuItemSortOrder = 0,
                GeolocationLat = 0.0,
                GeolocationLong = 0.0,
                GeolocationZoom = 0,
                Author = Membership.GetUser().UserName,
                ShowInMenu = false,
                ShowContactControl = false,
                AllowForComments = true,
                Index = true,
                RollupImage = ConfigurationManager.AppSettings["DefaultForumPostRollupImage"]
            };
        }

        /// <summary>
        /// Refreshes the local sitemap to reflect the changes
        /// </summary>
        /// <param name="page">Page entity</param>
        void UpdateVirtualFileCacheDependency(PageEntity page) {
            var storagePath = RoleEnvironment.GetLocalResource("VirtualForumPages").RootPath;
            var fileName = storagePath + page.PartitionKey;

            if (!File.Exists(fileName)) {
                var file = new StreamWriter(fileName);
                file.Close();
            } else File.SetLastWriteTimeUtc(fileName, DateTime.UtcNow);
        }

        /// <summary>
        /// Updates the local search index to reflect the changes
        /// </summary>
        /// <param name="page">Page entity</param>
        void UpdateSearchIndex(PageEntity page) {
            new LuceneClient().UpdateIndex(page);
        }

        /// <summary>
        /// Updates the local XML sitemap file to reflect the changes
        /// </summary>
        void UpdateXmlSitemapFile() {
            var baseUrl = ConfigurationManager.AppSettings["BaseUrl"];
            new XmlSitemapGenerator().GenerateFile(baseUrl);
        }

        /// <summary>
        /// Returns true if the page already exists
        /// </summary>
        /// <param name="datasource">Azure table storage data source</param>
        /// <param name="partitionKey">Partition key</param>
        /// <returns></returns>
        bool PageExists(AzureTableStorageDataSource datasource, string partitionKey) {
            return datasource.PageExists(partitionKey, _applicationName);
        }

        /// <summary>
        /// Get a page
        /// </summary>
        /// <param name="datasource">Azure table storage data source</param>
        /// <param name="partitionKey">Partition key</param>
        /// <returns></returns>
        PageEntity GetPage(AzureTableStorageDataSource datasource, string partitionKey) {
            return datasource.GetPage(partitionKey, _applicationName);
        }

        /// <summary>
        /// Is this a valid forum directory?
        /// </summary>
        /// <param name="page">Page entity</param>
        /// <returns>True of this is a valid forum directory</returns>
        bool IsValidForumDirectory(PageEntity page) {
            return page.VirtualPath.StartsWith(GetParentUrl());
        }

        /// <summary>
        /// Saves the forum page
        /// </summary>
        /// <param name="virtualPath">Virtual path to page to save</param>
        /// <returns></returns>
        PageEntity SavePage(string virtualPath) {
            var datasource = new AzureTableStorageDataSource();
            var partitionKey = new StringUtilities().ConvertToHex(virtualPath);

            PageEntity page;

            try {
                page = GetPage(datasource, partitionKey);
            }
            catch (System.Data.Services.Client.DataServiceQueryException)
            {
                page = null;
            }

            if (page == null)
            {
                // New article
                var createdDate = DateTime.UtcNow;
                page = CreatePageEntityFromFormData(createdDate, virtualPath);
                datasource.Insert(page);
            }
            else
            {
                // Already existing article
                if (!new AccessControlManager().UserHasEditPermissions(page) || !IsValidForumDirectory(page))
                {
                    FormsAuthentication.RedirectToLoginPage();
                    Response.End();
                    return null;
                }

                page.Title = txtTitle.Text;
                page.MainContent = txtMainContent.Text;

                datasource.Update(page);
            }

            return page;
        }

        /// <summary>
        /// Called when the user clicks the save button on the form. Saves the forum page to the database and 
        /// updates the cloud solution with the changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void butSave_Click(object sender, EventArgs e)
        {
            var virtualPath = GetVirtualPath();

            var page = SavePage(virtualPath);

            new ForumPageCache().Clear();

            UpdateVirtualFileCacheDependency(page);

            UpdateSearchIndex(page);

            UpdateXmlSitemapFile();

            // Make sure that all the other cloud instances are updated to reflect the page changes
            new CloudCommandClient().SendCommand("ForumPageCRUD", true);

            Response.Redirect(virtualPath);
        }

        /// <summary>
        /// Clears the form
        /// </summary>
        void ClearForm() {
            Response.Redirect(Request.RawUrl, true);
        }

        /// <summary>
        /// Called when the user clicks the clear button on the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void butClear_Click(object sender, EventArgs e) {
            ClearForm();
        }

        /// <summary>
        /// Deletes a page.
        /// </summary>
        /// <param name="virtualPath">Virtual path to page to delete</param>
        private void DeletePage(string virtualPath) {
            var datasource = new AzureTableStorageDataSource();
            var partitionKey = new StringUtilities().ConvertToHex(virtualPath);

            while (PageExists(datasource, partitionKey)) {
                var page = GetPage(datasource, partitionKey);

                if (!new AccessControlManager().UserHasEditPermissions(page) || !IsValidForumDirectory(page))
                {
                    FormsAuthentication.RedirectToLoginPage();
                    Response.End();
                    return;
                }

                datasource.Delete(page);

                new ForumPageCache().Clear();

                UpdateVirtualFileCacheDependency(page);

                new LuceneClient().DeleteFromIndex(page);

                UpdateXmlSitemapFile();

                // Make sure that all the other cloud instances are updated to reflect the page changes
                new CloudCommandClient().SendCommand("ForumPageCRUD", true);
            }
        }

        /// <summary>
        /// Called when the user clicks the delete button on the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void butDelete_Click(object sender, EventArgs e) {
            DeletePage(GetVirtualPath());
            ClearForm();
        }

    }
}