using System.Configuration;
using System.Data.Services.Client;
using System.Web.Security;
using VeraWAF.AzureQueue;
using VeraWAF.AzureTableStorage;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System;
using System.Globalization;
using System.Text;
using System.IO;
using System.Web;
using System.Web.Profile;
using VeraWAF.WebPages;
using VeraWAF.WebPages.Bll;
using VeraWAF.WebPages.Bll.Cloud;
using VeraWAF.WebPages.Bll.Security;
using VeraWAF.WebPages.Dal;

namespace VeraWAF.WebPages.Bll
{
    public class InitApplication
    {
        AzureTableStorageDataSource _datasource;

        /// <summary>
        /// Class constructor
        /// </summary>
        public InitApplication()
        {
            _datasource = new AzureTableStorageDataSource();
        }

        /// <summary>
        /// Check if a user exists
        /// </summary>
        /// <param name="userName">User name</param>
        /// <returns>True if the user is a admin</returns>
        bool UserExists(string userName)
        {
            try
            {
                if (Membership.GetUser(userName) == null) return false;
            }
            catch (DataServiceQueryException)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Creates the default admin user
        /// </summary>
        public void CreateAdminUserIfNotExists()
        {
            var adminName = ConfigurationManager.AppSettings["AdminName"];

            if (UserExists(adminName)) return;

            MembershipCreateStatus status;

            var user = Membership.CreateUser(
                adminName,
                ConfigurationManager.AppSettings["AdminPassword"],
                ConfigurationManager.AppSettings["AdminEmail"],
                ConfigurationManager.AppSettings["AdminQuestion"],
                ConfigurationManager.AppSettings["AdminAnswer"], true, null, out status);

            if (status != MembershipCreateStatus.Success || user == null) return;

            user.IsApproved = true;

            /*
            * Make sure that the admin user gets a OAuth consumer key & secret by default;
            * otherwise he/she will not be able to access some of the REST APIs.
            */
            var profile = ProfileBase.Create(adminName);    // Get the user profile
            profile.SetPropertyValue("OAuthConsumerKey", Guid.NewGuid().ToString());
            profile.SetPropertyValue("OAuthConsumerSecret", Guid.NewGuid().ToString());
            profile.Save();

            // Make sure the user info is updated in storage
            Membership.UpdateUser(user);
        }

        /// <summary>
        /// Checks if the role exists
        /// </summary>
        /// <param name="roleName">Role name</param>
        /// <returns>Returns True if the admin role exists</returns>
        bool RoleExists(string roleName)
        {
            try
            {
                return Roles.RoleExists(roleName);
            }
            catch (DataServiceQueryException)
            {
                return false;
            }
        }

        /// <summary>
        /// Creates the default admin role
        /// </summary>
        /// <param name="roleName">Role name</param>
        public void CreateRoleIfNotExists(string roleName)
        {
            if (RoleExists(roleName)) return;

            Roles.CreateRole(roleName);
        }

        /// <summary>
        /// Creates all the default database tables
        /// </summary>
        /// <returns></returns>
        public bool CreateTablesIfNotExists()
        {
            return new AzureTableStorageDataSource().CreateTablesIfTheyDontExists();
        }

        /// <summary>
        /// Creates all the default queues
        /// </summary>
        public void CreateQueuesIfNotExists()
        {
            new AzureQueueDataSource().CreateQueuesIfTheyDontExists();
        }

        /// <summary>
        /// Gives a blob the default security permissions
        /// </summary>
        /// <param name="blobContainer">Blob container</param>
        void SetBlobPermissions(CloudBlobContainer blobContainer)
        {
            // Setup the permissions on the container to be public
            var permissions = new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Container
            };

            blobContainer.SetPermissions(permissions);
        }

