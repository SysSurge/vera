using System;
using System.Collections.Specialized;
using System.ServiceModel.Web;
using System.Web;
using VeraWAF.WebPages.Dal;

namespace VeraWAF.WebPages.Bll.Security.OAuth
{
    /// <summary>
    /// OAuth utility class.
    /// </summary>
    public class OAuthUtils
    {
        /// <summary>
        /// Authenticate a user using his/her OAuth consumer key and secret.
        /// Will try and match the user's entity data consumer key from the VeraUsers table, and the OAuth
        /// signature.
        /// </summary>
        /// <param name="username">User name that is making the request</param>
        /// <param name="context">Current web context</param>
        /// <returns>True if the user was authenticated, or false if authentication failed</returns>
        /// <example>
        ///     // REST API method.
        ///     public string Custom1(string userId)
        ///     {
        ///         // Make sure the user is authorized using OAuth before continuing
        ///         if (!new OAuthUtils().Authenticate(userId, WebOperationContext.Current.IncomingRequest))
        ///             throw new WebFaultException<string>("Unauthorized access", HttpStatusCode.Unauthorized);
        ///
        ///         // Success, so return some JSON
        ///         var json = "{\"Result\":\"Hello " + userId + "\"}";
        ///         return json;
        ///     }
        /// </example>
        public bool Authenticate(string username, IncomingWebRequestContext context)
        {
            // Get the user entity data
            var user = new UserCache().GetUser(username);

            // User exists?
            if (user == null)
                return false;   // Not a known user, so we can't authenticate

            var isAuthenticated = false;

            var pa = context.UriTemplateMatch.QueryParameters;
            if (pa != null && pa["oauth_consumer_key"] != null)
            {
                // Make sure we use the external address and not the internal enpoint address
                var endPointUri = context.UriTemplateMatch.RequestUri;
                var originalUrl = endPointUri.Scheme + "://" + context.Headers["Host"] + endPointUri.PathAndQuery;

                // Get URI without the OAuth parameters
                var uri = originalUrl.Replace(context.UriTemplateMatch.RequestUri.Query, String.Empty);

                // Get the user's OAuth consumer secret
                var consumersecret = user.OAuthConsumerSecret;

                // Generate the OAuth signature
                string normalizedUrl;
                string normalizedRequestParameters;

                var signature = new OAuthBase().GenerateSignature(
                        new Uri(uri),
                        pa["oauth_consumer_key"],
                        consumersecret,
                        null, // token
                        null, //token secret
                        context.Method,
                        pa["oauth_timestamp"],
                        pa["oauth_nonce"],
                        out normalizedUrl,
                        out normalizedRequestParameters
                    );
                
                // Does the OAuth signatures match? If not then the authentication failed
                isAuthenticated = pa["oauth_signature"] == HttpUtility.UrlDecode(signature);
            }

            return isAuthenticated;
        }

        /// <summary>
        /// Generate a OAuth signature from the consumer side.
        /// </summary>
        /// <param name="consumerKey">Consumer key</param>
        /// <param name="consumerSecret">Consumer secret</param>
        /// <param name="uri">Absolute URL to resource being accessed</param>
        /// <param name="requestMethod">Request method. Ex. "GET" or "POST"</param>
        /// <param name="signatureType">Signature type</param>
        /// <param name="url">Normalized URL to resource</param>
        /// <param name="param">Normalized HTTP request parameters</param>
        /// <returns>The signatures</returns>
        /// <example>
        ///     string param;
        ///
        ///     Console.WriteLine("Enter OAuth enabled REST API URL(ex. \"http:///www.example.com/Interfaces/RestApiExt.svc/Custom1?userId=Admin\"). "
        ///         + "Note: Don't use localhost (ie. \"127.0.0.1:81\") addresses in the development environment as they are translated by Azure to "
        ///         + "your local endpoint and system name (ie. \"a2123-9023:82\"): ");
        ///     var url = Console.ReadLine();
        ///
        ///     Console.WriteLine("Enter HTTP method(GET or POST):");
        ///     var method = Console.ReadLine();
        ///
        ///     Console.WriteLine("Enter consumer key: ");
        ///     var consumerKey = Console.ReadLine();
        ///
        ///     Console.WriteLine("Enter consumer secret: ");
        ///     var consumerSecret = Console.ReadLine();
        ///
        ///     /// URI to REST API method
        ///     var uri = new Uri(url);
        ///
        ///     /// Create the consumer side OAuth signature
        ///     var signature = new OAuthUtils().GetSignature(consumerKey, consumerSecret, uri, method, out url, out param);
        ///
        ///     /// Call the REST API method MyMethod
        ///     var httpRequest = WebRequest.Create(String.Format("{0}?{1}&oauth_signature={2}", url, param, signature));
        ///     var httpResponse = httpRequest.GetResponse();
        ///
        ///     /// Get the HTTP response from the host and show it in the console
        ///     var stream = new StreamReader(httpResponse.GetResponseStream());
        ///
        ///     Console.WriteLine(stream.ReadToEnd());
        ///
        ///     Console.WriteLine("Done. Enter to exit.");
        ///     Console.ReadLine();
        /// </example>
        public string GetSignature(string consumerKey, string consumerSecret, Uri uri, string requestMethod,
            out string url, out string param, OAuthBase.SignatureTypes signatureType = OAuthBase.SignatureTypes.HMACSHA1)
        {
            var oAuth = new OAuthBase();
            var nonce = oAuth.GenerateNonce();
            var timeStamp = oAuth.GenerateTimeStamp();
            
            var signature = oAuth.GenerateSignature(uri, consumerKey,
                consumerSecret, String.Empty, String.Empty, requestMethod, timeStamp, nonce,
                OAuthBase.SignatureTypes.HMACSHA1, out url, out param);

            return signature;
        }
    }
}
