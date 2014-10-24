using System;
using Microsoft.WindowsAzure.StorageClient;

namespace VeraWAF.AzureTableStorage
{
    public class RoleEntity : AzureEntityBase
    {
        public RoleEntity(string partitionKey, string rowKey) : base(partitionKey, rowKey)
        {
            CreationDate = DateTime.UtcNow;
        }

        public RoleEntity()
            : this(Guid.NewGuid().ToString(), String.Empty)
        {
            CreationDate = DateTime.UtcNow;
        }

        /// <summary>
        /// User in the role
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Name of role
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// Comment about the role
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Role creation date
        /// </summary>
        public DateTime CreationDate { get; set; }
    }

}
