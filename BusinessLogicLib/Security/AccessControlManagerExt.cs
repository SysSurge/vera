using System;
using VeraWAF.WebPages.Dal.Interchange;

namespace VeraWAF.WebPages.Bll.Security
{
    public class AccessControlManagerExt
    {

        /// <summary>
        /// Check if a user has enough permissions to read a user profile property
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="propertyName">VeraUsers property name that is to be edited</param>
        /// <returns>True if the user can edit the property</returns>
        public bool UserHasProfileReadPermissions(string username, string propertyName)
        {
            // Add your own code here
            return false;
        }

        /// <summary>
        /// Check if a user has enough permissions to edit a user profile property
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="partitionKey">Partition key that identifies the row in the VeraUsers table</param>
        /// <param name="propertyName">VeraUsers property name that is to be edited</param>
        /// <returns>True if the user can edit the property</returns>
        public bool UserHasProfileEditPermissions(string username, string partitionKey, string propertyName)
        {
            // Add your own code here
            return false;
        }

        /// <summary>
        /// Check if a user has edit permissions to a Azure table row
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="fieldInfo">Azure table property information</param>
        /// <returns>True if the user has edit permissions to the Azure table row</returns>
        public bool UserHasEditPermissions(string username, TablePropertyInfo fieldInfo)
        {
            // Add your own code here
            return false;
        }

        public bool UserHasReadPermissions(string username, GenericTableQuery query)
        {
            // Add your own code here
            return false;
        }
    }
}
