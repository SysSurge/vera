using System;
using Microsoft.WindowsAzure.StorageClient;

namespace VeraWAF.AzureTableStorage
{
    public class CustomFieldEntity : AzureEntityBase
    {
        public int Value { get; set; }
    }
}
