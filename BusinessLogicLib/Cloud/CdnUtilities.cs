using System;
using System.Configuration;
using System.Web;

namespace VeraWAF.WebPages.Bll.Cloud {
    public class CdnUtilities {
        public string GetCdnUrl(string url)
        {
            var blobHttpEndpoint = ConfigurationManager.AppSettings["BlobHttpEndpoint"];
            if (url.StartsWith(blobHttpEndpoint))
                url = ConfigurationManager.AppSettings["CdnHttpEndpoint"] + url.Substring(blobHttpEndpoint.Length);
            else return url;

            // Make sure that we don't mix HTTP and HTTPS
            url = HttpContext.Current.Request.Url.Scheme + "://" + url;

            return url;
        }
    }
}