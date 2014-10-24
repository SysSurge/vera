using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Security;
using System.Xml;
using VeraWAF.CrossCuttingConcerns;
using VeraWAF.WebPages.Bll.Security.OAuth;

namespace VeraWAF.WebPages.Bll.Cloud
{
    /// <summary>
    /// Handles command requests to all nodes in the Azure cloud solution.
    /// </summary>
    public class CloudCommandClient
    {
        /// <summary>
        /// Sends a cloud command to a specific host using its internal address
        /// </summary>
        /// <param name="host">Hostname or IP address</param>
        /// <param name="command">Cloud command</param>
        /// <param name="protocol">(Optional)Host protocol, ex. "http". Default value is "http".</param>
        /// <param name="socket">(Optional)Host socket, ex. 80 for the defatul HTTP protocol socket. Default value is 80.</param>
        /// <param name="impersonateAdmin">(Optional)Impersonate the default admin when sending commands. Default value is true.</param>
        void SendHttpRequest(string username, string host, string command, string protocol = "http", int socket = 80, bool impersonateAdmin = true)
        {
            string param;

            // Build the target url
            var url = String.Format("{3}://{0}:{4}/Interfaces/RestApi.svc/CloudCommand/username/{2}/command/{1}",
                host, command, username, protocol, socket);

            // Should we impersonate the default admin when sending cloud commands?
            if (impersonateAdmin)
                username = ConfigurationManager.AppSettings["AdminName"];

            var userEntity = new Dal.UserCache().GetUser(username);
            if (userEntity == null)
                throw new ApplicationException("User not found");

            var consumerKey = userEntity.OAuthConsumerKey;
            var consumerSecret = userEntity.OAuthConsumerSecret;
            var method = "GET";

            // URI to REST API method
            var uri = new Uri(url);

            // Create the consumer side OAuth signature
            var signature = new OAuthUtils().GetSignature(consumerKey, consumerSecret, uri, method, out url, out param);

            // Call the REST API method MyMethod
            var httpRequest = WebRequest.Create(String.Format("{0}?{1}&oauth_signature={2}", url, param, signature));
            var httpResponse = httpRequest.GetResponse();

            // Get the HTTP response from the host and show it in the console
            var stream = new StreamReader(httpResponse.GetResponseStream());
            var json = stream.ReadToEnd();

#if DEBUG
            // Log command
            new LogEvent().AddEvent(ELogEventTypes.Info, string.Format("Response after sending cloud command \"{0}\" to {1}: {2}",
                command, uri.AbsolutePath, json == null ? "NULL" : json), ConfigurationManager.AppSettings["ApplicationName"]);
#endif
        }

        /// <summary>
        /// Sends a command to all the nodes in the cloud
        /// </summary>
        /// <param name="command">Command to send</param>
        /// <param name="skipCurrentInstance">Set to true to skip sending the command to the current node</param>
        /// <param name="impersonateAdmin">If true then the default admin is impersonated, if false the current user context is used</param>
        public void SendCommand(string command, bool skipCurrentInstance = false, bool impersonateAdmin = true)
        {
            MembershipUser user;

            // Should we impersonate the default admin when sending cloud commands?
            if (impersonateAdmin)
            {
                // Yes, impersonate the default admin user
                var username = ConfigurationManager.AppSettings["AdminName"];
                user = Membership.GetUser(username);
            }
            else
            {
                // No, use the current user
                user = Membership.GetUser();
            }

            if (user == null)
                throw new ApplicationException("Not signed in");

            foreach (RoleInstance roleInst in RoleEnvironment.CurrentRoleInstance.Role.Instances)
            {
                // Only send the command to other web roles that the one that issues the command?
                if (skipCurrentInstance && RoleEnvironment.CurrentRoleInstance.Id == roleInst.Id)
                    continue;

                if (roleInst.Role.Name == "Www")
                {
                    foreach (RoleInstanceEndpoint roleInstEndpoint in roleInst.InstanceEndpoints.Values.Where( ep => ep.Protocol == "tcp" ))
                    {
                        try
                        {
                            // Get endpoint address using the internal endpoint's IP address
                            new LogEvent().AddEvent(ELogEventTypes.Info, string.Format("Sending cloud command \"{0}\" to {1}",
                                command,roleInstEndpoint.IPEndpoint.Address.ToString()), ConfigurationManager.AppSettings["ApplicationName"]);

                            var uri = HttpContext.Current.Request.Url;


                            // Send the command to the node
                            SendHttpRequest(user.UserName, roleInstEndpoint.IPEndpoint.Address.ToString(), command, uri.Scheme,
                                uri.Port);
                        }
                        catch (Exception ex)
                        {
                            new LogEvent().AddEvent(ELogEventTypes.Error, string.Format("Could not send cloud command to {0}:{1}. {2}",
                                roleInstEndpoint.IPEndpoint.Address.ToString(), roleInstEndpoint.Protocol, ex.ToString()),
                                ConfigurationManager.AppSettings["ApplicationName"]);
                        }
                    }
                }
            }
        }

    }
}