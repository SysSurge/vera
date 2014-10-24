using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Security;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.ServiceModel.Web;
using System.Web.Hosting;
using System.Xml;
using VeraWAF.WebPages.Bll;
using VeraWAF.WebPages.Bll.Security.OAuth;
using VeraWAF.WebPages.Dal;
using VeraWAF.WebPages.Dal.Interchange;

namespace VeraWAF.WebPages.Interfaces
{
    /*
     * RESTful MVC search interface that works with both HTTP and HTTPS and uses JSON as an interchange format
     * WARNING: Don't turn ASP.NET compatibility mode on - did some tests and RESTful MVC scale badly.
     */
#if DEBUG
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerCall,
         IncludeExceptionDetailInFaults = true, MaxItemsInObjectGraph = 1000)]
#else
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerCall,
         IncludeExceptionDetailInFaults = false, MaxItemsInObjectGraph = 1000)]
#endif
    public class RestApiExt : IRestApiExt
    {
        /// <summary>
        /// Check if the REST APIs are enabled
        /// </summary>
        /// <returns>True if the REST APIs are enabled or False if they are disabled</returns>
        bool AreRestApisEnabled() {
            bool enableRestApis;
            return bool.TryParse(ConfigurationManager.AppSettings["EnableRestApis"], out enableRestApis) && enableRestApis;
        }

        /// <summary>
        /// OAuth REST API example
        /// </summary>
        /// <param name="username">User name</param>
        /// <returns>JSON success message if OAuth works</returns>
        //public string Custom1(string username)
        //{
        //    // Make sure the user is authorized using OAuth before continuing
        //    if (!AreRestApisEnabled() || !new OAuthUtils().Authenticate(username, WebOperationContext.Current.IncomingRequest))
        //        throw new WebFaultException<string>("Unauthorized access", HttpStatusCode.Unauthorized);

        //    // Success, so return some dummy JSON data to let the user know the authentication completed successfully
        //    var json = "{\"WOPR\":\"Greetings, " + username + ". Shall we play a game? How about a nice game of chess?\", \"Timestamp\":\""
        //        + DateTime.UtcNow.ToString("o") + "\"}";

        //    return json;
        //}
    }
}
