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
using VeraWAF.WebPages;
using VeraWAF.WebPages.Bll;

namespace VeraWAF.WebPages.Cloud
{
    public partial class WinEventLog : PageTemplateBase
    {
        const int PageSize = 25;
        int _totalNumberOfWinEventLogItems;
        int _totalPages;
        int _currentPage = 1;
        readonly WinEventLogUtils _winEventLogUtils;

        readonly AzureTableStorageDataSource _datasource;

        public WinEventLog()
        {
            _datasource = new AzureTableStorageDataSource();
            _winEventLogUtils = new WinEventLogUtils();
        }

        protected string GetLogItemType(System.Int32 logItemType) {
            return _winEventLogUtils.GetLogItemTypeHtml(logItemType);
        }

        void GetLogItems()
        {
            List<WADWindowsEventLogEntity> allWinEventLog;

            try
            {
                // Add filter, if any
                var typeFilter = cmbTypeFilter.SelectedValue;
                if (String.IsNullOrEmpty(typeFilter))
                    allWinEventLog = new List<WADWindowsEventLogEntity>(_datasource.GetWindowsEventLogEntries());
                else
                    allWinEventLog = new List<WADWindowsEventLogEntity>(_datasource.GetWindowsEventLogEntries().Where(
                            logItem => logItem.Level == int.Parse(typeFilter))
                        );

                _totalNumberOfWinEventLogItems = allWinEventLog.Count;
                _totalPages = ((_totalNumberOfWinEventLogItems - 1) / PageSize) + 1;

            }
            catch(Exception)
            {
                // Table is not created until the first event is logged
                NavigationPanel.Visible = false;
                _totalNumberOfWinEventLogItems = _totalPages = 0;
                return;
            }


            // Ensure that we do not navigate past the last page of users.
            if (_currentPage > _totalPages) {
                _currentPage = _totalPages;
                GetLogItems();
                return;
            }

            WinEventLogGrid.DataSource = allWinEventLog.Skip((_currentPage - 1) * PageSize).Take(PageSize);
            WinEventLogGrid.CurrentPageIndex = _currentPage;
            WinEventLogGrid.DataBind();
            CurrentPageLabel.Text = _currentPage.ToString(CultureInfo.InvariantCulture);
            TotalPagesLabel.Text = _totalPages.ToString(CultureInfo.InvariantCulture);

            NextButton.Visible = _currentPage != _totalPages;

            PreviousButton.Visible = _currentPage != 1;
        }

        public void NextButton_OnClick(object sender, EventArgs args) {
            _currentPage = Convert.ToInt32(CurrentPageLabel.Text);
            _currentPage++;
            GetLogItems();
        }

        public void PreviousButton_OnClick(object sender, EventArgs args) {
            _currentPage = Convert.ToInt32(CurrentPageLabel.Text);
            _currentPage--;
            GetLogItems();
        }

        protected void Page_Load(object sender, EventArgs e) {
            GetLogItems();
        }

        protected void WinEventLogGrid_Edit(object source, DataGridCommandEventArgs e)
        {
            // Go to a more detailed view of the log item entry
            var partitionKey = ((DataGrid)source).Items[e.Item.ItemIndex].Cells[0].Text;
            Response.Redirect(String.Format("/Cloud/WinEventLogItem.aspx?id={0}", 
                partitionKey), true);
        }

        /// <summary>
        /// Reloads the current page
        /// </summary>
        void ReloadPage()
        {
            Response.Redirect(Request.Path, true);
        }

        /// <summary>
        /// Clears the log
        /// </summary>
        void ClearLog()
        {
            _datasource.ClearWindowsEventLogEntries();
        }

        /// <summary>
        /// Called when the user clicks the clear button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void clearLog_Click(object sender, EventArgs e)
        {
            ClearLog();

            ReloadPage();
        }

        /// <summary>
        /// Clearing logs can take a long time, so we increase the timout values to cope
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // This page is very slow so temporarily increase the timeout
            Server.ScriptTimeout = 3600;    // One hour timeout
        }
    }
}