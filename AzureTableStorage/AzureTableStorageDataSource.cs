using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Services.Client;
using System.Linq;
using System.Linq.Expressions;
using System.Net;

namespace VeraWAF.AzureTableStorage
{
    public class AzureTableStorageDataSource
    {
        protected AzureTableStorageServiceContextExt _tableStorageServiceContext;

        CloudStorageAccount GetStorageAccount()
        {
            return CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("DataConnectionString"));
        }

        /// <summary>
        /// PUT HTTP requests smaller than 1460 bytes are ineffecient with nagling turned on.
        /// See: http://blogs.msdn.com/b/windowsazurestorage/archive/2010/06/25/nagle-s-algorithm-is-not-friendly-towards-small-requests.aspx
        /// </summary>
        void HandleNagleOnEndpoint(CloudStorageAccount account)
        {
            var tableServicePoint = ServicePointManager.FindServicePoint(account.TableEndpoint);
            tableServicePoint.UseNagleAlgorithm = bool.Parse(ConfigurationManager.AppSettings["UseNaglingWithTable"]);
        }

        AzureTableStorageServiceContextExt GetServiceContext(CloudStorageAccount account)
        {
            return new AzureTableStorageServiceContextExt(account.TableEndpoint.ToString(), account.Credentials);
        }

        void InitServiceContext()
        {
            var account = GetStorageAccount();

            HandleNagleOnEndpoint(account);

            _tableStorageServiceContext = GetServiceContext(account);
            _tableStorageServiceContext.RetryPolicy = RetryPolicies.Retry(3, TimeSpan.FromSeconds(1));
        }

        void CreateUserTableIfNotExists(CloudStorageAccount storageAccount)
        {
            storageAccount.CreateCloudTableClient().CreateTableIfNotExist(AzureTableStorageServiceContext.UsersTableName);
        }

        void CreateRoleTableIfNotExists(CloudStorageAccount storageAccount)
        {
            storageAccount.CreateCloudTableClient().CreateTableIfNotExist(AzureTableStorageServiceContext.RolesTableName);
        }

        void CreateSessionStateEntityIfNotExists(CloudStorageAccount storageAccount)
        {
            storageAccount.CreateCloudTableClient().CreateTableIfNotExist(AzureTableStorageServiceContext.SessionStatesTableName);
        }

        bool CreatePageEntityTableIfNotExists(CloudStorageAccount storageAccount) {
            return storageAccount.CreateCloudTableClient().CreateTableIfNotExist(AzureTableStorageServiceContext.PagesTableName);
        }

        /// <summary>
        /// Check if the page table exists
        /// </summary>
        /// <returns>True if the table exist</returns>
        public bool PageEntityTableExists()
        {
            return GetStorageAccount().CreateCloudTableClient().DoesTableExist(AzureTableStorageServiceContext.PagesTableName);
        }

        void CreateCustomFieldEntityTableIfNotExists(CloudStorageAccount storageAccount)
        {
            storageAccount.CreateCloudTableClient().CreateTableIfNotExist(AzureTableStorageServiceContext.CustomFieldsTableName);
        }

        void CreateFileEntityTableIfNotExists(CloudStorageAccount storageAccount) {
            storageAccount.CreateCloudTableClient().CreateTableIfNotExist(AzureTableStorageServiceContext.FilesTableName);
        }

        void CreateVoteEntityTableIfNotExists(CloudStorageAccount storageAccount) {
            storageAccount.CreateCloudTableClient().CreateTableIfNotExist(AzureTableStorageServiceContext.VotesTableName);
        }

        void CreateFavoriteEntityTableIfNotExists(CloudStorageAccount storageAccount) {
            storageAccount.CreateCloudTableClient().CreateTableIfNotExist(AzureTableStorageServiceContext.FavoritesTableName);
        }

        void CreateAclEntityTableIfNotExists(CloudStorageAccount storageAccount)
        {
            storageAccount.CreateCloudTableClient().CreateTableIfNotExist(AzureTableStorageServiceContext.AclTableName);
        }

        void CreateCloudLogEntityTableIfNotExists(CloudStorageAccount storageAccount)
        {
            storageAccount.CreateCloudTableClient().CreateTableIfNotExist(AzureTableStorageServiceContext.CloudLogTableName);
        }

        public bool CreateTablesIfTheyDontExists()
        {
            var storageAccount = GetStorageAccount();
           
            CreateUserTableIfNotExists(storageAccount);
            CreateRoleTableIfNotExists(storageAccount);
            CreateSessionStateEntityIfNotExists(storageAccount);

            bool exists = CreatePageEntityTableIfNotExists(storageAccount);

            CreateCustomFieldEntityTableIfNotExists(storageAccount);

            CreateFileEntityTableIfNotExists(storageAccount);

            CreateVoteEntityTableIfNotExists(storageAccount);
            CreateFavoriteEntityTableIfNotExists(storageAccount);

            CreateAclEntityTableIfNotExists(storageAccount);

            CreateCloudLogEntityTableIfNotExists(storageAccount);

            return exists;
        }

        public AzureTableStorageDataSource()
        {
            InitServiceContext();
        }

        #region Generic operations

        /// <summary>
        /// List the tables in this storage account
        /// </summary>
        public IEnumerable<string> ListAllTables()
        {
            var storageAccount = GetStorageAccount();

            // Create service client for credentialed access to the Table service.
            var tableClient = new CloudTableClient(storageAccount.TableEndpoint.ToString(),
                storageAccount.Credentials);

            return tableClient.ListTables();
        }

