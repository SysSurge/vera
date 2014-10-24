using System;
using System.Web;

namespace VeraWAF.WebPages.Bll
{
    public class ServerTools
    {
        public string GetClientIpAddress()
        {
            var context = HttpContext.Current;

            // If ipBehindProxy contains one or several IP's then remoteAddress will hold the IP address of a Proxy,
            // otherwise remoteAddress will be the IP of the client.
            var remoteAddress = context.Request.ServerVariables["REMOTE_ADDR"];
            if (String.IsNullOrEmpty(remoteAddress)) remoteAddress = "Spoofed";

            var ipBehindProxy = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            return String.IsNullOrEmpty(ipBehindProxy) ? String.Format("Client={0}", remoteAddress) : String.Format("Client={0} (Proxy={1})", ipBehindProxy, remoteAddress);
        }
    }
}