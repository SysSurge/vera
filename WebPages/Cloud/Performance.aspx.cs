using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using VeraWAF.Core.Templates;
using VeraWAF.WebPages.Bll;
using VeraWAF.WebPages.Bll.Cloud;

namespace VeraWAF.WebPages.Cloud
{
    public partial class Performance : PageTemplateBase
    {
        /// <summary>
        /// Get all the nodes in the cloud
        /// </summary>
        /// <returns>IP addresses to all the nodes in the cloud</returns>
        string GetNodes()
        {
            var cloudNodeIpAddresses = new CloudUtils().GetNodes();
            return String.Join(",", cloudNodeIpAddresses.Select(x => x.ToString()).ToArray());
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            nodeList.InnerText = GetNodes();
            cloudPassword.InnerText = ConfigurationManager.AppSettings["API_Key"];
        }
    }
}