        /// <summary>
        /// Get any AzureEntityBase table entity.
        /// Generic query.
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <param name="partitionKey">Partition key</param>
        /// <param name="applicationName"></param>
        /// <param name="mergeOption">Merge option</param>
        /// <returns>Entity data or null if not found</returns>
        /// <example>
        /// // Get some information about a customer
        /// var customer = new AzureTableStorageDataSource().GetAny<CustomerEntity>("Customers", "5e32379b-ab21-4d57-85ac-64f73556a42d", 
        ///     "www.example.com");
        /// // Show results
        /// Console.WriteLine(customer.Name + ", addr: " + customer.Address + ", joined: " + customer.CustomerSince);
        /// </example>
        public T GetAny<T>(string tableName, string partitionKey, string applicationName, MergeOption mergeOption = MergeOption.AppendOnly)
        {
            var storageAccount = GetStorageAccount();
            var tableEndpoint = storageAccount.TableEndpoint.AbsoluteUri;
            var tableServiceContext = new TableServiceContext(tableEndpoint, storageAccount.Credentials);
            
            var query = string.Format("{0}/{1}()?$filter={2}", tableEndpoint, tableName,
                    String.Format("PartitionKey eq '{0}'", partitionKey)
                );
    
            var queryResponse = tableServiceContext.Execute<T>(new Uri(query)) as QueryOperationResponse<T>;
            
            return queryResponse.ToList().FirstOrDefault();
        }

        /// <summary>
        /// Get any AzureEntityBase table entity.
        /// Generic query.
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <param name="applicationName"></param>
        /// <param name="filter">Returns only tables or entities that satisfy the specified filter. Note that no 
        /// more than 15 discrete comparisons are permitted.
        /// </param>
        /// <param name="select">Returns the desired properties of an entity from the set</param>
        /// <param name="properties">Returns only the top n tables or entities from the set. If not specified then all 
        /// entity properties are returned.</param>
        /// <param name="mergeOption">Merge options</param>
        /// <returns>Entity data or null if not found</returns>
        /// <remarks>Input parameters filter and properties must be URL encoded</remarks>
        /// <see cref="http://msdn.microsoft.com/library/azure/dd894031.aspx"/>
        /// <seealso cref="http://msdn.microsoft.com/en-us/library/azure/dd179421.aspx"/>
        /// <example>
        /// // Get some information about customers with a specific rating from the customers table
        /// var customers = new AzureTableStorageDataSource().GetAny<CustomerEntity>("Customers", "www.example.com",
        ///     "(Rating ge 3) and (Rating le 6)", new string[]{ PartitionKey, RowKey, Name, Address, CustomerSince } );
        /// 
        /// // Show results
        /// foreach(var customer in customers)
        ///     Console.WriteLine(customer.Name + ", addr: " + customer.Address + ", joined: " + customer.CustomerSince);
        /// </example>
        public IEnumerable<T> GetAny<T>(string tableName, string applicationName, string filter, string [] properties, 
            int top = 0, MergeOption mergeOption = MergeOption.AppendOnly)
        {
            var storageAccount = GetStorageAccount();
            var tableEndpoint = storageAccount.TableEndpoint.AbsoluteUri;
            var tableServiceContext = new TableServiceContext(tableEndpoint, storageAccount.Credentials);

            var query = String.Format("{0}/{1}()?$filter={2}", tableEndpoint, tableName, filter);
            if (properties != null && properties.Length > 0)
                query += "&$select=" + String.Join(",", properties);

            if (top > 0) query += "&$top=" + top;

            var queryResponse = tableServiceContext.Execute<T>(new Uri(query)) as QueryOperationResponse<T>;

            return queryResponse.ToList();
        }

        /// <summary>
        /// Generic update of a table entity
        /// </summary>
        /// <param name="entity">Entity data</param>
        /// <param name="tableName">Table name without the namespace</param>
        /// <param name="mergeOption">Merge option</param>
        public void Update<T>(T entity, string tableName, MergeOption mergeOption = MergeOption.AppendOnly)
        {
            _tableStorageServiceContext.MergeOption = mergeOption;
            _tableStorageServiceContext.Detach(entity);
            _tableStorageServiceContext.AttachTo(tableName, entity, "*");
            _tableStorageServiceContext.UpdateObject(entity);
            _tableStorageServiceContext.SaveChangesWithRetries();
        }

        #endregion

        #region User CRUD operations

        public IEnumerable<UserEntity> GetUsers(string applicationName, MergeOption mergeOption = MergeOption.AppendOnly)
        {
            _tableStorageServiceContext.MergeOption = mergeOption;
            var results = from c in _tableStorageServiceContext.Users
                          where c.ApplicationName == applicationName
                          select c;

            var query = results.AsTableServiceQuery();
            return query.Execute();
        }

        public UserEntity GetUser(string partitionKey, string applicationName, MergeOption mergeOption = MergeOption.AppendOnly)
        {
            _tableStorageServiceContext.MergeOption = mergeOption;
            var results = from c in _tableStorageServiceContext.Users
                          where c.PartitionKey == partitionKey && c.RowKey == String.Empty && c.ApplicationName == applicationName
                          select c;

            var query = results.AsTableServiceQuery();
            return query.Execute().FirstOrDefault();
        }

        public UserEntity FindUser(string username, string applicationName, MergeOption mergeOption = MergeOption.AppendOnly) {
            _tableStorageServiceContext.MergeOption = mergeOption;
            var results = from c in _tableStorageServiceContext.Users
                          where c.Username.Equals(username) && c.RowKey == String.Empty && c.ApplicationName == applicationName
                          select c;

            var query = results.AsTableServiceQuery();
            return query.Execute().FirstOrDefault();
        }

