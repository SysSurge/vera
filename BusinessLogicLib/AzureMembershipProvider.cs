using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using System.Data.Services.Client;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Configuration;
using System.Web.Security;
using VeraWAF.AzureTableStorage;
using VeraWAF.WebPages.Bll.Cloud;
using VeraWAF.WebPages.Dal;

namespace VeraWAF.WebPages.Bll
{
    /// <summary>
    /// Custom ASP.NET membership provider that uses the Azure Table Store to store membership data in a cloud.
    /// The code is based on a MSDN code example that used ODBC, see http://bit.ly/hiLtaw for the ODBC sample.
    /// Use at your own risk, the code was not made for a production environment but for sharing.
    /// Sample has same license as the MSDN sample has.
    /// </summary>
    public class AzureMembershipProvider : MembershipProvider
    {
        private UserCache _userCache;

        MachineKeySection machineKey;

        // Minimun password length
        int minRequiredPasswordLength = 6;

        // Minium non-alphanumeric char required
        int minRequiredNonAlphanumericCharacters = 0;

        // Enable - disable password retrieval
        bool enablePasswordRetrieval;

        // Enable - disable password reseting
        bool enablePasswordReset;

        /// Require security question and answer (this, for instance, is a functionality which not many people use)
        bool requiresQuestionAndAnswer;

        /// Application name
        string applicationName;

        // Max number of failed password attempts before the account is blocked, and time to reset that counter
        int maxInvalidPasswordAttempts;
        int passwordAttemptWindow;

        // Require email to be unique 
        bool requiresUniqueEmail;

        // Regular expression the password should match (empty for none)
        string passwordStrengthRegularExpression;

        MembershipPasswordFormat passwordFormat;

        /// <summary>
        /// A helper function to retrieve config values from the configuration file
        /// </summary>
        /// <param name="configValue"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        static string GetConfigValue(string configValue, string defaultValue)
        {
            return String.IsNullOrEmpty(configValue) ? defaultValue : configValue;
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            _userCache = new UserCache();

            // Initialize values from web.config.
            if (config == null) throw new ArgumentNullException("config");

            if (String.IsNullOrEmpty(name)) name = "CustomMembershipProvider";

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Custom Membership provider");
            }

            // Initialize the abstract base class.
            base.Initialize(name, config);

