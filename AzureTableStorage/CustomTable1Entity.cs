using System;
using Microsoft.WindowsAzure.StorageClient;

namespace VeraWAF.AzureTableStorage
{
    /// <summary>
    /// Example data entity for example Azure table CustomTable1.
    /// You can safely delete this as it is not in use anywhere
    /// </summary>
    public class CustomTable1Entity : AzureEntityBase
    {
        /// <summary>
        /// Custom field example
        /// </summary>
        public int CustomField1 { get; set; }

    }
}

