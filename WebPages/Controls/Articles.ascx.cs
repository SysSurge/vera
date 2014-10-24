using System;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using VeraWAF.WebPages.Bll.Resources;
using VeraWAF.WebPages.Bll;
using VeraWAF.WebPages.Dal;

namespace VeraWAF.WebPages.Controls {
    public partial class Articles : UserControl
    {
        private string _virtualPath;

        public int Mode { get; set; }
        public int Row1ArticleCount { get; set; }
        public int Row2ArticleCount { get; set; }
        public int Row3ArticleCount { get; set; }
        public int MaxArticleCount { get; set; }

        string GetParentPartitionKey()
        {
            var uri = _virtualPath;
            var uriString = uri.Remove(uri.Length - uri.Split('/').Last().Length);

            return uriString;
        }

        void ShowAllArticles()
        {
            var articles = new PageCache().GetPagesByVirtualPath(GetParentPartitionKey()).Where(page => page.IsPublished && page.VirtualPath != _virtualPath).OrderByDescending(page => page.Timestamp);
            if (!articles.Any()) return;

            var html = new StringBuilder();
            foreach (var article in articles)
                html.AppendFormat("<div class=\"articleAll\"><a class=\"articleTitle\" href=\"{0}\" property=\"sc:Article sc:URL\">{1}</a></div>", article.VirtualPath, article.Title);

            litNewsContainer.Text = html.ToString();
        }

        void ShowBigArticleList()
        {
            var articles = new PageCache().GetPagesByVirtualPath(GetParentPartitionKey()).Where(page => page.IsPublished && page.VirtualPath != _virtualPath).OrderByDescending(page => page.Timestamp);
            if (!articles.Any()) return;

            var dateUtilities = new DateUtilities();
            var baseUri = new UriUtilities().GetBase(Request.Url).ToString();
            var html = new StringBuilder();
            var userCache = new UserCache();
            var userUtilities = new UserUtilities();

            foreach (var article in articles) {

                var readablePublishedDate = dateUtilities.GetReadableDateAndTime(article.Timestamp);

                var author = userCache.GetUser(article.Author);

                if (string.IsNullOrWhiteSpace(article.RollupImage))
                    html.AppendFormat(Template.Search_article_result,
                        article.Title,
                        baseUri + article.VirtualPath.Substring(1),
                        readablePublishedDate,
                        HttpUtility.HtmlEncode(userUtilities.GetDisplayName(article.Author)),
                        author.PartitionKey,
                        article.RollupText);
                else
                    html.AppendFormat(Template.Search_article_result3,
                                      article.Title,
                                      baseUri + article.VirtualPath.Substring(1),
                                      readablePublishedDate,
                                      HttpUtility.HtmlEncode(userUtilities.GetDisplayName(article.Author)),
                                      author.PartitionKey,
                                      article.RollupText,
                                      article.RollupImage,
                                      article.RollupText);
            }

            litNewsContainer.Text = html.ToString();            
        }

        void ShowArticles() {

            switch (Mode)
            {
                case 0:
                    ShowAllArticles();
                    break;
                case 1:
                    ShowBigArticleList();
                    break;
                default:
                    throw new ApplicationException("Unknown mode");
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (SiteMap.CurrentNode != null)
            {
                _virtualPath = SiteMap.CurrentNode.Url;
                ShowArticles();
            }
        }
    }
}