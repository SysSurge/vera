using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using VeraWAF.AzureTableStorage;
using VeraWAF.Core.Templates;

namespace VeraWAF.WebPages.Edit {
    public partial class ViewPages : PageTemplateBase
    {
        const int PageSize = 25;
        int _totalNumberOfContentPages;
        int _totalPages;
        int _currentPage = 1;

        readonly AzureTableStorageDataSource _datasource;
        readonly string _applicationName;


        public ViewPages()
        {
            _datasource = new AzureTableStorageDataSource();
            _applicationName = ConfigurationManager.AppSettings["ApplicationName"];
        }

        /// <summary>
        /// Load pages from the VeraPages table
        /// </summary>
        /// <param name="filter"></param>
        void GetContentPages(string filter)
        {
            var pageCache = new Bll.PageCache();
            _totalNumberOfContentPages = pageCache.GetAllPages().Count();

            if (String.IsNullOrWhiteSpace(filter))
            {
                ContentPagesGrid.DataSource = pageCache.GetAllPages().Skip((_currentPage - 1) * PageSize).Take(PageSize);
            }
            else
            {
                // Add a filter to the Linq query
                ContentPagesGrid.DataSource = pageCache.GetAllPages().Where(cp =>
                    (cp.Author != null && cp.Author.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
                    || (cp.Ingress != null && cp.Ingress.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
                    || (cp.MenuItemName != null && cp.MenuItemName.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
                    || (cp.MenuItemDescription != null && cp.MenuItemDescription.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
                    || (cp.PartitionKey != null && cp.PartitionKey.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
                    || (cp.Title != null && cp.Title.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
                    || (cp.VirtualPath != null && cp.VirtualPath.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
                    ).Skip((_currentPage - 1) * PageSize).Take(PageSize);
            }

            //NumberOfUsers.Text = _totalNumberOfContentPages.ToString();

            _totalPages = ((_totalNumberOfContentPages - 1) / PageSize) + 1;

            // Ensure that we do not navigate past the last page of users.
            if (_currentPage > _totalPages)
            {
                _currentPage = _totalPages;
                GetContentPages(filter);
                return;
            }

            ContentPagesGrid.DataBind();
            CurrentPageLabel.Text = _currentPage.ToString();
            TotalPagesLabel.Text = _totalPages.ToString();

            NextButton.Visible = _currentPage != _totalPages;

            PreviousButton.Visible = _currentPage != 1;

            NavigationPanel.Visible = _totalNumberOfContentPages > 0;
        }

        //void GetContentPagesOld(string filter)
        //{
        //    var allContentPages = new List<PageEntity>(
        //        _datasource.GetPages(_applicationName).Where(page => String.IsNullOrWhiteSpace(page.ParentRowKey) ));

        //    _totalNumberOfContentPages = allContentPages.Count;
        //    _totalPages = ((_totalNumberOfContentPages - 1) / PageSize) + 1;

        //    // Ensure that we do not navigate past the last page of users.
        //    if (_currentPage > _totalPages) {
        //        _currentPage = _totalPages;
        //        GetContentPages(filter);
        //        return;
        //    }

        //    ContentPagesGrid.DataSource = allContentPages.Skip((_currentPage - 1) * PageSize).Take(PageSize);
        //    ContentPagesGrid.CurrentPageIndex = _currentPage;
        //    ContentPagesGrid.DataBind();
        //    CurrentPageLabel.Text = _currentPage.ToString(CultureInfo.InvariantCulture);
        //    TotalPagesLabel.Text = _totalPages.ToString(CultureInfo.InvariantCulture);

        //    NextButton.Visible = _currentPage != _totalPages;

        //    PreviousButton.Visible = _currentPage != 1;

        //    NavigationPanel.Visible = _totalNumberOfContentPages > 0;
        //}

        public void NextButton_OnClick(object sender, EventArgs args) {
            _currentPage = Convert.ToInt32(CurrentPageLabel.Text);
            _currentPage++;
            GetContentPages(txtQueryFilter.Text);
        }

        public void PreviousButton_OnClick(object sender, EventArgs args) {
            _currentPage = Convert.ToInt32(CurrentPageLabel.Text);
            _currentPage--;
            GetContentPages(txtQueryFilter.Text);
        }

        protected void Page_Load(object sender, EventArgs e) {
            GetContentPages(txtQueryFilter.Text);
        }

        protected void ContentPagesGrid_Edit(object source, DataGridCommandEventArgs e)
        {
            // Instead of editing the item in the datagrid we redirect to custom edit page
            var virtualPath = ((DataGrid)source).Items[e.Item.ItemIndex].Cells[0].Text;
            Response.Redirect("/CMS/EditPage.aspx?virtualPath=" + HttpUtility.UrlEncode(virtualPath), true);
        }

    }
}