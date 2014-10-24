using System;
using System.Text;

namespace VeraWAF.WebPages.Bll {
    public class UriUtilities {

        /// <summary>
        /// Get the base URI of a URL.
        /// </summary>
        /// <param name="absoluteUri">Absolute URL</param>
        /// <returns>Base URI</returns>
        /// <example>
        /// // Write http://www.example.com to the console
        /// Console.WriteLine(new UriUtilities().GetBase("http://www.example.com/subdir/test.txt"));
        /// </example>
        public Uri GetBase(Uri absoluteUri) {
            return new Uri(absoluteUri.Scheme + "://" + absoluteUri.Authority);
        }

        /// <summary>
        /// Converts a absolute URL to a relative one.
        /// </summary>
        /// <param name="absoluteUri"></param>
        /// <returns>Relative URL</returns>
        /// <example>
        /// // Write /subdir/test.txt to the console
        /// Console.WriteLine(new UriUtilities().ConvertAbsoluteToRelativeUri("http://www.example.com/subdir/test.txt"));
        /// </example>
        public Uri ConvertAbsoluteToRelativeUri(Uri absoluteUri) {
            return absoluteUri.IsAbsoluteUri ? new Uri(absoluteUri.PathAndQuery, UriKind.Relative) : absoluteUri;
        }

        /// <summary>
        /// Get the parent URL
        /// </summary>
        /// <param name="uri">URL</param>
        /// <returns>Parent URL</returns>
        /// <example>
        /// // Write http://www.example.com/a/b/ to the console
        /// Console.WriteLine(new UriUtilities().GetBase("http://www.example.com/a/b/test.txt"));
        /// </example>
        public Uri GetParentUri(Uri uri)
        {
            var uriString = uri.ToString().Replace("/default.aspx", String.Empty);
            if (uriString.EndsWith("/"))
                uriString = uriString.Substring(0, uriString.Length - 1);

            var segments = uriString.Split('/');
            var sb = new StringBuilder("/");
            for (var i = 1; i < segments.Length - 1; i++)
                sb.AppendFormat("{0}/", segments[i]);

            var parentUrl = sb.ToString();
            if (!parentUrl.EndsWith(".aspx")) parentUrl += "default.aspx";

            return new Uri(parentUrl, UriKind.RelativeOrAbsolute);
        }
    }
}