using System;
using System.Runtime.Serialization;

namespace VeraWAF.WebPages.Dal.Interchange
{
    /// <summary>
    /// Generic Azure Table Storage query information
    /// </summary>
    [DataContract]
    public class GenericTableQuery
    {
        /// <summary>
        /// Table name. Ex. "Customers"
        /// </summary>
        [DataMember]
        public string TableName;

        /// <summary>
        /// Query filter. Ex. "(Rating ge 3) and (Rating le 6)"
        /// </summary>
        /// <remarks>Returns only tables or entities that satisfy the specified filter. Note that no 
        /// more than 15 discrete comparisons are permitted.</remarks>
        [DataMember]
        public string Filter;

        /// <summary>
        /// Table property names. Ex. { PartitionKey, RowKey, Name, Address, CustomerSince }.
        /// If not specified then all entity properties are returned.
        /// </summary>
        /// <remarks>Returns the desired properties of an entity from the set</remarks>
        [DataMember]
        public string[] Properties;

        /// <summary>
        /// Number of results. 0 means all.
        /// </summary>
        /// <remarks>Returns only the top n tables or entities from the set</remarks>
        [DataMember]
        public int Top;
    }
}
