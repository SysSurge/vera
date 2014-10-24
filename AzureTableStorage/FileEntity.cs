using System;
using Microsoft.WindowsAzure.StorageClient;

namespace VeraWAF.AzureTableStorage
{
    public class FileEntity : AzureEntityBase
    {
        public string Description { get; set; }
        public string Url { get; set; }        
    }
}
