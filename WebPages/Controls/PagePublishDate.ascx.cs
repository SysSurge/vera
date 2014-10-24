using System;
using System.Web.UI;
using VeraWAF.AzureTableStorage;
using VeraWAF.Core.Templates;
using VeraWAF.WebPages.Bll;

namespace VeraWAF.WebPages.Controls
{
    /// <summary>
    /// Shows the page publish date inside a <time> HTML element.
    /// Uses TimeAgo functionality if JavaScript is enabled and bll.timeago.js is included; eg.
    /// "1 hour ago", "yesterday at 1 AM" etc.
    /// </summary>
    public partial class PagePublishDate : UserControl
    {
        /// <summary>
        /// Vera base page 
        /// </summary>
        PageTemplateBase _page;

        /// <summary>
        /// Set to tru to enable TimeAgo functionality if JavaScript is enabled and bll.timeago.js 
        /// is included; eg. "1 hour ago", "yesterday at 1 AM" etc.
        /// </summary>
        public bool EnableTimeAgo { get; set; }

        /// <summary>
        /// Class contructor
        /// </summary>
        public PagePublishDate()
        {
            // Enable timeago by default
            EnableTimeAgo = true;
        }

        DateTime GetArticlePublishedDate(PageEntity page)
        {
            return new DateTime(long.Parse(page.RowKey));
        }

        /// <summary>
        /// Show the page publish date
        /// </summary>
        /// <param name="page"></param>
        void SetArticlePublishedDate(PageEntity page)
        {
            var publishedDate = GetArticlePublishedDate(page);
            var dateTimeTools = new DateUtilities();

            timePublishedDate.Attributes["datetime"] = dateTimeTools.GetCustomIso8601Date(publishedDate);
            timePublishedDate.InnerText = dateTimeTools.GetReadableDateAndTime(publishedDate, ReadableDateAntTimeTypes.Abbreviated);
        }

        /// <summary>
        /// Add the timeAgo class if enabled
        /// </summary
        void InitTimeAgo()
        {
            if (EnableTimeAgo)
            {
                // Make sure we don't remove any existing classes
                var currentClassAttr = timePublishedDate.Attributes["class"];
                if (String.IsNullOrEmpty(currentClassAttr))
                    timePublishedDate.Attributes.Add("class", "timeAgo"); 
                else
                    timePublishedDate.Attributes.Add("class", currentClassAttr + " timeAgo"); 
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Get the parent page
            _page = Page as PageTemplateBase;
            if (_page == null)
                throw new ApplicationException("The page does not inherit from the VeraWAF.Core.Templates.PageTemplateBase page template.");

            // Get page entity data
            var pageEntity = _page.GetPageEntity();
            if (pageEntity != null)
            {
                phDate.Visible = true;

                InitTimeAgo();

                SetArticlePublishedDate(pageEntity);
            }

        }
    }
}