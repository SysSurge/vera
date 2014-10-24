using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Profile;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using VeraWAF.AzureTableStorage;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using VeraWAF.Core.Templates;
using VeraWAF.WebPages.Bll;
using VeraWAF.WebPages.Bll.Cloud;
using VeraWAF.WebPages.Dal;

namespace VeraWAF.WebPages.Account
{
    public partial class UserProfile : PageTemplateBase
    {
        const int PostListPageSize = 10;
        int _totalNumberOfPostsInPostList;
        int _totalPagesInPostList;
        int _currentPageInPostList = 1;

        const int PostListPageSize2 = 10;
        int _totalNumberOfPostsInPostList2;
        int _totalPagesInPostList2;
        int _currentPageInPostList2 = 1;

        const int PostListPageSize3 = 10;
        int _totalNumberOfPostsInPostList3;
        int _totalPagesInPostList3;
        int _currentPageInPostList3 = 1;

        readonly PageCache _pageCache;
        readonly UserUtilities _userUtilities;
        readonly DateUtilities _dateUtilities;

        string _profileUserId;
        MembershipUser _currentUser;
        ProfileBase _profile;

        CloudBlobClient _blobClient;
        CloudBlobContainer _blobContainer;

        public UserProfile()
        {
            _pageCache = new PageCache();
            _userUtilities = new UserUtilities();
            _dateUtilities = new DateUtilities();
        }

        void ShowControls(bool isCurrentUser)
        {
            PrivateGender.Visible = isCurrentUser;
            PublicGender.Visible = !isCurrentUser;
            UploadPortrait.Visible = isCurrentUser;
            SubmitButton.Visible = isCurrentUser;
            fsPortrait.Visible = isCurrentUser;
            FullName.Visible = isCurrentUser;
            FullNameLiteral.Visible = !isCurrentUser;
            Employer.Visible = isCurrentUser;
            Description.Visible = isCurrentUser;
            EmailLabel.Visible = isCurrentUser;
            Email.Visible = isCurrentUser;
            imgPortrait.Visible = !isCurrentUser;
            divUserOptions.Visible = isCurrentUser;
            ContactUser1.Visible = (bool)_profile.GetPropertyValue("AllowContactForm") && !isCurrentUser;

            // Do not show the OAuth fieldset unless the user has a consumer key
            fsOauth.Visible = isCurrentUser && !String.IsNullOrEmpty(_profile.GetPropertyValue("OAuthConsumerKey") as string);
        }

        ProfileBase GetProfile(object providerUserKey, MembershipUser currentUser, out bool isCurrentUser)
        {
            ProfileBase profile = null;

            if (currentUser != null && (providerUserKey == currentUser.ProviderUserKey || providerUserKey == null))
            {
                profile = HttpContext.Current.Profile;
                isCurrentUser = true;
            }
            else
            {
                isCurrentUser = false;

                if (providerUserKey != null)
                {
                    var user = Membership.GetUser(providerUserKey);
                    if (user != null)
                        profile = ProfileBase.Create(user.UserName, true); 
                }
            }

            return profile;
        }

        string GetGender()
        {
            string gender;

            switch ((int)_profile.GetPropertyValue("Gender"))
            {
                case -1:
                    gender = Bll.Resources.Profile.NotSet;
                    break;
                case 0:
                    gender = Bll.Resources.Profile.GenderFemale;
                    break;
                case 1:
                    gender = Bll.Resources.Profile.GenderMale;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(Bll.Resources.Profile.UnknownGenderError);
            }

            return gender;
        }

        void PopulateGenderDropDown()
        {
            var genderVal = (int)_profile.GetPropertyValue("Gender");
            PrivateGender.Items.Add(
                new ListItem(Bll.Resources.Profile.NotSet, "-1") { Selected = (genderVal == -1) }
                );
            PrivateGender.Items.Add(
                new ListItem(Bll.Resources.Profile.GenderFemale, "0") { Selected = (genderVal == 0) }
                );
            PrivateGender.Items.Add(
                new ListItem(Bll.Resources.Profile.GenderMale, "1") { Selected = (genderVal == 1) }
                );
        }

        void GetPortrait()
        {
            var blobAddressUri = (string)_profile.GetPropertyValue("PortraitBlobAddressUri");

            if (String.IsNullOrEmpty(blobAddressUri)) return;

            string imageUrl;
            string alt;

            if (blobAddressUri.Contains(":"))
            {
                imageUrl = blobAddressUri;
                alt = "Portrait";
            }
            else
            {
                var blob = _blobContainer.GetBlobReference(blobAddressUri);
                imageUrl = blob.Uri.ToString();
                alt = blob.Metadata["Description"];
            }

            imgPortrait.Src = PortraitImage.ImageUrl = new CdnUtilities().GetCdnUrl(imageUrl);
            imgPortrait.Alt = PortraitImage.AlternateText = alt;
        }

