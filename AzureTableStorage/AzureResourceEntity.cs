using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeraWAF.AzureTableStorage
{
    /// <summary>
    /// Azure resource entity that has an ownership attached to it.
    /// </summary>
    public class AzureResourceEntity : AzureEntityBase
    {
        public AzureResourceEntity(string partitionKey, string rowKey) : base(partitionKey, rowKey)
        {
        }

        public AzureResourceEntity() : this(Guid.NewGuid().ToString(), String.Empty)
        {
        }

        /// <summary>
        /// Resource owner
        /// </summary>
        public string Owner { get; set; }

    }
}
