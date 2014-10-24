using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Services.Client;
using System.Linq;
using System.Net;

namespace VeraWAF.AzureTableStorage
{
    /// <summary>
    /// Use this class to extend Vera with your own Azure table data queries
    /// </summary>
    public class AzureTableStorageDataSourceExt : AzureTableStorageDataSource
    {
        /// <summary>
        /// Example extention that get all the users that subscribe to newsletters.
        /// You can freely remove this example as it is not in real use anywhere.
        /// </summary>
        /// <param name="applicationName">Application name</param>
        /// <param name="mergeOption">Data merge option</param>
        /// <returns>All the users that subscribe to newsletters</returns>
        public IEnumerable<UserEntity> GetNewsletterUsers(string applicationName, MergeOption mergeOption = MergeOption.AppendOnly)
        {
            _tableStorageServiceContext.MergeOption = mergeOption;
            var results = from c in _tableStorageServiceContext.Users
                          where c.ApplicationName == applicationName && c.Newsletter == true
                          select c;

            var query = results.AsTableServiceQuery();
            return query.Execute();
        }

    }
}