        void InitControls(bool isCurrentUser)
        {
            ShowControls(isCurrentUser);

            var user = Membership.GetUser(_profile.UserName);

            var email = user.Email;
            var fullName = (string)_profile.GetPropertyValue("FullName");
            pUserName.Visible = String.IsNullOrWhiteSpace(fullName);

            if (isCurrentUser)
            {
                PopulateGenderDropDown();
                
                FullName.Text = (string)_profile.GetPropertyValue("FullName");
                Employer.Text = (string)_profile.GetPropertyValue("Employer");
                Description.Text = (string)_profile.GetPropertyValue("Description");
                Email.Text = email;
                Newsletter.Checked = (bool)_profile.GetPropertyValue("Newsletter");
                AllowContactForm.Checked = (bool)_profile.GetPropertyValue("AllowContactForm");
            }
            else
            {
                FullNameLiteral.Text = HttpUtility.HtmlEncode(fullName);
                PublicGender.Text = GetGender();
            }

            GetPortrait();
        }

        void InitBlob()
        {
            // Setup the connection to Windows Azure Storage
            var storageAccount = CloudStorageAccount.FromConfigurationSetting("DataConnectionString");
            _blobClient = storageAccount.CreateCloudBlobClient();

            // Get and create the container
            _blobContainer = _blobClient.GetContainerReference("publicfiles");
        }

        protected void SubmitButton_Click(Object sender, EventArgs eventArgs)
        {
            var genderVal = Int32.Parse(PrivateGender.SelectedValue);
            _profile.SetPropertyValue("Gender", genderVal);

            _profile.SetPropertyValue("FullName", FullName.Text);
            _profile.SetPropertyValue("Employer", Employer.Text);
            _profile.SetPropertyValue("Description", Description.Text);

            _profile.SetPropertyValue("Newsletter", Newsletter.Checked);
            _profile.SetPropertyValue("AllowContactForm", AllowContactForm.Checked);

            _profile.Save();

            //_currentUser.Email = Email.Text;
            Membership.UpdateUser(_currentUser);

            // Update all the other cloud instances by flushing their user caches
            new CloudCommandClient().SendCommand("ClearUserCache", true);
        }

        protected void PortraitSubmitButton_Click(object sender, EventArgs e)
        {
            if (_profile == null || String.IsNullOrEmpty(PortraitUploadControl.FileName)) return;

            var storageAccount = CloudStorageAccount.FromConfigurationSetting("DataConnectionString");
            _blobClient = storageAccount.CreateCloudBlobClient();

            // Make a unique blob name
            var extension = Path.GetExtension(PortraitUploadControl.FileName);

            // Create the Blob and upload the file
            var blobAddressUri = String.Format( "{0}{1}" , Guid.NewGuid(), extension);
            var blob = _blobContainer.GetBlobReference(blobAddressUri);

            var graphics = new GraphicUtilities();
            blob.UploadFromStream(graphics.CreateThumbnail(PortraitUploadControl.FileContent));

            // Update the profile with the new blob address Uri
            _profile.SetPropertyValue("PortraitBlobAddressUri", new CdnUtilities().GetCdnUrl(blobAddressUri) );

            // Set the metadata into the blob
            var userName = _currentUser.UserName;
            blob.Metadata["FileName"] = PortraitUploadControl.FileName;
            blob.Metadata["Submitter"] = userName;
            blob.Metadata["Type"] = "Portrait";
            blob.Metadata["Width"] = "64px";
            blob.Metadata["Height"] = "64px";
            blob.Metadata["Description"] = String.Format(Bll.Resources.Template.PortraitOf, userName);
            blob.SetMetadata();

            // Set the properties
            blob.Properties.ContentType = PortraitUploadControl.PostedFile.ContentType;
            blob.SetProperties();

            // Show the portrait
            GetPortrait();
        }