        public IEnumerable<UserEntity> FindUsers(string username, string applicationName, MergeOption mergeOption = MergeOption.AppendOnly)
        {
            _tableStorageServiceContext.MergeOption = mergeOption;
            var results = from c in _tableStorageServiceContext.Users
                          where c.Username.Contains(username) && c.RowKey == String.Empty && c.ApplicationName == applicationName
                          select c;

            var query = results.AsTableServiceQuery();
            return query.Execute();
        }

        public IEnumerable<UserEntity> FindInactiveUsers(string username, string applicationName, DateTime userInactiveSinceDate, MergeOption mergeOption = MergeOption.AppendOnly)
        {
            _tableStorageServiceContext.MergeOption = mergeOption;
            var results = from c in _tableStorageServiceContext.Users
                          where c.Username.Contains(username) && c.RowKey == String.Empty && c.ApplicationName == applicationName && c.LastActivityDate >= userInactiveSinceDate
                          select c;

            var query = results.AsTableServiceQuery();
            return query.Execute();
        }

        /// <summary>
        /// TODO: This method could be very slow with a lot of users, so look at making this faster somehow
        /// </summary>
        /// <param name="email">User e-mail</param>
        /// <returns></returns>
        public IEnumerable<UserEntity> GetUsersByEmail(string email, string applicationName, MergeOption mergeOption = MergeOption.AppendOnly)
        {
            _tableStorageServiceContext.MergeOption = mergeOption;
            var results = from c in _tableStorageServiceContext.Users
                          where c.Email == email && c.ApplicationName == applicationName
                          select c;

            var query = results.AsTableServiceQuery();
            return query.Execute();
        }

        public IEnumerable<UserEntity> GetOnlineUsers(DateTime compareTime, string applicationName, MergeOption mergeOption = MergeOption.AppendOnly)
        {
            _tableStorageServiceContext.MergeOption = mergeOption;
            var results = from c in _tableStorageServiceContext.Users
                          where c.LastActivityDate > compareTime && c.ApplicationName == applicationName
                          select c;

            var query = results.AsTableServiceQuery();
            return query.Execute();
        }

        public void Delete(UserEntity itemToDelete, MergeOption mergeOption = MergeOption.AppendOnly)
        {
            _tableStorageServiceContext.MergeOption = mergeOption;
            _tableStorageServiceContext.Detach(itemToDelete);
            _tableStorageServiceContext.AttachTo(AzureTableStorageServiceContext.UsersTableName, itemToDelete, "*");
            _tableStorageServiceContext.DeleteObject(itemToDelete);
            _tableStorageServiceContext.SaveChangesWithRetries();
        }

        public void Insert(UserEntity newItem, MergeOption mergeOption = MergeOption.AppendOnly)
        {
            _tableStorageServiceContext.MergeOption = mergeOption;
            _tableStorageServiceContext.AddObject(AzureTableStorageServiceContext.UsersTableName, newItem);
            _tableStorageServiceContext.SaveChangesWithRetries();
        }

        public void Update(UserEntity itemToUpdate, MergeOption mergeOption = MergeOption.AppendOnly)
        {
            _tableStorageServiceContext.MergeOption = mergeOption;
            _tableStorageServiceContext.Detach(itemToUpdate);
            _tableStorageServiceContext.AttachTo(AzureTableStorageServiceContext.UsersTableName, itemToUpdate, "*");
            _tableStorageServiceContext.UpdateObject(itemToUpdate);
            _tableStorageServiceContext.SaveChangesWithRetries();
        }

        #endregion

        #region Role CRUD operations

        public RoleEntity GetRole(string partitionKey, string applicationName, MergeOption mergeOption = MergeOption.AppendOnly)
        {
            _tableStorageServiceContext.MergeOption = mergeOption;
            var results = from c in _tableStorageServiceContext.Roles
                          where c.PartitionKey == partitionKey && c.ApplicationName == applicationName
                          select c;

            var query = results.AsTableServiceQuery();
            return query.Execute().FirstOrDefault();
        }

        public IEnumerable<RoleEntity> GetAllRoles(string applicationName, MergeOption mergeOption = MergeOption.AppendOnly)
        {
            _tableStorageServiceContext.MergeOption = mergeOption;
            var results = from c in _tableStorageServiceContext.Roles
                          where c.ApplicationName == applicationName
                          select c;

            var query = results.AsTableServiceQuery();
            return query.Execute();
        }

        public IEnumerable<RoleEntity> GetRolesForUser(string rowKey, string applicationName, MergeOption mergeOption = MergeOption.AppendOnly)
        {
            _tableStorageServiceContext.MergeOption = mergeOption;
            var results = from c in _tableStorageServiceContext.Roles
                          where c.RowKey == rowKey && c.ApplicationName == applicationName
                          select c;

            var query = results.AsTableServiceQuery();
            return query.Execute();
        }

        public IEnumerable<RoleEntity> GetUsersInRole(string partitionKey, string applicationName, MergeOption mergeOption = MergeOption.AppendOnly)
        {
            _tableStorageServiceContext.MergeOption = mergeOption;
            var results = from c in _tableStorageServiceContext.Roles
                          where c.PartitionKey == partitionKey && c.ApplicationName == applicationName
                          select c;

            var query = results.AsTableServiceQuery();
            return query.Execute();
        }

        public bool IsUserInRole(string partitionKey, string rowKey, string applicationName, MergeOption mergeOption = MergeOption.AppendOnly)
        {
            _tableStorageServiceContext.MergeOption = mergeOption;
            var results = from c in _tableStorageServiceContext.Roles
                          where c.PartitionKey == partitionKey && c.RowKey == rowKey && c.ApplicationName == applicationName
                          select c;

            var query = results.AsTableServiceQuery();
            var queryResult = query.Execute();

            return (queryResult.Count() > 0);
        }

