using System;
using System.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Yahoo.Yui.Compressor;

namespace VeraWAF.WebPages.Bll
{
    /// <summary>
    /// Compresses & merges JavaScript and/or CSS files.
    /// Will only compress & merge in release mode; in debug mode all files are linked independently
    /// </summary>
    public class PageCompressor
    {
        void AddDataToCache(string key, string data)
        {
            var cacheSlidingExpiration = new RuntimeConfiguration().GetVirtualFileCacheSlidingExpiration();

            HostingEnvironment.Cache.Add(key, data, null,
              Cache.NoAbsoluteExpiration,
              cacheSlidingExpiration,
              CacheItemPriority.Default, null);
        }

        /// <summary>
        /// Returns the latest release date
        /// </summary>
        /// <returns>Site latest release date</returns>
        string GetLatestReleaseDate()
        {
            return ConfigurationManager.AppSettings["rel"];
        }

        /// <summary>
        /// Replaces all CompressScript elements fromt the header with compressed and inline or linked script files
        /// </summary>
        /// <param name="inCompressor">YUICompress.NET compressor</param>
        /// <param name="inPath">Url</param>
        /// <param name="header">Current element container</param>
        /// <param name="newHeader">New element container</param>
        /// <param name="baseWebDir">Physical path to web directory</param>
        /// <param name="pseudoTagName">Tag name, example "CompressScripts" or "CompressStyles"</param>
        /// <param name="formattedHtmlMarkup">Formatted HTML markup</param>
        /// <param name="inlineMarkup">Inline markup</param>
        /// <param name="linkMarkup">Link markup</param>
        /// <param name="baseFolder">Base folder</param>
        public void CompressFiles(ICompressor inCompressor, string inPath, Control header, Control newHeader, string baseWebDir, 
            string pseudoTagName, string formattedHtmlMarkup, string inlineMarkup, string linkMarkup, string basePath)
        {
            if (header == null) return;

            var addControls = new List<LiteralControl>();
            var removeControls = new List<HtmlGenericControl>();

            foreach (var control in header.Controls)
            {
                var inType = control.GetType();
                if (inType.Name == "HtmlGenericControl")
                {
                    var compressBlockElement = control as HtmlGenericControl;
                    if (compressBlockElement == null || compressBlockElement.TagName != pseudoTagName) continue;

                    // We need to remove the pseudo element from the HTML head element
                    removeControls.Add(compressBlockElement);

                    // Show the generated HTML markup in this literal control
                    var scriptBlock = new LiteralControl();
#if !DEBUG
                    // Holds a cache key
                    string cacheKey;

                    var compressedFile = compressBlockElement.Attributes["CompressedFile"];
                    if (compressedFile == null || String.IsNullOrWhiteSpace(compressedFile))
                    {
                        // Create a cache key
                        cacheKey = inPath;
                    }
                    else
                    {
                        // Create a cache key
                        cacheKey = compressedFile;
                    }

                    // Check if the content is cached already
                    var cachedContent = HostingEnvironment.Cache.Get(cacheKey) as string;
#else
                    var cachedContent = String.Empty;
#endif
                    if (String.IsNullOrEmpty(cachedContent))
                    {
                        // Not cached, so create content
#if DEBUG
                        var scriptElementsHtml = new StringBuilder();
#else
                        var compressedScript = new StringBuilder(String.Format("/*! Readme: /Terms.aspx . Rel: {0} UTC */", DateTime.UtcNow));
#endif
                        var scriptLinks = compressBlockElement.InnerHtml.Split(
                            new string[] { Environment.NewLine, "\n\r", "\r\n", "\n", " " }, StringSplitOptions.RemoveEmptyEntries);
#if DEBUG
                        foreach (var scriptLink in scriptLinks)
                        {
                            scriptElementsHtml.AppendFormat(formattedHtmlMarkup, basePath + scriptLink, GetLatestReleaseDate()
                                , Environment.NewLine);
                        }

                        scriptBlock.Text = scriptElementsHtml.ToString();
#else
                        foreach (var scriptLink in scriptLinks)
                        {
                            var tmpFilePath = (baseWebDir + basePath + scriptLink).Trim();

                            // Make sure that the file exists, otherwise we just skip it
                            if (File.Exists(tmpFilePath))
                            {
                                // File exists, so compress it
                                var uncompressedScript = File.ReadAllText(tmpFilePath);
                                compressedScript.Append(inCompressor.Compress(uncompressedScript));
                            }
                        }

                        // Check if inline script or file
                        string htmlMarkup;

                        if (compressedFile == null || String.IsNullOrWhiteSpace(compressedFile))
                        {
                            // Inline script
                            htmlMarkup = String.Format(inlineMarkup, compressedScript.ToString());
                        }
                        else
                        {
                            // Generate script file
                            var fullUrl = basePath + compressedFile;
                            fullUrl = fullUrl.Trim().Replace("//", "/");
                            htmlMarkup = String.Format(linkMarkup, fullUrl, GetLatestReleaseDate());

                            // Create physical file name
                            var fullPath = basePath + compressedFile;
                            var compressedFileName = baseWebDir + fullPath;
                            compressedFileName = compressedFileName.Replace('/', '\\').Trim().Replace("\\\\", "\\");

                            // Write file to disk
                            File.WriteAllText(compressedFileName, compressedScript.ToString());
                        }

                        // Show the HTML markup
                        scriptBlock.Text = htmlMarkup;

                        // Add the HTML markup to cache for later fast lookup
                        AddDataToCache(cacheKey, htmlMarkup);
#endif
                    }
                    else
                    {
                        // Use the cached content
                        scriptBlock.Text = cachedContent;
                    }

                    addControls.Add(scriptBlock);
                }
            }

            // Add controls
            foreach (var control in addControls)
                newHeader.Controls.Add(control);

            // Remove controls
            foreach (var control in removeControls)
                header.Controls.Remove(control);
        }

    }
}
