using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeraWAF.AzureTableStorage
{
    /// <summary>
    /// Access control entity
    /// PrimaryKey is something that uniquely identifies the resource, like a class path.
    /// Ex. "VeraWAF.AzureTableStorage.PageEntity" or "VeraWAF.AzureTableStorage.UserEntity.Password"
    /// RowKey is the user or role qualified name
    /// </summary>
    public class AccessControlEntity : AzureEntityBase
    {
        /// <summary>
        /// Description of resources
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Permissions
        /// </summary>
        /// <see cref="VeraWAF.WebPages.Bll.Security.EPermission"/>
        public int Permissions { get; set; }

    }
}
