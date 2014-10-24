using System;
using System.Web;
using System.Web.Security;
using System.Web.Profile;
using System.Web.UI;
using VeraWAF.AzureTableStorage;
using VeraWAF.Core.Templates;
using VeraWAF.WebPages.Bll;

namespace VeraWAF.WebPages.Controls
{
    /// <summary>
    /// Shows a user portrait.
    /// </summary>
    /// <remarks>
    /// The user id is determined in the following sequence; UserId input field, UserIdFromRequestParam input field named 
    /// HTTP request parameter, the page author, and finally the currently signed in user.
    /// </remarks>
    public partial class UserPortrait : UserFieldControlBase
    {
        /// <summary>
        /// Vera base page 
        /// </summary>
        PageTemplateBase _page;

        /// <summary>
        /// Set to True to show the user name or to False to hide the user name.
        /// Default is true
        /// </summary>
        public bool ShowUserName { get; set; }

        /// <summary>
        /// Class constructor
        /// </summary>
        public UserPortrait()
        {
            ShowUserName = true;
        }

        /// <summary>
        /// Show the user's profile picture
        /// </summary>
        /// <remarks>
        /// Called by VeraWAF.WebPages.Bll.UserFieldControlBase after having loaded the user info.
        /// </remarks>
        /// <param name="user">User</param>
        protected override void FillControlFields(MembershipUser user)
        {
            // Get the user's profile
            var profile = ProfileBase.Create(user.UserName, true);

            // Get the user's name
            var userUtilities = new UserUtilities();
            var displayName = userUtilities.GetDisplayName(user);
            jailUserPortrait.alt = ProcessFieldData(displayName);

            // Get the image url
            var portraitUrl = userUtilities.GetPortraitUrl(profile);
            jailUserPortrait.src = portraitUrl;
            figCaption.Text = Server.HtmlEncode(displayName);

            // Add a link to the user's profile page
            lnkUserPortrait.HRef = String.Format("/Account/?id={0}", user.ProviderUserKey);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Get the parent page
            _page = Page as PageTemplateBase;
            if (_page == null)
                throw new ApplicationException("The page does not inherit from the VeraWAF.Core.Templates.PageTemplateBase page template.");

            // Get page entity data
            var pageEntity = _page.GetPageEntity();

            figCaption.Visible = ShowUserName;

            if (pageEntity == null) return;



            // Process user info and fill fields
            ProcessField(pageEntity);
        }

    }
}