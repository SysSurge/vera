using System;
using System.Configuration;
using System.Globalization;
using System.Web.Security;
using VeraWAF.AzureTableStorage;
using VeraWAF.Core.Templates;
using VeraWAF.WebPages.Bll;
using VeraWAF.CrossCuttingConcerns;
using VeraWAF.WebPages.Dal;

namespace VeraWAF.WebPages.Edit {
    public partial class EditPage : PageTemplateBase
    {
        /// <summary>
        /// Table storage client
        /// </summary>
        readonly TableStorageClient _tableStorageClient;

        /// <summary>
        /// Content types
        /// </summary>
        readonly ContentTypes _contentTypes;

        /// <summary>
        /// Application name
        /// </summary>
        readonly string _applicationName;

        /// <summary>
        /// Default page template
        /// </summary>
        readonly string _defaultTemplate;

        /// <summary>
        /// Virtual path
        /// </summary>
        string _virtualPath;

        /// <summary>
        /// RowKey property value from the page entity data
        /// </summary>
        string _rowKey;

        /// <summary>
        /// Classs constructor
        /// </summary>
        public EditPage()
        {
            // Get application name
            _applicationName = ConfigurationManager.AppSettings["ApplicationName"];

            // Get the default template
            _defaultTemplate = ConfigurationManager.AppSettings["DefaultPageTemplate"];

            // Create a content type object
            _contentTypes = new ContentTypes();

            _tableStorageClient = new TableStorageClient();
        }

        /// <summary>
        /// Initiate the map
        /// </summary>
        /// <param name="page"></param>
        void SetMap(PageEntity page) {
            if (page.GeolocationLong != 0.0 && page.GeolocationLat != 0.0) {
                Map.Lat = page.GeolocationLat;
                Map.Long = page.GeolocationLong;
                Map.Text = page.GeolocationText;
                Map.Zoom = page.GeolocationZoom;
                Map.Visible = true;
                Map.Refresh();
            }
        }

        /// <summary>
        /// Populate the form controls with the page entity data
        /// </summary>
        /// <param name="page">Page entity</param>
        void PopulateFormFieldsFromPageEntityData(PageEntity page)
        {
            fileExplorer.Value = page.VirtualPath;
            cmbTemplate.SelectedValue = page.Template;
            cmbStyle.SelectedValue = page.Style;
            txtTitle.Text = page.Title;
            txtIngress.Text = page.Ingress;
            txtMainContent.Text = page.MainContent;
            iuRollupImage.ImageUrl = page.RollupImage;
            txtRollupText.Text = page.RollupText;
            chkPublish.Checked = page.IsPublished;
            txtMenuItemName.Text = page.MenuItemName;
            txtMenuItemDescription.Text = page.MenuItemDescription;
            txtMenuItemSortOrder.Text = page.MenuItemSortOrder.ToString(CultureInfo.InvariantCulture);
            txtLat.Text = page.GeolocationLat.ToString(CultureInfo.InvariantCulture);
            txtLong.Text = page.GeolocationLong.ToString(CultureInfo.InvariantCulture);
            txtGeolocationText.Text = page.GeolocationText;
            txtZoom.Text = page.GeolocationZoom.ToString(CultureInfo.InvariantCulture);
            iuFigure.ImageUrl = page.Figure;
            iuFigure.Caption = page.FigureCaption;
            chkShowInMenu.Checked = page.ShowInMenu;
            txtPageMoved.Text = page.RedirectUrl;
            chkAllowForComments.Checked = page.AllowForComments;
            chkShowContactControl.Checked = page.ShowContactControl;
            txtAsideContent.Text = page.AsideContent;
            txtHeaderContent.Text = page.HeaderContent;
            chkIndex.Checked = page.Index;
            txtRdfSubjects.Text = page.RdfSubjects;
            txtCss.Text = page.Css;
            txtScripts.Text = page.Scripts;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // Add all the items to the combo box
            if (!Page.IsPostBack)
                cmbTemplate.SelectedValue = ConfigurationManager.AppSettings["DefaultPageTemplate"];
        }

        /// <summary>
        /// Called when the page loads
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            // get the page virtual path
            _virtualPath = Request["virtualPath"];

            // Get the page rowkey
            _rowKey = Request["rowKey"];

