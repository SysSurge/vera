using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Web.UI;
using VeraWAF.AzureTableStorage;
using VeraWAF.WebPages.Bll;

namespace VeraWAF.WebPages {

    /// <summary>
    /// This page is only accessible if VeraWAF has not been provisioned across the cloud.
    /// The class will create the initial VeraWAF web application solution. After provisioning
    /// has completed this page will be overridden by the VeraWAF virtual path provider and thus
    /// hidden from future access.
    /// </summary>
    public partial class Default : Page 
    {
        const string _currentPage = "Default.aspx";

        /// <summary>
        /// Touch the file to update the ASP.NET page cache dependency
        /// </summary>
        void UpdateFileCacheDependency()
        {
            var fileName = Server.MapPath(_currentPage);
            System.IO.File.SetLastWriteTimeUtc(fileName, DateTime.UtcNow);        
        }

        protected void Page_Load(object sender, EventArgs e) {

            var datasource = new AzureTableStorageDataSource();

            // Touch the physical file to update the ASP.NET page cache dependency
            UpdateFileCacheDependency();

            if (!datasource.PageEntityTableExists())
            {
                // Install database tables and content
                var initApp = new InitApplication();
                initApp.InstallAll();

                // Reload the page
                Response.Redirect(_currentPage, true);
            }
        }

    }
}