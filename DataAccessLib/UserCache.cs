using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Services.Client;
using System.Linq;
using System.Web.Caching;
using System.Web.Hosting;
using VeraWAF.AzureTableStorage;
//using VeraWAF.WebPages.Bll;

namespace VeraWAF.WebPages.Dal {

    /// <summary>
    /// In-memory cache of all the VeraWAF users.
    /// Local cloud node cache.
    /// </summary>
    public class UserCache {
        /// <summary>
        /// Cache key.
        /// Cache contains all the users indexed by their e-mail addresses.
        /// </summary>
        public const string CacheKey1 = "UserData1";

        /// <summary>
        /// Cache key.
        /// Cache contains all the users indexed by their user names.
        /// </summary>
        public const string CacheKey2 = "UserData2";

        /// <summary>
        /// Cache key.
        /// Cache contains all the users indexed by their ASP.NET membership provider user key.
        /// </summary>
        public const string CacheKey3 = "UserData3";

        /// <summary>
        /// Application name
        /// </summary>
        readonly string _applicationName;

        /// <summary>
        /// Class constructor
        /// </summary>
        public UserCache()
        {
            _applicationName = ConfigurationManager.AppSettings["ApplicationName"];
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="applicationName">Application name</param>
        public UserCache(string applicationName) {
            _applicationName = applicationName;
        }

        /// <summary>
        /// Add users to the cache
        /// </summary>
        /// <param name="users">Collection of users indexed by their e-mail addresses</param>
        void AddUsersToCache1(Dictionary<string, Dictionary<string, UserEntity>> users)
        {
            HostingEnvironment.Cache.Add(CacheKey1, users, null,
                                         Cache.NoAbsoluteExpiration,
                                         Cache.NoSlidingExpiration,
                                         CacheItemPriority.Default, null);
        }

        /// <summary>
        /// Get a collection of all users indexed by their user e-mail address
        /// </summary>
        /// <returns>A collection of user entities or null if no users</returns>
        Dictionary<string, Dictionary<string, UserEntity>> GetUsersIndexedByEmailFromCache()
        {
            return (Dictionary<string, Dictionary<string, UserEntity>>)HostingEnvironment.Cache.Get(CacheKey1);
        }

 
        /// <summary>
        /// Get all users from the Azure table storage table VeraUsers.
        /// </summary>
        /// <returns>A collection of all the users in the table VeraUsers</returns>
        IEnumerable<UserEntity> GetUsersFromStore()
        {
            var datasource = new AzureTableStorageDataSource();
            var applicationName = ConfigurationManager.AppSettings["ApplicationName"];
            return datasource.GetUsers(applicationName);
        }

        /// <summary>
        /// Get a collection of all users indexed by their user e-mail address
        /// </summary>
        /// <returns>A collection of user entities or null if no users</returns>    
        public Dictionary<string, Dictionary<string, UserEntity>> GetUsersIndexedByEmail()
        {
            var users = GetUsersIndexedByEmailFromCache();
            if (users == null)
            {
                users = new Dictionary<string, Dictionary<string, UserEntity>>();

                try
                {
                    var storedUsers = GetUsersFromStore();
                    foreach (var user in storedUsers)
                        if (users.ContainsKey(user.Email))
                            users[user.Email].Add(user.PartitionKey, user);
                        else
                            users.Add(user.Email, new Dictionary<string, UserEntity> { { user.PartitionKey, user } });
                }
                catch (DataServiceQueryException)
                {
                    // Exception caught if there are no Users in the table
                }

                AddUsersToCache1(users);
            }
            return users;
        }

        /// <summary>
        /// Add users to the cache
        /// </summary>
        /// <param name="users">Collection of users indexed by their user name</param>
        void AddUsersToCache2(Dictionary<string, UserEntity> users) {
            HostingEnvironment.Cache.Add(CacheKey2, users, null,
                                         Cache.NoAbsoluteExpiration,
                                         Cache.NoSlidingExpiration,
                                         CacheItemPriority.Default, null);
        }

        /// <summary>
        /// Get a collection of all users indexed by their user name
        /// </summary>
        /// <returns>A collection of user entities or null if no users</returns>    
        Dictionary<string, UserEntity> GetUsersIndexedByUserNameFromCache() {
            return (Dictionary<string, UserEntity>)HostingEnvironment.Cache.Get(CacheKey2);
        }

        /// <summary>
        /// Get a collection of all users indexed by their user name
        /// </summary>
        /// <returns>A collection of user entities or null if no users</returns>    
        public Dictionary<string, UserEntity> GetUsersIndexedByUserName() {
            var users = GetUsersIndexedByUserNameFromCache();
            if (users == null) {
                users = new Dictionary<string, UserEntity>();

                try
                {
                    var storedUsers = GetUsersFromStore();
                    
                    foreach (var user in storedUsers.Where(user => !users.ContainsKey(user.Username)))
                        users.Add(user.Username,  user);
                }
                catch (DataServiceQueryException) {
                    // Exception caught if there are no Users in the table
                }

                AddUsersToCache2(users);
            }
            return users;
        }


        /// <summary>
        /// Add users to the cache on the node instance
        /// </summary>
        /// <param name="users">Colelction of user entities indexed on their ASP.NET membership provider user keys</param>
        private void AddUsersToCache3(Dictionary<string, UserEntity> users) {
            HostingEnvironment.Cache.Add(CacheKey3, users, null,
                                         Cache.NoAbsoluteExpiration,
                                         Cache.NoSlidingExpiration,
                                         CacheItemPriority.Default, null);
        }

        /// <summary>
        /// Get a collection of all users indexed by their ASP.NET membership provider user key
        /// </summary>
        /// <returns>A collection of user entities or null if no users</returns>
        Dictionary<string, UserEntity> GetUsersIndexedByProviderUserKeyFromCache() {
            return (Dictionary<string, UserEntity>)HostingEnvironment.Cache.Get(CacheKey3);
        }

        /// <summary>
        /// Get a collection of all users indexed by their ASP.NET membership provider user key
        /// </summary>
        /// <returns>A collection of user entities or null if no users</returns>
        public Dictionary<string, UserEntity> GetUsersIndexedByProviderUserKey() {
            var users = GetUsersIndexedByProviderUserKeyFromCache();
            if (users == null) {
                users = new Dictionary<string, UserEntity>();

                try {
                    var storedUsers = GetUsersFromStore();

                    foreach (var user in storedUsers.Where(user => !users.ContainsKey(user.PartitionKey)))
                        users.Add(user.PartitionKey, user);

                } catch (DataServiceQueryException) {
                    // Exception caught if there are no Users in the table
                }

                AddUsersToCache3(users);
            }
            return users;
        }

        /// <summary>
        /// Get all users indexed by their user names
        /// </summary>
        /// <returns>A collection of user entities or null if no users</returns>
        public IEnumerable<UserEntity> GetUsers() {
            return GetUsersIndexedByUserName().Values;
        }

        /// <summary>
        /// Get user entity data by user name.
        /// </summary>
        /// <param name="userName">User name</param>
        /// <returns>Entity data or null if not found</returns>
        public UserEntity GetUser(string userName)
        {
            return GetUsersIndexedByUserName().FirstOrDefault(user => user.Key == userName).Value;            
        }

        /// <summary>
        /// Get user entity data by ASP.NET membership provider user key.
        /// </summary>
        /// <param name="providerUserKey">ASP.NET membership provider user key</param>
        /// <returns>Entity data or null if not found</returns>
        public UserEntity GetUser(object providerUserKey)
        {
            return GetUsersIndexedByProviderUserKey().FirstOrDefault(user => user.Key == providerUserKey.ToString()).Value;
        }

        /// <summary>
        /// Get user entity data by user e-mail.
        /// </summary>
        /// <param name="email">User e-mail</param>
        /// <returns>Entity data or null if not found</returns>
        public UserEntity GetUserByEmail(string email) 
        {
            var users = GetUsersIndexedByEmail().FirstOrDefault(user => user.Key == email).Value;

            // Potentially many users, but we only want one so pick the first one
            if (users != null && users.Count() > 0) return users.Values.FirstOrDefault();

            // No user with the e-mail address was found
            return null;
        }

        /// <summary>
        /// Find users by user name.
        /// </summary>
        /// <param name="userName">Full or partial user name filter</param>
        /// <returns>Collection of entity data or null if no matches were found</returns>
        public IEnumerable<UserEntity> FindUsers(string userName)
        {
            return (from user in GetUsersIndexedByUserName() where user.Key.Contains(userName) select user.Value).ToList();
        }

        /// <summary>
        /// Find users by user e-mail.
        /// </summary>
        /// <param name="email">User e-mail</param>
        /// <returns>Collection of entity data or null if no matches were found</returns>
        public IEnumerable<UserEntity> FindUsersByEmail(string email) {
            return GetUsersIndexedByEmail().FirstOrDefault(user => user.Key == email).Value.Values;
        }

        /// <summary>
        /// Get all inactive users
        /// </summary>
        /// <param name="username">Full or partial user name filter</param>
        /// <param name="userInactiveSinceDate">Inactivity date filter</param>
        /// <returns>Collection of entity data or null if no matches were found</returns>
        public IEnumerable<UserEntity> FindInactiveUsers(string username, DateTime userInactiveSinceDate)
        {
            return (from user in GetUsersIndexedByUserName() where user.Key.Contains(username) 
                        && user.Value.RowKey == String.Empty
                        && user.Value.LastActivityDate >= userInactiveSinceDate 
                    select user.Value).ToList();
        }

        /// <summary>
        /// Get all currently online users
        /// </summary>
        /// <param name="compareTime">Online since date filter</param>
        /// <returns>Collection of entity data or null if none are online</returns>
        public IEnumerable<UserEntity> GetOnlineUsers(DateTime compareTime)
        {
            return (from user in GetUsersIndexedByUserName() where user.Value.LastActivityDate >= compareTime
                    select user.Value).ToList();
        }

        /// <summary>
        /// Clears all the user caches on the current cloud node
        /// </summary>
        public void Clear2()
        {
            HostingEnvironment.Cache.Remove(CacheKey1);
            HostingEnvironment.Cache.Remove(CacheKey2);
            HostingEnvironment.Cache.Remove(CacheKey3);
        }

        /// <summary>
        /// Update the user cache with a new or existing user entity
        /// </summary>
        /// <param name="user">User entity</param>
        public void Update(UserEntity user)
        {
            var userDataSource = new AzureTableStorageDataSource();
            userDataSource.Update(user);

            var coll1 = GetUsersIndexedByEmail();
            if (coll1.ContainsKey(user.Email) && coll1[user.Email].ContainsKey(user.PartitionKey))
                coll1[user.Email][user.PartitionKey] = user;
            AddUsersToCache1(coll1);

            var coll2 = GetUsersIndexedByUserName();
            if (coll2.ContainsKey(user.Username))
                coll2[user.Username] = user;
            AddUsersToCache2(coll2);

            var coll3 = GetUsersIndexedByProviderUserKey();
            if (coll3.ContainsKey(user.PartitionKey))
                coll3[user.PartitionKey] = user;
            AddUsersToCache3(coll3);
        }

    }
}