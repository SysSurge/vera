using System;
using System.Runtime.Serialization;

namespace VeraWAF.WebPages.Dal.Interchange
{
    /// <summary>
    /// Information about a Azure table storage property.
    /// </summary>
    [DataContract]
    public class TablePropertyInfo
    {
        /// <summary>
        /// Azure table name. Ex. "VeraUsers"
        /// </summary>
        [DataMember]
        public string TableName { get; set; }

        /// <summary>
        /// Entity type. Ex. "UserEntity"
        /// </summary>
        [DataMember]
        public string EntityType { get; set; }

        /// <summary>
        /// Azure table row partition key that identifies the page
        /// </summary>
        [DataMember]
        public string PartitionKey { get; set; }

        /// <summary>
        /// Azure table row property name. Ex. "Username"
        /// </summary>
        [DataMember]
        public string PropertyName { get; set; }

        /// <summary>
        /// Azure table row property value. Ex. "Jill"
        /// </summary>
        [DataMember]
        public string PropertyValue { get; set; }
    }
}