            try
            {

                // Is this a new page, if so then set some default form data
                if (String.IsNullOrWhiteSpace(_virtualPath))
                {
                    if (!Page.IsPostBack)
                    {
                        // This is a new page, so set some default form values
                        fileExplorer.Value = Bll.Resources.Forms.NewPageVirtualPath;
                        txtMenuItemName.Text = Bll.Resources.Forms.NewPage;
                        txtTitle.Text = Bll.Resources.Forms.NewPage;
                        chkPublish.Checked = true;
                    }

                    // No page entity data to load, so return
                    return;
                }

                // Get page entity data
                var page = _tableStorageClient.GetPageByVirtualPath(_virtualPath, _rowKey, _applicationName);

                // Was any page enity data found?
                if (page != null)
                {
                    // Page entity data found, so load the form with its values

                    SetMap(page);

                    if (!Page.IsPostBack && !String.IsNullOrEmpty(_virtualPath))
                    {
                        // Fill all the form controls with data from the Page entity
                        PopulateFormFieldsFromPageEntityData(page);
                    }
                }
            }
            catch(Exception ex)
            {
                notifications.AddMessage(ex.Message);
                return;
            }
        }

        /// <summary>
        /// Get the site culture
        /// </summary>
        /// <returns></returns>
        CultureInfo GetSiteCulture() {
            return new CultureInfo(ConfigurationManager.AppSettings["SiteCulture"]);
        }

        /// <summary>
        /// Create the page row key from the page creation date
        /// </summary>
        /// <param name="createdDate">Date</param>
        /// <returns>Page row key</returns>
        string GetRowKey(DateTime createdDate)
        {
            return String.IsNullOrWhiteSpace(_rowKey)
                       ? _tableStorageClient.GetPageRowKey(createdDate)
                       : _rowKey;
        }

