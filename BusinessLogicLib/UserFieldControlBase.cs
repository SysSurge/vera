using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.Profile;
using System.Web.UI;
using VeraWAF.AzureTableStorage;

namespace VeraWAF.WebPages.Bll
{
    /// <summary>
    /// Abstract base class for Vera user field controls.
    /// Contains all the shared functionality between Vera user field controls
    /// </summary>
    public abstract class UserFieldControlBase : UserControl
    {
        /// <summary>
        /// Use the current user.
        /// Default value is false.
        /// </summary>
        /// <remarks>Overrides UserId</remarks>
        public bool UseCurrentUser { get; set; }

        /// <summary>
        /// User's partition key. If empty the page author is used.
        /// </summary>
        /// <remarks>Overrides UserIdFromRequestParam</remarks>
        public string UserId { get; set; }

        /// <summary>
        /// HTTP request parameter that contains the user's partition key. 
        /// </summary>
        /// <remarks>Overridden by UserId if in use</remarks>
        public string UserIdFromRequestParam { get; set; }

        /// <summary>
        /// Set to true to HTML encode the output.
        /// Default value is false.
        /// </summary>
        public bool HtmlEncode { get; set; }

        /// <summary>
        /// Set to true to URL encode the output.
        /// Default value is false.
        /// </summary>
        public bool UrlEncode { get; set; }

        /// <summary>
        /// Set to true to allow for BBCodes.
        /// Default value is false.
        /// </summary>
        public bool EnableBbCodes { get; set; }

        /// <summary>
        /// Optional string formatting
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// Optional regular expression replacement pattern
        /// </summary>
        /// <remarks>
        /// RegExReplaceReplacement must have a value.
        /// </remarks>
        public string RegExReplacePattern { get; set; }

        /// <summary>
        /// Optional regular expression replacement replacement.
        /// </summary>
        /// <remarks>
        /// RegExReplacePattern must have a value.
        /// </remarks>
        public string RegExReplaceReplacement { get; set; }

        /// <summary>
        /// Class constructor
        /// </summary>
        public UserFieldControlBase()
        {
            // Turn off HTML encoding off by default
            HtmlEncode = false;

            // Turn off URL encoding off by default
            UrlEncode = false;

            // Turn off BBCodes encoding off by default
            EnableBbCodes = false;

            // Don't force the current user by default
            UseCurrentUser = false;
        }

        /// <summary>
        /// Pure virtual method that allows the control to fill its control fields.
        /// </summary>
        /// <param name="user">ASP.NET membership user</param>
        protected abstract void FillControlFields(MembershipUser user);

        /// <summary>
        /// Load the current user
        /// </summary>
        void LoadCurrentUserInfo()
        {
            var user = Membership.GetUser();
            if (user != null)
            {
                Visible = true;

                // Show the specified user
                FillControlFields(user);
            }
            else Visible = false;
        }

        /// <summary>
        /// Show the portrait of the page author
        /// </summary>
        /// <param name="author">User name</param>
        void LoadPageAuthorInfo(string author)
        {
            var user = Membership.GetUser(author);
            if (user != null)
            {
                Visible = true;

                // Show the specified user
                FillControlFields(user);
            }
            else Visible = false;
        }

        /// <summary>
        /// Show the portrait of a specific user
        /// </summary>
        /// <param name="providerUserKey">User's partition key or ASP.NET membership provider user key</param>
        void LoadSpecificUserInfo(object providerUserKey)
        {
            var user = Membership.GetUser(providerUserKey);
            if (user != null)
            {
                Visible = true;

                // Show the specified user
                FillControlFields(user);
            }
            else Visible = false;
        }

        /// <summary>
        /// Processes the user field control.
        /// </summary>
        /// <remarks>Fill the control field(s) by calling FillControlFields if success.</remarks>
        /// <param name="pageEntity">Page entity or null if not a virtual page.</param>
        protected void ProcessField(PageEntity pageEntity)
        {
            // Force loading of the current user?
            if (UseCurrentUser)
            {
                // Load info about the current user
                LoadCurrentUserInfo();
                return;
            }

            // Get user id from input fields
            if (String.IsNullOrEmpty(UserId) && !String.IsNullOrEmpty(UserIdFromRequestParam))
                UserId = Request[UserIdFromRequestParam];    // Get user id from HTTP request parameter

            // Use the page author or a specific one?
            if (String.IsNullOrEmpty(UserId))
            {
                if (pageEntity != null && !String.IsNullOrEmpty(pageEntity.Author))
                {
                    // Use the page author
                    LoadPageAuthorInfo(pageEntity.Author);
                }
                else
                {
                    // Use the current user
                    LoadCurrentUserInfo();
                }
            }
            else
            {
                // Use a specific user
                LoadSpecificUserInfo(UserId as object);
            }
        }

        /// <summary>
        /// Processes the field data with optional formatting, regular expression replacement, a.s.f.
        /// </summary>
        /// <param name="fieldData">Field data</param>
        protected string ProcessFieldData(string fieldData)
        {
            var processedData = fieldData;

            // Make sure we encode the output if so requested

            // HTML encode?
            if (HtmlEncode == true)
                processedData = Server.HtmlEncode(processedData);  // HTML encode

            // Url encode?
            if (UrlEncode == true)
                processedData = Server.UrlEncode(processedData);  // Url encode

            // Regular expression replacement?
            if (!String.IsNullOrEmpty(RegExReplacePattern))
            {
                var regEx = new Regex(RegExReplacePattern);
                if (regEx != null)
                    processedData = regEx.Replace(processedData, RegExReplaceReplacement);
            }

            // Use BBCode formatting?
            if (EnableBbCodes)
                processedData = new BbCode().Format(processedData);

            return processedData;
        }

    }
}
