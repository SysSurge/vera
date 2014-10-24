using System;
using Microsoft.WindowsAzure.StorageClient;

namespace VeraWAF.AzureTableStorage
{
    public class FavoriteEntity : AzureEntityBase
    {
        /// <summary>
        /// Holds a favorite for something
        /// </summary>
        /// <param name="partitionKey">Something that identifies the favorite</param>
        /// <param name="rowKey">User partition key</param>
        public FavoriteEntity(string partitionKey, string rowKey) : base(partitionKey, rowKey)
        {
        }

        public FavoriteEntity()
            : this(Guid.NewGuid().ToString(), String.Empty)
        {
        }

    }
}
