using System;
using System.Web;
using System.Web.UI;
using VeraWAF.WebPages.Bll;

namespace VeraWAF.WebPages.Templates {
    public partial class RedirectToParent : Page {

        void RedirectUserToParentFolder()
        {
            var absoluteUri = HttpContext.Current.Request.Url;
            var uriUtlities = new UriUtilities();
            var relativeUri = uriUtlities.ConvertAbsoluteToRelativeUri(absoluteUri);

            if (relativeUri.ToString() != "/" && !relativeUri.ToString().Equals("/default.aspx", StringComparison.InvariantCultureIgnoreCase))
            {
                var parentUri = uriUtlities.GetParentUri(relativeUri);
                Response.Redirect(parentUri.ToString());
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            RedirectUserToParentFolder();
        }
    }
}