        public bool IsUserInRoleByName(string username, string rolename, string applicationName, MergeOption mergeOption = MergeOption.AppendOnly)
        {
            _tableStorageServiceContext.MergeOption = mergeOption;
            var results = from c in _tableStorageServiceContext.Roles
                          where c.UserName == username && c.RoleName == rolename && c.ApplicationName == applicationName
                          select c;

            var query = results.AsTableServiceQuery();
            var queryResult = query.Execute();

            return (queryResult.Count() > 0);
        }

        public IEnumerable<RoleEntity> GetUserInRole(string partitionKey, string rowKey, string applicationName, MergeOption mergeOption = MergeOption.AppendOnly)
        {
            _tableStorageServiceContext.MergeOption = mergeOption;
            var results = from c in _tableStorageServiceContext.Roles
                          where c.PartitionKey == partitionKey && c.RowKey == rowKey && c.ApplicationName == applicationName
                          select c;

            var query = results.AsTableServiceQuery();
            return query.Execute();
        }

        public bool RoleExists(string partitionKey, string applicationName, MergeOption mergeOption = MergeOption.AppendOnly)
        {
            _tableStorageServiceContext.MergeOption = mergeOption;
            var results = from c in _tableStorageServiceContext.Roles
                          where c.PartitionKey == partitionKey && c.ApplicationName == applicationName
                          select c;

            var query = results.AsTableServiceQuery();
            return query.Execute().Count() > 0;
        }

        public bool RoleExistsByRoleName(string roleName, string applicationName, MergeOption mergeOption = MergeOption.AppendOnly)
        {
            _tableStorageServiceContext.MergeOption = mergeOption;
            var results = from c in _tableStorageServiceContext.Roles
                          where c.RoleName == roleName && c.ApplicationName == applicationName
                          select c;

            var query = results.AsTableServiceQuery();
            return query.Execute().Count() > 0;
        }

        public IEnumerable<RoleEntity> FindUsersInRole(string userNameToMatch, string applicationName, MergeOption mergeOption = MergeOption.AppendOnly)
        {
            _tableStorageServiceContext.MergeOption = mergeOption;
            var results = from c in _tableStorageServiceContext.Roles
                          where c.UserName == userNameToMatch && c.ApplicationName == applicationName
                          select c;

            var query = results.AsTableServiceQuery();
            return query.Execute();
        }

        public void DeleteUserInRole(string userNameToMatch, string applicationName, MergeOption mergeOption = MergeOption.AppendOnly)
        {
            _tableStorageServiceContext.MergeOption = mergeOption;
            var results = from c in _tableStorageServiceContext.Roles
                          where c.UserName == userNameToMatch && c.ApplicationName == applicationName
                          select c;

            foreach (var user in results)
                _tableStorageServiceContext.DeleteObject(user);

            _tableStorageServiceContext.SaveChangesWithRetries();
        }

        public void Delete(RoleEntity itemToDelete, MergeOption mergeOption = MergeOption.AppendOnly)
        {
            _tableStorageServiceContext.MergeOption = mergeOption;
            _tableStorageServiceContext.Detach(itemToDelete);
            _tableStorageServiceContext.AttachTo(AzureTableStorageServiceContext.RolesTableName, itemToDelete, "*");
            _tableStorageServiceContext.DeleteObject(itemToDelete);
            _tableStorageServiceContext.SaveChangesWithRetries();
        }

        public void Insert(RoleEntity newItem, MergeOption mergeOption = MergeOption.AppendOnly)
        {
            _tableStorageServiceContext.MergeOption = mergeOption;
            _tableStorageServiceContext.AddObject(AzureTableStorageServiceContext.RolesTableName, newItem);
            _tableStorageServiceContext.SaveChangesWithRetries();
        }

        public void Update(RoleEntity itemToUpdate, MergeOption mergeOption = MergeOption.AppendOnly)
        {
            _tableStorageServiceContext.MergeOption = mergeOption;
            _tableStorageServiceContext.Detach(itemToUpdate);
            _tableStorageServiceContext.AttachTo(AzureTableStorageServiceContext.RolesTableName, itemToUpdate, "*");
            _tableStorageServiceContext.UpdateObject(itemToUpdate);
            _tableStorageServiceContext.SaveChangesWithRetries();
        }
        #endregion

        #region Session state CRUD operations

        public IEnumerable<SessionStateEntity> GetSessions(string applicationName, MergeOption mergeOption = MergeOption.AppendOnly) {
            _tableStorageServiceContext.MergeOption = mergeOption;
            var results = from c in _tableStorageServiceContext.SessionState
                          where c.ApplicationName == applicationName
                          select c;

            var query = results.AsTableServiceQuery();
            return query.Execute();
        }

        public IEnumerable<SessionStateEntity> GetSessions(string sessionId, string applicationName, MergeOption mergeOption = MergeOption.AppendOnly)
        {
            _tableStorageServiceContext.MergeOption = mergeOption;
            var results = from c in _tableStorageServiceContext.SessionState
                          where c.PartitionKey == sessionId && c.RowKey == String.Empty && c.ApplicationName == applicationName
                          select c;

            var query = results.AsTableServiceQuery();
            return query.Execute();
        }

        public IEnumerable<SessionStateEntity> GetSessions(string sessionId, string applicationName, int lockId, MergeOption mergeOption = MergeOption.AppendOnly)
        {
            _tableStorageServiceContext.MergeOption = mergeOption;
            var results = from c in _tableStorageServiceContext.SessionState
                          where c.PartitionKey == sessionId && c.RowKey == String.Empty && c.ApplicationName == applicationName
                                && c.LockId == lockId
                          select c;

            var query = results.AsTableServiceQuery();
            return query.Execute();
        }

