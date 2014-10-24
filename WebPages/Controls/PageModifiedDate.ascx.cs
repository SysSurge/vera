using System;
using System.Web.UI;
using VeraWAF.AzureTableStorage;
using VeraWAF.Core.Templates;
using VeraWAF.WebPages.Bll;

namespace VeraWAF.WebPages.Controls
{
    public partial class PageModifiedDate : UserControl
    {
        PageTemplateBase _page;

        /// <summary>
        /// Set to tru to enable TimeAgo functionality if JavaScript is enabled and bll.timeago.js 
        /// is included; eg. "1 hour ago", "yesterday at 1 AM" etc.
        /// </summary>
        public bool EnableTimeAgo { get; set; }

        /// <summary>
        /// Class contructor
        /// </summary>
        public PageModifiedDate()
        {
            // Enable timeago by default
            EnableTimeAgo = true;
        }

        /// <summary>
        /// Show the date the page was last modified
        /// </summary>
        /// <param name="page">Page entity</param>
        void SetArticleModifiedDate(PageEntity page)
        {
            var dateTimeTools = new DateUtilities();
            var modifideDate = page.Timestamp;

            timeModifiedDate.Attributes["datetime"] = dateTimeTools.GetCustomIso8601Date(modifideDate);
            timeModifiedDate.InnerText = dateTimeTools.GetReadableDateAndTime(modifideDate, ReadableDateAntTimeTypes.Abbreviated);
        }

        /// <summary>
        /// Add the timeAgo class if enabled
        /// </summary>
        void InitTimeAgo()
        {
            if (EnableTimeAgo)
            {
                // Make sure we don't remove any existing classes
                var currentClassAttr = timeModifiedDate.Attributes["class"];
                if (String.IsNullOrEmpty(currentClassAttr))
                    timeModifiedDate.Attributes.Add("class", "timeAgo");
                else
                    timeModifiedDate.Attributes.Add("class", currentClassAttr + " timeAgo");
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

                SetArticleModifiedDate(pageEntity);
            }
        }
    }
}