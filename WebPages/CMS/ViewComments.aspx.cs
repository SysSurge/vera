using VeraWAF.AzureTableStorage;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using VeraWAF.Core.Templates;

namespace VeraWAF.WebPages.Edit {
    public partial class ViewComments : PageTemplateBase
    {
        const int PageSize = 25;
        int _totalNumberOfContentPages;
        int _totalPages;
        int _currentPage = 1;

        readonly AzureTableStorageDataSource _datasource;
        readonly string _applicationName;


        public ViewComments()
        {
            _datasource = new AzureTableStorageDataSource();
            _applicationName = ConfigurationManager.AppSettings["ApplicationName"];
        }

        void GetComments()
        {
            var allContentPages = new List<PageEntity>(
                _datasource.GetPages(_applicationName).Where(page => !String.IsNullOrWhiteSpace(page.ParentRowKey) ));

            _totalNumberOfContentPages = allContentPages.Count;
            _totalPages = ((_totalNumberOfContentPages - 1) / PageSize) + 1;

            // Ensure that we do not navigate past the last page of users.
            if (_currentPage > _totalPages) {
                _currentPage = _totalPages;
                GetComments();
                return;
            }

            ContentPagesGrid.DataSource = allContentPages.Skip((_currentPage - 1) * PageSize).Take(PageSize).OrderByDescending(comment => comment.Timestamp);
            ContentPagesGrid.CurrentPageIndex = _currentPage;
            ContentPagesGrid.DataBind();
            CurrentPageLabel.Text = _currentPage.ToString(CultureInfo.InvariantCulture);
            TotalPagesLabel.Text = _totalPages.ToString(CultureInfo.InvariantCulture);

            NextButton.Visible = _currentPage != _totalPages;

            PreviousButton.Visible = _currentPage != 1;

            NavigationPanel.Visible = _totalNumberOfContentPages > 0;
        }

        public void NextButton_OnClick(object sender, EventArgs args) {
            _currentPage = Convert.ToInt32(CurrentPageLabel.Text);
            _currentPage++;
            GetComments();
        }

        public void PreviousButton_OnClick(object sender, EventArgs args) {
            _currentPage = Convert.ToInt32(CurrentPageLabel.Text);
            _currentPage--;
            GetComments();
        }

        protected void Page_Load(object sender, EventArgs e) {
            if (IsPostBack) return;

            GetComments();
        }

        protected void CommentsGrid_Edit(object source, DataGridCommandEventArgs e)
        {
            // Instead of editing the item in the datagrid we redirect to custom edit page
            var virtualPath = ((DataGrid)source).Items[e.Item.ItemIndex].Cells[0].Text;
            var rowKey = ((DataGrid)source).Items[e.Item.ItemIndex].Cells[1].Text;
            Response.Redirect(String.Format("/CMS/EditPage.aspx?virtualPath={0}&rowKey={1}",
                HttpUtility.UrlEncode(virtualPath), rowKey), 
                true);
        }
    }
}