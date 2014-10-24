using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.Profile;
using System.Web.Security;
using System.Web.UI;
using VeraWAF.AzureTableStorage;
using VeraWAF.Core.Templates;
using VeraWAF.CrossCuttingConcerns;
using VeraWAF.WebPages;
using VeraWAF.WebPages.Bll;
using VeraWAF.WebPages.Dal;

namespace VeraWAF.WebPages.Controls {

    /// <summary>
    /// Allows the user to comment a page
    /// </summary>
    public partial class Comments : UserControl
    {
        /// <summary>
        /// Vera base page 
        /// </summary>
        PageTemplateBase _page;

        /// <summary>
        /// Set to true to allow voting by other users on the comment.
        /// </summary>
        public bool AllowVoting { get; set; }

        /// <summary>
        /// Set to true to show the signature of the person who made the comment
        /// </summary>
        public bool ShowSignature { get; set; }

        /// <summary>
        /// Set to true to show some metadata about the person who made the comment
        /// </summary>
        public bool ShowUserMetaData { get; set; }

        /// <summary>
        /// Application name
        /// </summary>
        private readonly string _applicationName;

        /// <summary>
        /// Virtual path
        /// </summary>
        private string _virtualPath;

        /// <summary>
        /// Virtual files
        /// </summary>
        public string VirtualFiles { get; set; }

        /// <summary>
        /// Class constructor
        /// </summary>
        public Comments()
        {
            _applicationName = ConfigurationManager.AppSettings["ApplicationName"];
        }

        /// <summary>
        /// Shows all the comment about the page
        /// </summary>
        /// <param name="comments">List of comments</param>
        void ShowComments(IEnumerable<PageEntity> comments)
        {
            var bbCodes = new BbCode();
            var userUtilities = new UserUtilities();

            foreach (var comment in comments)
            {
                var user = Membership.GetUser(comment.Author);
                if (user != null)
                {
                    var displayName = HttpUtility.HtmlEncode(userUtilities.GetDisplayName(user));
                    var profile = ProfileBase.Create(user.UserName, true);

                    var portraitUrl = userUtilities.GetPortraitUrl(profile);

                    var commentUserControl = (Comment)LoadControl("~/Controls/Comment.ascx");

                    commentUserControl.Timestamp = comment.Timestamp;
                    commentUserControl.ProviderUserKey = user.ProviderUserKey;
                    commentUserControl.UserDisplayName = Server.HtmlEncode(displayName);
                    commentUserControl.RowKey = comment.RowKey;
                    commentUserControl.VirtualPath = _virtualPath;
                    commentUserControl.PortraitUrl = portraitUrl;
                    commentUserControl.MainContent = bbCodes.Format(HttpUtility.HtmlEncode(comment.MainContent));
                    commentUserControl.PartitionKey = comment.PartitionKey;
                    commentUserControl.AllowVoting = AllowVoting;
                    commentUserControl.UserSocialScore = (int)profile.GetPropertyValue("SocialPoints");
                    commentUserControl.UserJoinedDate = user.CreationDate;
                    commentUserControl.UserLastActiveDate = user.LastActivityDate;
                    commentUserControl.ShowUserMetaData = ShowUserMetaData;
                    commentUserControl.UserRoles = Roles.GetRolesForUser(user.UserName);

                    if (ShowSignature)
                    {
                        var signature = profile.GetPropertyValue("Description") as string;
                        if (!String.IsNullOrWhiteSpace(signature))
                        {
                            const int maxSignatureCharacters = 300;
                            if (signature.Length > maxSignatureCharacters)
                                signature = signature.Substring(0, maxSignatureCharacters);

                            commentUserControl.ShowSignature = ShowSignature;
                            commentUserControl.Signature = bbCodes.Format(HttpUtility.HtmlEncode(signature));                            
                        }
                    }

                    panComments.Controls.Add(commentUserControl);
                }
            }

        }

        /// <summary>
        /// Initiate comments by loading the comment entities from the page cache.
        /// A comment is stored as a page entity
        /// </summary>
        void InitComments()
        {
            var comments = new PageCache().GetCommentsByVirtualPath(_virtualPath);
            ShowComments(comments);
        }

