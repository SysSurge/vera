using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace VeraWAF.AzureTableStorage
{
    public class AzureTableStorageServiceContext : TableServiceContext
    {
        public const string UsersTableName = "VeraUsers";
        public const string RolesTableName = "VeraRoles";
        public const string SessionStatesTableName = "VeraSessionStates";
        public const string PagesTableName = "VeraPages";
        public const string FilesTableName = "VeraFiles";
        public const string VotesTableName = "VeraVotes";
        public const string FavoritesTableName = "VeraFavorites";
        public const string CloudLogTableName = "VeraCloudLog";
        public const string CustomFieldsTableName = "VeraCustomFields";
        public const string AclTableName = "VeraAccessControlList";
        public const string WindowsEventLogsTableName = "WADWindowsEventLogsTable";

        public AzureTableStorageServiceContext() : base(string.Empty, null)
        {
            // Intentionally empty
        }

        public AzureTableStorageServiceContext(string baseAddress, StorageCredentials credentials)
            : base(baseAddress, credentials)
        {
            // Intentionally empty
        }

        /// <summary>
        /// Allows for Azure table queries against VeraUsers table.
        /// </summary>
        public IQueryable<UserEntity> Users
        {
            get
            {
                return CreateQuery<UserEntity>(UsersTableName);
            }
        }

        /// <summary>
        /// Allows for Azure table queries against VeraRoles table.
        /// </summary>
        public IQueryable<RoleEntity> Roles
        {
            get
            {
                return CreateQuery<RoleEntity>(RolesTableName);
            }
        }

        /// <summary>
        /// Allows for Azure table queries against VeraSessionStates table.
        /// </summary>
        public IQueryable<SessionStateEntity> SessionState
        {
            get
            {
                return CreateQuery<SessionStateEntity>(SessionStatesTableName);
            }
        }

        /// <summary>
        /// Allows for Azure table queries against VeraPages table.
        /// </summary>
        public IQueryable<PageEntity> Page
        {
            get {
                return CreateQuery<PageEntity>(PagesTableName);
            }
        }

        /// <summary>
        /// Allows for Azure table queries against VeraFiles table.
        /// </summary>
        public IQueryable<FileEntity> File
        {
            get {
                return CreateQuery<FileEntity>(FilesTableName);
            }
        }

        /// <summary>
        /// Allows for Azure table queries against VeraVotes table.
        /// </summary>
        public IQueryable<VoteEntity> Vote
        {
            get {
                return CreateQuery<VoteEntity>(VotesTableName);
            }
        }

        /// <summary>
        /// Allows for Azure table queries against VeraFavorites table.
        /// </summary>
        public IQueryable<FavoriteEntity> Favorite
        {
            get {
                return CreateQuery<FavoriteEntity>(FavoritesTableName);
            }
        }

        /// <summary>
        /// Allows for Azure table queries against VeraCloudLog table.
        /// </summary>
        public IQueryable<CloudLogEntity> CloudLog
        {
            get {
                return CreateQuery<CloudLogEntity>(CloudLogTableName);
            }
        }

        /// <summary>
        /// Allows for Azure table queries against VeraCustomFields table.
        /// </summary>
        public IQueryable<CustomFieldEntity> CustomFields
        {
            get
            {
                return CreateQuery<CustomFieldEntity>(CustomFieldsTableName);
            }
        }

        /// <summary>
        /// Allows for Azure table queries against VeraAccessControlList table.
        /// </summary>
        public IQueryable<AccessControlEntity> AccessControlList
        {
            get
            {
                return CreateQuery<AccessControlEntity>(AclTableName);
            }
        }

        /// <summary>
        /// Allows for Azure table queries against WADWindowsEventLogsTable table.
        /// </summary>
        public IQueryable<WADWindowsEventLogEntity> WindowsEventLog
        {
            get
            {
                return CreateQuery<WADWindowsEventLogEntity>(WindowsEventLogsTableName);
            }
        }

    }
}