using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;
using System.Web.Security;
using System.Web.UI;
using System.Xml;
using VeraWAF.AzureTableStorage;
using HtmlAgilityPack;
using VeraWAF.WebPages.Bll;
using VeraWAF.WebPages.Dal;

namespace VeraWAF.WebPages {
    public partial class Syndication : Page {
        private const int TimeToLiveMinutes = 10;
        private const int DefaultMaxNumberOfItemsInRssFeed = 100;
        private readonly FileManager _fileManager;

        public Syndication()
        {
            _fileManager = new FileManager();
        }

        string GetPublishedDate(PageEntity page) {
            return page.Timestamp.ToString("R");
        }

        void OutputRssItemPublishedDate(PageEntity page, XmlWriter writer) {
            var publishedDate = GetPublishedDate(page);
            if (publishedDate == null) return;

            writer.WriteStartElement("pubDate");

            writer.WriteString(publishedDate);

            writer.WriteEndElement();
        }

        void OutputRssItemAuthor(PageEntity page, XmlWriter writer)
        {
            if (!String.IsNullOrWhiteSpace(page.Author))
            {
                var user = Membership.GetUser(page.Author);
                if (user != null)
                {
                    var authorFullName = new UserUtilities().GetDisplayName(user.UserName);
                    var authorEmail = user.Email.EndsWith("@example.com") ? user.Email : "anonymous@example.com";
                    var authorText = String.Format("{0} ({1})", authorEmail, authorFullName);

                    writer.WriteStartElement("author");

                    writer.WriteCData(authorText);

                    writer.WriteEndElement();
                }
            }
        }

        string GetImageMimeType(string url) {
            var imageParts = url.Split('.');
            return string.Format("image/{0}", imageParts[imageParts.Length - 1].ToLower().Replace("jpg", "jpeg"));
        }

        string GetSiteUrl()
        {
            return Request.Url.Scheme + "://" + Request.Url.Authority;            
        }

        void OutputRssItemEnclosure(PageEntity page, XmlWriter writer) {
            try
            {
                var imageUrl = page.RollupImage;
                if (String.IsNullOrWhiteSpace(imageUrl)) return;

                var fullUrl = imageUrl.Contains(":") ? imageUrl : GetSiteUrl() + imageUrl;

                writer.WriteStartElement("enclosure");
                writer.WriteAttributeString("url", EncodeUrl(fullUrl));
                writer.WriteAttributeString("type", GetImageMimeType(imageUrl));
                writer.WriteAttributeString("length", _fileManager.GetFileSize(imageUrl).ToString(CultureInfo.InvariantCulture) );
                
                writer.WriteEndElement();

            } catch (FileNotFoundException) {
                // Intentionally empty to handle all access denied errors            
            } catch (Exception) {
                // Intentionally empty to handle all access denied errors
            }
        }

        void OutputRssItemDescription(PageEntity page, XmlWriter writer)
        {
            string description;

            if (String.IsNullOrWhiteSpace(page.MainContent)) {
                if (String.IsNullOrWhiteSpace(page.RollupText)) {
                    if (String.IsNullOrWhiteSpace(page.Ingress)) return;
                    description = page.Ingress;
                } else description = page.RollupText;
            } else description = page.MainContent;

            string imageUrl;
            string imageMarkup;
            if (String.IsNullOrWhiteSpace(page.Figure)) {
                imageUrl = String.IsNullOrWhiteSpace(page.RollupImage) ? String.Empty : page.RollupImage;

            } else imageUrl = page.Figure;

            if (String.IsNullOrWhiteSpace(imageUrl))
            {
                imageMarkup = String.Empty;
            }
            else
            {
                var itemLink = GetSiteUrl() + page.VirtualPath;
                if (!imageUrl.Contains(":")) imageUrl = GetSiteUrl() + imageUrl;

                imageMarkup = String.Format("<a style=\"float:left; padding:0 1em 1em 0\" href=\"{0}\" title=\"{1}\"><img style=\"float:left; border:none\" src=\"{2}\" alt=\"{1}\" /></a>",
                    itemLink, "Item image", imageUrl);
            }

            var doc = new HtmlDocument();
            doc.LoadHtml(description);
            var markup = String.Format("{0}<span style=\"float:left\">{1}</span>", imageMarkup, doc.DocumentNode.InnerText.Trim());

            writer.WriteStartElement("description");
            writer.WriteCData(markup);
            writer.WriteEndElement();
        }

        string GetPageTitle(PageEntity page)
        {
            return String.IsNullOrWhiteSpace(page.Title) ? "No title" : page.Title;
        }

