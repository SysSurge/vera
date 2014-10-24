using System;
using System.IO;
using System.Net;

namespace VeraWAF.WebPages.Bll.OAuth.OAuthTester
{
    /// <summary>
    /// Tester for VeraWAF OAuth authentication.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            string param;

            Console.WriteLine("Enter OAuth enabled REST API URL(ex. \"http://www.example.com/Interfaces/RestApiExt.svc/Custom1/username/Admin\"): ");
            var url = Console.ReadLine();

            Console.WriteLine("Enter HTTP method(GET or POST):");
            var method = Console.ReadLine();

            Console.WriteLine("Enter user consumer key from the VeraUsers table: ");
            var consumerKey = Console.ReadLine();

            Console.WriteLine("Enter user consumer secret from the VeraUsers table: ");
            var consumerSecret = Console.ReadLine();

            //var url = "http://127.0.0.1:81/Interfaces/RestApiExt.svc/Custom1/username/Admin";
            //var consumerKey = "0a1b255b-3f36-4137-92b4-d6b2af2073fc";
            //var consumerSecret = "ecb5190c-ff41-4f85-93f9-6dfdccb5e959";
            //var method = "GET";

            // URI to REST API method
            var uri = new Uri(url);

            // Create the consumer side OAuth signature
            var signature = new OAuthUtils().GetSignature(consumerKey, consumerSecret, uri, method, out url, out param);

            // Call the REST API method MyMethod
            var httpRequest = WebRequest.Create(String.Format("{0}?{1}&oauth_signature={2}", url, param, signature));
            var httpResponse = httpRequest.GetResponse();

            // Get the HTTP response from the host and show it in the console
            var stream = new StreamReader(httpResponse.GetResponseStream());

            Console.WriteLine(stream.ReadToEnd());

            Console.WriteLine("Done. Enter to exit.");
            Console.ReadLine();
        }
    }
}
