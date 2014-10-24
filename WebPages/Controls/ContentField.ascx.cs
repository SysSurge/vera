using System;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using VeraWAF.AzureTableStorage;
using VeraWAF.Core.Templates;
using VeraWAF.WebPages.Bll;
using VeraWAF.WebPages.Bll.Security.OAuth;

namespace VeraWAF.WebPages.Controls
{
    /// <summary>
    /// Shows the content of an arbitrary field in the VeraPages table.
    /// </summary>
    public partial class ContentField : UserControl
    {
        PageTemplateBase _page;

        /// <summary>
        /// Text box display mode
        /// </summary>
        public InputBox.ETextBoxMode TextBoxMode { get; set; }

        /// <summary>
        /// Name of column in the VeraPages table to show.
        /// Default value is "PartitionKey".
        /// </summary>
        /// <remarks>
        /// Combine with Format for more output control.
        /// </remarks>
        public string PropertyName { get; set; }

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
        /// Set to true to allow for BBCodes.
        /// Default value is false.
        /// </summary>
        public bool EnableBbCodes { get; set; }

        /// <summary>
        /// Control width when the field control is in editing mode
        /// </summary>
        public string EditWidth { get; set; }

        /// <summary>
        /// Control height when the field control is in editing mode
        /// </summary>
        public string EditHeight { get; set; }

        /// <summary>
        /// Help text in edit mode.
        /// </summary>
        public string EditHelpText { get; set; }

        /// <summary>
        /// Class constructor
        /// </summary>
        public ContentField()
        {
            // Turn encoding off by default
            HtmlEncode = false;
            UrlEncode = false;

            PropertyName = "PartitionKey";
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


        string GetPageHeaderText()
        {
            // Get the help text
            return String.IsNullOrEmpty(EditHelpText) ? Bll.Resources.Forms.DefaultFieldEditHelpText : EditHelpText;
        }

        /// <summary>
        /// Get the REST API Url for updating pages
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns></returns>
        Uri GetPageUpdateApiUrl(string username)
        {
            // Create a OAuth url
            var baseUri = String.Format("{0}://{1}", Request.Url.Scheme, Request.Headers["Host"]);
            return new Uri(String.Format("{0}/Interfaces/RestApi.svc/Update/username/{1}",
                baseUri, username));
        }

        /// <summary>
        /// Create a OAuth signature for editing the page using the REST APIs
        /// </summary>
        string CreateOAuthSignature(UserEntity userEntity, ref string url, ref string param, string httpMethod)
        {
            if (userEntity == null) return String.Empty; // Not signed in

            var consumerKey = userEntity.OAuthConsumerKey;
            var consumerSecret = userEntity.OAuthConsumerSecret;

            var uri = new Uri(url);

            return new OAuthUtils().GetSignature(consumerKey, consumerSecret, uri, httpMethod, out url, out param);
        }

        /// <summary>
        /// Enable control editing if the page is in editing mode
        /// </summary>
        void SetViewMode(PageEntity pageEntity, string rawMarkup)
        {
            // Is this page in edit mode? If so then we add a OAuth signature to the HTML element
            int editPageMode;
            if (int.TryParse(Request["Edit"], out editPageMode) && (EPageEditMode)editPageMode != EPageEditMode.None)
            {
                litMarkup.Visible = false;
                inputBox.Visible = true;

                // Set the size of the input box
                inputBox.Width = EditWidth;
                inputBox.Height = EditHeight;

                // Add current markup to control
                inputBox.Text = rawMarkup;

                // Set the field help text 
                inputBox.HelpText = GetPageHeaderText();

                // Get the URL to the update REST API
                var user = _page.GetCurrentUserMembership();
                if (user != null)
                {
                    // Get the update page REST API url
                    var apiOAuthUrl = GetPageUpdateApiUrl(user.UserName);
                    inputBox.UpdateUrl = apiOAuthUrl;

                    // Get the OAuth signature and parameters for the update page REST API url
                    string url = apiOAuthUrl.ToString();
                    string param = String.Empty;
                    var userEntity = _page.GetCurrentUserMembershipEntity();
                    inputBox.UpdateOAuthSignature = CreateOAuthSignature(userEntity, ref url, ref param, "POST");
                    inputBox.UpdateOAuthParameters = param;
                }

                inputBox.TextBoxMode = TextBoxMode;
            }
            else
            {
                // Not in edit mode

                // Make sure we encode the output if so requested
                litMarkup.Text = ProcessFieldData(rawMarkup);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _page = Page as PageTemplateBase;
            if (_page == null)
            {
                litMarkup.Text = "Error: The page does not inherit from the VeraWAF.Core.Templates.PageTemplateBase page template.";
                return;
            }

            var pageEntity = _page.GetPageEntity();
            if (pageEntity == null) return;

            string rawMarkup;

            try
            {
                var myType = typeof(PageEntity);

                // Get the PropertyInfo object by passing the property name.
                var myPropInfo = myType.GetProperty(PropertyName);

                var pageFieldData = myPropInfo.GetValue(pageEntity);
                if (pageFieldData == null)
                {
                    // Not found
                    litMarkup.Text = String.Empty;
                    return;
                }

                // If format is supplied then we don't try to convert the type
                if (String.IsNullOrEmpty(Format))
                {
                    try
                    {
                        // Try to convert the type
                        rawMarkup = Convert.ChangeType(pageFieldData, typeof(string)) as string;
                    }
                    catch (InvalidCastException)
                    {
                        litMarkup.Text = String.Format("Error: Cannot convert a \"{0}\" to a System.String.", myPropInfo.PropertyType.FullName);
                        return;
                    }
                }
                else
                {
                    rawMarkup = String.Format(Format, pageFieldData);
                }
            }
            catch (Exception)
            {
                rawMarkup = String.Format("Error: Page property \"{0}\" not found.", PropertyName);
            }

            // Set the AzureVeraPages table content page partition key
            inputBox.PartitionKey = pageEntity.PartitionKey;

            // Set the Azure table proeprty name
            inputBox.PropertyName = PropertyName;

            // Set the correct control view mode
            SetViewMode(pageEntity, rawMarkup);
        }
    }
}