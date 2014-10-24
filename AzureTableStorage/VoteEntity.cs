using System;
using Microsoft.WindowsAzure.StorageClient;

namespace VeraWAF.AzureTableStorage
{
    public class VoteEntity : AzureResourceEntity
    {
        /// <summary>
        /// Holds a vote for something
        /// </summary>
        /// <param name="partitionKey">Something that identifies what was voted for</param>
        /// <param name="rowKey">User partition key</param>
        public VoteEntity(string partitionKey, string rowKey) : base(partitionKey, rowKey)
        {
        }

        public VoteEntity()
            : this(Guid.NewGuid().ToString(), String.Empty)
        {
        }

        public int Value { get; set; }
    }
}
