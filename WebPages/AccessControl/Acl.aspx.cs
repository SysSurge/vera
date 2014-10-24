using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Xml;
using VeraWAF.AzureTableStorage;
using VeraWAF.Core.Templates;
using VeraWAF.WebPages.Bll;
using VeraWAF.WebPages.Bll.Security;

namespace VeraWAF.WebPages.AccessControl
{
    public partial class Acl : PageTemplateBase
    {
        readonly FileManager _fileManager;

        public Acl()
        {
            _fileManager = new FileManager();
        }

        /// <summary>
        /// Get all the file elements recursively from the web path
        /// </summary>
        /// <returns>File elements in an array</returns>
        List<StoredResourceInfo> GetFileElementNames()
        {
            const string fileMatch = @"(.*?)\.(aspx|css|htm|html|js|xhtml|xml|gif|jif|jfif|jpeg|jpg|png|tif|tiff|lic|md|txt)$";
            const string directoryIgnore = @"(App_Data|App_GlobalResources|App_Start|Batch|bin|Controls|obj|Properties|Templates)$";

            var found = new List<StoredResourceInfo>();
            var basePath = HttpContext.Current.Server.MapPath("~");
            basePath = basePath.Replace('/', '\\');
            
            _fileManager.DirSearch(ref found, basePath, true, basePath, null, fileMatch, directoryIgnore);

            return found;
        }

        /// <summary>
        /// Adds information about physical files to a XML document
        /// </summary>
        /// <param name="xmlDoc">XML documents</param>
        /// <param name="rootNode">Root node</param>
        /// <returns>Updated XML doxument</returns>
        XmlDocument AddPhysicalFiles(XmlDocument xmlDoc, XmlElement rootNode)
        {
            var fileElements = GetFileElementNames();

            var currentNode = rootNode;

            foreach (var fileEl in fileElements)
            {
                switch (fileEl.elementType)
                {
                    case StoredResourceType.Directory:
                    {
                        // Directory found

                        // Create a XML node for the directory data
                        var directoryNode = xmlDoc.CreateElement("directory");

                        var dirName = fileEl.elementName;

                        // Add the display name
                        var nameAttr = xmlDoc.CreateAttribute("name");
                        var displayName = _fileManager.GetDisplayName(dirName);

                        nameAttr.Value = String.Format("<i class=\"fa fa-folder-o\"></i> {0}", displayName);
                        directoryNode.Attributes.Append(nameAttr);

                        // Add the value
                        var valueAttr = xmlDoc.CreateAttribute("path");
                        valueAttr.Value = dirName;
                        directoryNode.Attributes.Append(valueAttr);

                        // Find the parent directory
                        while (true)
                        {
                            if (currentNode == null)
                            {
                                // We're at the root of the XML document, so set the root as current node
                                currentNode = rootNode;
                                break;
                            }

                            if (dirName.StartsWith(currentNode.GetAttribute("path")))
                            {
                                // Found a parent node, so set it
                                break;
                            }

                            currentNode = currentNode.ParentNode as XmlElement;
                        }

                        // Add the node to its parent
                        currentNode.AppendChild(directoryNode);

                        // Set the current node to this directory
                        currentNode = directoryNode;
                        break;
                    }
                    case StoredResourceType.File:
                    {
                        // File found
                        var fileName = fileEl.elementName;
                        string displayName = _fileManager.GetDisplayName(fileName);

                        // Create a XML node for the file data
                        var fileNode = xmlDoc.CreateElement("file");

                        // Add the display name
                        var nameAttr = xmlDoc.CreateAttribute("name");
                        nameAttr.Value = String.Format("<i class=\"fa {1}\"></i> {0}", displayName, _fileManager.GetFileIcon(fileName));
                        fileNode.Attributes.Append(nameAttr);

                        // Add the value
                        var valueAttr = xmlDoc.CreateAttribute("path");
                        valueAttr.Value = fileName;
                        fileNode.Attributes.Append(valueAttr);

                        currentNode.AppendChild(fileNode);

                        break;
                    }
                }
            }

            // Return the XML document
            return xmlDoc;
        }