        void OutputRssItemTitle(PageEntity page, XmlWriter writer) {
            writer.WriteStartElement("title");

            var rawText = Regex.Replace(GetPageTitle(page), "<.*?>", string.Empty);

            writer.WriteString(rawText);

            writer.WriteEndElement();
        }

        string EncodeUrl(string url) {
            // Since the URLs come partially encoded and sometimes not encoded we must make our own custom encoder to handle all of these scenarios
            return url.Replace(" ", "%20").Replace("+", "%20").Replace("\\", "/").Replace("%3a", ":").Replace("%3A", ":").Replace("%2f", "/").Replace("%2F", "/").Replace("%2e", ".").Replace("%2E", ".").Replace("æ", "%c3%a6").Replace("ø", "%c3%b8").Replace("å", "%c3%a5").Replace("Æ", "%c3%86").Replace("Ø", "%c3&98").Replace("Å", "%c3%85");
        }

        void OutputRssItemLink(PageEntity page, XmlWriter writer) {
            writer.WriteStartElement("link");

            var itemLinkUrl = GetSiteUrl() + page.VirtualPath;

            writer.WriteString(EncodeUrl(itemLinkUrl));

            writer.WriteEndElement();
        }

        void OutputRssItemComments(PageEntity page, XmlWriter writer) {
            writer.WriteStartElement("comments");

            var itemLinkUrl = GetSiteUrl() + page.VirtualPath + "#comments";

            writer.WriteString(EncodeUrl(itemLinkUrl));

            writer.WriteEndElement();
        }

        void OutputRssItemGuid(PageEntity page, XmlWriter writer) {
            writer.WriteStartElement("guid");
            writer.WriteAttributeString("isPermaLink", "true");

            writer.WriteString(GetSiteUrl() + page.VirtualPath);

            writer.WriteEndElement();
        }

        void OutputGeoRss(double lat, double lon, XmlWriter writer) {
            writer.WriteStartElement("georss", "point", null);
            writer.WriteString(String.Format("{0:0.######} {1:0.######}", lat, lon));
            writer.WriteEndElement();
        }

        void OutputItemGeoRss(PageEntity page, XmlWriter writer) {
            var lat = page.GeolocationLat;
            var lon = page.GeolocationLong;

            if (lat != 0.0 && lon != 0.0) OutputGeoRss(lat, lon, writer);
        }

        void OutputRssForListItem(PageEntity page, XmlWriter writer) {
            writer.WriteStartElement("item");

            OutputRssItemTitle(page, writer);
            OutputRssItemLink(page, writer);
            
            if (page.AllowForComments) OutputRssItemComments(page, writer);

            OutputRssItemDescription(page, writer);
            OutputRssItemAuthor(page, writer);
            OutputRssItemPublishedDate(page, writer);
            OutputRssItemGuid(page, writer);
            OutputItemGeoRss(page, writer);
            OutputRssItemEnclosure(page, writer);

            writer.WriteEndElement();
        }

        void AddRssStartElement(XmlWriter writer) {
            writer.WriteStartElement("rss");
            writer.WriteAttributeString("version", null, "2.0");
            writer.WriteAttributeString("xmlns", "atom", null, "http://www.w3.org/2005/Atom");
            writer.WriteAttributeString("xmlns", "georss", null, "http://www.georss.org/georss");
        }

        void AddXmlDeclaration(XmlWriter writer) {
            writer.WriteStartDocument();
            writer.Flush();
        }

        void AddTopComment(XmlWriter writer)
        {
            writer.WriteComment("VeraCMS RSS 2.0 generator " + DateTime.Now.ToString("R"));
            writer.Flush();
        }

        #region Header

        void AddChannelTitle(string title, XmlWriter writer) {
            if (String.IsNullOrEmpty(title)) return;

            var realTitle = String.IsNullOrWhiteSpace(title) ? "No title" : title;

            writer.WriteStartElement("title");
            writer.WriteCData(realTitle);
            writer.WriteEndElement();
        }


        void AddChannelLink(string link, XmlWriter writer) {
            if (String.IsNullOrEmpty(link)) return;

            writer.WriteStartElement("link");
            writer.WriteString(link);
            writer.WriteEndElement();
        }

        void AddChannelDescription(string description, XmlWriter writer) {
            if (String.IsNullOrEmpty(description)) return;

            writer.WriteStartElement("description");
            writer.WriteCData(description);
            writer.WriteEndElement();
        }