        /// <summary>
        /// Creates all the default blobs containers
        /// </summary>
        public void CreateBlobIfNotExists()
        {
            // Setup the connection to Windows Azure Storage
            var storageAccount = CloudStorageAccount.FromConfigurationSetting("DataConnectionString");
            var blobClient = storageAccount.CreateCloudBlobClient();

            // Get and create the container
            var blobContainer = blobClient.GetContainerReference("publicfiles");
            if (blobContainer.CreateIfNotExist()) SetBlobPermissions(blobContainer);
        }

        /// <summary>
        /// Creates a table row key based on a date
        /// </summary>
        /// <param name="createdDate">Date</param>
        /// <returns>The table row key</returns>
        string GetRowKey(DateTime createdDate)
        {
            return createdDate.Ticks.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Convert a UTF-8 string to a ASCII string
        /// </summary>
        /// <param name="utf8String">UTF-8 string</param>
        /// <returns>ASCII string</returns>
        public byte[] Utf8ToAscii(string utf8String)
        {
            var utf8Bytes = Encoding.UTF8.GetBytes(utf8String);
            return Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding("windows-1252"), utf8Bytes);
        }

        /// <summary>
        /// Converts a UTF-8 string to hexadecimal number string
        /// </summary>
        /// <param name="utf8String">UTF-8 string</param>
        /// <returns>Hexadecimal number string</returns>
        public string ConvertToHex(string utf8String)
        {
            var hex = new StringBuilder();
            var asciiString = Utf8ToAscii(utf8String);

            foreach (var c in asciiString) hex.AppendFormat("{0:x2}", c);

            return hex.ToString();
        }

        /// <summary>
        /// Creates a page entity
        /// </summary>
        /// <param name="virtualPath">Virtual path</param>
        /// <param name="menuItemName">Menu item name</param>
        /// <param name="menuItemDescription">Menu item description</param>
        /// <param name="title">Page title</param>
        /// <param name="mainContent">Page main content</param>
        /// <param name="showInMenu">Set to True to show the page in the menu</param>
        /// <param name="index">Set to True to add the page to the search index</param>
        /// <param name="template">Page template</param>
        /// <param name="headerContent">Page header content</param>
        /// <param name="asideContent">Page splash content</param>
        /// <param name="ingress">Page ingress</param>
        /// <returns>Page entity</returns>
        PageEntity CreatePageEntityFromFormData(string virtualPath, string menuItemName, 
            string menuItemDescription, string title, string mainContent, bool showInMenu, 
            bool index, string template, string headerContent, string asideContent, 
            string ingress)
        {
            return new PageEntity
            {
                PartitionKey = ConvertToHex(virtualPath),
                RowKey = GetRowKey(DateTime.UtcNow),
                ApplicationName = ConfigurationManager.AppSettings["ApplicationName"],
                Template = template,
                Title = title,
                MainContent = mainContent,
                HeaderContent = headerContent,
                VirtualPath = virtualPath,
                IsPublished = true,
                MenuItemSortOrder = 0,
                GeolocationLat = 0.0,
                GeolocationLong = 0.0,
                GeolocationZoom = 0,
                Author = String.Format("[{0}]", Bll.Resources.Solution.SystemUserName),
                ShowInMenu = showInMenu,
                ShowContactControl = false,
                AllowForComments = false,
                Index = true,
                RdfSubjects = String.Empty,
                RollupImage = String.Empty,
                MenuItemName = menuItemName,
                MenuItemDescription = menuItemDescription,
                AsideContent = asideContent, Ingress = ingress
            };
        }

