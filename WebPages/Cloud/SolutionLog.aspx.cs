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

namespace VeraWAF.WebPages.Cloud
{
    public partial class SolutionLog : PageTemplateBase
    {
        /// <summary>
        /// Max number of items per page
        /// </summary>
        const int PageSize = 25;

        /// <summary>
        /// Total number of items
        /// </summary>
        int _totalNumberOfSolutionLogItems;

        /// <summary>
        /// Total number of pages
        /// </summary>
        int _totalPages;

        /// <summary>
        /// Current page number
        /// </summary>
        int _currentPage = 1;

        /// <summary>
        /// Azure table data source
        /// </summary>
        readonly AzureTableStorageDataSource _datasource;

        /// <summary>
        /// Application name
        /// </summary>
        readonly string _applicationName;


        /// <summary>
        /// Class constructor
        /// </summary>
        public SolutionLog()
        {
            _datasource = new AzureTableStorageDataSource();
            _applicationName = ConfigurationManager.AppSettings["ApplicationName"];
        }

        /// <summary>
        /// Get the color for a log item
        /// </summary>
        /// <param name="logItemType">Log item type</param>
        /// <returns>The correct HTML SPAN element markup with the CSS color code for the log item type</returns>
        protected string GetLogItemTypeColor(string logItemType)
        {
            string color;

            switch (logItemType)
            {
                case "Info":
                    color = "black";
                    break;
                case "Warning":
                    color = "orange";
                    break;
                case "Error":
                    color = "red";
                    break;
                default:
                    color = "black";
                    break;
            }

            return String.Format("<span style=\"color:{1}\">{0}</span>", logItemType, color);
        }

        /// <summary>
        /// Load log items
        /// </summary>
        void GetLogItems()
        {
            List<CloudLogEntity> allSolutionLog;

            // Add filter, if any
            var typeFilter = cmbTypeFilter.SelectedValue;
            if (String.IsNullOrEmpty(typeFilter))
                allSolutionLog = new List<CloudLogEntity>(_datasource.GetCloudLogEntries(_applicationName));
            else
                allSolutionLog = new List<CloudLogEntity>(_datasource.GetCloudLogEntries(_applicationName).Where(logItem => logItem.RowKey == typeFilter));

            _totalNumberOfSolutionLogItems = allSolutionLog.Count;
            _totalPages = ((_totalNumberOfSolutionLogItems - 1) / PageSize) + 1;

            // Ensure that we do not navigate past the last page of users.
            if (_currentPage > _totalPages) {
                _currentPage = _totalPages;
                GetLogItems();
                return;
            }

            SolutionLogGrid.DataSource = allSolutionLog.Skip((_currentPage - 1) * PageSize).Take(PageSize);
            SolutionLogGrid.CurrentPageIndex = _currentPage;
            SolutionLogGrid.DataBind();
            CurrentPageLabel.Text = _currentPage.ToString(CultureInfo.InvariantCulture);
            TotalPagesLabel.Text = _totalPages.ToString(CultureInfo.InvariantCulture);

            NextButton.Visible = _currentPage != _totalPages;

            PreviousButton.Visible = _currentPage != 1;

            NavigationPanel.Visible = _totalNumberOfSolutionLogItems > 0;
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

        protected void SolutionLogGrid_Edit(object source, DataGridCommandEventArgs e)
        {
            // Go to a more detailed view of the log item entry
            var partitionKey = ((DataGrid)source).Items[e.Item.ItemIndex].Cells[0].Text;
            Response.Redirect(String.Format("/Cloud/SolutionLogItem.aspx?id={0}", 
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
            _datasource.ClearCloudLogEntries(_applicationName);
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