using System;
using System.Configuration;
using System.Net;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;

namespace VeraWAF.AzureQueue
{
    public class AzureQueueContext
    {
        /// <summary>
        /// E-mail Azure queue name
        /// </summary>
        public const string EmailQueueName = "email";

        /// <summary>
        /// Azure Cloud Storage account
        /// </summary>
        public CloudStorageAccount Account { get; private set; }

        /// <summary>
        /// Azure Cloud Queue client object
        /// </summary>
        public CloudQueueClient QueueClient { get; private set; }

        /// <summary>
        /// Retry policy when the queue fails a task.
        /// Ex. when an e-mail recipient is not available the queue will wait for
        /// "QueueDequeueRetrySleepSeconds" setting seconds before retrying to send the e-mail again.
        /// </summary>
        /// <returns>Retry policy</returns>
        RetryPolicy GetRetryPolicy()
        {
            // Retry after one hour if fails
            return RetryPolicies.Retry(3, TimeSpan.FromSeconds(
                    int.Parse(ConfigurationManager.AppSettings["QueueDequeueRetrySleepSeconds"]))
                );            
        }

        /// <summary>
        /// Get settings from a hosted service configuration or .NET configuration file
        /// </summary>
        /// <param name="configurationSettingName">Name of .NET configuration file settings</param>
        /// <param name="hostedService">If true then settings from the hosted service is used</param>
        public AzureQueueContext(string configurationSettingName, bool hostedService)
        {
            if (hostedService)
            {
                // Use settings from the hosted service
                CloudStorageAccount.SetConfigurationSettingPublisher(
                    (configName, configSettingPublisher) =>
                    {
                        var connectionString = RoleEnvironment.GetConfigurationSettingValue(configName);
                        configSettingPublisher(connectionString);
                    }
                );
            }
            else
            {
                // Use the settings from the configuration
                CloudStorageAccount.SetConfigurationSettingPublisher(
                    (configName, configSettingPublisher) =>
                    {
                        var connectionString = ConfigurationManager.ConnectionStrings[configName].ConnectionString;
                        configSettingPublisher(connectionString);
                    }
                );
            }

            // Get the Azure Cloud storage account
            Account = CloudStorageAccount.FromConfigurationSetting(configurationSettingName);

            // Create a cloud queue client object
            QueueClient = Account.CreateCloudQueueClient();

            // Get the queue retry policy
            QueueClient.RetryPolicy = GetRetryPolicy();
        }

        /// <summary>
        /// Handle HTTP nagling.
        /// PUT HTTP requests smaller than 1460 bytes are ineffecient with nagling turned on.
        /// See: http://blogs.msdn.com/b/windowsazurestorage/archive/2010/06/25/nagle-s-algorithm-is-not-friendly-towards-small-requests.aspx
        /// </summary>
        void HandleNagleOnEndpoint(CloudStorageAccount account)
        {
            var queueServicePoint = ServicePointManager.FindServicePoint(account.QueueEndpoint);
            queueServicePoint.UseNagleAlgorithm = bool.Parse(ConfigurationManager.AppSettings["UseNaglingWithQueue"]);
        }

        /// <summary>
        /// Get settings from a cloud storage account
        /// </summary>
        /// <param name="connectionString">Connection string to the cloud storage account</param>
        public AzureQueueContext(string connectionString)
        {
            // Get the cloud storage account
            Account = CloudStorageAccount.Parse(connectionString);

            // Set HTTP nagling
            HandleNagleOnEndpoint(Account);

            // Create a cloud queue client object
            QueueClient = Account.CreateCloudQueueClient();

            // Get the queue retry policy
            QueueClient.RetryPolicy = GetRetryPolicy();
        }
    }
}