        /// <summary>
        /// Creates all the default pages
        /// </summary>
        public void AddDefaultPages()
        {
            // Add the root page
            PageEntity page = CreatePageEntityFromFormData("/",
                Bll.Resources.Template.RootMenuItemName,
                Bll.Resources.Template.RootMenuItemDesc, Bll.Resources.InitialPages.FrontPageTitle, String.Empty,
                true, false, ConfigurationManager.AppSettings["FrontpageTemplate"], String.Empty, String.Empty, String.Empty);
            _datasource.Insert(page);

            // Add the front page
            page = CreatePageEntityFromFormData("/Default.aspx", Bll.Resources.Template.FrontpageMenuItemName,
                Bll.Resources.InitialPages.FrontPageTitle, Bll.Resources.InitialPages.FrontPageTitle,
                Bll.Resources.InitialPages.FrontPageMainContent, true, false, ConfigurationManager.AppSettings["FrontpageTemplate"],
                Bll.Resources.InitialPages.FrontPageHeaderContent, Bll.Resources.InitialPages.FrontPageAsideContent, String.Empty
                );
            _datasource.Insert(page);

            // Add the commenting help page
            page = CreatePageEntityFromFormData("/Help/Commenting.aspx", Bll.Resources.InitialPages.CommentingMenuItemName,
                Bll.Resources.InitialPages.CommentingTitle, Bll.Resources.InitialPages.CommentingTitle,
                String.Format(Bll.Resources.InitialPages.CommentMainContent, ConfigurationManager.AppSettings["ApplicationName"]), false, true,
                ConfigurationManager.AppSettings["GenericTemplate"], String.Empty, String.Empty, String.Format(Bll.Resources.InitialPages.CommentingIngress,
                ConfigurationManager.AppSettings["ApplicationName"]));
            _datasource.Insert(page);

            // Add the privacy page
            page = CreatePageEntityFromFormData("/Privacy.aspx", Bll.Resources.InitialPages.PrivacyTitle,
                Bll.Resources.InitialPages.PrivacyTitle, Bll.Resources.InitialPages.PrivacyTitle,
                String.Format(Bll.Resources.InitialPages.PrivacyMainContent, ConfigurationManager.AppSettings["companyName"]), false, true,
                ConfigurationManager.AppSettings["GenericTemplate"], String.Empty, String.Empty, String.Format(Bll.Resources.InitialPages.PrivacyIngress,
                ConfigurationManager.AppSettings["ApplicationName"]));
            _datasource.Insert(page);

            // Add the query syntax page
            page = CreatePageEntityFromFormData("/Help/Search-query-syntax.aspx", Bll.Resources.InitialPages.QuerySyntaxTitle,
                Bll.Resources.InitialPages.QuerySyntaxTitle, Bll.Resources.InitialPages.QuerySyntaxTitle,
                Bll.Resources.InitialPages.QuerySyntaxMainContent, false, true, 
                ConfigurationManager.AppSettings["GenericTemplate"], String.Empty, String.Empty, String.Empty);
            _datasource.Insert(page);

            // Add the RSS 2.0 syndication page
            var title = String.Format(Bll.Resources.InitialPages.SyndicationTitle, ConfigurationManager.AppSettings["companyName"]);
            page = CreatePageEntityFromFormData("/Feeds.aspx", title, title, title,
                String.Format(Bll.Resources.InitialPages.SyndicationMainContent, ConfigurationManager.AppSettings["ApplicationName"]), false, true,
                ConfigurationManager.AppSettings["GenericTemplate"], String.Empty, String.Empty, Bll.Resources.InitialPages.SyndicationIngress);
            _datasource.Insert(page);

            // Add the terms page
            var mainContent = String.Format(Bll.Resources.InitialPages.TermsMainContent, ConfigurationManager.AppSettings["companyName"], 
                DateTime.UtcNow.Year);
            page = CreatePageEntityFromFormData("/Terms.aspx", Bll.Resources.InitialPages.TermsTitle,
                Bll.Resources.InitialPages.TermsTitle, Bll.Resources.InitialPages.TermsTitle,
                mainContent, false, true, ConfigurationManager.AppSettings["GenericTemplate"], String.Empty, String.Empty,
                String.Format(Bll.Resources.InitialPages.TermsIngress, ConfigurationManager.AppSettings["ApplicationName"]));
            _datasource.Insert(page);

            try
            {
                // Make sure that all the cloud instances are updated to reflect the page changes
                new CloudCommandClient().SendCommand("PageCRUD");
            }
            catch(Exception)
            {
                /*
                 * Cloud command will fail by design when the solution is created for the first time, this is normal behaviour as the
                 * user is not signed in as there are no users created yet. So we just ignore the thrown exception and continue as if
                 * nothing bad has happened.
                 */
            }
        }

