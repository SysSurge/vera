using System;
using System.Configuration;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using VeraWAF.AzureTableStorage;
using VeraWAF.Core.Templates;
using VeraWAF.WebPages.Bll;
using VeraWAF.WebPages.Dal;

namespace VeraWAF.WebPages.Templates {
    public partial class ForumSection : PageTemplateBase
    {
        const int PageSize = 25;
        int _totalNumberOfContentPages;
        int _totalPages;
        int _currentPage = 1;

        readonly ForumPageCache _pageCache;
        readonly UserUtilities _userUtilities;
        readonly DateUtilities _dateUtilities;

        public ForumSection()
        {
            _pageCache = new ForumPageCache();
            _userUtilities = new UserUtilities();
            _dateUtilities = new DateUtilities();
        }

        private DateTime GetArticlePublishedDate(PageEntity page) {
            return new DateTime(long.Parse(page.RowKey));
        }

        private string GetVirtualPath() {
            return Request.Url.AbsolutePath;
        }

        public int GetNumberOfVotes(string virtualPath) {
            var sum = 0;

            try {
                sum = _pageCache.GetAllPages().Where(page => page.VirtualPath == virtualPath).Sum(
                    page => new VoteCache().GetVotes(page.PartitionKey + page.RowKey).Values.Sum(vote => vote.Value));
            } catch (Exception) {
                // Intentionally empty to avoid NullException from VoteCache().GetVotes()
            }

            return sum;
        }

        public int GetNumberOfFavorites(string favoriteItemId) {
            var sum = 0;

            try {
                sum = new FavoriteCache().GetFavorites(favoriteItemId).Values.Count;
            } catch (Exception) {
                // Intentionally empty
            }

            return sum;
        }

        public string Docals(string virtualPath)
        {
            try
            {
                var page = _pageCache.GetPageByVirtualPath(virtualPath);
                var author = Membership.GetUser(page.Author);
                var numReplies = _pageCache.GetCommentsByVirtualPath(virtualPath).Count();
                var pubDate = GetArticlePublishedDate(page);

                return String.Format(Bll.Resources.Forums.ForumSectionItemMarkup, 
                    _dateUtilities.GetCustomIso8601Date(pubDate),
                    _dateUtilities.GetReadableDateAndTime(pubDate),
                    _dateUtilities.GetCustomIso8601Date(page.Timestamp),
                    _dateUtilities.GetReadableDateAndTime(page.Timestamp),
                    author.ProviderUserKey,
                    HttpUtility.HtmlEncode(_userUtilities.GetDisplayName(author)),
                    page.VirtualPath,
                    numReplies,
                    HttpUtility.HtmlEncode(page.Title),
                    GetNumberOfVotes(virtualPath),
                    GetNumberOfFavorites(page.PartitionKey)
                    );
            }
            catch (Exception)
            {
                return String.Format(Bll.Resources.Forums.ForumSectionItemErrorMessage, virtualPath);
            }
        }

        string GetParentUrl() {
            return Request.Url.AbsolutePath.Substring(0, Request.Url.AbsolutePath.LastIndexOf('/'));
        }

        void GetPages()
        {
            var virtualPath = GetParentUrl();

            var allContentPages = new List<PageEntity>(_pageCache.GetPagesByVirtualPath(virtualPath).Where(page => page.IsPublished
                        && page.Template == ConfigurationManager.AppSettings["DefaultForumPostTemplate"]));

            _totalNumberOfContentPages = allContentPages.Count;
            _totalPages = ((_totalNumberOfContentPages - 1) / PageSize) + 1;

            // Ensure that we do not navigate past the last page of users.
            if (_currentPage > _totalPages) {
                _currentPage = _totalPages;
                GetPages();
                return;
            }

            ContentPagesGrid.DataSource = allContentPages.Skip((_currentPage - 1) * PageSize).Take(PageSize).OrderByDescending(comment => comment.Timestamp);
            ContentPagesGrid.CurrentPageIndex = _currentPage;
            ContentPagesGrid.DataBind();
            CurrentPageLabel.Text = _currentPage.ToString(CultureInfo.InvariantCulture);
            TotalPagesLabel.Text = _totalPages.ToString(CultureInfo.InvariantCulture);

            NextButton.Visible = _currentPage != _totalPages;

            PreviousButton.Visible = _currentPage != 1;

            NavigationPanel.Visible = _totalNumberOfContentPages > 0;
        }

        public void NextButton_OnClick(object sender, EventArgs args) {
            _currentPage = Convert.ToInt32(CurrentPageLabel.Text);
            _currentPage++;
            GetPages();
        }

        public void PreviousButton_OnClick(object sender, EventArgs args) {
            _currentPage = Convert.ToInt32(CurrentPageLabel.Text);
            _currentPage--;
            GetPages();
        }

        protected void Page_Load(object sender, EventArgs e) {
            if (!IsPostBack) GetPages();
        }

    }
}