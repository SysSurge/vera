using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using System.Data.Services.Client;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;
using VeraWAF.AzureTableStorage;

namespace VeraWAF.WebPages.Bll
{
    public class AzureRoleProvider : RoleProvider
    {
        public override string ApplicationName { get; set; }

        public override void Initialize(string name, NameValueCollection config)
        {
            if (config == null) throw new ArgumentNullException("config");

            if (String.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            // Initialize the abstract base class.
            base.Initialize(name, config);

            ApplicationName = String.IsNullOrEmpty(
                config["applicationName"]) ?
                System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath :
                config["applicationName"];
        }

        public override bool IsUserInRole(string username, string rolename)
        {
            var roleDataSource = new AzureTableStorageDataSource();

            try
            {
                return roleDataSource.IsUserInRole(
                    GetProviderKeyFromRoleName(rolename).ToString(),
                    GetRowKeyFromUserName(username).ToString(),
                    ApplicationName,
                    MergeOption.NoTracking);
            }
            catch (DataServiceQueryException)
            {
                return false;
            }
        }

        public override string[] GetRolesForUser(string username)
        {
            var roleNames = new ArrayList();

            var roleDataSource = new AzureTableStorageDataSource();
            var roles = roleDataSource.GetRolesForUser(GetRowKeyFromUserName(username).ToString(), ApplicationName, MergeOption.NoTracking);

            try
            {
                foreach (var roleEntity in roles) roleNames.Add(roleEntity.RoleName);
            }
            catch (DataServiceQueryException)
            {
                // If the table does not exist a DataServiceQueryException is thrown
            }

            return (string[])roleNames.ToArray(typeof(string));
        }

        public override void CreateRole(string rolename)
        {
            if (RoleExists(rolename)) throw new ProviderException("Role name already exists.");

            // Add the admin as the first user to any new role that is created
            var roleDataSource = new AzureTableStorageDataSource();
            var adminName = ConfigurationManager.AppSettings["AdminName"];

            try
            {
                roleDataSource.Insert(new RoleEntity(GetProviderKeyFromRoleName(rolename).ToString(), GetRowKeyFromUserName(adminName).ToString())
                {
                    ApplicationName = this.ApplicationName,
                    RoleName = rolename,
                    UserName = adminName
                });
            }
            catch (DataServiceRequestException)
            {
                // Nothing to do
            }
        }

        public override bool DeleteRole(string rolename, bool throwOnPopulatedRole)
        {
            if (!RoleExists(rolename))
                throw new ProviderException("Role does not exist.");

            if (throwOnPopulatedRole && GetUsersInRole(rolename).Length > 0)
                throw new ProviderException("Cannot delete a populated role.");

            // A role is automatically deleted when the last user is removed, so nothing to do here until then
            return true;
        }

        public override bool RoleExists(string rolename)
        {
            var roleDataSource = new AzureTableStorageDataSource();

            try
            {
                return roleDataSource.RoleExists(GetProviderKeyFromRoleName(rolename).ToString(), ApplicationName, MergeOption.NoTracking);
            }
            catch (DataServiceQueryException)
            {
                return false;
            }
        }

        /// <summary>
        /// The provider key is a Guid made from the role name MD5 hash 
        /// </summary>
        /// <param name="rolename">User name</param>
        /// <returns>Provider key</returns>
        Guid GetProviderKeyFromRoleName(string rolename)
        {
            var unicodeEncoding = new UnicodeEncoding();
            var message = unicodeEncoding.GetBytes(rolename);

            MD5 hashString = new MD5CryptoServiceProvider();

            return new Guid(hashString.ComputeHash(message));
        }

        /// <summary>
        /// The provider key is a Guid made from the username MD5 hash 
        /// </summary>
        /// <param name="username">User name</param>
        /// <returns>Provider key</returns>
        Guid GetRowKeyFromUserName(string username)
        {
            var unicodeEncoding = new UnicodeEncoding();
            var message = unicodeEncoding.GetBytes(username);

            MD5 hashString = new MD5CryptoServiceProvider();

            return new Guid(hashString.ComputeHash(message));
        }

        public override void AddUsersToRoles(string[] usernames, string[] rolenames)
        {
            if (rolenames.Any(rolename => !RoleExists(rolename)))
                throw new ProviderException("Role name not found.");

            if (usernames.Any(username => rolenames.Any(rolename => IsUserInRole(username, rolename))))
                throw new ProviderException("User is already in role.");

            var roleDataSource = new AzureTableStorageDataSource();

            foreach (var username in usernames)
                foreach (var rolename in rolenames)
                    roleDataSource.Insert(new RoleEntity(GetProviderKeyFromRoleName(rolename).ToString(), GetRowKeyFromUserName(username).ToString())
                    {
                        ApplicationName = this.ApplicationName,
                        RoleName = rolename,
                        UserName = username
                    });
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] rolenames)
        {
            if (rolenames.Any(rolename => !RoleExists(rolename)))
                throw new ProviderException("Role name not found.");

            if ((from username in usernames
                 from rolename in rolenames
                 where !IsUserInRole(username, rolename)
                 select username).Any())
                throw new ProviderException("User is not in role.");

            var roleDataSource = new AzureTableStorageDataSource();

            foreach (var userInRole in from username in usernames
                                       from rolename in rolenames
                                       select roleDataSource.GetUserInRole
                                       (
                                            GetProviderKeyFromRoleName(rolename).ToString(),
                                            GetRowKeyFromUserName(username).ToString(),
                                            ApplicationName
                                       )
                    )

                roleDataSource.Delete(userInRole.First());
        }

        public override string[] GetUsersInRole(string rolename)
        {
            var userNames = new ArrayList();

            var roleDataSource = new AzureTableStorageDataSource();
            var roles = roleDataSource.GetUsersInRole(GetProviderKeyFromRoleName(rolename).ToString(), ApplicationName, MergeOption.NoTracking);

            foreach (var roleEntity in roles)
                userNames.Add(roleEntity.UserName);

            return (string[])userNames.ToArray(typeof(string));
        }

        public override string[] GetAllRoles()
        {
            var roleNames = new ArrayList();

            var roleDataSource = new AzureTableStorageDataSource();
            var roles = roleDataSource.GetAllRoles(ApplicationName, MergeOption.NoTracking);

            foreach (var roleEntity in roles)
                roleNames.Add(roleEntity.RoleName);

            return (string[])roleNames.ToArray(typeof(string));
        }

        public override string[] FindUsersInRole(string rolename, string usernameToMatch)
        {
            var roleNames = new ArrayList();

            var roleDataSource = new AzureTableStorageDataSource();
            var roles = roleDataSource.FindUsersInRole(GetProviderKeyFromRoleName(rolename).ToString(), ApplicationName, MergeOption.NoTracking);

            foreach (var roleEntity in roles)
                roleNames.Add(roleEntity.RoleName);

            return (string[])roleNames.ToArray(typeof(string));
        }

    }
}