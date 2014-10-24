using System;
using System.Text;
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
    /// Shows the role that the user is a member of.
    /// </summary>
    /// <remarks>
    /// The user id is determined in the following sequence; UserId input field, UserIdFromRequestParam input field named 
    /// HTTP request parameter, the page author, and finally the currently signed in user.
    /// </remarks>
    public partial class UserRoles : UserFieldControlBase
    {
        /// <summary>
        /// User role display modes
        /// </summary>
        public enum EUserRoleDisplayMode
        {
            /// <summary>
            /// Comma seperated list
            /// </summary>
            Csv, 
            /// <summary>
            /// HTML unordered list
            /// </summary>
            UnorderedList
        }

        /// <summary>
        /// Vera base page 
        /// </summary>
        PageTemplateBase _page;

        /// <summary>
        /// Display mode. Default value is EUserRoleDisplayMode.UnorderedList.
        /// </summary>
        public EUserRoleDisplayMode DisplayMode { get; set; }

        /// <summary>
        /// Set to true to display the text "Roles:" in front of the role list.
        /// Default value is false.
        /// </summary>
        public bool ShowLabel { get; set; }
        
        /// <summary>
        /// Class constructor
        /// </summary>
        public UserRoles()
        {
            // Set the default display mode to an unordered list
            DisplayMode = EUserRoleDisplayMode.UnorderedList;

            // Don't show the label by default
            ShowLabel = false;
        }

        /// <summary>
        /// Show the member roles as a csv
        /// </summary>
        /// <param name="user">User</param>
        void ShowAsCsv(MembershipUser user)
        {
            var rolesForUser = Roles.GetRolesForUser(user.UserName);
            if (rolesForUser.Length > 0)
            {
                var userRolesText = new StringBuilder();

                // Show the label before the list?
                if (ShowLabel)
                    userRolesText.AppendFormat("{0}: ", Bll.Resources.Template.Roles);

                var isFirst = true;

                foreach (var role in rolesForUser)
                {
                    if (isFirst)
                        userRolesText.Append(role);
                    else
                        userRolesText.AppendFormat(", {0}", role);

                    isFirst = false;
                }

                litRoles.Text = userRolesText.ToString();
            }
            else litRoles.Text = Bll.Resources.Controls.None;   // "None" text
        }

        /// <summary>
        /// Show the member roles as an unordered list
        /// </summary>
        /// <param name="user">User</param>
        void ShowAsUnorderedList(MembershipUser user)
        {
            var rolesForUser = Roles.GetRolesForUser(user.UserName);
            if (rolesForUser.Length > 0)
            {
                var userRolesText = new StringBuilder();

                // Show the label before the list?
                if (ShowLabel)
                    userRolesText.AppendFormat("{0}: <ul>", Bll.Resources.Template.Roles);
                else
                    userRolesText.Append("<ul>");

                foreach (var role in rolesForUser)
                    userRolesText.AppendFormat("<li>{0}</li>", role);

                userRolesText.Append("</ul>");

                litRoles.Text = userRolesText.ToString();
            }
            else litRoles.Text = Bll.Resources.Controls.None;   // "None" text
        }

        /// <summary>
        /// Show the user's role memberships.
        /// </summary>
        /// <remarks>
        /// Called by VeraWAF.WebPages.Bll.UserFieldControlBase after having loaded the user info.
        /// </remarks>
        /// <param name="user">User</param>
        protected override void FillControlFields(MembershipUser user)
        {
            switch(DisplayMode)
            {
                case EUserRoleDisplayMode.Csv:
                    ShowAsCsv(user);
                    break;
                case EUserRoleDisplayMode.UnorderedList:
                    ShowAsUnorderedList(user);
                    break;
                default:
                    throw new ArgumentException("Unknown display mode");
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

            // Process user info and fill fields
            ProcessField(pageEntity);
        }

    }
}