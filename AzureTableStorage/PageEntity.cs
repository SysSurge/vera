using System;
using Microsoft.WindowsAzure.StorageClient;

namespace VeraWAF.AzureTableStorage
{
    public class PageEntity : AzureResourceEntity
    {
        /// <summary>
        /// Page template. Ex. "Templates\Default\Frontpage.aspx"
        /// </summary>
        public string Template { get; set; }

        /// <summary>
        /// The style that the page uses, ex. "Default" or "BlueSky"
        /// </summary>
        public string Style { get; set; }

        /// <summary>
        /// Rollup text. As shown in search.
        /// </summary>
        public string RollupText { get; set; }

        /// <summary>
        /// Rollup image. As shown in search.
        /// </summary>
        public string RollupImage { get; set; }

        /// <summary>
        /// Virtual path to page entity. Ex. "/default.aspx"
        /// </summary>
        public string VirtualPath { get; set; }

        /// <summary>
        /// Page title text or markup
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Page ingress text or markup
        /// </summary>
        public string Ingress { get; set; }

        /// <summary>
        /// Main page text or markup
        /// </summary>
        public string MainContent { get; set; }

        /// <summary>
        /// Set to True to make the page visible to all, or to False to hide page from all.
        /// </summary>
        public bool IsPublished { get; set; }

        /// <summary>
        /// The text that is shown in the menus
        /// </summary>
        public string MenuItemName { get; set; }

        /// <summary>
        /// The description text that is shown in menus
        /// </summary>
        public string MenuItemDescription { get; set; }

        /// <summary>
        /// Menu item sort order
        /// </summary>
        public int MenuItemSortOrder { get; set; }

        /// <summary>
        /// Set to True to show the page in the menus, or to False to hide the page from the menus
        /// </summary>
        public bool ShowInMenu { get; set; }

        /// <summary>
        /// Set the geoloaction latitude to show location on maps
        /// </summary>
        public double GeolocationLat { get; set; }

        /// <summary>
        /// Set the geoloaction longitude to show location on maps
        /// </summary>
        public double GeolocationLong { get; set; }

        /// <summary>
        /// Set the map text description
        /// </summary>
        public string GeolocationText { get; set; }

        /// <summary>
        /// Set the map zoom. Lower numbers increases zoom
        /// </summary>
        public int GeolocationZoom { get; set; }

        /// <summary>
        /// Page author. 
        /// Contains the username from the VeraUsers table.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Page main image
        /// </summary>
        public string Figure { get; set; }

        /// <summary>
        /// Caption for the page main image
        /// </summary>
        public string FigureCaption { get; set; }

        /// <summary>
        /// Page permanent redirection url
        /// </summary>
        public string RedirectUrl { get; set; }

        /// <summary>
        /// Parent page row key. Used to connect comments to their pages.
        /// </summary>
        public string ParentRowKey { get; set; }

        /// <summary>
        /// Set to True to enable comments on the page
        /// </summary>
        public bool AllowForComments { get; set; }

        /// <summary>
        /// Set to True to show the contact author user control on the page
        /// </summary>
        public bool ShowContactControl { get; set; }

        /// <summary>
        /// HTML5 HEADER element content as text or markup
        /// </summary>
        public string HeaderContent { get; set; }

        /// <summary>
        /// HTML5 ASIDE element content as text or markup
        /// </summary>
        public string AsideContent { get; set; }

        /// <summary>
        /// Set to True to search index the page
        /// </summary>
        public bool Index { get; set; }

        /// <summary>
        /// CSV of RDFa subjects that describes the page content
        /// </summary>
        public string RdfSubjects { get; set; }

        /// <summary>
        /// CSV or inline stylesheet files
        /// </summary>
        public string Css { get; set; }

        /// <summary>
        /// CSV or inline JavaScript files
        /// </summary>
        public string Scripts { get; set; }

        /// <summary>
        /// Override mime type. Ex. "text/html"
        /// </summary>
        public string Mime { get; set; }

        /// <summary>
        /// Override text encoding. Ex. "utf-8"
        /// </summary>
        public string Encoding { get; set; }

    }
}