        public IEnumerable<SessionStateEntity> GetSessions(string sessionId, string applicationName, DateTime expires, MergeOption mergeOption = MergeOption.AppendOnly)
        {
            _tableStorageServiceContext.MergeOption = mergeOption;
            var results = from c in _tableStorageServiceContext.SessionState
                          where c.PartitionKey == sessionId && c.RowKey == String.Empty && c.ApplicationName == applicationName
                                && c.Expires > expires
                          select c;

            var query = results.AsTableServiceQuery();
            return query.Execute();
        }

        public IEnumerable<SessionStateEntity> GetSessions(string sessionId, string applicationName, bool isLocked, DateTime expires, MergeOption mergeOption = MergeOption.AppendOnly)
        {
            _tableStorageServiceContext.MergeOption = mergeOption;
            var results = from c in _tableStorageServiceContext.SessionState
                          where c.PartitionKey == sessionId && c.RowKey == String.Empty && c.ApplicationName == applicationName
                                && c.Locked == isLocked && c.Expires > expires
                          select c;

            var query = results.AsTableServiceQuery();
            return query.Execute();
        }

        public void Delete(SessionStateEntity itemToDelete, MergeOption mergeOption = MergeOption.AppendOnly)
        {
            _tableStorageServiceContext.MergeOption = mergeOption;
            _tableStorageServiceContext.Detach(itemToDelete);
            _tableStorageServiceContext.AttachTo(AzureTableStorageServiceContext.SessionStatesTableName, itemToDelete, "*");
            _tableStorageServiceContext.DeleteObject(itemToDelete);
            _tableStorageServiceContext.SaveChangesWithRetries();
        }

        public void Insert(SessionStateEntity newItem, MergeOption mergeOption = MergeOption.AppendOnly)
        {
            _tableStorageServiceContext.MergeOption = mergeOption;
            _tableStorageServiceContext.AddObject(AzureTableStorageServiceContext.SessionStatesTableName, newItem);
            _tableStorageServiceContext.SaveChangesWithRetries();
        }

        public void Update(SessionStateEntity itemToUpdate, MergeOption mergeOption = MergeOption.AppendOnly)
        {
            _tableStorageServiceContext.MergeOption = mergeOption;
            _tableStorageServiceContext.Detach(itemToUpdate);
            _tableStorageServiceContext.AttachTo(AzureTableStorageServiceContext.SessionStatesTableName, itemToUpdate, "*");
            _tableStorageServiceContext.UpdateObject(itemToUpdate);
            _tableStorageServiceContext.SaveChangesWithRetries();
        }

        /// <summary>
        /// Get all expired session-states
        /// TODO: This will be slow since it does a full table scan, consider adding a table indexed on expired to make this quicker
        /// </summary>
        /// <param name="applicationName">Application name</param>
        /// <param name="expires">Session-state expiration date</param>
        /// <param name="mergeOption">Client entity tracking merge option</param>
        /// <returns>Expired sessions</returns>
        public IEnumerable<SessionStateEntity> GetExpiredSessions(string applicationName, DateTime expires, MergeOption mergeOption = MergeOption.AppendOnly)
        {
            _tableStorageServiceContext.MergeOption = mergeOption;
            var results = from c in _tableStorageServiceContext.SessionState
                          where c.RowKey == String.Empty && c.ApplicationName == applicationName
                                && c.Expires <= expires
                          select c;

            var query = results.AsTableServiceQuery();
            return query.Execute();
        }
        #endregion

        #region Page CRUD operations

        public IEnumerable<PageEntity> GetPages(string partitionKey, string applicationName, MergeOption mergeOption = MergeOption.AppendOnly) {
            _tableStorageServiceContext.MergeOption = mergeOption;
            var results = from c in _tableStorageServiceContext.Page
                          where c.PartitionKey.StartsWith(partitionKey) && c.ApplicationName == applicationName
                          select c;

            var query = results.AsTableServiceQuery();
            return query.Execute();
        }

        public IEnumerable<PageEntity> GetPages(string applicationName, MergeOption mergeOption = MergeOption.AppendOnly) {
            _tableStorageServiceContext.MergeOption = mergeOption;
            var results = from c in _tableStorageServiceContext.Page
                          where c.ApplicationName == applicationName
                          select c;

            var query = results.AsTableServiceQuery();
            return query.Execute();
        }


        public PageEntity GetPage(string partitionKey, string applicationName, MergeOption mergeOption = MergeOption.AppendOnly) {
            _tableStorageServiceContext.MergeOption = mergeOption;
            var results = from c in _tableStorageServiceContext.Page
                          where c.PartitionKey == partitionKey && c.ApplicationName == applicationName
                          select c;

            var query = results.AsTableServiceQuery();
            return query.Execute().FirstOrDefault();
        }

        public PageEntity GetPage(string virtualPath, string rowKey, string applicationName, MergeOption mergeOption = MergeOption.AppendOnly)
        {
            _tableStorageServiceContext.MergeOption = mergeOption;
            var results = from c in _tableStorageServiceContext.Page
                          where c.PartitionKey == virtualPath && c.RowKey == rowKey && c.ApplicationName == applicationName
                          select c;

            var query = results.AsTableServiceQuery();
            return query.Execute().FirstOrDefault();
        }

        public bool PageExists(string partitionKey, string applicationName, MergeOption mergeOption = MergeOption.AppendOnly) {
            _tableStorageServiceContext.MergeOption = mergeOption;
            var results = from c in _tableStorageServiceContext.Page
                          where c.PartitionKey == partitionKey && c.ApplicationName == applicationName
                          select c;

            var query = results.AsTableServiceQuery();
            return query.Execute().Any();
        }