        #region Latest posts
        void GetPages(string author) {
            var allContentPages = new List<PageEntity>(_pageCache.GetPagesByAuthor(author).Where(page => page.IsPublished
                        && page.Template == "ForumPage.aspx"));

            _totalNumberOfPostsInPostList = allContentPages.Count;
            _totalPagesInPostList = ((_totalNumberOfPostsInPostList - 1) / PostListPageSize) + 1;

            // Ensure that we do not navigate past the last page of users.
            if (_currentPageInPostList > _totalPagesInPostList) {
                _currentPageInPostList = _totalPagesInPostList;
                GetPages(_profile.UserName);
                return;
            }

            ContentPagesGrid.DataSource = allContentPages.Skip((_currentPageInPostList - 1) * PostListPageSize).Take(PostListPageSize).OrderByDescending(comment => comment.Timestamp);
            ContentPagesGrid.CurrentPageIndex = _currentPageInPostList;
            ContentPagesGrid.DataBind();
            CurrentPageLabel.Text = _currentPageInPostList.ToString(CultureInfo.InvariantCulture);
            TotalPagesLabel.Text = _totalPagesInPostList.ToString(CultureInfo.InvariantCulture);

            NextButton.Visible = _currentPageInPostList != _totalPagesInPostList;

            PreviousButton.Visible = _currentPageInPostList != 1;

            NavigationPanel.Visible = _totalNumberOfPostsInPostList > 0;
        }

        public void NextButton_OnClick(object sender, EventArgs args) {
            _currentPageInPostList = Convert.ToInt32(CurrentPageLabel.Text);
            _currentPageInPostList++;
            GetPages(_profile.UserName);
        }

        public void PreviousButton_OnClick(object sender, EventArgs args) {
            _currentPageInPostList = Convert.ToInt32(CurrentPageLabel.Text);
            _currentPageInPostList--;
            GetPages(_profile.UserName);
        }
        #endregion

        #region Latest replies
        void GetPages2(string author) {
            var allContentPages = new List<PageEntity>(_pageCache.GetPagesByAuthor(author).Where(page => page.IsPublished
                        && String.IsNullOrWhiteSpace(page.Template) && !String.IsNullOrWhiteSpace(page.ParentRowKey) ));

            _totalNumberOfPostsInPostList2 = allContentPages.Count;
            _totalPagesInPostList2 = ((_totalNumberOfPostsInPostList2 - 1) / PostListPageSize2) + 1;

            // Ensure that we do not navigate past the last page of users.
            if (_currentPageInPostList2 > _totalPagesInPostList2) {
                _currentPageInPostList2 = _totalPagesInPostList2;
                GetPages2(_profile.UserName);
                return;
            }

            ContentPagesGrid2.DataSource = allContentPages.Skip((_currentPageInPostList2 - 1) * PostListPageSize2).Take(PostListPageSize2).OrderByDescending(comment => comment.Timestamp);
            ContentPagesGrid2.CurrentPageIndex = _currentPageInPostList2;
            ContentPagesGrid2.DataBind();
            CurrentPageLabel2.Text = _currentPageInPostList2.ToString(CultureInfo.InvariantCulture);
            TotalPagesLabel2.Text = _totalPagesInPostList2.ToString(CultureInfo.InvariantCulture);

            NextButton2.Visible = _currentPageInPostList2 != _totalPagesInPostList2;

            PreviousButton2.Visible = _currentPageInPostList2 != 1;

            NavigationPanel2.Visible = _totalNumberOfPostsInPostList2 > 0;
        }

        public void NextButton2_OnClick(object sender, EventArgs args) {
            _currentPageInPostList2 = Convert.ToInt32(CurrentPageLabel2.Text);
            _currentPageInPostList2++;
            GetPages2(_profile.UserName);
        }

        public void PreviousButton2_OnClick(object sender, EventArgs args) {
            _currentPageInPostList2 = Convert.ToInt32(CurrentPageLabel2.Text);
            _currentPageInPostList2--;
            GetPages2(_profile.UserName);
        }
        #endregion

        #region Favorites
        void GetPages3(string providerUserKey)
        {
            var dataSource = new AzureTableStorageDataSource();
            var allContentPages = new List<FavoriteEntity>(dataSource.GetFavoritesByUser(providerUserKey, 
                ConfigurationManager.AppSettings["ApplicationName"]));

            _totalNumberOfPostsInPostList3 = allContentPages.Count;
            _totalPagesInPostList3 = ((_totalNumberOfPostsInPostList3 - 1) / PostListPageSize3) + 1;

            // Ensure that we do not navigate past the last page of users.
            if (_currentPageInPostList3 > _totalPagesInPostList3) {
                _currentPageInPostList3 = _totalPagesInPostList3;
                GetPages3(_profileUserId);
                return;
            }

            ContentPagesGrid3.DataSource = allContentPages.Skip((_currentPageInPostList3 - 1) * 
                PostListPageSize3).Take(PostListPageSize3).OrderByDescending(comment => comment.Timestamp);
            ContentPagesGrid3.CurrentPageIndex = _currentPageInPostList3;
            ContentPagesGrid3.DataBind();
            CurrentPageLabel3.Text = _currentPageInPostList3.ToString(CultureInfo.InvariantCulture);
            TotalPagesLabel3.Text = _totalPagesInPostList3.ToString(CultureInfo.InvariantCulture);

            NextButton3.Visible = _currentPageInPostList3 != _totalPagesInPostList3;

            PreviousButton3.Visible = _currentPageInPostList3 != 1;

            NavigationPanel3.Visible = _totalNumberOfPostsInPostList3 > 0;
        }

