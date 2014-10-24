using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace VeraWAF.AzureTableStorage
{
    /// <summary>
    /// Use this class to extend Vera with your own Azure tables
    /// </summary>
    public class AzureTableStorageServiceContextExt : AzureTableStorageServiceContext
    {
        public AzureTableStorageServiceContextExt() : base()
        {
            // Intentionally empty
        }

        public AzureTableStorageServiceContextExt(string baseAddress, StorageCredentials credentials) : base(baseAddress, credentials)
        {
            // Intentionally empty
        }

        /// <summary>
        /// Example custom table name.
        /// You can safely delte this as it is only an example and not used anywhere
        /// </summary>
        public const string CustomTableName1 = "CustomTable1";

        /// <summary>
        /// Allows for Azure table queries against CustomTable1.
        /// You can safely delte this as it is only an example and not used anywhere
        /// </summary>
        public IQueryable<CustomTable1Entity> CustomTable1Entity
        {
            get
            {
                return CreateQuery<CustomTable1Entity>(CustomTableName1);
            }
        }

    }
}
