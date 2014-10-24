using System;
using Microsoft.WindowsAzure.StorageClient;

namespace VeraWAF.AzureTableStorage
{
    public class SessionStateEntity : AzureEntityBase
    {
        public SessionStateEntity(string partitionKey, string rowKey) : base(partitionKey, rowKey)
        {
        }

        public SessionStateEntity()
            : this(Guid.NewGuid().ToString(), String.Empty)
        {
        }

        public DateTime Expires { get; set; }
        public DateTime LockDate { get; set; }
        public int LockId { get; set; }
        public int TimeOut { get; set; }
        public bool Locked { get; set; }
        public string SessionItems { get; set; }
        public int Flags { get; set; }

    }
}
