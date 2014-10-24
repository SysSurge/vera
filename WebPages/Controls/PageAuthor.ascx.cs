using System;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using VeraWAF.AzureTableStorage;
using VeraWAF.Core.Templates;
using VeraWAF.WebPages.Bll;

namespace VeraWAF.WebPages.Controls
{
    public partial class PageAuthor : UserControl
    {
        /// <summary>
        /// Vera base page 
        /// </summary>
        PageTemplateBase _page;

        /// <summary>
        /// Show the page author
        /// </summary>
        /// <param name="author">Membership user</param>
        void SetArticleAuthor(MembershipUser author)
        {
            if (author != null)
            {
                lnkAuthor.NavigateUrl = "/Account/?id=" + author.ProviderUserKey;

                // Get a nice looking user name
                var niceUserName = new UserUtilities().GetDisplayName(author);

                lnkAuthor.Text = HttpUtility.HtmlEncode(niceUserName);
            }
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
                phDate.Visible = true;

                // Get the membership user
                var author = _page.GetPageAuthor();

                // Show the page author
                SetArticleAuthor(author);
            }

        }
    }
}