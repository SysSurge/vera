using System;
using System.Text;
using System.Web;
using VeraWAF.Core.Templates;
using VeraWAF.WebPages.Bll;
using VeraWAF.WebPages.Bll.Search;

namespace VeraWAF.WebPages {
    public partial class Search : PageTemplateBase 
    {
        /// <summary>
        /// Called when the user clicks the search button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void butSubmit_Click(object sender, EventArgs e) {
            Response.Redirect(String.Format("/Search.aspx?query={0}", HttpUtility.UrlEncode(txtSearch.Text)), true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                var mode = Request["mode"];

                // 404 HTTP Page Not Found error messages may be directed to the search page
                h2PageNotFound.Visible = !String.IsNullOrWhiteSpace(mode) && mode == "page-not-found";

                // Process the search
                var queryRequest = Request["query"];
                if (!String.IsNullOrEmpty(queryRequest))
                {
                    // Get the base Url
                    var baseUri = new UriUtilities().GetBase(Request.Url).ToString();

                    // Show the HTML markup with the search results
                    litSearchResults.Text = new SearchQueryHelper().ProcessQueryHtml(baseUri, queryRequest);

                    // Who the query in the search input box
                    txtSearch.Text = queryRequest;
                }
            }
        }

    }
}