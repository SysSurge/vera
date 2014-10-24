using System;
using VeraWAF.WebPages.Dal.Interchange;

namespace VeraWAF.WebPages.Bll
{
    /// <summary>
    /// Extended table storage clients
    /// </summary>
    public class TableStorageClientEx
    {
        /// <summary>
        /// Updates a property value in a gerneric Azure table storage table
        /// </summary>
        /// <param name="fieldData">Azure table field data</param>
        /// <param name="applicationName">Application name</param>
        /// <returns>Entity data for a custom table</returns>
        public object Update(TablePropertyInfo fieldData, string applicationName)
        {
            switch (fieldData.TableName)
            {
                //case "CustomTable1":
                //    return some_logic();
                default:
                    return null;
            }
        }
    }
}
