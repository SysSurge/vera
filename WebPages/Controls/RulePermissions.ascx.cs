using System;
using System.Configuration;
using System.Web.UI;
using VeraWAF.AzureTableStorage;

namespace VeraWAF.WebPages.Controls
{
    public partial class RulePermissions : UserControl
    {
        /// <summary>
        /// Partition key
        /// </summary>
        public string PartitionKey { get; set; }

        /// <summary>
        /// Row key
        /// </summary>
        public string RowKey { get; set; }

        /// <summary>
        /// Allow read access
        /// </summary>
        public bool AllowRead { get; set; }

        /// <summary>
        /// Allow write access
        /// </summary>
        public bool AllowWrite { get; set; }

        /// <summary>
        /// Allow delete access
        /// </summary>
        public bool AllowDelete { get; set; }

        /// <summary>
        /// Deny read access
        /// </summary>
        public bool DenyRead { get; set; }

        /// <summary>
        /// Deny write access
        /// </summary>
        public bool DenyWrite { get; set; }

        /// <summary>
        /// Deny delete access
        /// </summary>
        public bool DenyDelete { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            var markup = RowKey.Replace("ROLE:", "<i class=\"fa fa-users\"></i> ");
            markup = markup.Replace("USER:", "<i class=\"fa fa-user\"></i> ");
            litUserOrRoleName.Text = markup;

            chkAllowRead.Checked = AllowRead;
            chkAllowWrite.Checked = AllowWrite;
            chkAllowDelete.Checked = AllowDelete;
            chkDenyRead.Checked = DenyRead;
            chkDenyWrite.Checked = DenyWrite;
            chkDenyDelete.Checked = DenyDelete;
        }

        protected void butRemove_Click(object sender, EventArgs e)
        {
            var datasource = new AzureTableStorageDataSource();
            var rule = new AccessControlEntity();
            rule.ApplicationName = ConfigurationManager.AppSettings["ApplicationName"];
            rule.PartitionKey = PartitionKey;
            rule.RowKey = RowKey;

            datasource.Delete(rule);
        }
    }
}