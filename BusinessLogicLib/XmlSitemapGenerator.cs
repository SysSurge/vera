using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web.Hosting;
using System.Xml;
using VeraWAF.AzureTableStorage;
using VeraWAF.WebPages.Dal;

namespace VeraWAF.WebPages.Bll {
    public class XmlSitemapGenerator
    {
        const string XmlNamespace = "http://www.sitemaps.org/schemas/sitemap/0.9";
        IEnumerable<PageEntity> _pages;

        List<PageEntity> GetSiteMapPages() {
            return new List<PageEntity>(
                new PageCache().GetAllPages().Where(page => page.IsPublished && String.IsNullOrWhiteSpace(page.ParentRowKey))
                );
        }

        public XmlSitemapGenerator()
        {
            _pages = GetSiteMapPages();
        }

        float GetPagePriority(PageEntity page)
        {
            float priority;
            
            if (page.VirtualPath == "/" || page.VirtualPath.Equals("/default.aspx", StringComparison.InvariantCultureIgnoreCase))
                priority = 1.0f;
            else if (page.VirtualPath.EndsWith("/default.aspx", StringComparison.InvariantCultureIgnoreCase))
                priority = 0.8f;
            else if (page.VirtualPath.EndsWith(".html", StringComparison.InvariantCultureIgnoreCase)
                || page.VirtualPath.EndsWith(".htm", StringComparison.InvariantCultureIgnoreCase))
                priority = 0.1f;
            else priority = 0.5f;

            return priority;
        }

        string GetChangeFrequency(PageEntity page)
        {
            string changeFreqText;
            var pageLastModifiedTimeSpan = new TimeSpan(DateTime.UtcNow.Ticks - page.Timestamp.Ticks);

            if (pageLastModifiedTimeSpan.TotalHours <= 24)
                changeFreqText = "hourly";
            else if (pageLastModifiedTimeSpan.TotalDays <= 7)
                changeFreqText = "daily";
            else if (pageLastModifiedTimeSpan.TotalDays <= 30)
                changeFreqText = "weekly";
            else if (pageLastModifiedTimeSpan.TotalDays <= 365)
                changeFreqText = "monthly";
            else changeFreqText = "yearly";

            return changeFreqText;
        }

        void ProcessSiteMapUrls(XmlDocument xmlDocument, XmlElement rootXmlNode, string baseUrl)
        {
            foreach (var page in _pages) {
                var urlXmlNode = xmlDocument.CreateElement("url", XmlNamespace);
                rootXmlNode.AppendChild(urlXmlNode);

                var locXmlNode = xmlDocument.CreateElement("loc", XmlNamespace);
                locXmlNode.InnerText = baseUrl + page.VirtualPath;
                urlXmlNode.AppendChild(locXmlNode);

                var lastModXmlNode = xmlDocument.CreateElement("lastmod", XmlNamespace);
                lastModXmlNode.InnerText = new DateUtilities().GetCustomIso8601Date(page.Timestamp);
                urlXmlNode.AppendChild(lastModXmlNode);

                var changeFreqText = GetChangeFrequency(page);

                var changeFreqXmlNode = xmlDocument.CreateElement("changefreq", XmlNamespace);
                changeFreqXmlNode.InnerText = changeFreqText;
                urlXmlNode.AppendChild(changeFreqXmlNode);

                var priority = GetPagePriority(page);

                var priorityXmlNode = xmlDocument.CreateElement("priority", XmlNamespace);
                priorityXmlNode.InnerText = String.Format(new CultureInfo(ConfigurationManager.AppSettings["SiteCulture"]), 
                    "{0:0.#}", priority);
                urlXmlNode.AppendChild(priorityXmlNode);
            }
        }

        public void GenerateFile(string baseUrl)
        {
            // Remove any trailing /'s
            if (baseUrl.EndsWith("/")) baseUrl = baseUrl.Substring(0, baseUrl.Length - 1);

            var xmlDocument = new XmlDocument();
            var xmlDecleration = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlDocument.AppendChild(xmlDecleration);

            var rootXmlNode = xmlDocument.CreateElement("urlset", XmlNamespace);
            xmlDocument.AppendChild(rootXmlNode);

            ProcessSiteMapUrls(xmlDocument, rootXmlNode, baseUrl);

            xmlDocument.Save(HostingEnvironment.ApplicationPhysicalPath + "sitemap.xml");
        }
    }
}