using System;
using System.Collections.Generic;


namespace VeraWAF.WebPages.Dal.Interchange
{
    /// <summary>
    /// Generic result from a query
    /// </summary>
    [Serializable]
    public class GenericResult
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public GenericResult(bool result = true)
        {
            success = result;
            timestamp = DateTime.UtcNow;
        }

        /// <summary>
        /// True if success, false if error
        /// </summary>
        public bool success;

        /// <summary>
        /// UTC Timestamp
        /// </summary>
        public DateTime timestamp;

        /// <summary>
        /// Generic object container
        /// </summary>
        object[] objects; 
    }
}
