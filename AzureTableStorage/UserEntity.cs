using System;
using Microsoft.WindowsAzure.StorageClient;

namespace VeraWAF.AzureTableStorage
{
    public class UserEntity : AzureEntityBase
    {
       
        public UserEntity(string partitionKey, string rowKey) : base(partitionKey, rowKey)
        {
        }

        public UserEntity() : this(Guid.NewGuid().ToString(), String.Empty)
        {
        }

        #region Membership provider properties
        public String Username { get; set; }
        public String Password { get; set; }
        public String Email { get; set; }
        public String PasswordQuestion { get; set; }
        public String PasswordAnswer { get; set; }
        public bool IsApproved { get; set; }
        public String Comment { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastLoginDate { get; set; }
        public DateTime LastPasswordChangedDate { get; set; }
        public DateTime LastActivityDate { get; set; }
        public bool IsLockedOut { get; set; }
        public DateTime LastLockedOutDate { get; set; }
        public int FailedPasswordAttemptCount { get; set; }
        public DateTime FailedPasswordAttemptWindowStart { get; set; }
        public int FailedPasswordAnswerAttemptCount { get; set; }
        public DateTime FailedPasswordAnswerAttemptWindowStart { get; set; }
        public bool IsDeleted { get; set; }
        #endregion

        #region Profile provider properties
        public int Gender { get; set; }
        public DateTime LastProfileUpdatedDate { get; set; }
        public DateTime Birthday { get; set; }
        public string PortraitBlobAddressUri { get; set; }
        public string Country { get; set; }
        public string Industry { get; set; }

        public string JobCategory { get; set; }
        public string CompanySize { get; set; }
        public string ProfileComment { get; set; }

        public bool Newsletter { get; set; }

        public string ClientIpAddress { get; set; }

        public string FullName { get; set; }
        public string Employer { get; set; }
        public string Description { get; set; }

        public string AuthProvider { get; set; }

        public bool AllowContactForm { get; set; }
        public int SocialPoints { get; set; }

        public int NumberOfForumPosts { get; set; }

        /// <summary>
        /// OAuth consumer key.
        /// </summary>
        public string OAuthConsumerKey { get; set; }

        /// <summary>
        /// OAuth consumer secret.
        /// </summary>
        public string OAuthConsumerSecret { get; set; }

        #endregion
    }
}
