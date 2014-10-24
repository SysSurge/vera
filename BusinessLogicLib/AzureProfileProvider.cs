using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using System.Data.Services.Client;
using System.Linq;
using System.Web.Profile;
using VeraWAF.AzureTableStorage;
using VeraWAF.WebPages.Dal;

namespace VeraWAF.WebPages.Bll
{
    /// <summary>
    /// Vera ASP.NET Profile provider.
    /// Persists all profile provider data in the Azure Table Storage.
    /// </summary>
    /// <remarks>
    /// Add your custom profile properties to AzureProfileProviderEx.
    /// </remarks>
    public class AzureProfileProvider : ProfileProvider
    {
        /// <summary>
        /// Cached user profiles.
        /// </summary>
        /// <remarks>
        /// In-memory cache.
        /// </remarks>
        UserCache _userCache;

        /// <summary>
        /// Standard profile provider property names. 
        /// </summary>
        /// <remarks>
        /// Add your custom profile properties to PropertyNamesEx.
        /// </remarks>
        public struct PropertyNames
        {
            public const string Gender = "Gender";
            public const string Birthdate = "Birthdate";
            public const string PortraitBlobAddressUri = "PortraitBlobAddressUri";
            public const string Tags = "Tags";
            public const string Country = "Country";
            public const string Industry = "Industry";
            public const string JobCategory = "JobCategory";
            public const string CompanySize = "CompanySize";
            public const string ServerOs = "ServerOs";
            public const string ClientOs = "ClientOs";
            public const string ProfileComment = "ProfileComment";
            public const string Newsletter = "Newsletter";
            public const string ClientIpAddress = "ClientIpAddress";
            public const string FullName = "FullName";
            public const string Employer = "Employer";
            public const string Description = "Description";
            public const string AuthProvider = "AuthProvider";

            /// <summary>
            /// Allow contact names
            /// </summary>
            public const string AllowContactForm = "AllowContactForm";

            /// <summary>
            /// Social score.
            /// </summary>
            public const string SocialPoints = "SocialPoints";

            /// <summary>
            /// Total number of forum posts.
            /// </summary>
            public const string NumberOfForumPosts = "NumberOfForumPosts";

            /// <summary>
            /// OAuth consumer key
            /// </summary>
            public const string OAuthConsumerKey = "OAuthConsumerKey";

            /// <summary>
            /// OAuth consumer secret
            /// </summary>
            public const string OAuthConsumerSecret = "OAuthConsumerSecret";
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            _userCache = new UserCache();

            // Initialize values from web.config.
            if (config == null) throw new ArgumentNullException("config");

            if (String.IsNullOrEmpty(name)) throw new ArgumentNullException("config");

            // Initialize the abstract base class.
            base.Initialize(name, config);

