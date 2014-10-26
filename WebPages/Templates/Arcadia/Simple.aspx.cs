using System;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using VeraWAF.AzureTableStorage;
using HtmlAgilityPack;
using VeraWAF.Core.Templates;
using VeraWAF.WebPages.Bll;
using VeraWAF.WebPages.Bll.Cloud;
using VeraWAF.WebPages.Dal;

namespace VeraWAF.WebPages.Templates {
    public partial class Simple : PageTemplateBase 
    {
        DateTime GetArticlePublishedDate(PageEntity page) {
            return new DateTime(long.Parse(page.RowKey));
        }

        void SetArticlePublishedDate(PageEntity page) {
            var dateTimeTools = new DateUtilities();
            var publishedDate = GetArticlePublishedDate(page);

            timePublishedDate.Attributes["datetime"] = dateTimeTools.GetCustomIso8601Date(publishedDate);
            timePublishedDate.InnerText = dateTimeTools.GetReadableDateAndTime(publishedDate, ReadableDateAntTimeTypes.Abbreviated);
        }

        void SetArticleModifiedDate(PageEntity page) {
            var dateTimeTools = new DateUtilities();
            var modifideDate = page.Timestamp;

            timeModifiedDate.Attributes["datetime"] = dateTimeTools.GetCustomIso8601Date(modifideDate);
            timeModifiedDate.InnerText = dateTimeTools.GetReadableDateAndTime(modifideDate, ReadableDateAntTimeTypes.Abbreviated);
        }


        UserEntity GetAuthor(PageEntity page) {
            var userCache = new UserCache();
            return userCache.GetUser(page.Author);
        }

        void SetArticleTitle(PageEntity page) {
            litTitle.Text = page.Title;
        }

        void SetArticleIngress(PageEntity page) {
            litIngress.Text = page.Ingress;
        }

        string ChangeToJailImages(string markup) {
            var doc = new HtmlDocument();
            doc.LoadHtml(markup);

            var imageTags = doc.DocumentNode.SelectNodes("//img");
            if (imageTags != null)
                foreach (var img in imageTags)
                    img.ParentNode.InnerHtml = String.Format(VeraWAF.WebPages.Bll.Resources.Controls.JailImageMarkup3,
                                                             img.Attributes["src"].Value);

            return doc.DocumentNode.OuterHtml;
        }

        void SetArticleBody(PageEntity page) {
            litBody.Text = ChangeToJailImages(page.MainContent);
        }

        string GetJailImageMarkup(string src, string alt) {
            return String.Format(VeraWAF.WebPages.Bll.Resources.Controls.JailImageMarkup2, new CdnUtilities().GetCdnUrl(src), alt);
        }

        void SetFigure(PageEntity page) {
            if (String.IsNullOrWhiteSpace(page.Figure)) return;

            var figureCaptionMarkup = String.Empty;
            if (!String.IsNullOrWhiteSpace(page.FigureCaption))
                figureCaptionMarkup = String.Format("<figcaption>{0}</figcaption>", page.FigureCaption);

            var jailImageMarkup = GetJailImageMarkup(page.Figure, page.FigureCaption);

            litFigure.Text = String.Format("<figure>{0}{1}</figure>", jailImageMarkup, figureCaptionMarkup);
        }

        void SetMap(PageEntity page) {
            if (page.GeolocationLong != 0.0 && page.GeolocationLat != 0.0) {
                Map.Lat = page.GeolocationLat;
                Map.Long = page.GeolocationLong;
                Map.Text = page.GeolocationText;
                Map.Zoom = page.GeolocationZoom;
                Map.Visible = true;
            }
        }

        void InitFooterControls(PageEntity page)
        {
            SetArticlePublishedDate(page);
            SetArticleModifiedDate(page);

            //SetArticleAuthor(page);            
        }

        void InitWidgets(PageEntity page) {
            SetMap(page);

            contactUser1.Visible = page.ShowContactControl;

            comments1.Visible = page.AllowForComments;
        }

        void InitControls(PageEntity page)
        {
            InitFooterControls(page);
            
            SetArticleTitle(page);
            SetArticleIngress(page);

            SetArticleBody(page);

            SetFigure(page);

            SetMap(page);

            InitWidgets(page);
        }

        void SetPageTitle(PageEntity page) {
            if (!String.IsNullOrWhiteSpace(page.Title))
                Page.Title = Regex.Replace(page.Title, @"<[^>]*>", String.Empty);
        }

        string GetVirtualPath() {
            return Request.Url.AbsolutePath;
        }

        void SetPageMetaDescription(PageEntity page) {
            var htmlDoc = new HtmlDocument();

            if (String.IsNullOrWhiteSpace(page.RollupText))
                metaDescription.Visible = false;
            else {
                htmlDoc.LoadHtml(page.RollupText);

                const int maxAllowedMetaDescriptionLength = 155;
                var rawText = htmlDoc.DocumentNode.InnerText.Trim();
                if (rawText.Length > maxAllowedMetaDescriptionLength)
                    rawText = rawText.Substring(0, maxAllowedMetaDescriptionLength - 4) + "...";

                metaDescription.Attributes["content"] = rawText;
            }
        }

        void SetPageRdfaCreatedDate(PageEntity page) {
            var dateTimeTools = new DateUtilities();
            var publishedDate = GetArticlePublishedDate(page);
            metaRdfaCreated.Attributes["content"] = dateTimeTools.GetCustomIso8601Date(publishedDate);
        }

        void SetPageRdfaModifiedDate(PageEntity page) {
            var dateTimeTools = new DateUtilities();
            var modifiedDate = page.Timestamp;
            metaRdfaModified.Attributes["content"] = dateTimeTools.GetCustomIso8601Date(modifiedDate);
        }

        void SetPageRdfaSubjects(PageEntity page) {
            if (!String.IsNullOrWhiteSpace(page.RdfSubjects)) {
                var markup = new StringBuilder();
                var subjects = page.RdfSubjects.Split(',');

                foreach (var normalizedUri in subjects.Select(subject => Regex.Replace(subject, @"\s", String.Empty)))
                    markup.AppendFormat("<meta rel=\"dc:subject\" href=\"{0}\" />", normalizedUri);

                litRdfaSubjects.Text = markup.ToString();
            }
        }

        void SetPageMetaData(PageEntity page) {
            metaMade.Attributes["href"] = "mailto:" + ConfigurationManager.AppSettings["AdminEmail"];

            SetPageRdfaCreatedDate(page);
            SetPageRdfaModifiedDate(page);
            SetPageRdfaSubjects(page);
            metaRdfaCreator.Attributes["content"] = page.Author;

            SetPageMetaDescription(page);

            metaAuthor.Attributes["content"] = page.Author;
        }

        protected void Page_Load(object sender, EventArgs e) {
            var page = new PageCache().GetPageByVirtualPath(GetVirtualPath());

            if (!String.IsNullOrWhiteSpace(page.RedirectUrl)) Response.RedirectPermanent(page.RedirectUrl);

            SetPageMetaData(page);
            SetPageTitle(page);
            InitControls(page);
        }
    }
}