        /// <summary>
        /// Add default Access Control List data
        /// </summary>
        public void AddDefaultAcls()
        {
            var accessControl = new AccessControlManager();

            // Flags that allows all access to a resource
            var allowAll = AccessControlManager.EPermission.AllowRead | AccessControlManager.EPermission.AllowWrite | AccessControlManager.EPermission.AllowDelete;

            // Flags that allows read access only to a resource
            var readOnly = AccessControlManager.EPermission.AllowRead | AccessControlManager.EPermission.DenyWrite | AccessControlManager.EPermission.DenyDelete;

            // Flags that denies all access to a resource
            var denyAll = AccessControlManager.EPermission.DenyRead | AccessControlManager.EPermission.DenyWrite | AccessControlManager.EPermission.DenyDelete;

            // Give admins full control on all tables
            var admins = accessControl.GetRoleQualifiedName(ConfigurationManager.AppSettings["AdminRoleName"]);
            accessControl.AddAccessControlRule(admins, accessControl.GetTableQualifiedName(String.Empty), allowAll);

            // Give editors full access to the VeraFiles table
            var editors = accessControl.GetRoleQualifiedName(ConfigurationManager.AppSettings["EditorRoleName"]);
            accessControl.AddAccessControlRule(editors, accessControl.GetTableQualifiedName(AzureTableStorageServiceContext.FilesTableName), allowAll);

            // Give editors full access to the VeraPages table
            accessControl.AddAccessControlRule(editors, accessControl.GetTableQualifiedName(AzureTableStorageServiceContext.PagesTableName), allowAll);

            // Give the pseudo role "Owners" full access to their own user data, but restrict some properties
            var owner = accessControl.GetRoleQualifiedName(ConfigurationManager.AppSettings["OwnerRoleName"]);
            accessControl.AddAccessControlRule(owner, accessControl.GetTableQualifiedName(AzureTableStorageServiceContext.UsersTableName), allowAll);
            accessControl.AddAccessControlRule(owner, accessControl.GetTableQualifiedName(AzureTableStorageServiceContext.UsersTableName, "Comment"),
                AccessControlManager.EPermission.DenyRead | AccessControlManager.EPermission.DenyWrite | AccessControlManager.EPermission.DenyDelete);
            accessControl.AddAccessControlRule(owner, accessControl.GetTableQualifiedName(AzureTableStorageServiceContext.UsersTableName, "Password"),
                readOnly);
            accessControl.AddAccessControlRule(owner, accessControl.GetTableQualifiedName(AzureTableStorageServiceContext.UsersTableName, "Email"),
                readOnly);
            accessControl.AddAccessControlRule(owner, accessControl.GetTableQualifiedName(AzureTableStorageServiceContext.UsersTableName, "PasswordQuestion"),
                readOnly);
            accessControl.AddAccessControlRule(owner, accessControl.GetTableQualifiedName(AzureTableStorageServiceContext.UsersTableName, "PasswordAnswer"),
                readOnly);
            accessControl.AddAccessControlRule(owner, accessControl.GetTableQualifiedName(AzureTableStorageServiceContext.UsersTableName, "IsApproved"),
                readOnly);
            accessControl.AddAccessControlRule(owner, accessControl.GetTableQualifiedName(AzureTableStorageServiceContext.UsersTableName, "LastPasswordChangedDate"),
                readOnly);
            accessControl.AddAccessControlRule(owner, accessControl.GetTableQualifiedName(AzureTableStorageServiceContext.UsersTableName, "LastLockedOutDate"),
                readOnly);
            accessControl.AddAccessControlRule(owner, accessControl.GetTableQualifiedName(AzureTableStorageServiceContext.UsersTableName, "FailedPasswordAttemptCount"),
                readOnly);
            accessControl.AddAccessControlRule(owner, accessControl.GetTableQualifiedName(AzureTableStorageServiceContext.UsersTableName, "FailedPasswordAttemptWindowStart"),
                readOnly);
            accessControl.AddAccessControlRule(owner, accessControl.GetTableQualifiedName(AzureTableStorageServiceContext.UsersTableName, "FailedPasswordAnswerAttemptCount"),
                readOnly);
            accessControl.AddAccessControlRule(owner, accessControl.GetTableQualifiedName(AzureTableStorageServiceContext.UsersTableName, "FailedPasswordAnswerAttemptWindowStart"),
                readOnly);
            accessControl.AddAccessControlRule(owner, accessControl.GetTableQualifiedName(AzureTableStorageServiceContext.UsersTableName, "ProfileComment"),
                denyAll);
            accessControl.AddAccessControlRule(owner, accessControl.GetTableQualifiedName(AzureTableStorageServiceContext.UsersTableName, "ClientIpAddress"),
                denyAll);
            accessControl.AddAccessControlRule(owner, accessControl.GetTableQualifiedName(AzureTableStorageServiceContext.UsersTableName, "AuthProvider"),
                readOnly);
            accessControl.AddAccessControlRule(owner, accessControl.GetTableQualifiedName(AzureTableStorageServiceContext.UsersTableName, "OAuthConsumerKey"),
                readOnly);
            accessControl.AddAccessControlRule(owner, accessControl.GetTableQualifiedName(AzureTableStorageServiceContext.UsersTableName, "OAuthConsumerSecret"),
                readOnly);

            // Give the pseudo role "Owners" full access access to their own pages
            accessControl.AddAccessControlRule(owner, accessControl.GetTableQualifiedName(AzureTableStorageServiceContext.PagesTableName), allowAll);

            // Give the pseudo role "Owners" full access access to their votes
            accessControl.AddAccessControlRule(owner, accessControl.GetTableQualifiedName(AzureTableStorageServiceContext.VotesTableName), allowAll);

            // Give the pseudo role "Everyone" read access to the VeraPages table
            var everyone = accessControl.GetRoleQualifiedName(ConfigurationManager.AppSettings["EveryoneRoleName"]);
            accessControl.AddAccessControlRule(everyone, accessControl.GetTableQualifiedName(AzureTableStorageServiceContext.PagesTableName), readOnly);

            // Give the pseudo role "Everyone" read access to the VeraUsers table but deny access to some restricted properties in the table
            accessControl.AddAccessControlRule(everyone, accessControl.GetTableQualifiedName(AzureTableStorageServiceContext.UsersTableName), readOnly);
            accessControl.AddAccessControlRule(everyone, accessControl.GetTableQualifiedName(AzureTableStorageServiceContext.UsersTableName, "Password"),
                denyAll);
            accessControl.AddAccessControlRule(everyone, accessControl.GetTableQualifiedName(AzureTableStorageServiceContext.UsersTableName, "Email"),
                denyAll);
            accessControl.AddAccessControlRule(everyone, accessControl.GetTableQualifiedName(AzureTableStorageServiceContext.UsersTableName, "PasswordQuestion"),
                denyAll);
            accessControl.AddAccessControlRule(everyone, accessControl.GetTableQualifiedName(AzureTableStorageServiceContext.UsersTableName, "PasswordAnswer"),
                denyAll);
            accessControl.AddAccessControlRule(everyone, accessControl.GetTableQualifiedName(AzureTableStorageServiceContext.UsersTableName, "IsApproved"),
                denyAll);
            accessControl.AddAccessControlRule(everyone, accessControl.GetTableQualifiedName(AzureTableStorageServiceContext.UsersTableName, "Comment"),
                denyAll);
            accessControl.AddAccessControlRule(everyone, accessControl.GetTableQualifiedName(AzureTableStorageServiceContext.UsersTableName, "LastPasswordChangedDate"),
                denyAll);
            accessControl.AddAccessControlRule(everyone, accessControl.GetTableQualifiedName(AzureTableStorageServiceContext.UsersTableName, "IsLockedOut"),
                denyAll);
            accessControl.AddAccessControlRule(everyone, accessControl.GetTableQualifiedName(AzureTableStorageServiceContext.UsersTableName, "LastLockedOutDate"),
                denyAll);
            accessControl.AddAccessControlRule(everyone, accessControl.GetTableQualifiedName(AzureTableStorageServiceContext.UsersTableName, "FailedPasswordAttemptCount"),
                denyAll);
            accessControl.AddAccessControlRule(everyone, accessControl.GetTableQualifiedName(AzureTableStorageServiceContext.UsersTableName, "FailedPasswordAttemptWindowStart"),
                denyAll);
            accessControl.AddAccessControlRule(everyone, accessControl.GetTableQualifiedName(AzureTableStorageServiceContext.UsersTableName, "FailedPasswordAnswerAttemptCount"),
                denyAll);
            accessControl.AddAccessControlRule(everyone, accessControl.GetTableQualifiedName(AzureTableStorageServiceContext.UsersTableName, "FailedPasswordAnswerAttemptWindowStart"),
                denyAll);
            accessControl.AddAccessControlRule(everyone, accessControl.GetTableQualifiedName(AzureTableStorageServiceContext.UsersTableName, "Country"),
                denyAll);
            accessControl.AddAccessControlRule(everyone, accessControl.GetTableQualifiedName(AzureTableStorageServiceContext.UsersTableName, "Industry"),
                denyAll);
            accessControl.AddAccessControlRule(everyone, accessControl.GetTableQualifiedName(AzureTableStorageServiceContext.UsersTableName, "JobCategory"),
                denyAll);
            accessControl.AddAccessControlRule(everyone, accessControl.GetTableQualifiedName(AzureTableStorageServiceContext.UsersTableName, "CompanySize"),
                denyAll);
            accessControl.AddAccessControlRule(everyone, accessControl.GetTableQualifiedName(AzureTableStorageServiceContext.UsersTableName, "ProfileComment"),
                denyAll);
            accessControl.AddAccessControlRule(everyone, accessControl.GetTableQualifiedName(AzureTableStorageServiceContext.UsersTableName, "Newsletter"),
                denyAll);
            accessControl.AddAccessControlRule(everyone, accessControl.GetTableQualifiedName(AzureTableStorageServiceContext.UsersTableName, "ClientIpAddress"),
                denyAll);
            accessControl.AddAccessControlRule(everyone, accessControl.GetTableQualifiedName(AzureTableStorageServiceContext.UsersTableName, "Employer"),
                denyAll);
            accessControl.AddAccessControlRule(everyone, accessControl.GetTableQualifiedName(AzureTableStorageServiceContext.UsersTableName, "AuthProvider"),
                denyAll);
            accessControl.AddAccessControlRule(everyone, accessControl.GetTableQualifiedName(AzureTableStorageServiceContext.UsersTableName, "OAuthConsumerKey"),
                denyAll);
            accessControl.AddAccessControlRule(everyone, accessControl.GetTableQualifiedName(AzureTableStorageServiceContext.UsersTableName, "OAuthConsumerSecret"),
                denyAll);
        }

        /// <summary>
        /// Create the default roles like admin & editors
        /// </summary>
        void AddRoles()
        {
            CreateRoleIfNotExists(ConfigurationManager.AppSettings["AdminRoleName"]);
            CreateRoleIfNotExists(ConfigurationManager.AppSettings["EditorRoleName"]);
        }

        /// <summary>
        /// Install all the fundamental features
        /// </summary>
        public void InstallAll()
        {
            CreateQueuesIfNotExists();
            CreateAdminUserIfNotExists();

            AddRoles();

            CreateBlobIfNotExists();

            if (CreateTablesIfNotExists())
            {
                AddDefaultPages();

                AddDefaultAcls();
            }
        }
    }
}