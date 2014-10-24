using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Services.Client;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using VeraWAF.AzureTableStorage;
using VeraWAF.Core.Templates;
using VeraWAF.WebPages.Bll;
using VeraWAF.WebPages.Bll.Cloud;

namespace VeraWAF.WebPages.Edit.Files {
    public partial class ViewFiles : PageTemplateBase
    {
        const int PageSize = 25;
        int _totalNumberOfFiles;
        int _totalPages;
        int _currentPage = 1;

        readonly AzureTableStorageDataSource _datasource;
        readonly string _applicationName;


        public ViewFiles()
        {
            _datasource = new AzureTableStorageDataSource();
            _applicationName = ConfigurationManager.AppSettings["ApplicationName"];
        }

        void GetFiles()
        {
            List<FileEntity> allFiles;

            try
            {
                allFiles = new List<FileEntity>(_datasource.GetFiles(_applicationName));
            }
            catch (DataServiceQueryException)
            {
                // Table is empty
                return;
            }

            _totalNumberOfFiles = allFiles.Count;
            _totalPages = ((_totalNumberOfFiles - 1) / PageSize) + 1;

            // Ensure that we do not navigate past the last page of users.
            if (_currentPage > _totalPages) {
                _currentPage = _totalPages;
                GetFiles();
                return;
            }

            FilesGrid.DataSource = allFiles.Skip((_currentPage - 1) * PageSize).Take(PageSize);
            FilesGrid.CurrentPageIndex = _currentPage;
            FilesGrid.DataBind();
            CurrentPageLabel.Text = _currentPage.ToString(CultureInfo.InvariantCulture);
            TotalPagesLabel.Text = _totalPages.ToString(CultureInfo.InvariantCulture);

            NextButton.Visible = _currentPage != _totalPages;

            PreviousButton.Visible = _currentPage != 1;

            NavigationPanel.Visible = _totalNumberOfFiles > 0;
        }

        public void NextButton_OnClick(object sender, EventArgs args) {
            _currentPage = Convert.ToInt32(CurrentPageLabel.Text);
            _currentPage++;
            GetFiles();
        }

        public void PreviousButton_OnClick(object sender, EventArgs args) {
            _currentPage = Convert.ToInt32(CurrentPageLabel.Text);
            _currentPage--;
            GetFiles();
        }

        void ShowBytesUsed()
        {
            var totalBytesUsed = new FileManager().GetStorageUsageByteSize() / (1024 * 1024);
            litByteCount.Text = totalBytesUsed.ToString("#,#", CultureInfo.CreateSpecificCulture(
                ConfigurationManager.AppSettings["SiteCulture"]
                ));
        }

        protected void Page_Load(object sender, EventArgs e) {
            if (IsPostBack) return;

            GetFiles();

            ShowBytesUsed();
        }

        protected void FilesGrid_Edit(object source, DataGridCommandEventArgs e)
        {
            // Instead of editing the item in the datagrid we redirect to custom edit page
            var partitionKey = ((DataGrid)source).Items[e.Item.ItemIndex].Cells[0].Text;
            Response.Redirect("/CMS/Files/EditFile.aspx?file=" + HttpUtility.UrlEncode(partitionKey));
        }

        public string Docals(string url)
        {
            var cdnUrl = new CdnUtilities().GetCdnUrl(url);
            return String.Format("<a href='{0}' target='_blank'>{0}</a>", cdnUrl);
        }
    }
}