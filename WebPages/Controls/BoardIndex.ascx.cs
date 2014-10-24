using System;
using System.Web;
using System.Web.UI;

namespace VeraWAF.WebPages.Controls {
    public partial class BoardIndex : UserControl {
        protected void Page_Load(object sender, EventArgs e) {
            if (SiteMap.CurrentNode != null) {
                siteMapDataSource.StartingNodeUrl = SiteMap.CurrentNode.Url;
            }
        }
    }
}