        /// <summary>
        /// Show the sign-in if user not already signed in
        /// </summary>
        void InitSignIn()
        {
            var isLoggedIn = Membership.GetUser() != null;
            signInToCommentContainer.Visible = !isLoggedIn;
            lnkSignIn.HRef = String.Format("/Account/Login.aspx?ReturnUrl={0}", HttpUtility.UrlEncode(Request.Url.PathAndQuery));
        }

        /// <summary>
        /// Initiate the user controls
        /// </summary>
        void InitControls()
        {
            InitComments();
            InitSignIn();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Get the parent page
            _page = Page as PageTemplateBase;
            if (_page != null)
            {
                // Get page entity data
                var pageEntity = _page.GetPageEntity();
                if (pageEntity != null)
                {
                    // Does the page entity data say anything about showing the control?
                    Visible = pageEntity.AllowForComments;
                }
            }

            _virtualPath = Request.Url.AbsolutePath;

            InitControls();
        }

        /// <summary>
        /// Creates a new page entity from the controls on the form
        /// </summary>
        /// <param name="parent">Parent page entity; this is the page the comments are "attached" to</param>
        /// <returns></returns>
        PageEntity CreatePageEntityFromCommentForm(PageEntity parent)
        {
            return new PageEntity {
                PartitionKey = parent.PartitionKey,
                RowKey = DateTime.UtcNow.Ticks.ToString(CultureInfo.InvariantCulture),
                ApplicationName = _applicationName,
                MainContent = txtComment.Text,
                VirtualPath = _virtualPath,
                IsPublished = true,
                MenuItemSortOrder = 0,
                GeolocationLat = 0.0,
                GeolocationLong = 0.0,
                GeolocationZoom = 10,
                Author = Membership.GetUser().UserName,
                ShowInMenu = false,
                ParentRowKey = parent.RowKey
            };
        }

        /// <summary>
        /// Update virtual file cache to reflect the changes for the sitemap provider
        /// </summary>
        /// <param name="page"></param>
        void UpdateVirtualFileCacheDependency(PageEntity page) {
            var storagePath = RoleEnvironment.GetLocalResource(VirtualFiles).RootPath;
            var fileName = storagePath + page.PartitionKey;

            if (!File.Exists(fileName)) {
                var file = new StreamWriter(fileName);
                file.Close();
            } else File.SetLastWriteTimeUtc(fileName, DateTime.UtcNow);
        }

        /// <summary>
        /// Save the comment. All fields are stored as a new page entity in the database.
        /// </summary>
        /// <returns>New page entity for the comment.</returns>
        PageEntity SaveComment() {
            var datasource = new AzureTableStorageDataSource();
            var partitionKey = new StringUtilities().ConvertToHex(_virtualPath);
            var parentPage = datasource.GetPage(partitionKey, _applicationName);
            var comment = CreatePageEntityFromCommentForm(parentPage);

            datasource.Insert(comment);

            return comment;
        }

        /// <summary>
        /// Called when the user submits the comment.
        /// Will store the comment in the database.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void butSumbit_Click(object sender, EventArgs e)
        {
            var maxNumberOfCommentsPerHour = long.Parse(ConfigurationManager.AppSettings["MaxNumberOfCommentsPerHour"]);
            if (new HammerProtection(maxNumberOfCommentsPerHour, HammeringMode.Hours,
                HammerTypes.CommentsHammering).HostIsHammering(new ServerTools().GetClientIpAddress())) {

                // The user is hammering, so don't waste any more processing resources on him
                HttpContext.Current.Response.Redirect("/ErrorPages/CommentsHammering.aspx", true);
            }

            var comment = SaveComment();

            UpdateVirtualFileCacheDependency(comment);

            Response.Redirect(_virtualPath + "#" + comment.RowKey, true);
        }

        /// <summary>
        /// Initiate the form menu.
        /// </summary>
        void InitMenu() {
            var currentUser = Membership.GetUser();

            if (currentUser != null) {
                panMenu.Visible = true;

                var userutilities = new UserUtilities();

                imgPortrait.src = userutilities.GetPortraitUrl();
                litPortraitCaption.Text = userutilities.GetDisplayName(currentUser);

            } else panMenu.Visible = false;
        }

        protected override void OnInit(EventArgs e)
        {
            InitMenu();

            base.OnInit(e);
        }
    }
}