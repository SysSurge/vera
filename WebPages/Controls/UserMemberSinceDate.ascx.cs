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
    /// Shows the date when a user registered his/her account in a <time> HTML element that uses time ago functionality.
    /// </summary>
    /// <remarks>
    /// The user id is determined in the following sequence; UserId input field, UserIdFromRequestParam input field named 
    /// HTTP request parameter, the page author, and finally the currently signed in user.
    /// </remarks>
    public partial class UserMemberSinceDate : UserFieldControlBase
    {
        /// <summary>
        /// Vera base page 
        /// </summary>
        PageTemplateBase _page;

        /// <summary>
        /// Add the date to the user control
        /// </summary>
        /// <remarks>
        /// Called by VeraWAF.WebPages.Bll.UserFieldControlBase after having loaded the user info.
        /// </remarks>
        /// <param name="user">User</param>
        protected override void FillControlFields(MembershipUser user)
        {
            var dateUtils = new DateUtilities();
            MemberSince.InnerText = dateUtils.GetReadableDate(user.CreationDate);
            MemberSince.Attributes["datetime"] = dateUtils.GetCustomIso8601Date(user.CreationDate);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Get the parent page
            _page = Page as PageTemplateBase;
            if (_page == null)
                throw new ApplicationException("The page does not inherit from the VeraWAF.Core.Templates.PageTemplateBase page template.");

            // Get page entity data
            var pageEntity = _page.GetPageEntity();

            // Process user info and fill fields
            ProcessField(pageEntity);
        }

    }
}