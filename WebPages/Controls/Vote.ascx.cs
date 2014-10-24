using System;
using System.Linq;
using System.Globalization;
using System.Web.Security;
using System.Web.UI;
using VeraWAF.AzureTableStorage;
using VeraWAF.Core.Templates;
using VeraWAF.WebPages.Bll.Cloud;
using VeraWAF.WebPages.Dal;

namespace VeraWAF.WebPages.Controls {
    public partial class Vote : UserControl
    {
        /// <summary>
        /// Vera base page 
        /// </summary>
        PageTemplateBase _page;

        /// <summary>
        /// Vote cache
        /// </summary>
        private readonly VoteCache _voteCache;

        /// <summary>
        /// Vote ID
        /// </summary>
        public string VoteItemId { get; set; }

        /// <summary>
        /// Redirect URL after a vote has taken place
        /// </summary>
        public string RedirectUrl { get; set; }
 
        /// <summary>
        /// Membership provider user key to the user getting the vote
        /// </summary>
        public object UserGettingVote { get; set; }

        /// <summary>
        /// Class constructor
        /// </summary>
        public Vote()
        {
            _voteCache = new VoteCache();
        }

        /// <summary>
        /// Returns the vote score
        /// </summary>
        /// <returns></returns>
        int GetVoteScore()
        {
            var votes = _voteCache.GetVotes(VoteItemId);
            return votes == null ? 0 : votes.Sum(vote => vote.Value.Value);
        }

        /// <summary>
        /// Initiate the vote control with the score
        /// </summary>
        /// <param name="voteScore">Vote score</param>
        void InitVoteScoreControls(int voteScore)
        {
            voteCounter.Text = voteScore.ToString(CultureInfo.InvariantCulture);

            string colorClass;
            if (voteScore == 0) colorClass = String.Empty;
            else if (voteScore > 0) colorClass = "positive";
            else colorClass = "negative";

            divVoteCounter.Attributes["class"] = "voteCount center " + colorClass;
        }

        /// <summary>
        /// Set the correct values to the vote control from the page entity data
        /// </summary>
        /// <param name="page">Page entity</param>
        /// <param name="author">Page author's membership user</param>
        void InitVoteControl(PageEntity page, MembershipUser author)
        {
            UserGettingVote = author.ProviderUserKey;
            VoteItemId = page.PartitionKey + page.RowKey;
            RedirectUrl = page.VirtualPath;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Get the parent page
            _page = Page as PageTemplateBase;
            if (_page == null)
                throw new ApplicationException("The page does not inherit from the VeraWAF.Core.Templates.PageTemplateBase page template.");

            // Get page entity data
            var pageEntity = _page.GetPageEntity();
            if (pageEntity != null)
            {
                Visible = true;

                var author = _page.GetPageAuthor();
                InitVoteControl(pageEntity, author);

                InitVoteScoreControls(GetVoteScore());
            }
            else Visible = false;
        }

        /// <summary>
        /// Get the membership provider user key
        /// </summary>
        /// <returns>Provider user key</returns>
        string GetProviderUserKey()
        {
            String providerUserKey;
            var user = Membership.GetUser();
            if (user == null)
            {
                FormsAuthentication.RedirectToLoginPage();
                Response.End();
                providerUserKey = null;
            } else providerUserKey = user.ProviderUserKey.ToString();

            return providerUserKey;
        }

        /// <summary>
        /// Flush cloud instance caches
        /// </summary>
        void FlushCaches()
        {
            // Clear some local instance caches
            new VoteCache().Clear();
            new UserCache().Clear2();

            // Make sure that all the other cloud instances are updated to reflect the vote changes
            new CloudCommandClient().SendCommand("VoteCRUD", false);

            // Update all the other cloud instances by flushing their user caches
            new CloudCommandClient().SendCommand("ClearUserCache", false);
        }

        /// <summary>
        /// Called when the user clicks the upvote button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void butUpVote_OnClick(object sender, EventArgs e)
        {
            // Set the number of points to vote up
            const int maxVoteUp = 5;

            // Update the number of points for the vote item and the user social points
            _voteCache.AddVote(VoteItemId, GetProviderUserKey(), UserGettingVote.ToString(), maxVoteUp);

            // Make sure all the caches are updated with the changes
            FlushCaches();

            Response.Redirect(RedirectUrl);
        }

        /// <summary>
        /// Called when the user clicks the downvote button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void butDownVote_OnClick(object sender, EventArgs e)
        {
            // Set the number of points to vote down
            const int maxVoteDown = 5;

            // Update the number of points for the vote item and the user social points
            _voteCache.AddVote(VoteItemId, GetProviderUserKey(), UserGettingVote.ToString(), maxVoteDown);

            // Make sure all the caches are updated with the changes
            FlushCaches();

            Response.Redirect(RedirectUrl);
        }

    }
}