        void AddChannelLastBuildDate(DateTime lastBuildDate, XmlWriter writer) {
            writer.WriteStartElement("lastBuildDate");
            writer.WriteString(lastBuildDate.ToString("R"));
            writer.WriteEndElement();
        }

        void AddChannelImageUrl(PageEntity rootPage, string url, int imageWidth, int imageHeight, XmlWriter writer) {
            if (String.IsNullOrEmpty(url)) return;

            writer.WriteStartElement("image");

            writer.WriteStartElement("url");
            if (url.Contains(":")) writer.WriteString(url);
            else writer.WriteString(GetSiteUrl() + url);
            writer.WriteEndElement();

            writer.WriteStartElement("title");
            writer.WriteCData(GetPageTitle(rootPage));
            writer.WriteEndElement();

            writer.WriteStartElement("link");
            writer.WriteString(GetSiteUrl() + rootPage.VirtualPath);
            writer.WriteEndElement();

            writer.WriteStartElement("width");
            writer.WriteString(imageWidth.ToString(CultureInfo.InvariantCulture));
            writer.WriteEndElement();

            writer.WriteStartElement("height");
            writer.WriteString(imageHeight.ToString(CultureInfo.InvariantCulture));
            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        void AddChannelSelfLink(XmlWriter writer) {
            writer.WriteStartElement("atom", "link", null);
            writer.WriteAttributeString("href", GetSiteUrl() + HttpContext.Current.Request.RawUrl);
            writer.WriteAttributeString("rel", "self");
            writer.WriteAttributeString("type", "application/rss+xml");
            writer.WriteEndElement();
        }

        void AddRssGenerator(XmlWriter writer) {
            writer.WriteStartElement("generator");
            writer.WriteString("VeraCMS RSS 2.0 generator");
            writer.WriteEndElement();
        }

        void AddRssLanguage(XmlWriter writer) {
            writer.WriteStartElement("language");
            writer.WriteString(ConfigurationManager.AppSettings["SiteCulture"]);
            writer.WriteEndElement();
        }

        void AddRssDocs(XmlWriter writer) {
            writer.WriteStartElement("docs");
            writer.WriteString("http://blogs.law.harvard.edu/tech/rss");
            writer.WriteEndElement();
        }

        string GetCopyrightYearSpan(int startYear) {
            var currentYear = DateTime.Now.Year;
            var difference = currentYear - startYear;
            string yearSpan;

            switch (difference) {
                case 0:
                    yearSpan = currentYear.ToString(CultureInfo.InvariantCulture);
                    break;
                case 1:
                    yearSpan = String.Format("{0},{1}", startYear, currentYear);
                    break;
                default:
                    yearSpan = String.Format("{0}-{1}", startYear, currentYear);
                    break;
            }

            return yearSpan;
        }

        int GetSiteLaunchYear() {
            return new RuntimeConfiguration().GetSiteLaunchDate().Year;
        }

        void AddRssCopyright(XmlWriter writer) {
            writer.WriteStartElement("copyright");
            writer.WriteString(String.Format("Copyright © {0} {1}", GetCopyrightYearSpan(GetSiteLaunchYear()),
                ConfigurationManager.AppSettings["companyName"]));
            writer.WriteEndElement();
        }

        void AddRssManagingEditor(XmlWriter writer) {
            writer.WriteStartElement("managingEditor");
            writer.WriteString(String.Format("{0} ({1})", 
                ConfigurationManager.AppSettings["rssManagingEditorEmail"],
                ConfigurationManager.AppSettings["rssManagingEditorName"]));
            writer.WriteEndElement();
        }

        void AddRssWebMaster(XmlWriter writer) {
            writer.WriteStartElement("webMaster");
            writer.WriteString(String.Format("{0} ({1})",
                ConfigurationManager.AppSettings["AdminEmail"],
                ConfigurationManager.AppSettings["AdminName"]));
            writer.WriteEndElement();
        }

        void AddRssTimeToLive(XmlWriter writer) {
            writer.WriteStartElement("ttl");
            writer.WriteString(TimeToLiveMinutes.ToString(CultureInfo.InvariantCulture));
            writer.WriteEndElement();
        }

        PageEntity RootPage(IEnumerable<PageEntity> pages, string rootUrl)
        {
            var pageCache = new PageCache();
            var rootPage = pageCache.GetPageByVirtualPath(rootUrl);
            
            if (rootPage == null && rootUrl.EndsWith("/", StringComparison.InvariantCultureIgnoreCase))
                rootPage = pageCache.GetPageByVirtualPath(rootUrl + "default.aspx");

            return rootPage;
        }

        PageEntity GetNewestPage(IEnumerable<PageEntity> pages)
        {
            return pages.OrderByDescending(page => page.Timestamp).FirstOrDefault();
        }

        void AddRssHeader(string rootUrl, IEnumerable<PageEntity> pages, XmlWriter writer) {
            var rootPage = RootPage(pages, rootUrl);
            if (rootPage != null)
            {
                AddChannelTitle(rootPage.Title, writer);
                
                if (!String.IsNullOrWhiteSpace(rootPage.RollupText))
                    AddChannelDescription(rootPage.RollupText , writer);

                // Add RSS feed icon
                if (!String.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["rssChannelImage"]))
                {
                    var channelImage = String.Format("{0}{1}", ConfigurationManager.AppSettings["BaseUrl"],
                        ConfigurationManager.AppSettings["rssChannelImage"]);

                    AddChannelImageUrl(rootPage, channelImage, int.Parse(ConfigurationManager.AppSettings["rssChannelImageWidth"]),
                        int.Parse(ConfigurationManager.AppSettings["rssChannelImageHeight"]), writer);
                    
                }

                AddChannelLastBuildDate(GetNewestPage(pages).Timestamp, writer);

                AddChannelLink(GetSiteUrl() + rootUrl, writer);
                AddChannelSelfLink(writer);
            }

            AddRssGenerator(writer);
            AddRssLanguage(writer);
            AddRssCopyright(writer);
            AddRssManagingEditor(writer);
            AddRssWebMaster(writer);
            AddRssDocs(writer);
            AddRssTimeToLive(writer);
        }

