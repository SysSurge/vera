using System;
using System.Configuration;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web;
using VeraWAF.AzureTableStorage;
using VeraWAF.WebPages.Bll;
using VeraWAF.WebPages.Bll.Security;
using VeraWAF.WebPages.Bll.Security.OAuth;
using VeraWAF.WebPages.Bll.Search;
using VeraWAF.WebPages.Dal.Interchange;

namespace VeraWAF.WebPages.Interfaces
{
    /*
     * RESTful MVC search interface that works with both HTTP and HTTPS and uses JSON as an interchange format
     * WARNING: Don't turn ASP.NET compatibility mode on as it causes RESTful MVC to scale badly.
     */
#if DEBUG
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerCall,
         IncludeExceptionDetailInFaults = true, MaxItemsInObjectGraph = 1000)]
#else
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerCall,
         IncludeExceptionDetailInFaults = false, MaxItemsInObjectGraph = 1000)]
#endif
    public class RestApi : IRestApi
    {
        /// <summary>
        /// Check if the REST APIs are enabled
        /// </summary>
        /// <returns>True if the REST APIs are enabled or False if they are disabled</returns>
        bool AreRestApisEnabled()
        {
            // Enable REST APIs?
            bool enableRestApis;
            return bool.TryParse(ConfigurationManager.AppSettings["EnableRestApis"], out enableRestApis) && enableRestApis;
        }

        /// <summary>
        /// Search the Web Content Management system
        /// </summary>
        /// <param name="queryTerms">Query</param>
        /// <returns>JSON with the query results as a VeraWAF.WebPages.Dal.Interchange.QueryResults data structure</returns>
        public string Query(string queryTerms) {
            if (!AreRestApisEnabled())
                throw new WebFaultException<string>("Unauthorized access", HttpStatusCode.Unauthorized);

            var queryResults = new SearchQueryHelper().ProcessQuery(queryTerms);
            return new Interchange().JsonSerialize(queryResults);
        }

        /// <summary>
        /// Get the current processor usage and available internal memory resources
        /// </summary>
        /// <param name="apiKey">API key</param>
        /// <param name="samplingIntervalMs">Sampling interval in milliseconds</param>
        /// <returns>JSON with the diagnostics as a VeraWAF.WebPages.Bll.DiagnosticHelper.DiagnosticStatistics data structure</returns>
        public string PerformanceCounter(string apiKey, int samplingIntervalMs)
        {
            // Make sure the user is authorized using the API key before continuing
            if (!AreRestApisEnabled() || ConfigurationManager.AppSettings["API_Key"] != apiKey)
                throw new WebFaultException<string>("Unauthorized access", HttpStatusCode.Unauthorized);

            var performanceStats = new DiagnosticHelper().GetPerformanceStatistics(samplingIntervalMs);

            return new Interchange().JsonSerialize(performanceStats);
        }

        /// <summary>
        /// Run a cloud command.
        /// Uses OAuth authentication and looks for a match with username's consumer key & secret.
        /// Caller must be a member of the admins role.
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="command">Command to execute</param>
        /// <returns>JSON with the result as a VeraWAF.WebPages.Bll.CloudCommand.CloudCommandResult structure</returns>
        public string CloudCommand(string username, string command) {
            return CloudCommandArgs(username, command, null);
        }

        /// <summary>
        /// Run a cloud command with arguments
        /// Uses OAuth authentication and looks for a match with username's consumer key & secret.
        /// Caller must be a member of the admins role.
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="command">Command to execute</param>
        /// <param name="args">Arguments</param>
        /// <returns>JSON with the result as a VeraWAF.WebPages.Bll.CloudCommand.CloudCommandResult structure</returns>
        public string CloudCommandArgs(string username, string command, string args)
        {
            // Make sure the user is authorized using OAuth before continuing. User must also be an administrator
            if (!AreRestApisEnabled() || !new OAuthUtils().Authenticate(username, WebOperationContext.Current.IncomingRequest)
                || !new AccessControlManager().UserIsAdmin(username))
                throw new WebFaultException<string>("Unauthorized access", HttpStatusCode.Unauthorized);

            var argsArr = args.Split(',');

            // Execute cloud command on this cloud node instance
            var result = new Bll.Cloud.CloudCommand().Execute(command, argsArr);

            // Return a success message as JSON
            return new Interchange().JsonSerialize(result);
        }

        /// <summary>
        /// Creates a Shared Access Signature (SAS) that will allow the user to upload a file to the Azure blob.
        /// Caller must be a admin or an editor 
        /// Uses OAuth to authenticate the user using consumer keys & secrets.
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="containerAddress">Blob container address. Ex. "publicfiles"</param>
        /// <param name="folder">Subfolder. Ex. "archive"</param>
        /// <returns>JSON message with timestamp if success</returns>
        public string GetBlobUploadUrl(string username, string containerAddress, string folder)
        {
            // Make sure the user is authorized using OAuth before continuing. User must also be an administrator or an editor
            if (!AreRestApisEnabled() || !new OAuthUtils().Authenticate(username, WebOperationContext.Current.IncomingRequest)
                || (!new AccessControlManager().UserIsAdmin(username) && !new AccessControlManager().UserIsEditor(username)))
                throw new WebFaultException<string>("Unauthorized access", HttpStatusCode.Unauthorized);

            // Create a upload URL with a Shared Access Signature (SAS)
            var sasUploadUrl = new Bll.Cloud.CloudUtils().GetBlobUploadUrl(containerAddress, folder);

            // Return the SAS upload URL as JSON
            return new Interchange().JsonSerialize(sasUploadUrl);
        }

        /// <summary>
        /// Updates a table property.
        /// Uses OAuth for authentification
        /// </summary>
        /// <param name="username">OAuth username</param>
        /// <param name="fieldData">Azure table storage field data</param>
        /// <returns>JSON with the result as a VeraWAF.WebPages.Dal.Interchange.GenericResult data structure</returns>
        public string Update(string username, TablePropertyInfo fieldData)
        {
            // Make sure the user is authorized using OAuth before continuing. User must also be an administrator or an editor
            if (!AreRestApisEnabled() || !new OAuthUtils().Authenticate(username, WebOperationContext.Current.IncomingRequest)
                || !new AccessControlManager().UserHasEditPermissions(username, fieldData))
                throw new WebFaultException<string>("Unauthorized access", HttpStatusCode.Unauthorized);

            // Get Azure table entity type
            var entityType = Type.GetType(
                    String.Format("VeraWAF.AzureTableStorage.{0}, VeraWAF.AzureTableStorage", fieldData.EntityType)
                );

            // Update the table
            MethodInfo method = typeof(TableStorageClient).GetMethod("UpdateGeneric");
            MethodInfo generic = method.MakeGenericMethod(entityType);
            generic.Invoke(new TableStorageClient(), new object[]{ entityType, fieldData, ConfigurationManager.AppSettings["ApplicationName"] });

            //new TableStorageClient().Update<entityType>(entityType, fieldData, ConfigurationManager.AppSettings["ApplicationName"]);

            return new Interchange().JsonSerialize(new GenericResult());
        }

        public string Select(string username, GenericTableQuery query)
        {
            // Make sure the user is authorized using OAuth before continuing
            if (!AreRestApisEnabled() || !new OAuthUtils().Authenticate(username, WebOperationContext.Current.IncomingRequest)
                || !new AccessControlManager().UserHasReadPermissions(username, query))
                throw new WebFaultException<string>("Unauthorized access", HttpStatusCode.Unauthorized);

            return new Interchange().JsonSerialize(new GenericResult());
        }

    }

}
