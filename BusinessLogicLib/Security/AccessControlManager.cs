using System;
using System.Configuration;
using System.Web;
using System.Web.Security;
using VeraWAF.AzureTableStorage;
using VeraWAF.WebPages.Dal;
using VeraWAF.WebPages.Dal.Interchange;

namespace VeraWAF.WebPages.Bll.Security
{
    /// <summary>
    /// Handles the solutions role-based access control
    /// </summary>
    public class AccessControlManager : AccessControlManagerExt
    {
        public enum EPermission
        {
            /// <summary>
            /// Allow read access
            /// </summary>
            AllowRead = 1,

            /// <summary>
            /// Allow write access
            /// </summary>
            AllowWrite = 2,

            /// <summary>
            /// Allow delete access
            /// </summary>
            AllowDelete = 4,

            /// <summary>
            /// Deny read access
            /// </summary>
            DenyRead = 8,

            /// <summary>
            /// Deny write access
            /// </summary>
            DenyWrite = 16,

            /// <summary>
            /// Deny delete access
            /// </summary>
            DenyDelete = 32
        }

        public bool UserIsAdmin()
        {
            var context = HttpContext.Current;
            if (context == null) return false;
            return context.User.IsInRole("Admins") || context.User.IsInRole("Editors");
        }

        /// <summary>
        /// Check if a user is in a role
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="rolename">Role name</param>
        /// <returns>True if the user is an administrator</returns>
        public bool UserIsInRole(string username, string rolename)
        {
            var datasource = new AzureTableStorageDataSource();
            var applicationName = ConfigurationManager.AppSettings["ApplicationName"];
            return datasource.IsUserInRoleByName(username, rolename, applicationName);
        }

        /// <summary>
        /// Check if a user is an editor
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns>True if the user is an editor</returns>
        public bool UserIsEditor(string username)
        {
            return UserIsInRole(username, GetEditorRoleName());
        }

        /// <summary>
        /// Check if a user is member of a specific role
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns>True if the user is member of the role</returns>
        public bool UserIsAdmin(string username)
        {
            return UserIsInRole(username, GetAdminRoleName());
        }

        /// <summary>
        /// Get the administrator role name
        /// </summary>
        /// <returns>The administrator role name</returns>
        public string GetAdminRoleName()
        {
            return ConfigurationManager.AppSettings["AdminRoleName"];
        }

        /// <summary>
        /// Get the editors role name
        /// </summary>
        /// <returns>The administrator role name</returns>
        public string GetEditorRoleName()
        {
            return ConfigurationManager.AppSettings["EditorRoleName"];
        }

        /// <summary>
        /// Check if the current user is an admin or editor
        /// </summary>
        /// <returns>Returns true if the current user is a admin or editor</returns>
        public bool UserIsAdminOrEditor()
        {
            var context = HttpContext.Current;
            if (context == null) return false;
            return context.User.IsInRole(GetAdminRoleName()) || context.User.IsInRole(GetEditorRoleName());
        }

        /// <summary>
        /// Check if the current user can edit the page
        /// </summary>
        /// <param name="page">Page</param>
        /// <returns>True if the current user can edit the page</returns>
        public bool UserHasEditPermissions(PageEntity page)
        {
            return (Membership.GetUser() != null && (Membership.GetUser().UserName == page.Author || UserIsAdminOrEditor()));
        }

        /// <summary>
        /// Check if a user has permissions to edit a page
        /// </summary>
        /// <param name="username"></param>
        /// <param name="partitionKey"></param>
        /// <returns></returns>
        public bool UserHasPageEditPermissions(string username, string partitionKey)
        {
            var user = Membership.GetUser(username);
            if (user == null) return false; // User not found
            var page = new PageCache().GetPageByPartitionKey(partitionKey);
            return (user != null && page != null && (user.UserName == page.Author || UserIsAdminOrEditor()));
        }

        /// <summary>
        /// Check if a user has enough permissions to read a user profile property
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="partitionKey">Partition key that identifies the row in the VeraUsers table</param>
        /// <param name="propertyName">VeraUsers property name that is to be edited</param>
        /// <returns>True if the user can edit the property</returns>
        public bool UserHasProfileReadPermissions(string username, string propertyName)
        {
            // Is admin?
            if (UserIsAdmin(username))
                return true;    // Admins have full access to all properties

            // Make sure that the property can be edited by the user self
            switch (propertyName)
            {
                case "AllowContactForm":
                case "Birthday":
                case "CompanySize":
                case "Country":
                case "Employer":
                case "FullName":
                case "Gender":
                case "Industry":
                case "JobCategory":
                case "Newsletter":
                case "PasswordAnswer":
                case "PasswordQuestion":
                case "PortraitBlobAddressUri":
                    return true;
            }

            return base.UserHasProfileReadPermissions(username, propertyName);
        }

