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
    public partial class WinEventLogItem : PageTemplateBase
    {
        readonly AzureTableStorageDataSource _datasource;
        string _partitionKey;
        readonly WinEventLogUtils _winEventLogUtils;

        public WinEventLogItem()
        {
            _datasource = new AzureTableStorageDataSource();
            _winEventLogUtils = new WinEventLogUtils();
        }

        void PopulateFormFields(WADWindowsEventLogEntity logEntry)
        {
            litType.Text = _winEventLogUtils.GetLogItemTypeHtml(logEntry.Level);
            litInternalId.Text = logEntry.PartitionKey;

            var dateFormatRule = "{0:" + ConfigurationManager.AppSettings["DateFormat"] + "}";
            litTimestamp.Text = String.Format(dateFormatRule, logEntry.Timestamp);
            litEventId.Text = logEntry.EventId.ToString();

            litRole.Text = logEntry.Role;
            litRoleInstance.Text = logEntry.RoleInstance;
            litDeploymentId.Text = logEntry.DeploymentId;
            litChannel.Text = logEntry.Channel;
            litProviderName.Text = logEntry.ProviderName;
            litPid.Text = logEntry.Pid.ToString();
            litTid.Text = logEntry.Tid.ToString();

            txtDescription.Text = Server.HtmlEncode(logEntry.Description);
            txtRawXml.Text = logEntry.RawXml;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _partitionKey = Request["id"];
            if (String.IsNullOrWhiteSpace(_partitionKey)) return;

            var file = _datasource.GetWindowsEventLogEntry(_partitionKey);
            if (file != null && !Page.IsPostBack && !String.IsNullOrEmpty(_partitionKey))
                PopulateFormFields(file);
        }

    }
}