        #endregion

        int GetMaxNumberOfItemsInRssFeed() {
            return DefaultMaxNumberOfItemsInRssFeed;
        }

        void OutputRss(string rootUrl, IEnumerable<PageEntity> pages, XmlWriter writer) {
            AddXmlDeclaration(writer);
            AddTopComment(writer);

            AddRssStartElement(writer); // <rss version="2.0">

            writer.WriteStartElement("channel");   // <channel>

            AddRssHeader(rootUrl, pages, writer);

            var itemLimit = GetMaxNumberOfItemsInRssFeed();
            var countItems = 0;
            foreach (var page in pages.Where(page1 => page1.VirtualPath != rootUrl)) {
                OutputRssForListItem(page, writer);

                if (++countItems == itemLimit) break;
            }

            writer.WriteEndElement();   // </channel>

            writer.WriteEndElement();   // </rss>
        }

        IEnumerable<PageEntity> GetPagesByQueryPath(string path) {
            if (path.StartsWith("/Forums/"))
                return new PageCache().GetAllPages().Where(page => 
                    page.VirtualPath.StartsWith(path) 
                    && page.IsPublished 
                    && String.IsNullOrWhiteSpace(page.ParentRowKey)
                    && page.Index
                    ).OrderByDescending(page => page.RowKey);

            // Ignore forum messages
            return new PageCache().GetAllPages().Where(page =>
                page.VirtualPath.StartsWith(path)
                && !page.VirtualPath.StartsWith("/Forums/")
                && page.IsPublished
                && String.IsNullOrWhiteSpace(page.ParentRowKey)
                && page.Index
                ).OrderByDescending(page => page.RowKey);
        }

        string GetQueryPath() {
            return Request.QueryString["path"];
        }

        string GetRssXml(string rootUrl,  IEnumerable<PageEntity> pages)
        {
            var key = "Syndication_" + rootUrl;
            var rssXml = HostingEnvironment.Cache[key];

            if (rssXml == null)
            {
                var sb = new MemoryStream();
                var settings = new XmlWriterSettings
                                    {
                                        Indent = true,
                                        Encoding = Encoding.UTF8
                                    };

                var xmlWriter = XmlWriter.Create(sb, settings);

                OutputRss(rootUrl, pages, xmlWriter);

                xmlWriter.Flush();
                sb.Position = 0;
                
                rssXml = new StreamReader(sb).ReadToEnd();

                HostingEnvironment.Cache.Insert(key, rssXml, null, DateTime.Now.AddMinutes(TimeToLiveMinutes), Cache.NoSlidingExpiration);
            }
            
            return rssXml as string;
        }

        protected void Page_Load(object sender, EventArgs e) {
            Response.Clear();
            Response.ContentType = "application/rss+xml";
            Response.Charset = "utf-8";

            var rootUrl = GetQueryPath();
            if (!String.IsNullOrWhiteSpace(rootUrl))
            {
                var pages = GetPagesByQueryPath(rootUrl);
                if (pages != null)
                    Response.Write(GetRssXml(rootUrl, pages));
            }

        }
    }
}