        public void NextButton3_OnClick(object sender, EventArgs args) {
            _currentPageInPostList3 = Convert.ToInt32(CurrentPageLabel3.Text);
            _currentPageInPostList3++;
            GetPages3(_profileUserId);
        }

        public void PreviousButton3_OnClick(object sender, EventArgs args) {
            _currentPageInPostList3 = Convert.ToInt32(CurrentPageLabel3.Text);
            _currentPageInPostList3--;
            GetPages3(_profileUserId);
        }
        #endregion

        /// <summary>
        /// Set the company names on some of the controls
        /// </summary>
        void SetCompanyNames()
        {
            var companyName = ConfigurationManager.AppSettings["companyName"];

            litCompanyName1.Text = companyName;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            _profileUserId = Request["id"];
        }

        protected void Page_Load(object sender, EventArgs e) {
            bool isCurrentUser;

            InitBlob();

            _currentUser = Membership.GetUser();
            _profile = GetProfile(_profileUserId, _currentUser, out isCurrentUser);

            if (_profile == null || _currentUser == null) {
                // Profile was not found or user not authenticated
                FormsAuthentication.RedirectToLoginPage();
            }

            if (_profile == null) return;

            if (isCurrentUser) 
            {
                // Show the OAuth fieldset
                fsOauth.Visible = true;

                // Hide the OAuth secret to prevent accidentally showing it
                if (!IsPostBack) OAuthConsumerSecret.Visible = false;

                // Add a link to the public profile page
                lnkPublicProfile.Visible = true;
                _profileUserId = (string)_currentUser.ProviderUserKey.ToString();
                lnkPublicProfile.NavigateUrl = "/Account/?id=" + _currentUser.ProviderUserKey;

            } 
            else 
            {
                var profileUser = Membership.GetUser(_profile.UserName);
                ContactUser1.ToUserEmail = profileUser.Email;

                var profileUserDisplayName = new UserUtilities().GetDisplayName(profileUser, _profile);
                ContactUser1.Legend = String.Format(Bll.Resources.Template.SendMessageTo, 
                    HttpUtility.HtmlEncode(profileUserDisplayName));
            }

            if (!IsPostBack)
            {
                InitControls(isCurrentUser);
                GetPages(_profile.UserName);
                GetPages2(_profile.UserName);
                GetPages3(_profileUserId);

            }

            SetCompanyNames();
        }

        private DateTime GetArticlePublishedDate(PageEntity page) {
            return new DateTime(long.Parse(page.RowKey));
        }

        public int GetNumberOfVotes(string virtualPath)
        {
            var sum = 0;
            
            try
            {
                sum = _pageCache.GetAllPages().Where(page => page.VirtualPath == virtualPath).Sum(
                    page => new VoteCache().GetVotes(page.PartitionKey + page.RowKey).Values.Sum(vote => vote.Value));
            }
            catch (Exception)
            {
                // Intentioally blank to avoid NullException from VoteCache().GetVotes()
            }

            return sum;
        }

        public int GetNumberOfFavorites(string favoriteItemId) {
            var sum = 0;

            try
            {
                sum = new FavoriteCache().GetFavorites(favoriteItemId).Values.Count;
            }
            catch (Exception)
            {
                // Intentioally blank
            }

            return sum;
        }

        public string Docals(string virtualPath) {
            if (_pageCache == null) return String.Empty;

            var page = _pageCache.GetPageByVirtualPath(virtualPath);
            if (page == null) return String.Empty;

            var author = Membership.GetUser(page.Author);
            if (author == null) return String.Empty;

            var numReplies = _pageCache.GetCommentsByVirtualPath(virtualPath).Count();
            if (numReplies == 0) return String.Empty;

            var pubDate = GetArticlePublishedDate(page);

            return String.Format(Bll.Resources.Template.ProfileDocals,
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

        public string Docals2(string partitionKey)
        {
            if (_pageCache == null) return String.Empty;
            var pageEntity = _pageCache.GetPageByPartitionKey(partitionKey);
            if (pageEntity == null) return String.Empty;
            return Docals(pageEntity.VirtualPath);
        }

        /// <summary>
        /// Called when the user wants to see his/her OAuth consumer secret
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void butShowConsumerSecret_Click(object sender, EventArgs e)
        {
            // Hide the obfuscated secret and button
            hiddenSecret.Visible = false;
            butShowConsumerSecret.Visible = false;

            // Show the actual consumer secret
            OAuthConsumerSecret.Visible = true;
        }
    }
}