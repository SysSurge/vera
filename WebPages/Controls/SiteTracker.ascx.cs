using System;
using System.Web.UI;
using VeraWAF.WebPages.Bll.Resources;

namespace VeraWAF.WebPages.Controls {

    /// <summary>
    /// Handles your site tracker.
    /// Example is your Google Analytics tracking javascript code block.
    /// </summary>
    /// <remarks>
    /// Tracking is disabled in debug made to prevent the logging of development and staging traffic.
    /// </remarks>
    public partial class SiteTracker : UserControl {
        protected void Page_Load(object sender, EventArgs e)
        {
#if DEBUG
            this.Visible = false;
#else
            litSiteTracker.Text = ThirdParty.SiteTrackerHtml;
#endif
        }
    }
}