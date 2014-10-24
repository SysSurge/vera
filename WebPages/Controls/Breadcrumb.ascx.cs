using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;

namespace VeraWAF.WebPages.Controls {
    public partial class Breadcrumb : UserControl {

        string GetBreadcrumbNavigationMarkup() {
            var sb = new StringBuilder("<a href=\"/\">Home</a>");
            var path = new StringBuilder("/");

            foreach (var folder in Request.Path.Split('/').Skip(1).TakeWhile(folder => !folder.EndsWith(".aspx"))) {
                path.AppendFormat("{0}/", folder);

                var fullPath = path.ToString();
                if (fullPath.EndsWith("/")) fullPath += "default.aspx";

                sb.AppendFormat(" <i class=\"fa fa-arrow-circle-right right-arrow\"></i> <a href=\"{0}\">{1}</a>", fullPath, folder.Replace('-', ' '));
            }

            if (path.ToString() == "/")
            {
                breadcrumbContainer.Visible = false;
                return String.Empty;
            }


            
            return sb.ToString();
        }

        string GetBreadcrumbNavigationMarkupUsingSiteMap(SiteMapNode currentSitemapNode) {
            var sb = new StringBuilder();

            if (currentSitemapNode != null) {
                var stack = new Stack<SiteMapNode>();

                while (currentSitemapNode != null) {
                    stack.Push(currentSitemapNode);
                    currentSitemapNode = currentSitemapNode.ParentNode;
                }

                if (stack.Count > 2) {
                    currentSitemapNode = stack.Pop();
                    while (currentSitemapNode != SiteMap.CurrentNode) {
                        sb.AppendFormat("<a href=\"{0}\" title=\"{1}\" type=\"text/html\">{2}</a>",
                                        currentSitemapNode.Url, currentSitemapNode.Description, currentSitemapNode.Title);

                        if (stack.Peek() != SiteMap.CurrentNode)
                            sb.Append(" &gt; ");

                        currentSitemapNode = stack.Pop();
                    }
                } 
                else breadcrumbContainer.Visible = false;

            } else breadcrumbContainer.Visible = false;

            return sb.ToString();
        }

        protected void Page_Load(object sender, EventArgs e) {
            var currentSitemapNode = SiteMap.CurrentNode;

            litBreadcrumb.Text = currentSitemapNode == null
                                     ? GetBreadcrumbNavigationMarkup()
                                     : GetBreadcrumbNavigationMarkupUsingSiteMap(currentSitemapNode);
        }
    }
}