using System;
using System.Configuration;
using System.Globalization;
using System.Web.UI;
using VeraWAF.AzureTableStorage;
using VeraWAF.Core.Templates;

namespace VeraWAF.WebPages.Controls
{
    public partial class Map : UserControl
    {
        /// <summary>
        /// Vera base page 
        /// </summary>
        PageTemplateBase _page;

        /// <summary>
        /// Map latitude location
        /// </summary>
        public double Lat { get; set; }

        /// <summary>
        /// Map longitude location
        /// </summary>
        public double Long { get; set; }

        /// <summary>
        /// Map zoom level
        /// </summary>
        public int Zoom { get; set; }

        /// <summary>
        /// HTML markup that is showed when the user clicks the map marker
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Refreshes the map with its content
        /// </summary>
        public void Refresh()
        {
            var siteCulture = new CultureInfo(ConfigurationManager.AppSettings["SiteCulture"]); // Make sure the numbers uses dots and not commas
            litMap.Text = String.IsNullOrWhiteSpace(Text) ?
                String.Format(VeraWAF.WebPages.Bll.Resources.Controls.Map2, Lat.ToString("0.######", siteCulture), 
                    Long.ToString("0.######", siteCulture), Zoom) :
                String.Format(VeraWAF.WebPages.Bll.Resources.Controls.Map1, Lat.ToString("0.######", siteCulture), 
                    Long.ToString("0.######", siteCulture), Text, Zoom);            
        }

        /// <summary>
        /// Set map data based on the page entity
        /// </summary>
        /// <param name="page">Page entity</param>
        private void SetMap(PageEntity page)
        {
            if (page.GeolocationLong != 0.0 && page.GeolocationLat != 0.0)
            {
                Lat = page.GeolocationLat;
                Long = page.GeolocationLong;
                Text = page.GeolocationText;
                Zoom = page.GeolocationZoom;
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
                // Is the map supposed to be visible?
                Visible = pageEntity.GeolocationLong != 0.0 && pageEntity.GeolocationLat != 0.0;

                if (Visible)
                {
                    SetMap(pageEntity);

                    Refresh();
                }
            }
        }
    }
}