        /// <summary>
        /// Adds information about physical files to a XML document
        /// </summary>
        /// <param name="xmlDoc">XML documents</param>
        /// <param name="rootNode">Root node</param>
        /// <returns>Updated XML doxument</returns>
        XmlDocument AddVirtualFiles(XmlDocument xmlDoc, XmlElement rootNode, SiteMapNode siteMapNode)
        {
            if (siteMapNode == null) return xmlDoc;

            var currentNode = rootNode;

            foreach (SiteMapNode fileEl in siteMapNode.ChildNodes)
            {
                if (fileEl.HasChildNodes)
                {
                    // Directory found

                    // Create a XML node for the directory data
                    var directoryNode = xmlDoc.CreateElement("directory");

                    var dirName = fileEl.Url;

                    // Add the display name
                    var nameAttr = xmlDoc.CreateAttribute("name");
                    string displayName = _fileManager.GetDisplayName(dirName);

                    nameAttr.Value = String.Format("<i class=\"fa fa-folder-o\"></i> {0}", displayName);
                    directoryNode.Attributes.Append(nameAttr);

                    // Add the value
                    var valueAttr = xmlDoc.CreateAttribute("path");
                    valueAttr.Value = dirName;
                    directoryNode.Attributes.Append(valueAttr);

                    // Find the parent directory
                    while (true)
                    {
                        if (currentNode == null)
                        {
                            // We're at the root of the XML document, so set the root as current node
                            currentNode = rootNode;
                            break;
                        }

                        if (dirName.StartsWith(currentNode.GetAttribute("path")))
                        {
                            // Found a parent node, so set it
                            break;
                        }

                        currentNode = currentNode.ParentNode as XmlElement;
                    }

                    // Add the node to its parent
                    currentNode.AppendChild(directoryNode);

                    // Set the current node to this directory
                    currentNode = directoryNode;

                    // Enumerate child node
                    xmlDoc = AddVirtualFiles(xmlDoc, currentNode, fileEl);
                }
                else
                {
                    // File found
                    var fileName = fileEl.Url;
                    string displayName = _fileManager.GetDisplayName(fileName);

                    // Create a XML node for the file data
                    var fileNode = xmlDoc.CreateElement("file");

                    // Add the display name
                    var nameAttr = xmlDoc.CreateAttribute("name");
                    nameAttr.Value = String.Format("<i class=\"fa {1}\"></i> {0}", displayName, _fileManager.GetFileIcon(fileName));
                    fileNode.Attributes.Append(nameAttr);

                    // Add the value
                    var valueAttr = xmlDoc.CreateAttribute("path");
                    valueAttr.Value = fileName;
                    fileNode.Attributes.Append(valueAttr);

                    currentNode.AppendChild(fileNode);
                }
            }

            return xmlDoc;
        }

        /// <summary>
        /// Add the tables and tale properties to a XML document
        /// </summary>
        XmlDocument BuildXmlData()
        {
            var xmlDoc = new XmlDocument();
            var datasource = new AzureTableStorageDataSource();

            // Create a root node for the physical files
            var rootNode = xmlDoc.CreateElement("resources");
            var rootNodeNameAttr = xmlDoc.CreateAttribute("name");
            rootNodeNameAttr.Value = String.Format("<i class=\"fa fa-cubes\"></i> {0}", Bll.Resources.Controls.Resources);
            rootNode.Attributes.Append(rootNodeNameAttr);
            xmlDoc.AppendChild(rootNode);

            // Add all the table names
            var tableNames = datasource.ListAllTables();

            // Add database root node
            var dbRootNode = xmlDoc.CreateElement("database");
            var dbRootNodeNameAttr = xmlDoc.CreateAttribute("name");
            dbRootNodeNameAttr.Value = String.Format("<i class=\"fa fa-database\"></i> {0}", Bll.Resources.Controls.Database);
            dbRootNode.Attributes.Append(dbRootNodeNameAttr);
            rootNode.AppendChild(dbRootNode);

            // Add tables node
            var tablesNode = xmlDoc.CreateElement("tables");
            var tablesNodeNameAttr = xmlDoc.CreateAttribute("name");
            tablesNodeNameAttr.Value = String.Format("<i class=\"fa fa-table\"></i> {0}", Bll.Resources.Controls.Tables);
            tablesNode.Attributes.Append(tablesNodeNameAttr);
            var tablesNodeValueAttr = xmlDoc.CreateAttribute("path");
            tablesNodeValueAttr.Value = String.Format("{0}:TABLE:", ConfigurationManager.AppSettings["ApplicationName"]);
            tablesNode.Attributes.Append(tablesNodeValueAttr);

            dbRootNode.AppendChild(tablesNode);

            foreach(var tableName in tableNames)
            {
                // Add table
                var tableNode = xmlDoc.CreateElement("table");
                var tableNodeNameAttr = xmlDoc.CreateAttribute("name");
                tableNodeNameAttr.Value = String.Format("<i class=\"fa fa-table\"></i> {0}", tableName);
                tableNode.Attributes.Append(tableNodeNameAttr);

                // Add the value
                var valueAttr = xmlDoc.CreateAttribute("path");
                valueAttr.Value = String.Format("{0}:TABLE:{1}", ConfigurationManager.AppSettings["ApplicationName"], tableName);
                tableNode.Attributes.Append(valueAttr);

                tablesNode.AppendChild(tableNode);
            }

            // Phsycial files storage
            var physFilesRootNode = xmlDoc.CreateElement("storage");
            var physFilesRootNodeNameAttr = xmlDoc.CreateAttribute("name");
            physFilesRootNodeNameAttr.Value = String.Format("<i class=\"fa fa-floppy-o\"></i> {0}", Bll.Resources.Controls.PhysicalFiles);
            physFilesRootNode.Attributes.Append(physFilesRootNodeNameAttr);
            rootNode.AppendChild(physFilesRootNode);

            // Add the physical files
            xmlDoc = AddPhysicalFiles(xmlDoc, physFilesRootNode);

            // Virtual files storage
            var virtFilesRootNode = xmlDoc.CreateElement("storage");
            var virtFilesRootNodeNameAttr = xmlDoc.CreateAttribute("name");
            virtFilesRootNodeNameAttr.Value = String.Format("<i class=\"fa fa-files-o\"></i> {0}", Bll.Resources.Controls.VirtualFiles);
            virtFilesRootNode.Attributes.Append(virtFilesRootNodeNameAttr);
            rootNode.AppendChild(virtFilesRootNode);

            // Add the virtual files
            xmlDoc = AddVirtualFiles(xmlDoc, virtFilesRootNode, SiteMap.Provider.RootNode);

            return xmlDoc;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // Add all the items to the combo box
            var xmlDoc = BuildXmlData();

            // Use the XML document as a data source
            treeViewXmlDataSource.Data = xmlDoc.OuterXml;
        }