        public bool PageExists(string partitionKey, string rowKey, string applicationName, MergeOption mergeOption = MergeOption.AppendOnly) {
            _tableStorageServiceContext.MergeOption = mergeOption;
            var results = from c in _tableStorageServiceContext.Page
                          where c.PartitionKey == partitionKey && c.RowKey == rowKey && c.ApplicationName == applicationName
                          select c;

            var query = results.AsTableServiceQuery();
            return query.Execute().Any();
        }

        public void Insert(PageEntity newItem, MergeOption mergeOption = MergeOption.AppendOnly) {
            _tableStorageServiceContext.MergeOption = mergeOption;
            _tableStorageServiceContext.AddObject(AzureTableStorageServiceContext.PagesTableName, newItem);
            _tableStorageServiceContext.SaveChangesWithRetries();
        }

        public void Update(PageEntity itemToUpdate, MergeOption mergeOption = MergeOption.AppendOnly) {
            _tableStorageServiceContext.MergeOption = mergeOption;
            _tableStorageServiceContext.Detach(itemToUpdate);
            _tableStorageServiceContext.AttachTo(AzureTableStorageServiceContext.PagesTableName, itemToUpdate, "*");
            _tableStorageServiceContext.UpdateObject(itemToUpdate);
            _tableStorageServiceContext.SaveChangesWithRetries();
        }

        public void Delete(PageEntity itemToDelete, MergeOption mergeOption = MergeOption.AppendOnly)
        {
            _tableStorageServiceContext.MergeOption = mergeOption;
            _tableStorageServiceContext.Detach(itemToDelete);
            _tableStorageServiceContext.AttachTo(AzureTableStorageServiceContext.PagesTableName, itemToDelete, "*");
            _tableStorageServiceContext.DeleteObject(itemToDelete);
            _tableStorageServiceContext.SaveChangesWithRetries();
        }

        #endregion

        #region Access Control CRUD oeprations
        public void Insert(AccessControlEntity newItem, MergeOption mergeOption = MergeOption.AppendOnly)
        {
            _tableStorageServiceContext.MergeOption = mergeOption;
            _tableStorageServiceContext.AddObject(AzureTableStorageServiceContext.AclTableName, newItem);
            _tableStorageServiceContext.SaveChangesWithRetries();
        }

        public void Update(AccessControlEntity itemToUpdate, MergeOption mergeOption = MergeOption.AppendOnly)
        {
            _tableStorageServiceContext.MergeOption = mergeOption;
            _tableStorageServiceContext.Detach(itemToUpdate);
            _tableStorageServiceContext.AttachTo(AzureTableStorageServiceContext.AclTableName, itemToUpdate, "*");
            _tableStorageServiceContext.UpdateObject(itemToUpdate);
            _tableStorageServiceContext.SaveChangesWithRetries();
        }

        public void Delete(AccessControlEntity itemToDelete, MergeOption mergeOption = MergeOption.AppendOnly)
        {
            _tableStorageServiceContext.MergeOption = mergeOption;
            _tableStorageServiceContext.Detach(itemToDelete);
            _tableStorageServiceContext.AttachTo(AzureTableStorageServiceContext.AclTableName, itemToDelete, "*");
            _tableStorageServiceContext.DeleteObject(itemToDelete);
            _tableStorageServiceContext.SaveChangesWithRetries();
        }

        /// <summary>
        /// Check if a Access Control List rule exists
        /// </summary>
        /// <param name="partitionKey">Resource qualified name in the form [type]:[resource[(property)]].
        /// Ex. "TABLE:VeraPages" for a resource w/o a property and "TABLE:VeraUsers(Password)" with a property</param>
        /// <param name="rowKey">User or role qualified name in the form [USER|ROLE]:[name]</param>
        /// <param name="applicationName">Application name</param>
        /// <param name="mergeOption">Merge option</param>
        /// <returns>True if the Access Control List rule exists</returns>
        public bool AclRuleExists(string partitionKey, string rowKey, string applicationName, MergeOption mergeOption = MergeOption.AppendOnly)
        {
            _tableStorageServiceContext.MergeOption = mergeOption;
            var results = from c in _tableStorageServiceContext.AccessControlList
                          where c.PartitionKey == partitionKey && c.RowKey == rowKey && c.ApplicationName == applicationName
                          select c;

            var query = results.AsTableServiceQuery();
            return query.Execute().Any();
        }

        public IEnumerable<AccessControlEntity> GetAllRules(string applicationName, MergeOption mergeOption = MergeOption.AppendOnly)
        {
            _tableStorageServiceContext.MergeOption = mergeOption;
            var results = from c in _tableStorageServiceContext.AccessControlList
                          where c.ApplicationName == applicationName
                          select c;

            var query = results.AsTableServiceQuery();
            return query.Execute();
        }


        public IEnumerable<AccessControlEntity> GetRules(string partitionKey, string applicationName,
            MergeOption mergeOption = MergeOption.AppendOnly)
        {
            _tableStorageServiceContext.MergeOption = mergeOption;
            var results = from c in _tableStorageServiceContext.AccessControlList
                          where c.PartitionKey == partitionKey && c.ApplicationName == applicationName
                          select c;

            var query = results.AsTableServiceQuery();
            return query.Execute();
        }

        #endregion

        #region File CRUD operations

        public FileEntity GetFile(string partitionKey, string applicationName, MergeOption mergeOption = MergeOption.AppendOnly) {
            _tableStorageServiceContext.MergeOption = mergeOption;
            var results = from c in _tableStorageServiceContext.File
                          where c.PartitionKey == partitionKey && c.ApplicationName == applicationName
                          select c;

            var query = results.AsTableServiceQuery();
            return query.Execute().FirstOrDefault();
        }

        public IEnumerable<FileEntity> GetFiles(string applicationName, MergeOption mergeOption = MergeOption.AppendOnly) {
            _tableStorageServiceContext.MergeOption = mergeOption;
            var results = from c in _tableStorageServiceContext.File
                          where c.ApplicationName == applicationName
                          select c;

            var query = results.AsTableServiceQuery();
            return query.Execute();
        }