            ApplicationName = String.IsNullOrEmpty(config["applicationName"]) ? System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath : config["applicationName"];
        }

        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection collection)
        {
            var userName = (string)context["UserName"];

            var userEntity = _userCache.GetUser(userName);
            if (userEntity == null) throw new ProviderException("User not found.");

            // The serializeAs attribute is ignored in this provider implementation.
            var svc = new SettingsPropertyValueCollection();

            foreach (SettingsProperty prop in collection)
            {
                var pv = new SettingsPropertyValue(prop);

                switch (prop.Name)
                {
                    case PropertyNames.Gender:
                        pv.PropertyValue = userEntity.Gender;
                        break;
                    case PropertyNames.Birthdate:
                        pv.PropertyValue = userEntity.Birthday;
                        break;
                    case PropertyNames.PortraitBlobAddressUri:
                        pv.PropertyValue = userEntity.PortraitBlobAddressUri;
                        break;
                    case PropertyNames.Country:
                        pv.PropertyValue = userEntity.Country;
                        break;
                    case PropertyNames.Industry:
                        pv.PropertyValue = userEntity.Industry;
                        break;
                    case PropertyNames.JobCategory:
                        pv.PropertyValue = userEntity.JobCategory;
                        break;
                    case PropertyNames.CompanySize:
                        pv.PropertyValue = userEntity.CompanySize;
                        break;
                    case PropertyNames.ProfileComment:
                        pv.PropertyValue = userEntity.ProfileComment;
                        break;
                    case PropertyNames.Newsletter:
                        pv.PropertyValue = userEntity.Newsletter;
                        break;
                    case PropertyNames.ClientIpAddress:
                        pv.PropertyValue = userEntity.ClientIpAddress;
                        break;
                    case PropertyNames.Employer:
                        pv.PropertyValue = userEntity.Employer;
                        break;
                    case PropertyNames.FullName:
                        pv.PropertyValue = userEntity.FullName;
                        break;
                    case PropertyNames.Description:
                        pv.PropertyValue = userEntity.Description;
                        break;
                    case PropertyNames.AuthProvider:
                        pv.PropertyValue = userEntity.AuthProvider;
                        break;
                    case PropertyNames.AllowContactForm:
                        pv.PropertyValue = userEntity.AllowContactForm;
                        break;
                    case PropertyNames.SocialPoints:
                        pv.PropertyValue = userEntity.SocialPoints;
                        break;
                    case PropertyNames.NumberOfForumPosts:
                        pv.PropertyValue = userEntity.NumberOfForumPosts;
                        break;
                    case PropertyNames.OAuthConsumerKey:
                        pv.PropertyValue = userEntity.OAuthConsumerKey;
                        break;
                    case PropertyNames.OAuthConsumerSecret:
                        pv.PropertyValue = userEntity.OAuthConsumerSecret;
                        break;
                    default:
                        throw new ProviderException("Unsupported property.");
                }

                svc.Add(pv);
            }

            return svc;
        }

        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection)
        {
            // The serializeAs attribute is ignored in this provider implementation.
            var userName = (string)context["UserName"];

            var userEntity = _userCache.GetUser(userName);
            if (userEntity == null) throw new ProviderException("User not found.");

            // Only update the database if something has changed since this method is called quite often by the provider
            bool hasChanged = false;

            foreach (SettingsPropertyValue pv in collection)
            {
                switch (pv.Property.Name)
                {
                    case PropertyNames.Gender:
                        if (userEntity.Gender == (int)pv.PropertyValue) break;
                        hasChanged = true;
                        userEntity.Gender = (int)pv.PropertyValue;
                        break;
                    case PropertyNames.Birthdate:
                        if (userEntity.Birthday == (DateTime)pv.PropertyValue) break;
                        hasChanged = true;
                        userEntity.Birthday = (DateTime)pv.PropertyValue;
                        break;
                    case PropertyNames.PortraitBlobAddressUri:
                        if (userEntity.PortraitBlobAddressUri == (string)pv.PropertyValue) break;
                        hasChanged = true;
                        userEntity.PortraitBlobAddressUri = (string)pv.PropertyValue;
                        break;
                    case PropertyNames.Country:
                        if (userEntity.Country == (string)pv.PropertyValue) break;
                        hasChanged = true;
                        userEntity.Country = (string)pv.PropertyValue;
                        break;
                    case PropertyNames.Industry:
                        if (userEntity.Industry == (string)pv.PropertyValue) break;
                        hasChanged = true;
                        userEntity.Industry = (string)pv.PropertyValue;
                        break;
                    case PropertyNames.JobCategory:
                        if (userEntity.JobCategory == (string)pv.PropertyValue) break;
                        hasChanged = true;
                        userEntity.JobCategory = (string)pv.PropertyValue;
                        break;
                    case PropertyNames.CompanySize:
                        if (userEntity.CompanySize == (string)pv.PropertyValue) break;
                        hasChanged = true;
                        userEntity.CompanySize = (string)pv.PropertyValue;
                        break;
                    case PropertyNames.ProfileComment:
                        if (userEntity.ProfileComment == (string)pv.PropertyValue) break;
                        hasChanged = true;
                        userEntity.ProfileComment = (string)pv.PropertyValue;
                        break;
                    case PropertyNames.Newsletter:
                        if (userEntity.Newsletter == (pv.PropertyValue != null && (bool)pv.PropertyValue)) break;
                        hasChanged = true;
                        userEntity.Newsletter = pv.PropertyValue != null && (bool)pv.PropertyValue;
                        break;
                    case PropertyNames.ClientIpAddress:
                        if (userEntity.ClientIpAddress == (string)pv.PropertyValue) break;
                        hasChanged = true;
                        userEntity.ClientIpAddress = (string)pv.PropertyValue;
                        break;
                    case PropertyNames.FullName:
                        if (userEntity.FullName == (string)pv.PropertyValue) break;
                        hasChanged = true;
                        userEntity.FullName = (string)pv.PropertyValue;
                        break;
                    case PropertyNames.Employer:
                        if (userEntity.Employer == (string)pv.PropertyValue) break;
                        hasChanged = true;
                        userEntity.Employer = (string)pv.PropertyValue;
                        break;
                    case PropertyNames.Description:
                        if (userEntity.Description == (string)pv.PropertyValue) break;
                        hasChanged = true;
                        userEntity.Description = (string)pv.PropertyValue;
                        break;
                    case PropertyNames.AuthProvider:
                        if (userEntity.AuthProvider == (string)pv.PropertyValue) break;
                        hasChanged = true;
                        userEntity.AuthProvider = (string)pv.PropertyValue;
                        break;
                    case PropertyNames.AllowContactForm:
                        if (userEntity.AllowContactForm == (bool)pv.PropertyValue) break;
                        hasChanged = true;
                        userEntity.AllowContactForm = (bool)pv.PropertyValue;
                        break;
                    case PropertyNames.SocialPoints:
                        if (userEntity.SocialPoints == (int)pv.PropertyValue) break;
                        hasChanged = true;
                        userEntity.SocialPoints = (int)pv.PropertyValue;
                        break;
                    case PropertyNames.NumberOfForumPosts:
                        if (userEntity.NumberOfForumPosts == (int)pv.PropertyValue) break;
                        hasChanged = true;
                        userEntity.NumberOfForumPosts = (int)pv.PropertyValue;
                        break;
                    case PropertyNames.OAuthConsumerKey:
                        if (userEntity.OAuthConsumerKey == (string)pv.PropertyValue) break;
                        hasChanged = true;
                        userEntity.OAuthConsumerKey = (string)pv.PropertyValue;
                        break;
                    case PropertyNames.OAuthConsumerSecret:
                        if (userEntity.OAuthConsumerSecret == (string)pv.PropertyValue) break;
                        hasChanged = true;
                        userEntity.OAuthConsumerSecret = (string)pv.PropertyValue;
                        break;
                    default:
                        throw new ProviderException("Unsupported property.");
                }
            }

            // Has the profile changed?
            if (hasChanged)
            {
                // Store when profile was updated
                userEntity.LastProfileUpdatedDate = DateTime.Now;

                _userCache.Update(userEntity);

                // Make sure that all the other cloud instances are updated to reflect the user changes
                //new Bll.CloudCommand().SendCommand("ClearUserCache", true);
            }
        }

        public override string ApplicationName { get; set; }

        bool DeleteProfile(string userName)
        {
            // TODO: Add delete profile code
            return true;
        }

        public override int DeleteProfiles(ProfileInfoCollection profiles)
        {
            return profiles.Cast<ProfileInfo>().Count(p => DeleteProfile(p.UserName));
        }

        public override int DeleteProfiles(string[] usernames)
        {
            return usernames.Count(DeleteProfile);
        }

        public override int DeleteInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
        {
            throw new NotImplementedException();
        }

        public override ProfileInfoCollection GetAllProfiles(ProfileAuthenticationOption authenticationOption, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override ProfileInfoCollection GetAllInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Verifies input parameters for page size and page index. 
        /// Called by GetAllProfiles, GetAllInactiveProfiles, 
        /// FindProfilesByUserName, and FindInactiveProfilesByUserName.
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        void CheckParameters(int pageIndex, int pageSize)
        {
            if (pageIndex < 0) throw new ArgumentException("Page index must 0 or greater.");
            if (pageSize < 1) throw new ArgumentException("Page size must be greater than 0.");
        }

        IEnumerable<UserEntity> FindUsersByName(string usernameToMatch, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords)
        {
            var userEntities = _userCache.FindInactiveUsers(usernameToMatch, userInactiveSinceDate);

            var users = new List<UserEntity>();

            try
            {
                totalRecords = userEntities.Count();
            }
            catch (DataServiceQueryException)
            {
                totalRecords = 0;
            }

            if (totalRecords == 0) return users;

            var counter = 0;
            var startIndex = pageSize * pageIndex;
            var endIndex = startIndex + pageSize - 1;

            foreach (var userEntity in userEntities)
            {
                if (counter >= startIndex) users.Add(userEntity);

                if (counter >= endIndex) break;

                counter++;
            }

            return users;
        }

        ProfileInfo GetProfileInfoFromUserEnity(UserEntity userEntity)
        {
            const bool isAnonymous = false;
            const int profileInfoSize = 0;

            // ProfileInfo.Size not currently implemented.
            return new ProfileInfo(userEntity.Username, isAnonymous, userEntity.LastActivityDate, userEntity.LastProfileUpdatedDate, profileInfoSize);
        }

        /// <summary>
        /// Retrieves a count of profiles and creates a 
        /// ProfileInfoCollection from the profile data in the 
        /// storage. 
        /// </summary>
        /// <param name="authenticationOption"></param>
        /// <param name="usernameToMatch"></param>
        /// <param name="userInactiveSinceDate"></param>
        /// <param name="pageIndex">Specifying a pageIndex of 0 retrieves a count of the results only.</param>
        /// <param name="pageSize"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        ProfileInfoCollection GetProfileInfo(
            ProfileAuthenticationOption authenticationOption,
            string usernameToMatch,
            object userInactiveSinceDate,
            int pageIndex,
            int pageSize,
            out int totalRecords)
        {
            if (authenticationOption == ProfileAuthenticationOption.Anonymous)
                throw new NotImplementedException("ProfileAuthenticationOption.Anonymous");

            if (userInactiveSinceDate == null) userInactiveSinceDate = DateTime.Now;

            var users = FindUsersByName(usernameToMatch, (DateTime)userInactiveSinceDate, pageIndex, pageSize, out totalRecords);

            var profiles = new ProfileInfoCollection();

            // No profiles found.
            if (totalRecords <= 0) { return profiles; }

            // Count profiles only.
            if (pageSize == 0) { return profiles; }

            foreach (var userEntity in users)
                profiles.Add(GetProfileInfoFromUserEnity(userEntity));

            return profiles;
        }

        public override ProfileInfoCollection FindProfilesByUserName(ProfileAuthenticationOption authenticationOption, string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            CheckParameters(pageIndex, pageSize);

            return GetProfileInfo(authenticationOption, usernameToMatch, null, pageIndex, pageSize, out totalRecords);
        }

        public override ProfileInfoCollection FindInactiveProfilesByUserName(ProfileAuthenticationOption authenticationOption, string usernameToMatch, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords)
        {
            CheckParameters(pageIndex, pageSize);

            return GetProfileInfo(authenticationOption, usernameToMatch, userInactiveSinceDate, pageIndex, pageSize, out totalRecords);
        }
    }
}