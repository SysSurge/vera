using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.UI;
using VeraWAF.AzureTableStorage;
using VeraWAF.Core.Templates;
using VeraWAF.CrossCuttingConcerns;
using VeraWAF.WebPages.Bll;

namespace VeraWAF.WebPages.Cloud
{
    public partial class SolutionLogItem : PageTemplateBase
    {
        readonly AzureTableStorageDataSource _datasource;
        readonly string _applicationName;
        string _partitionKey;

        public SolutionLogItem()
        {
            _datasource = new AzureTableStorageDataSource();
            _applicationName = ConfigurationManager.AppSettings["ApplicationName"];
        }

        void PopulateFormFields(CloudLogEntity logEntry)
        {
            litType.Text = logEntry.RowKey;
            litInternalId.Text = logEntry.PartitionKey;
            var dateFormatRule = "{0:" + ConfigurationManager.AppSettings["DateFormat"] + "}";
            litTimestamp.Text = String.Format(dateFormatRule, logEntry.Timestamp);
            txtDescription.Text = logEntry.Message;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _partitionKey = Request["id"];
            if (String.IsNullOrWhiteSpace(_partitionKey)) return;

            var file = _datasource.GetCloudLogEntry(_partitionKey, _applicationName);
            if (file != null && !Page.IsPostBack && !String.IsNullOrEmpty(_partitionKey))
                PopulateFormFields(file);
        }
    }
}