        public bool FileExists(string partitionKey, string applicationName, MergeOption mergeOption = MergeOption.AppendOnly) {
            _tableStorageServiceContext.MergeOption = mergeOption;
            var results = from c in _tableStorageServiceContext.File
                          where c.PartitionKey == partitionKey && c.ApplicationName == applicationName
                          select c;

            var query = results.AsTableServiceQuery();
            return query.Execute().Any();
        }

        public void Delete(FileEntity itemToDelete, MergeOption mergeOption = MergeOption.AppendOnly) {
            _tableStorageServiceContext.MergeOption = mergeOption;
            _tableStorageServiceContext.Detach(itemToDelete);
            _tableStorageServiceContext.AttachTo(AzureTableStorageServiceContext.FilesTableName, itemToDelete, "*");
            _tableStorageServiceContext.DeleteObject(itemToDelete);
            _tableStorageServiceContext.SaveChangesWithRetries();
        }

        public void Insert(FileEntity newItem, MergeOption mergeOption = MergeOption.AppendOnly) {
            _tableStorageServiceContext.MergeOption = mergeOption;
            _tableStorageServiceContext.AddObject(AzureTableStorageServiceContext.FilesTableName, newItem);
            _tableStorageServiceContext.SaveChangesWithRetries();
        }

        public void Update(FileEntity itemToUpdate, MergeOption mergeOption = MergeOption.AppendOnly) {
            _tableStorageServiceContext.MergeOption = mergeOption;
            _tableStorageServiceContext.Detach(itemToUpdate);
            _tableStorageServiceContext.AttachTo(AzureTableStorageServiceContext.FilesTableName, itemToUpdate, "*");
            _tableStorageServiceContext.UpdateObject(itemToUpdate);
            _tableStorageServiceContext.SaveChangesWithRetries();
        }
        #endregion

        #region Vote CRUD operations

        public VoteEntity GetVote(string partitionKey, string applicationName, MergeOption mergeOption = MergeOption.AppendOnly) {
            _tableStorageServiceContext.MergeOption = mergeOption;
            var results = from c in _tableStorageServiceContext.Vote
                          where c.PartitionKey == partitionKey && c.ApplicationName == applicationName
                          select c;

            var query = results.AsTableServiceQuery();
            return query.Execute().FirstOrDefault();
        }

        public IEnumerable<VoteEntity> GetVotes(string applicationName, MergeOption mergeOption = MergeOption.AppendOnly) {
            _tableStorageServiceContext.MergeOption = mergeOption;
            var results = from c in _tableStorageServiceContext.Vote
                          where c.ApplicationName == applicationName
                          select c;

            var query = results.AsTableServiceQuery();
            return query.Execute();
        }

        public void Delete(VoteEntity itemToDelete, MergeOption mergeOption = MergeOption.AppendOnly) {
            _tableStorageServiceContext.MergeOption = mergeOption;
            _tableStorageServiceContext.Detach(itemToDelete);
            _tableStorageServiceContext.AttachTo(AzureTableStorageServiceContext.VotesTableName, itemToDelete, "*");
            _tableStorageServiceContext.DeleteObject(itemToDelete);
            _tableStorageServiceContext.SaveChangesWithRetries();
        }

        public void Insert(VoteEntity newItem, MergeOption mergeOption = MergeOption.AppendOnly) {
            _tableStorageServiceContext.MergeOption = mergeOption;
            _tableStorageServiceContext.AddObject(AzureTableStorageServiceContext.VotesTableName, newItem);
            _tableStorageServiceContext.SaveChangesWithRetries();
        }

        public void Update(VoteEntity itemToUpdate, MergeOption mergeOption = MergeOption.AppendOnly) {
            _tableStorageServiceContext.MergeOption = mergeOption;
            _tableStorageServiceContext.Detach(itemToUpdate);
            _tableStorageServiceContext.AttachTo(AzureTableStorageServiceContext.VotesTableName, itemToUpdate, "*");
            _tableStorageServiceContext.UpdateObject(itemToUpdate);
            _tableStorageServiceContext.SaveChangesWithRetries();
        }
        #endregion

        #region Favorite CRUD operations

        public FavoriteEntity GetFavorite(string partitionKey, string applicationName, MergeOption mergeOption = MergeOption.AppendOnly) {
            _tableStorageServiceContext.MergeOption = mergeOption;
            var results = from c in _tableStorageServiceContext.Favorite
                          where c.PartitionKey == partitionKey && c.ApplicationName == applicationName
                          select c;

            var query = results.AsTableServiceQuery();
            return query.Execute().FirstOrDefault();
        }

        public IEnumerable<FavoriteEntity> GetFavorites(string applicationName, MergeOption mergeOption = MergeOption.AppendOnly) {
            _tableStorageServiceContext.MergeOption = mergeOption;
            var results = from c in _tableStorageServiceContext.Favorite
                          where c.ApplicationName == applicationName
                          select c;

            var query = results.AsTableServiceQuery();
            return query.Execute();
        }

        public IEnumerable<FavoriteEntity> GetFavoritesByUser(string rowKey, string applicationName, MergeOption mergeOption = MergeOption.AppendOnly) {
            _tableStorageServiceContext.MergeOption = mergeOption;
            var results = from c in _tableStorageServiceContext.Favorite
                          where c.RowKey == rowKey && c.ApplicationName == applicationName
                          select c;

            var query = results.AsTableServiceQuery();
            return query.Execute();
        }

