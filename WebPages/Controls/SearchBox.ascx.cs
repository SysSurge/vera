using System;
using System.Web;
using System.Web.UI;

namespace VeraWAF.WebPages.Controls {
    public partial class SearchBox : UserControl {
        protected void Page_Load(object sender, EventArgs e) {
        }

        protected void butSearch_Click(object sender, EventArgs e)
        {
            Response.Redirect(String.Format("/Search.aspx?query={0}", HttpUtility.UrlEncode(txtSearchbox.Text)), true);
        }
    }
}