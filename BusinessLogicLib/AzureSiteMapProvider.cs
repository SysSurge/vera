using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using VeraWAF.AzureTableStorage;
using VeraWAF.CrossCuttingConcerns;
using VeraWAF.WebPages.Dal;

namespace VeraWAF.WebPages.Bll {

    public class AzureSiteMapProvider : SiteMapProvider {

        const string DefaultTemplate = "RedirectToParent.aspx";
        const string DefaultMenuItemName = "[No title]";
        const string DefaultMenuItemDescription = "[No description]";

        SiteMapProvider _parentSiteMapProvider;
        SiteMapNode _rootNode;
        ArrayList _siteMapNodes;
        ArrayList _childParentRelationship;

        AzureTableStorageDataSource _datasource;
        string _applicationName;

        // Implement the CurrentNode property.
        public override SiteMapNode CurrentNode {
            get {
                var currentUrl = FindCurrentUrl();

                // Find the SiteMapNode that represents the current page.
                var currentNode = FindSiteMapNode(currentUrl);
                return currentNode;
            }
        }

        // Implement the RootNode property.
        public override SiteMapNode RootNode {
            get {
                return _rootNode;
            }
        }
        // Implement the ParentProvider property.
        public override SiteMapProvider ParentProvider {
            get {
                return _parentSiteMapProvider;
            }
            set {
                _parentSiteMapProvider = value;
            }
        }

        // Implement the RootProvider property.
        public override SiteMapProvider RootProvider {
            get {
                // If the current instance belongs to a provider hierarchy, it
                // cannot be the RootProvider. Rely on the ParentProvider.
                return ParentProvider != null ? ParentProvider.RootProvider : this;
            }
        }
        // Implement the FindSiteMapNode method.
        public override SiteMapNode FindSiteMapNode(string rawUrl) {
            // Does the root node match the URL?
            if (RootNode != null && RootNode.Url == rawUrl) return RootNode;

            SiteMapNode candidate;

            // Retrieve the SiteMapNode that matches the URL.
            lock (this) candidate = GetNode(_siteMapNodes, rawUrl);

            return candidate;
        }

        // Implement the GetChildNodes method.
        public override SiteMapNodeCollection GetChildNodes(SiteMapNode node) {
            var children = new SiteMapNodeCollection();

            // Iterate through the ArrayList and find all nodes that have the specified node as a parent.
            lock (this) {
                foreach (var child in from object t in _childParentRelationship select ((DictionaryEntry)t).Key as string 
                    into nodeUrl let parent = GetNode(_childParentRelationship, nodeUrl) where parent != null && node.Url == parent.Url 
                    select FindSiteMapNode(nodeUrl))
                        if (child != null) children.Add(child);
                        else throw new ApplicationException("ArrayLists not in sync.");
            }
            return children;
        }

        protected override SiteMapNode GetRootNodeCore() {
            return RootNode;
        }

        // Implement the GetParentNode method.
        public override SiteMapNode GetParentNode(SiteMapNode node) {
            // Check the childParentRelationship table and find the parent of the current node.
            // If there is no parent, the current node is the RootNode.
            SiteMapNode parent;

            lock (this) {
                // Get the Value of the node in childParentRelationship
                parent = GetNode(_childParentRelationship, node.Url);
            }
            return parent;
        }

        // Implement the ProviderBase.Initialize property.
        // Initialize is used to initialize the state that the Provider holds, but
        // not actually build the site map.
        public override void Initialize(string name, NameValueCollection attributes) {
            lock (this) {

                base.Initialize(name, attributes);

                _datasource = new AzureTableStorageDataSource();
                _applicationName = ConfigurationManager.AppSettings["ApplicationName"];

                //if (attributes != null) ;
                _siteMapNodes = new ArrayList();
                _childParentRelationship = new ArrayList();

                // Build the site map in memory.
                LoadSiteMapFromStore();
            }
        }

        SiteMapNode GetNode(ArrayList list, string url) {
            return (from DictionaryEntry item in list where (string)item.Key == url select item.Value as SiteMapNode).FirstOrDefault();
        }

        // Get the URL of the currently displayed page.
        string FindCurrentUrl() {
            try {
                // The current HttpContext.
                var currentContext = HttpContext.Current;

                if (currentContext != null) return currentContext.Request.Path;

                throw new Exception("HttpContext.Current is Invalid");

            } catch (Exception e) {
                throw new NotSupportedException("This provider requires a valid context.", e);
            }
        }

        PageEntity CreateDummyPage(string virtualPath)
        {
            return new PageEntity {
                PartitionKey = new StringUtilities().ConvertToHex(virtualPath),
                RowKey = DateTime.UtcNow.Ticks.ToString(CultureInfo.InvariantCulture),
                ApplicationName = _applicationName,
                Template = DefaultTemplate,
                VirtualPath = virtualPath,
                IsPublished = true,
                ShowInMenu = true,
                MenuItemName = DefaultMenuItemName,
                MenuItemDescription = DefaultMenuItemDescription
            };
        }

        SiteMapNode AddSiteMapNode(PageEntity page, List<PageEntity> pages)
        {
            if (FindSiteMapNode(page.VirtualPath) != null) return null;

            var siteMapNode = new SiteMapNode(this, page.VirtualPath, page.VirtualPath, page.MenuItemName, page.MenuItemDescription);

            var parentUrl = new UriUtilities().GetParentUri(new Uri(page.VirtualPath, UriKind.RelativeOrAbsolute)).ToString();

            // Is this a root node yet?
            if (_rootNode == null && ( page.VirtualPath.Equals("/default.aspx", StringComparison.InvariantCultureIgnoreCase) ) ) 
                _rootNode = siteMapNode;
            else {
                // If not the root node, add the node to the various collections.
                _siteMapNodes.Add(new DictionaryEntry(siteMapNode.Url, siteMapNode));

                // The parent node has already been added to the collection.
                var parentNode = FindSiteMapNode(parentUrl);
                if (parentNode == null)
                {
                    // The parent node was not found, so see if its later on in pages
                    var parentPage = pages.FirstOrDefault(page1 => page1.VirtualPath == parentUrl);

                    if (parentPage == null) {
                        // The parent page does not exist, so create a dummy for it
                        //parentPage = CreateDummyPage(parentUrl);
                        //_datasource.Insert(parentPage);
                        return null;
                    }

                    parentNode = AddSiteMapNode(parentPage, pages);
                }

                _childParentRelationship.Add(new DictionaryEntry(siteMapNode.Url, parentNode));
            }

            return siteMapNode;
        }

        List<PageEntity> GetSiteMapPages()
        {
            return new List<PageEntity>(
                new PageCache().GetAllPages().Where(
                    page => page.ShowInMenu 
                    && page.IsPublished 
                    && String.IsNullOrWhiteSpace(page.ParentRowKey)).OrderBy(page => page.MenuItemSortOrder)
                );
        }

        protected virtual void LoadSiteMapFromStore() {
            // If a root node exists, LoadSiteMapFromStore has already
            // been called, and the method can return.
            if (_rootNode == null) Refresh();
        }

        public void Refresh()
        {
            lock (this)
            {
                _rootNode = null;

                List<PageEntity> pages;

                try
                {
                    pages = GetSiteMapPages();
                }
                catch (Exception)
                {
                    // An exception caught here may be because the application has not initiated yet
                    return;
                }

                // Clear the state of the collections and rootNode
                _siteMapNodes.Clear();
                _childParentRelationship.Clear();

                foreach (var article in pages) AddSiteMapNode(article, pages);
            }
        }
    }
}