        /// <summary>
        /// Check if a user has enough permissions to edit a user profile property
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="partitionKey">Partition key that identifies the row in the VeraUsers table</param>
        /// <param name="propertyName">VeraUsers property name that is to be edited</param>
        /// <returns>True if the user can edit the property</returns>
        public bool UserHasProfileEditPermissions(string username, string partitionKey, string propertyName)
        {
            var userEntity = new UserCache().GetUser(partitionKey as object);
            if (userEntity == null)
                return false;   // User entity not found

            // Is admin?
            if (UserIsAdmin(username))
                return true;    // Admins have full access to all properties

            if (userEntity.Username != username)
                return false;   // Can only edit ones own info or must be an admin

            // Make sure that the property can be edited by the user self
            switch (propertyName)
            {
                case "AllowContactForm":
                case "Birthday":
                case "CompanySize":
                case "Country":
                case "Employer":
                case "FullName":
                case "Gender":
                case "Industry":
                case "JobCategory":
                case "Newsletter":
                case "PasswordAnswer":
                case "PasswordQuestion":
                case "PortraitBlobAddressUri":
                    return true;
            }

            return base.UserHasProfileEditPermissions(username, partitionKey, propertyName);
        }

        /// <summary>
        /// Check if a user has edit permissions to a Azure table row
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="fieldInfo">Azure table property information</param>
        /// <returns>True if the user has edit permissions to the Azure table row</returns>
        public bool UserHasEditPermissions(string username, TablePropertyInfo fieldInfo)
        {
            if (UserIsAdmin(username))
                return true;    // Admins have full access to all tables and table properties

            // Figure out what kind of table we are talking about
            switch (fieldInfo.TableName)
            {
                case AzureTableStorageServiceContext.FilesTableName:
                // VeraFiles table
                case AzureTableStorageServiceContext.FavoritesTableName:
                    // VeraFavories table
                    return UserIsEditor(username);
                case AzureTableStorageServiceContext.PagesTableName:
                    // VeraPages table
                    return UserHasPageEditPermissions(username, fieldInfo.PartitionKey);
                case AzureTableStorageServiceContext.UsersTableName:
                    // VeraUsers table
                    return UserHasProfileEditPermissions(username, fieldInfo.PartitionKey, fieldInfo.PropertyName);
            }

            return base.UserHasEditPermissions(username, fieldInfo);
        }

        public bool UserHasReadPermissions(string username, GenericTableQuery query)
        {
            if (UserIsAdmin(username))
                return true;    // Admins have full access to all tables and table properties

            // Figure out what kind of table we are talking about
            switch (query.TableName)
            {
                case AzureTableStorageServiceContext.FilesTableName:
                    // VeraFiles table
                case AzureTableStorageServiceContext.FavoritesTableName:
                    // VeraFavories table
                    return UserIsEditor(username);
                case AzureTableStorageServiceContext.PagesTableName:
                    // VeraPages table
                    return true;
                case AzureTableStorageServiceContext.UsersTableName:
                    // VeraUsers table
                    //if (UserHasProfileEditPermissions(username, fieldInfo.PropertyName))
                        return false;
            }

            return base.UserHasReadPermissions(username, query);
        }

        /// <summary>
        /// Get a table resource's qualified name
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <param name="propertyName">Property name</param>
        /// <returns>Resource qualified name in the form appName:TABLE:<tableName>:<propertyName></returns>
        public string GetTableQualifiedName(string tableName, string propertyName = null)
        {
            const string TableBase = "TABLE";
            return String.IsNullOrEmpty(propertyName) ?
                String.Format("{0}:{1}:{2}", ConfigurationManager.AppSettings["ApplicationName"], TableBase, tableName) :
                String.Format("{0}:{1}:{2}:{3}", ConfigurationManager.AppSettings["ApplicationName"], TableBase, tableName, propertyName);
        }

        /// <summary>
        /// Get a role's qualified name
        /// </summary>
        /// <param name="roleName">Role name</param>
        /// <returns>Role qualified name</returns>
        public string GetRoleQualifiedName(string roleName)
        {
            const string RoleBase = "ROLE";
            return String.IsNullOrEmpty(roleName) ? RoleBase : String.Format("{0}:{1}", RoleBase, roleName);
        }

        /// <summary>
        /// Creates a new ACL rule
        /// </summary>
        /// <param name="userOrRole">User or role qualified name. Ex. "ROLE:Admins"</param>
        /// <param name="resourceQualifiedName">Resource qualified name. Ex. "TABLE:VeraPages"</param>
        /// <param name="permissions">Access permissions</param>
        public void AddAccessControlRule(string userOrRole, string resourceQualifiedName, EPermission permissions)
        {
            var applicationName = ConfigurationManager.AppSettings["ApplicationName"];
            var accessControlEntity = new AccessControlEntity();
            accessControlEntity.ApplicationName = applicationName;
            accessControlEntity.PartitionKey = resourceQualifiedName;
            accessControlEntity.RowKey = userOrRole;

            accessControlEntity.Permissions = (int)permissions;

            // Add the rule to the database or update existing
            var datasource = new AzureTableStorageDataSource();
            if (datasource.AclRuleExists(accessControlEntity.PartitionKey, accessControlEntity.RowKey,
                applicationName, System.Data.Services.Client.MergeOption.NoTracking))
            {
                // Rule already exists, so update
                datasource.Update(accessControlEntity);
            }
            else
            {
                // New rule, so create it
                datasource.Insert(accessControlEntity);
            }
        }

    }
}