            #region Set membership settings
            applicationName = GetConfigValue(config["applicationName"], System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
            maxInvalidPasswordAttempts = Convert.ToInt32(GetConfigValue(config["maxInvalidPasswordAttempts"], "5"));
            passwordAttemptWindow = Convert.ToInt32(GetConfigValue(config["passwordAttemptWindow"], "10"));
            minRequiredNonAlphanumericCharacters = Convert.ToInt32(GetConfigValue(config["minRequiredNonAlphanumericCharacters"], "1"));
            minRequiredPasswordLength = Convert.ToInt32(GetConfigValue(config["minRequiredPasswordLength"], "7"));
            passwordStrengthRegularExpression = Convert.ToString(GetConfigValue(config["passwordStrengthRegularExpression"], ""));
            enablePasswordReset = Convert.ToBoolean(GetConfigValue(config["enablePasswordReset"], "true"));
            enablePasswordRetrieval = Convert.ToBoolean(GetConfigValue(config["enablePasswordRetrieval"], "true"));
            requiresQuestionAndAnswer = Convert.ToBoolean(GetConfigValue(config["requiresQuestionAndAnswer"], "false"));
            requiresUniqueEmail = Convert.ToBoolean(GetConfigValue(config["requiresUniqueEmail"], "true"));
            #endregion

            #region Determine password format settings
            var tempFormat = config["passwordFormat"] ?? "Hashed";
            switch (tempFormat)
            {
                case "Hashed":
                    passwordFormat = MembershipPasswordFormat.Hashed;
                    break;
                case "Encrypted":
                    passwordFormat = MembershipPasswordFormat.Encrypted;
                    break;
                case "Clear":
                    passwordFormat = MembershipPasswordFormat.Clear;
                    break;
                default:
                    throw new ProviderException("Password format not supported.");
            }
            #endregion

            // Get encryption and decryption key information from the configuration.
            var cfg = WebConfigurationManager.OpenWebConfiguration(System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
            machineKey = (MachineKeySection)cfg.GetSection("system.web/machineKey");

            if (machineKey.ValidationKey.Contains("AutoGenerate"))
                if (PasswordFormat != MembershipPasswordFormat.Clear)
                    throw new ProviderException("Hashed or Encrypted passwords are not supported with auto-generated keys.");
        }

        public override string ApplicationName
        {
            get { return applicationName; }
            set { applicationName = value; }
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            if (!ValidateUser(username, oldPassword)) return false;

            var args = new ValidatePasswordEventArgs(username, newPassword, true);

            OnValidatingPassword(args);

            if (args.Cancel)
                if (args.FailureInformation != null)
                    throw args.FailureInformation;
                else
                    throw new MembershipPasswordException("Change password canceled due to new password validation failure.");

            var userEntity = _userCache.GetUser(username);
            if (userEntity == null) return false;

            userEntity.Password = EncodePassword(newPassword);
            userEntity.LastPasswordChangedDate = userEntity.LastActivityDate = DateTime.Now;

            _userCache.Update(userEntity);

            // Make sure that all the other cloud instances are updated to reflect the user changes
            //new Bll.CloudCommand().SendCommand("ClearUserCache", true);

            return true;
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            if (!ValidateUser(username, password)) return false;

            var userEntity = _userCache.GetUser(username);
            if (userEntity == null) return false;

            userEntity.PasswordQuestion = newPasswordQuestion;
            userEntity.PasswordAnswer = newPasswordAnswer;
            userEntity.LastActivityDate = DateTime.Now;

            _userCache.Update(userEntity);

            // Make sure that all the other cloud instances are updated to reflect the user changes
            //new Bll.CloudCommand().SendCommand("ClearUserCache", true);

            return true;
        }

        /// <summary>
        /// Converts a hexadecimal string to a byte array. Used to convert encryption key values from the configuration
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        static byte[] HexToByte(string hexString)
        {
            var returnBytes = new byte[hexString.Length / 2];

            for (var i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);

            return returnBytes;
        }

        /// <summary>
        /// Encrypts, Hashes, or leaves the password clear based on the PasswordFormat
        /// TODO: wqeqewqe
        /// </summary>
        /// <param name="password">Password</param>
        /// <returns>Encoded password</returns>
        string EncodePassword(string password)
        {
            var encodedPassword = password;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    break;
                case MembershipPasswordFormat.Encrypted:
                    encodedPassword = Convert.ToBase64String(EncryptPassword(Encoding.Unicode.GetBytes(password)));
                    break;
                case MembershipPasswordFormat.Hashed:
                    /* I don't use machineKey.ValidationKey as a hash key since it changes for each port which makes it useless in an development environment
                    , but you probably should in a production environment.*/
                    var hash = new HMACSHA1 { Key = HexToByte(ConfigurationManager.AppSettings["StaticKey"]) };
                    encodedPassword = Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(password)));
                    break;
                default:
                    throw new ProviderException("Unsupported password format.");
            }

