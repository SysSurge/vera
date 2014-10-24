using System;
using Microsoft.WindowsAzure.StorageClient;

namespace VeraWAF.AzureTableStorage
{
    public class CloudLogEntity : AzureEntityBase
    {
        public string Message { get; set; }
    }
}
