using System;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using VeraWAF.AzureTableStorage;
using HtmlAgilityPack;
using VeraWAF.WebPages.Bll;
using VeraWAF.WebPages.Bll.Cloud;
using VeraWAF.WebPages.Dal;

namespace VeraWAF.WebPages.Templates
{
    public partial class NewsSection : Page
    {
        private void SetArticleTitle(PageEntity page) {
            litTitle.Text = page.Title;
        }

        private void SetArticleIngress(PageEntity page) {
            litIngress.Text = page.Ingress;
        }

        private void SetArticleBody(PageEntity page) {
            litBody.Text = new GraphicUtilities().ChangeToJailImages(page.MainContent);
        }

        private string GetJailImageMarkup(string src, string alt) {
            return String.Format(VeraWAF.WebPages.Bll.Resources.Controls.JailImageMarkup2, new CdnUtilities().GetCdnUrl(src), alt);
        }

        private void SetFigure(PageEntity page) {
            if (String.IsNullOrWhiteSpace(page.Figure)) return;

            var figureCaptionMarkup = String.Empty;
            if (!String.IsNullOrWhiteSpace(page.FigureCaption))
                figureCaptionMarkup = String.Format("<figcaption>{0}</figcaption>", page.FigureCaption);

            var jailImageMarkup = GetJailImageMarkup(page.Figure, page.FigureCaption);

            litFigure.Text = String.Format("<figure>{0}{1}</figure>", jailImageMarkup, figureCaptionMarkup);
        }

        private void InitControls(PageEntity page) {
            SetArticleTitle(page);
            SetArticleIngress(page);
            SetArticleBody(page);
            SetFigure(page);
        }

        private void SetPageTitle(PageEntity page) {
            if (!String.IsNullOrWhiteSpace(page.Title))
                Page.Title = Regex.Replace(page.Title, @"<[^>]*>", String.Empty);
        }

        private string GetVirtualPath() {
            return Request.Url.AbsolutePath;
        }

        private void SetPageMetaDescription(PageEntity page) {
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

        private DateTime GetArticlePublishedDate(PageEntity page) {
            return new DateTime(long.Parse(page.RowKey));
        }

        private void SetPageRdfaCreatedDate(PageEntity page) {
            var dateTimeTools = new DateUtilities();
            var publishedDate = GetArticlePublishedDate(page);
            metaRdfaCreated.Attributes["content"] = dateTimeTools.GetCustomIso8601Date(publishedDate);
        }

        private void SetPageRdfaModifiedDate(PageEntity page) {
            var dateTimeTools = new DateUtilities();
            var modifiedDate = page.Timestamp;
            metaRdfaModified.Attributes["content"] = dateTimeTools.GetCustomIso8601Date(modifiedDate);
        }

        private void SetPageRdfaSubjects(PageEntity page) {
            if (!String.IsNullOrWhiteSpace(page.RdfSubjects)) {
                var markup = new StringBuilder();
                var subjects = page.RdfSubjects.Split(',');

                foreach (var normalizedUri in subjects.Select(subject => Regex.Replace(subject, @"\s", String.Empty)))
                    markup.AppendFormat("<meta rel=\"dc:subject\" href=\"{0}\" />", normalizedUri);

                litRdfaSubjects.Text = markup.ToString();
            }
        }

        private void SetPageMetaData(PageEntity page) {
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