        public void Delete(FavoriteEntity itemToDelete, MergeOption mergeOption = MergeOption.AppendOnly) {
            _tableStorageServiceContext.MergeOption = mergeOption;
            _tableStorageServiceContext.Detach(itemToDelete);
            _tableStorageServiceContext.AttachTo(AzureTableStorageServiceContext.FavoritesTableName, itemToDelete, "*");
            _tableStorageServiceContext.DeleteObject(itemToDelete);
            _tableStorageServiceContext.SaveChangesWithRetries();
        }

        public void Insert(FavoriteEntity newItem, MergeOption mergeOption = MergeOption.AppendOnly) {
            _tableStorageServiceContext.MergeOption = mergeOption;
            _tableStorageServiceContext.AddObject(AzureTableStorageServiceContext.FavoritesTableName, newItem);
            _tableStorageServiceContext.SaveChangesWithRetries();
        }

        public void Update(FavoriteEntity itemToUpdate, MergeOption mergeOption = MergeOption.AppendOnly) {
            _tableStorageServiceContext.MergeOption = mergeOption;
            _tableStorageServiceContext.Detach(itemToUpdate);
            _tableStorageServiceContext.AttachTo(AzureTableStorageServiceContext.FavoritesTableName, itemToUpdate, "*");
            _tableStorageServiceContext.UpdateObject(itemToUpdate);
            _tableStorageServiceContext.SaveChangesWithRetries();
        }
        #endregion

        #region CloudLog CRUD operations

        /// <summary>
        /// Clears all the log items
        /// </summary>
        /// <param name="applicationName">Application name</param>
        /// <param name="mergeOption">Merge option</param>
        public void ClearCloudLogEntries(string applicationName, MergeOption mergeOption = MergeOption.AppendOnly)
        {
            var logItems = GetCloudLogEntries(applicationName);
            foreach (var logItem in logItems)
            {
                _tableStorageServiceContext.DeleteObject(logItem);
                _tableStorageServiceContext.SaveChangesWithRetries();
            }
        }

        public IEnumerable<CloudLogEntity> GetCloudLogEntries(string applicationName, MergeOption mergeOption = MergeOption.AppendOnly)
        {
            _tableStorageServiceContext.MergeOption = mergeOption;
            var results = from c in _tableStorageServiceContext.CloudLog
                          where c.ApplicationName == applicationName
                          select c;

            var query = results.AsTableServiceQuery();
            return query.Execute().OrderByDescending(c => c.PartitionKey);
        }

        public CloudLogEntity GetCloudLogEntry(string partitionKey, string applicationName, MergeOption mergeOption = MergeOption.AppendOnly)
        {
            _tableStorageServiceContext.MergeOption = mergeOption;
            var results = from c in _tableStorageServiceContext.CloudLog
                          where c.PartitionKey == partitionKey && c.ApplicationName == applicationName
                          select c;

            var query = results.AsTableServiceQuery();
            return query.Execute().FirstOrDefault();
        }

        public void Delete(CloudLogEntity itemToDelete, MergeOption mergeOption = MergeOption.AppendOnly)
        {
            _tableStorageServiceContext.MergeOption = mergeOption;
            _tableStorageServiceContext.Detach(itemToDelete);
            _tableStorageServiceContext.AttachTo(AzureTableStorageServiceContext.CloudLogTableName, itemToDelete, "*");
            _tableStorageServiceContext.DeleteObject(itemToDelete);
            _tableStorageServiceContext.SaveChangesWithRetries();
        }

        public void Insert(CloudLogEntity newItem, MergeOption mergeOption = MergeOption.AppendOnly)
        {
            _tableStorageServiceContext.MergeOption = mergeOption;
            _tableStorageServiceContext.AddObject(AzureTableStorageServiceContext.CloudLogTableName, newItem);
            _tableStorageServiceContext.SaveChangesWithRetries();
        }

        public void Update(CloudLogEntity itemToUpdate, MergeOption mergeOption = MergeOption.AppendOnly)
        {
            _tableStorageServiceContext.MergeOption = mergeOption;
            _tableStorageServiceContext.Detach(itemToUpdate);
            _tableStorageServiceContext.AttachTo(AzureTableStorageServiceContext.CloudLogTableName, itemToUpdate, "*");
            _tableStorageServiceContext.UpdateObject(itemToUpdate);
            _tableStorageServiceContext.SaveChangesWithRetries();
        }
        #endregion

        #region Windows Event log CRUD operations


        /// <summary>
        /// Clears all the log items
        /// </summary>
        /// <param name="mergeOption">Merge option</param>
        public void ClearWindowsEventLogEntries(MergeOption mergeOption = MergeOption.AppendOnly)
        {
            var logItems = GetWindowsEventLogEntries();
            foreach (var logItem in logItems)
            {
                _tableStorageServiceContext.DeleteObject(logItem);
                _tableStorageServiceContext.SaveChangesWithRetries();
            }
        }

        public IEnumerable<WADWindowsEventLogEntity> GetWindowsEventLogEntries(MergeOption mergeOption = MergeOption.AppendOnly)
        {
            _tableStorageServiceContext.MergeOption = mergeOption;
            var results = from c in _tableStorageServiceContext.WindowsEventLog
                          select c;

            var query = results.AsTableServiceQuery();
            return query.Execute().OrderByDescending(c => c.PartitionKey);
        }

        public WADWindowsEventLogEntity GetWindowsEventLogEntry(string partitionKey, MergeOption mergeOption = MergeOption.AppendOnly)
        {
            _tableStorageServiceContext.MergeOption = mergeOption;
            var results = from c in _tableStorageServiceContext.WindowsEventLog
                          where c.PartitionKey == partitionKey 
                          select c;

            var query = results.AsTableServiceQuery();
            return query.Execute().FirstOrDefault();
        }

        #endregion

    }
}