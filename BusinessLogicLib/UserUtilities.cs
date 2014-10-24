using System;
using System.Web;
using System.Web.Profile;
using System.Web.Security;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using VeraWAF.WebPages.Bll.Cloud;

namespace VeraWAF.WebPages.Bll {
    public class UserUtilities {
        CloudBlobClient GetBlobClient() {
            var storageAccount = CloudStorageAccount.FromConfigurationSetting("DataConnectionString");
            return storageAccount.CreateCloudBlobClient();
        }

        CloudBlobContainer GetBlobContainer() {
            return GetBlobClient().GetContainerReference("publicfiles");
        }

        public string GetPortraitUrl(ProfileBase currentUserProfile) {

            string userPortraitUrl;
            var portraitFileName = (string)currentUserProfile.GetPropertyValue("PortraitBlobAddressUri");
            if (String.IsNullOrWhiteSpace(portraitFileName))
            {
                const string defaultPortrait = "/Images/none.png";
                userPortraitUrl = defaultPortrait;
            } else if (portraitFileName.Contains(":"))
                userPortraitUrl = portraitFileName;
            else
            {
                var blob = GetBlobContainer().GetBlobReference(portraitFileName);
                userPortraitUrl = new CdnUtilities().GetCdnUrl(blob.Uri.ToString());
            }

            return HttpUtility.HtmlEncode(userPortraitUrl);
        }

        public string GetPortraitUrl(string userName)
        {
            var profile = ProfileBase.Create(userName);
            return GetPortraitUrl(profile);
        }

        public string GetPortraitUrl()
        {
            var currentUserProfile = HttpContext.Current.Profile;
            return GetPortraitUrl(currentUserProfile);
        }

        public string GetDisplayName(MembershipUser user, ProfileBase profile) {
            string displayName;

            if (user == null) displayName = null;
            else {
                var fullName = profile.GetPropertyValue("FullName") as string;
                displayName = String.IsNullOrWhiteSpace(fullName) ? user.UserName : fullName;
            }

            return HttpUtility.HtmlEncode(displayName);
        }

        public string GetDisplayName(MembershipUser user)
        {
            var profile = ProfileBase.Create(user.UserName);
            var fullName = profile.GetPropertyValue("FullName") as string;

            if (fullName == null || String.IsNullOrWhiteSpace(fullName))
                fullName = user.UserName;
            else
                fullName = String.Format("{0}({1})", fullName, user.UserName);

            fullName = fullName.Trim();

            // Make sure the user name is not too long as it could ruin the layout
            const int minUserNameLength = 4;
            const int maxUserNameLength = 50;

            if (fullName.Length > maxUserNameLength)
                fullName = fullName.Substring(0, maxUserNameLength);
            else if (fullName.Length < minUserNameLength)
                fullName = user.UserName;

            return fullName;
        }

        public string GetDisplayName(string userName)
        {
            var user = Membership.GetUser(userName);
            var fullName = user == null ? null : GetDisplayName(user);
            return fullName;
        }

        public string GetDisplayName()
        {
            var currentUser = Membership.GetUser();
            var fullName = currentUser == null ? null : GetDisplayName(currentUser);
            return fullName;
        }

    }
}