            return encodedPassword;
        }

        /// <summary>
        /// Decrypts or leaves the password clear based on the PasswordFormat
        /// </summary>
        /// <param name="encodedPassword">Password to decode</param>
        /// <returns>The decoded password</returns>
        string DecodePassword(string encodedPassword)
        {
            var password = encodedPassword;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    break;
                case MembershipPasswordFormat.Encrypted:
                    password = Encoding.Unicode.GetString(DecryptPassword(Convert.FromBase64String(password)));
                    break;
                case MembershipPasswordFormat.Hashed:
                    throw new ProviderException("Cannot decode a hashed password.");
                default:
                    throw new ProviderException("Unsupported password format.");
            }

            return password;
        }


        /// <summary>
        /// The provider key is a Guid made from the username MD5 hash 
        /// </summary>
        /// <param name="username">User name</param>
        /// <returns>Provider key</returns>
        Guid GetPartitionKeyFromUsername(string username)
        {
            var unicodeEncoding = new UnicodeEncoding();
            var message = unicodeEncoding.GetBytes(username);

            MD5 hashString = new MD5CryptoServiceProvider();

            return new Guid(hashString.ComputeHash(message));
        }

        struct DefaultValues
        {
            public const int Gender = -1;
            public const bool IsDeleted = false;
            public const bool IsLockedOut = false;
            public const bool IsApproved = false;
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            #region Validate password

            var args = new ValidatePasswordEventArgs(username, password, true);

            OnValidatingPassword(args);

            if (args.Cancel)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            if (RequiresUniqueEmail && !String.IsNullOrEmpty(GetUserNameByEmail(email)))
            {
                status = MembershipCreateStatus.DuplicateEmail;
                return null;
            }

            #endregion

            if (GetUser(username, false) != null)
            {
                status = MembershipCreateStatus.DuplicateUserName;
                return null;
            }

            var userDataSource = new AzureTableStorageDataSource();
            try
            {
                userDataSource.Insert(new UserEntity(GetPartitionKeyFromUsername(username).ToString(), String.Empty)
                {
                    Username = username,
                    Password = EncodePassword(password),
                    CreationDate = DateTime.Now,
                    IsDeleted = DefaultValues.IsDeleted,
                    IsLockedOut = DefaultValues.IsLockedOut,
                    IsApproved = DefaultValues.IsApproved,
                    ApplicationName = applicationName,
                    Email = email,
                    PasswordQuestion = passwordQuestion,
                    PasswordAnswer = passwordAnswer,
                    Gender = DefaultValues.Gender,
                    LastProfileUpdatedDate = DateTime.Now,
                    LastActivityDate = DateTime.Now,
                    LastPasswordChangedDate = DateTime.Now,
                    LastLoginDate = new DateTime(1970, 1, 1),
                    LastLockedOutDate = new DateTime(1970, 1, 1),
                    FailedPasswordAttemptWindowStart = new DateTime(1970, 1, 1),
                    FailedPasswordAnswerAttemptWindowStart = new DateTime(1970, 1, 1),
                    Birthday = new DateTime(1970, 1, 1)
                });
            }
            catch (DataServiceRequestException)
            {
                status = MembershipCreateStatus.ProviderError;
                return null;

            }

            _userCache.Clear2();

            // Assert that the user has been added to the store
            var newUser = GetUser(username, false);
            status = newUser == null ? MembershipCreateStatus.UserRejected : MembershipCreateStatus.Success;

            if (status == MembershipCreateStatus.Success)
            {
                // Make sure that all the other cloud instances are updated to reflect the user changes
                //new Bll.CloudCommand().SendCommand("ClearUserCache", true);
            }

            return newUser;
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            var userDataSource = new AzureTableStorageDataSource();
            var providerKey = GetPartitionKeyFromUsername(username);

            var userEntity = userDataSource.GetUser(providerKey.ToString(), applicationName);
            if (userEntity == null || userEntity.IsDeleted) return false;

            if (deleteAllRelatedData) userDataSource.Delete(userEntity);    // Physically delete the account
            else
            {
                // Only mark the account as deleted, don't physically delete it
                userEntity.IsDeleted = true;
                userEntity.LastActivityDate = DateTime.Now;
                userDataSource.Update(userEntity);
            }

            _userCache.Clear2();

            // Make sure that all the other cloud instances are updated to reflect the user changes
            new CloudCommandClient().SendCommand("ClearUserCache", true);

            return true;
        }

        public override bool EnablePasswordReset
        {
            get { return enablePasswordReset; }
        }

        public override bool EnablePasswordRetrieval
        {
            get { return enablePasswordRetrieval; }
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            var userEntities = _userCache.FindUsersByEmail(emailToMatch);

            var users = new MembershipUserCollection();

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
                if (counter >= startIndex) users.Add(GetUserFromEntity(userEntity));

                if (counter >= endIndex) break;

                counter++;
            }

            return users;
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            var userEntities = _userCache.FindUsers(usernameToMatch);

            var users = new MembershipUserCollection();

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
                if (counter >= startIndex) users.Add(GetUserFromEntity(userEntity));

                if (counter >= endIndex) break;

                counter++;
            }

            return users;
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            var users = new MembershipUserCollection();
            var userEntities = _userCache.GetUsers();

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
                if (counter >= startIndex) users.Add(GetUserFromEntity(userEntity));

                if (counter >= endIndex) break;

                counter++;
            }

            return users;
        }

        public override int GetNumberOfUsersOnline()
        {
            var onlineSpan = new TimeSpan(0, Membership.UserIsOnlineTimeWindow, 0);
            var compareTime = DateTime.Now.Subtract(onlineSpan);

            var userEntities = _userCache.GetOnlineUsers(compareTime);

            var numberOfUsers = 0;

            try
            {
                numberOfUsers = userEntities.Count();
            }
            catch (DataServiceQueryException)
            {
                // Intentionally empty block
            }

            return numberOfUsers;
        }

        /// <summary>
        /// Compares password values based on the MembershipPasswordFormat
        /// </summary>
        /// <param name="password">Cleartext password</param>
        /// <param name="dbpassword">Encoded or hashed password</param>
        /// <returns>True if same values</returns>
        bool CheckPassword(string password, string dbpassword)
        {
            var pass1 = password;
            var pass2 = dbpassword;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Encrypted:
                    pass2 = DecodePassword(dbpassword);
                    break;
                case MembershipPasswordFormat.Hashed:
                    pass1 = EncodePassword(password);
                    break;
                default:
                    break;
            }

            return pass1 == pass2;
        }

        void SendAccountLockedEmail(Guid providerKey, string userName, string emailAddress)
        {
            // Now lets create an email message
            var emailMessage = new StringBuilder();
            var applicationName = ConfigurationManager.AppSettings["ApplicationName"];
            var header = Resources.Email.EmailHeader.Replace("{0}", String.Format(Resources.Email.LockedOutHeader, 
                applicationName));
            emailMessage.Append(header);
            emailMessage.AppendFormat(Resources.Email.LockedOutBody1, userName, MaxInvalidPasswordAttempts, applicationName);
            emailMessage.AppendFormat(Resources.Email.LockedOutBody2, applicationName);
            var url = string.Format("http://{1}/Account/UnlockAccount.aspx?Id={0}", providerKey, applicationName);
            emailMessage.AppendFormat(Resources.Email.LockedOutBody3, url);
            var footer = String.Format(Resources.Email.EmailFooter, String.Format(Resources.Email.LockedOutReason, 
                applicationName));
            emailMessage.Append(footer);
            var fromEmail = ConfigurationManager.AppSettings["fromEmail"];

            // Send the email
            var messagingClient = new MessagingClient();
            messagingClient.SendEmail(fromEmail, emailAddress, String.Format(Resources.Email.LockedOutSubject, applicationName), 
                emailMessage.ToString());
        }

        /// <summary>
        /// A helper method that performs the checks and updates associated with password failure tracking
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="failureType">Failure type; "password" or "passwordAnswer" accepted</param>
        void UpdateFailureCount(string username, string failureType)
        {
            var providerKey = GetPartitionKeyFromUsername(username);
            var userEntity = _userCache.GetUser(username);

            if (userEntity == null || userEntity.IsDeleted) return;

            var windowStart = new DateTime();
            var failureCount = 0;

            switch (failureType)
            {
                case "password":
                    failureCount = userEntity.FailedPasswordAttemptCount;
                    windowStart = userEntity.FailedPasswordAttemptWindowStart;
                    break;
                case "passwordAnswer":
                    failureCount = userEntity.FailedPasswordAnswerAttemptCount;
                    windowStart = userEntity.FailedPasswordAnswerAttemptWindowStart;
                    break;
            }

            var windowEnd = windowStart.AddMinutes(PasswordAttemptWindow);

            if (failureCount == 0 || DateTime.Now > windowEnd)
            {
                // First password failure or outside of PasswordAttemptWindow. 
                // Start a new password failure count from 1 and a new window starting now.
                switch (failureType)
                {
                    case "password":
                        userEntity.FailedPasswordAttemptCount = 1;
                        userEntity.FailedPasswordAttemptWindowStart = DateTime.Now;
                        break;
                    case "passwordAnswer":
                        userEntity.FailedPasswordAnswerAttemptCount = 1;
                        userEntity.FailedPasswordAnswerAttemptWindowStart = DateTime.Now;
                        break;
                }
            }
            else
            {
                if (failureCount++ >= MaxInvalidPasswordAttempts)
                {
                    // Password attempts have exceeded the failure threshold. Lock out the user.
                    userEntity.IsLockedOut = true;
                    userEntity.LastLockedOutDate = DateTime.Now;

                    // Notify that the user has been locked out by e-mail
                    SendAccountLockedEmail(providerKey, userEntity.Username, userEntity.Email);
                }
                else
                {
                    // Password attempts have not exceeded the failure threshold. Update
                    // the failure counts. Leave the window the same.
                    switch (failureType)
                    {
                        case "password":
                            userEntity.FailedPasswordAttemptCount = failureCount;
                            break;
                        case "passwordAnswer":
                            userEntity.FailedPasswordAnswerAttemptCount = failureCount;
                            break;
                    }
                }
            }

            _userCache.Update(userEntity);

            // Make sure that all the other cloud instances are updated to reflect the user changes
            //new Bll.CloudCommand().SendCommand("ClearUserCache", true);

        }

        public override string GetPassword(string username, string answer)
        {
            if (!EnablePasswordRetrieval) throw new ProviderException("Password Retrieval Not Enabled.");

            if (PasswordFormat == MembershipPasswordFormat.Hashed) throw new ProviderException("Cannot retrieve Hashed passwords.");

            var userEntity = _userCache.GetUser(username);
            if (userEntity == null) throw new MembershipPasswordException("The supplied user name is not found.");

            if (userEntity.IsLockedOut)
                throw new MembershipPasswordException("The supplied user is locked out.");

            if (userEntity.IsDeleted)
                throw new MembershipPasswordException("The supplied user does not exist.");

            if (RequiresQuestionAndAnswer && !CheckPassword(answer, userEntity.PasswordAnswer))
            {
                UpdateFailureCount(username, "passwordAnswer");

                throw new MembershipPasswordException("Incorrect password answer.");
            }

            string password;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Encrypted:
                    password = DecodePassword(userEntity.Password);
                    break;
                case MembershipPasswordFormat.Clear:
                    password = userEntity.Password;
                    break;
                default:
                    throw new MembershipPasswordException("Only encrypted or plaintext passwords can be retrieved.");
            }

            userEntity.FailedPasswordAnswerAttemptCount = 0;
            userEntity.LastActivityDate = DateTime.Now;

            _userCache.Update(userEntity);

            // Make sure that all the other cloud instances are updated to reflect the user changes
            //new Bll.CloudCommand().SendCommand("ClearUserCache", true);

            return password;
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            return GetUser(GetPartitionKeyFromUsername(username), userIsOnline);
        }

        MembershipUser GetUserFromEntity(UserEntity userEntity)
        {
            return new MembershipUser(
                    this.Name,
                    userEntity.Username,
                    new Guid(userEntity.PartitionKey),
                    userEntity.Email,
                    userEntity.PasswordQuestion,
                    userEntity.Comment,
                    userEntity.IsApproved,
                    userEntity.IsLockedOut,
                    userEntity.CreationDate,
                    userEntity.LastLoginDate,
                    userEntity.LastActivityDate,
                    userEntity.LastPasswordChangedDate,
                    userEntity.LastLockedOutDate
                );
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            var userEntity = _userCache.GetUser(providerUserKey);
            return userEntity == null ? null : GetUserFromEntity(userEntity);
        }

        public override string GetUserNameByEmail(string email)
        {
            var userEntity = _userCache.GetUserByEmail(email);

            return userEntity == null ? null : userEntity.Username;
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { return maxInvalidPasswordAttempts; }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return minRequiredNonAlphanumericCharacters; }
        }

        public override int MinRequiredPasswordLength
        {
            get { return minRequiredPasswordLength; }
        }

        public override int PasswordAttemptWindow
        {
            get { return passwordAttemptWindow; }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { return passwordFormat; }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { return passwordStrengthRegularExpression; }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { return requiresQuestionAndAnswer; }
        }

        public override bool RequiresUniqueEmail
        {
            get { return requiresUniqueEmail; }
        }

        public override string ResetPassword(string username, string answer)
        {
            if (!EnablePasswordReset)
                throw new NotSupportedException("Password reset is not enabled.");

            if (answer == null && RequiresQuestionAndAnswer)
            {
                UpdateFailureCount(username, "passwordAnswer");

                throw new ProviderException("Password answer required for password reset.");
            }

            const int newPasswordLength = 8;
            var newPassword = Membership.GeneratePassword(newPasswordLength, MinRequiredNonAlphanumericCharacters);

            var args = new ValidatePasswordEventArgs(username, newPassword, true);

            OnValidatingPassword(args);

            if (args.Cancel)
                if (args.FailureInformation != null)
                    throw args.FailureInformation;
                else
                    throw new MembershipPasswordException("Reset password canceled due to password validation failure.");

            var userEntity = _userCache.GetUser(username);
            if (userEntity == null || userEntity.IsDeleted)
                throw new MembershipPasswordException("The supplied user name is not found.");

            if (userEntity.IsLockedOut)
                throw new MembershipPasswordException("The supplied user is locked out.");

            var passwordAnswer = userEntity.PasswordAnswer;

            if (RequiresQuestionAndAnswer && !answer.Trim().Equals(passwordAnswer.Trim(), StringComparison.InvariantCultureIgnoreCase))
            {
                UpdateFailureCount(username, "passwordAnswer");

                throw new MembershipPasswordException("Incorrect password answer.");
            }

            userEntity.Password = EncodePassword(newPassword);
            userEntity.LastPasswordChangedDate = userEntity.LastActivityDate = DateTime.Now;

            var userDataSource = new AzureTableStorageDataSource();
            userDataSource.Update(userEntity);

            return newPassword;
        }

        public override bool UnlockUser(string username)
        {
            var userEntity = _userCache.GetUser(username);

            if (userEntity == null || userEntity.IsDeleted) return false;

            userEntity.IsLockedOut = false;
            userEntity.LastActivityDate = DateTime.Now;

            _userCache.Update(userEntity);

            return true;
        }

        public override void UpdateUser(MembershipUser user)
        {
            var userEntity = _userCache.GetUser(user.UserName);

            if (userEntity == null || userEntity.IsDeleted)
                throw new MembershipPasswordException("The supplied user name is not found.");

            if (userEntity.IsLockedOut)
                throw new MembershipPasswordException("The supplied user is locked out.");

            // Only store data of there has been any changes to prevent unneeded trips to the database
            bool hasChanged = false;

            if (userEntity.Email != user.Email)
            {
                hasChanged = true;
                userEntity.Email = user.Email;
            }

            if (userEntity.Comment != user.Comment)
            {
                hasChanged = true;
                userEntity.Comment = user.Comment;
            }

            if (userEntity.IsApproved != user.IsApproved)
            {
                hasChanged = true;
                userEntity.IsApproved = user.IsApproved;
            }

            if (hasChanged)
            {
                _userCache.Update(userEntity);
            }
        }

        public override bool ValidateUser(string username, string password)
        {
            var userEntity = _userCache.GetUser(username);

            if (userEntity == null || userEntity.IsDeleted || userEntity.IsLockedOut) return false;

            var isValid = false;
            var isApproved = userEntity.IsApproved;
            var pwd = userEntity.Password;

            if (CheckPassword(password, pwd))
            {
                userEntity.LastActivityDate = DateTime.Now;

                if (isApproved)
                {
                    isValid = true;

                    userEntity.LastLoginDate = DateTime.Now;
                    userEntity.FailedPasswordAttemptCount = 0;
                }

                _userCache.Update(userEntity);
            }
            else UpdateFailureCount(username, "password");

            return isValid;
        }
    }
}