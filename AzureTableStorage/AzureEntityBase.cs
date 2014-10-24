using System;
using Microsoft.WindowsAzure.StorageClient;

namespace VeraWAF.AzureTableStorage
{
    /// <summary>
    /// Base class for Azure entities
    /// </summary>
    public class AzureEntityBase : TableServiceEntity
    {
        public AzureEntityBase(string partitionKey, string rowKey) : base(partitionKey, rowKey)
        {
        }

        public AzureEntityBase() : this(Guid.NewGuid().ToString(), String.Empty)
        {
        }

        /// <summary>
        /// Application name from the "ApplicationName" key in Web.Config file
        /// </summary>
        public string ApplicationName { get; set; }

    }
}