        void FillDialog()
        {
            if (TreeView1.SelectedNode == null) 
                return; // Nothing selected

            // Load all the rules as the dialog is now open
            var datasource = new AzureTableStorageDataSource();

            var fullPath = TreeView1.SelectedNode.Value.Replace('\\', '/');

            var rules = datasource.GetRules(fullPath, ConfigurationManager.AppSettings["ApplicationName"]);

            foreach (var rule in rules)
            {
                var ruleControl = (Controls.RulePermissions)LoadControl("~/Controls/RulePermissions.ascx");

                ruleControl.PartitionKey = rule.PartitionKey;
                ruleControl.RowKey = rule.RowKey;

                // Bitmasks determins the permissions to the resource
                ruleControl.AllowRead = (rule.Permissions & (int)AccessControlManager.EPermission.AllowRead) != 0;
                ruleControl.AllowWrite = (rule.Permissions & (int)AccessControlManager.EPermission.AllowWrite) != 0;
                ruleControl.AllowDelete = (rule.Permissions & (int)AccessControlManager.EPermission.AllowDelete) != 0;
                ruleControl.DenyRead = (rule.Permissions & (int)AccessControlManager.EPermission.DenyRead) != 0;
                ruleControl.DenyWrite = (rule.Permissions & (int)AccessControlManager.EPermission.DenyWrite) != 0;
                ruleControl.DenyDelete = (rule.Permissions & (int)AccessControlManager.EPermission.DenyDelete) != 0;

                phRules.Controls.Add(ruleControl);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Add the dialog dynamic controls
            FillDialog();
        }


        /// <summary>
        /// Called when the user selects a node in the treeview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void TreeView1_SelectedNodeChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(TreeView1.SelectedNode.Value)) return;

            litResource.Text = TreeView1.SelectedNode.Value;

            // Show the file explorer modal dialog
            fsDialog.Visible = true;

            // Make sure that it is visible using CSS too
            fsDialog.Attributes.Add("style", "display:inline-block");

            // Make the background cover visible too
            fileexplorerBg.Attributes.Add("style", "display:inline-block");
        }

        /// <summary>
        /// Hides the file explorer modal dialog
        /// </summary>
        void HideDialog()
        {
            // Hide the file explorer modal dialog
            fsDialog.Visible = false;

            // Make sure that it is hidden using CSS too
            fsDialog.Attributes.Remove("style");

            // Make the background covering hidden too
            fileexplorerBg.Attributes.Remove("style");
        }

        /// <summary>
        /// Called when the user clicks the OK button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void butSelect_Click(object sender, EventArgs e)
        {
            // Concat path and filename into the resulting full path
            //txtResult.Text = txtFullPath.Text + txtFilename.Text;

            HideDialog();
        }

        /// <summary>
        /// Called when the user clicks the Cancel button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void butCancel_Click(object sender, EventArgs e)
        {
            HideDialog();
        }

        protected void butAdd_Click(object sender, EventArgs e)
        {
            var datasource = new AzureTableStorageDataSource();

            var rule = new AccessControlEntity();
            rule.ApplicationName = ConfigurationManager.AppSettings["ApplicationName"];
            rule.PartitionKey = litResource.Text;
            rule.RowKey = txtUserOrRole.Text;

            // Is this a role or a user?
            if (!datasource.RoleExistsByRoleName(txtUserOrRole.Text, rule.ApplicationName)
                && datasource.FindUser(txtUserOrRole.Text, rule.ApplicationName) == null)
            {
                notifications.AddMessage("User or role not found");
                HideDialog();
                return;
            }

            if (datasource.AclRuleExists(rule.PartitionKey, rule.RowKey, rule.ApplicationName))
            {
                notifications.AddMessage("Rule exists");
                HideDialog();
                return;
            }

            datasource.Insert(rule);
        }
    }
}