        /// <summary>
        /// Create a page entity from the form data
        /// </summary>
        /// <param name="createdDate">Page creation date</param>
        /// <returns>Page entity</returns>
        PageEntity CreatePageEntityFromFormData(DateTime createdDate)
        {
            // Assert date
            if (createdDate == null) createdDate = DateTime.UtcNow;

            // Get the culture, default to current if not found
            var cultureInfo = GetSiteCulture();
            if (cultureInfo == null) cultureInfo = CultureInfo.CurrentCulture;

            // Assert virtual path
            if (String.IsNullOrWhiteSpace(fileExplorer.Value))
                throw new ArgumentException("User entered an empty virtual path");

            // Get author
            var author = Membership.GetUser() == null || String.IsNullOrWhiteSpace(Membership.GetUser().UserName) ?
                string.Format("[{0}]", Bll.Resources.Solution.AnonymousUserName) : Membership.GetUser().UserName;

            // Geo location
            var geoLocLat = String.IsNullOrWhiteSpace(txtLat.Text) ? 0.0 : double.Parse(txtLat.Text, cultureInfo);
            var geoLocLong = String.IsNullOrWhiteSpace(txtLong.Text) ? 0.0 : double.Parse(txtLong.Text, cultureInfo);

            // Geo location zoom
            int geolocationZoom;
            if (!int.TryParse(txtZoom.Text, out geolocationZoom)) geolocationZoom = 0;

            // Geo location zoom
            int menuSortOrder;
            if (!int.TryParse(txtMenuItemSortOrder.Text, out menuSortOrder)) menuSortOrder = 0;

            // Create the new page entity
            return new PageEntity
            {
                // Add a key that uniquely identifies the content page
                PartitionKey = new StringUtilities().ConvertToHex(fileExplorer.Value),
                // Row key is a timestamp
                RowKey = GetRowKey(createdDate),
                // Application name
                ApplicationName = _applicationName,
                // Set the template, ex. "Default\frontpage.aspx" for ASPX pages only
                Template = fileExplorer.Value.EndsWith(".aspx", StringComparison.CurrentCultureIgnoreCase) ? cmbTemplate.SelectedValue : String.Empty,
                // Page title
                Title = txtTitle == null ? String.Empty : txtTitle.Text,
                // Page ingress
                Ingress = txtIngress == null ? String.Empty : txtIngress.Text,
                // Page main content
                MainContent = txtMainContent == null ? String.Empty : txtMainContent.Text,
                // Rollup image as seen when searching
                RollupImage = iuRollupImage == null || String.IsNullOrWhiteSpace(iuRollupImage.ImageUrl) ? String.Empty : iuRollupImage.ImageUrl,
                // Rollup text as seen when searching
                RollupText = txtRollupText == null ? String.Empty : txtRollupText.Text,
                // Virtual path, ex. "/default.aspx"
                VirtualPath = fileExplorer == null ? String.Empty : fileExplorer.Value,
                // If True then the page is visible to all
                IsPublished = chkPublish == null ? true : chkPublish.Checked,
                // Menu item name
                MenuItemName = txtMenuItemName == null ? String.Empty : txtMenuItemName.Text,
                // Menu item description
                MenuItemDescription = txtMenuItemDescription == null ? String.Empty : txtMenuItemDescription.Text,
                // Menu item sort order
                MenuItemSortOrder = menuSortOrder,
                // Geo location map latitude
                GeolocationLat = geoLocLat,
                // Geo location map longitude
                GeolocationLong = geoLocLong,
                // Map caption
                GeolocationText = txtGeolocationText.Text,
                // Map zoom
                GeolocationZoom = geolocationZoom,
                // Page author from the VeraUsers table Username field
                Author = author,
                // Main image
                Figure = iuFigure == null || iuFigure.ImageUrl == null ? String.Empty : iuFigure.ImageUrl,
                // Main image caption
                FigureCaption = iuFigure == null || iuFigure.Caption == null ? String.Empty : iuFigure.Caption,
                // If true then show the page in the menus
                ShowInMenu = chkShowInMenu == null ? true : chkShowInMenu.Checked,
                // Permamently redirected to this url
                RedirectUrl = txtPageMoved == null ? String.Empty : txtPageMoved.Text,
                // If true then we show the contact author user control
                ShowContactControl = chkShowContactControl == null ? true : chkShowContactControl.Checked,
                // If true then we show page comments
                AllowForComments = chkAllowForComments == null ? true : chkAllowForComments.Checked,
                // HTML5 aside page content
                AsideContent = txtAsideContent == null ? String.Empty : txtAsideContent.Text,
                // HTML5 header page content
                HeaderContent = txtHeaderContent == null ? String.Empty : txtHeaderContent.Text,
                // If true then the page is search indexed
                Index = chkIndex == null ? true : chkIndex.Checked,
                // CSV of RDFa subjects
                RdfSubjects = txtRdfSubjects == null ? String.Empty : txtRdfSubjects.Text,
                // CSV of stylesheet links
                Css = txtCss == null ? String.Empty : txtCss.Text,
                // CSV of JavsScript links
                Scripts = txtScripts == null ? String.Empty : txtScripts.Text,
                // Page encoding. Ex. "utf-8"
                Encoding = ConfigurationManager.AppSettings["DefaultHtmlEncoding"],
                // Mimetype. Ex. "text/html"
                Mime = _contentTypes.GetInfoBySuffix("aspx").MimeType,
                // Style. Ex. "Default" or "SkyBlue" for ASPX pages only
                Style = fileExplorer.Value.EndsWith(".aspx", StringComparison.CurrentCultureIgnoreCase) ? cmbStyle.SelectedValue : String.Empty
            };
        }

        /// <summary>
        /// Called when the user clicks the save button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void butUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(fileExplorer.Value))
                    throw new ApplicationException("No virtual path");

                // Save the page
                var partitionKey = new StringUtilities().ConvertToHex(fileExplorer.Value);
                var page = CreatePageEntityFromFormData(DateTime.UtcNow);
                _tableStorageClient.UpdatePage(page, true, partitionKey, _applicationName);

                // Update the form map
                SetMap(page);
            }
            catch (Exception ex)
            {
                notifications.AddMessage(ex.Message);
                return;
            }
        }

        /// <summary>
        /// Clear the form by reloading it
        /// </summary>
        void ClearForm()
        {
            Response.Redirect("/CMS/EditPage.aspx", true);            
        }

        /// <summary>
        /// Called when the user clicks the Clear button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void butClear_Click(object sender, EventArgs e) {
            ClearForm();
        }

        /// <summary>
        /// Called when the user clicks the Delete button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void butDelete_Click(object sender, EventArgs e)
        {
            try
            {
                var partitionKey = new StringUtilities().ConvertToHex(fileExplorer.Value);
                _tableStorageClient.DeletePage(partitionKey, _applicationName);
                ClearForm();
            }
            catch (Exception ex)
            {
                notifications.AddMessage(ex.Message);
                return;
            }
        }

    }
}