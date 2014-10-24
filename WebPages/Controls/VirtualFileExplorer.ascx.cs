using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Serialization;
using VeraWAF.WebPages.Bll;

namespace VeraWAF.WebPages.Controls
{
    /// <summary>
    /// Allows the user to browser & select from all available virtual and physical files and directories.
    /// </summary>
    /// <remarks>
    /// Styled in fileexplorer.css.
    /// </remarks>
    /// <example>
    /// <%@ Register src="~/Controls/VirtualFileExplorer.ascx" tagname="VirtualFileExplorer" tagprefix="uc1" %>
    /// 
    /// <uc1:VirtualFileExplorer id="fileExplorer" RemoveBasePath="true" ShowFiles="true" ShowPath="true" ShowFullPath="false" 
    ///     FileMatch="(.*?)\.(aspx|css|htm|html|js|xhtml|xml)$" 
    ///     DirectoryIgnore="(App_Data|App_GlobalResources|App_Start|Batch|bin|Controls|obj|Properties|Templates)$" runat="server">
    /// </uc1:VirtualFileExplorer>
    /// </example>
    public partial class VirtualFileExplorer : UserControl
    {
        /// <summary>
        /// File manager
        /// </summary>
        readonly FileManager _fileManager;

        /// <summary>
        /// Path to root directory. All paths are relative to the web directory
        /// and should start with a "/" character
        /// Default value is the web directory "/"
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Set to True to remove the base path from all the paths.
        /// Default is False.
        /// </summary>
        public bool RemoveBasePath { get; set; }

        /// <summary>
        /// Set to True to show the directories.
        /// Default is True.
        /// </summary>
        public bool ShowDirectories { get; set; }

        /// <summary>
        /// Set to True to show the files.
        /// Default is True.
        /// </summary>
        public bool ShowFiles { get; set; }

        /// <summary>
        /// Set to True to enable directory selection in the dropdown list.
        /// Default is true.
        /// </summary>
        public bool EnableDirectories { get; set; }

        /// <summary>
        /// Set if something should be selected by default
        /// </summary>
        public string SelectionMatch { get; set; }

        /// <summary>
        /// Set if some directories be matched. Regular expression
        /// </summary>
        public string DirectoryMatch { get; set; }

        /// <summary>
        /// Set if some files should be matched. Regular expression
        /// </summary>
        public string FileMatch { get; set; }

        /// <summary>
        /// Set if some directories be ignored. Regular expression
        /// </summary>
        public string DirectoryIgnore { get; set; }

        /// <summary>
        /// Set if some files should be ignored. Regular expression
        /// </summary>
        public string FileIgnore { get; set; }

        /// <summary>
        /// Set to True to show the path.
        /// Default is True.
        /// </summary>
        public bool ShowPath { get; set; }

        /// <summary>
        /// Set to True to show the full path when displaying the filenames.
        /// Default is True.
        /// </summary>
        public bool ShowFullPath { get; set; }

        /// <summary>
        /// Set to True to show the physical files in the dialog.
        /// Default is True.
        /// </summary>
        public bool ShowPhysicalFiles { get; set; }

        /// <summary>
        /// Set to True to show the virtual files in the dialog.
        /// Default is True.
        /// </summary>
        public bool ShowVirtualFiles { get; set; }

        /// <summary>
        /// Get or set the selected file
        /// </summary>
        public string Value
        {
            get
            {
                return txtResult.Text;
            }
            set
            {
                txtResult.Text = value;
            }
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        public VirtualFileExplorer()
        {
            // Set the default path to the web directory
            Path = "/";

            // Show the directories
            ShowDirectories = true;

            // Show the files
            ShowFiles = true;

            // Enable directory selection 
            EnableDirectories = true;

            // Show the path
            ShowPath = true;

            // Show the path with the file names
            ShowFullPath = true;

            // Create file manager
            _fileManager = new FileManager();

            // Show both the physical and virtual files by default
            ShowPhysicalFiles = true;
            ShowVirtualFiles = true;
        }

        /// <summary>
        /// Get all the file elements recursively from the web path
        /// </summary>
        /// <returns>File elements in an array</returns>
        List<StoredResourceInfo> GetFileElementNames()
        {
            var found = new List<StoredResourceInfo>();
            var basePath = HttpContext.Current.Server.MapPath("~") + Path;
            basePath = basePath.Replace('/', '\\');
            _fileManager.DirSearch(ref found, basePath, RemoveBasePath, basePath, DirectoryMatch, FileMatch, DirectoryIgnore, FileIgnore);
            return found;
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

                    if (ShowDirectories)
                    {
                        var dirName = fileEl.Url;

                        // Add the display name
                        var nameAttr = xmlDoc.CreateAttribute("name");
                        string displayName = _fileManager.GetDisplayName(dirName, ShowFullPath);

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
                    if (!ShowFiles) break;

                    // File found
                    var fileName = fileEl.Url;
                    string displayName = _fileManager.GetDisplayName(fileName, ShowFullPath);

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

                        if (ShowDirectories)
                        {
                            var dirName = fileEl.elementName;

                            // Add the display name
                            var nameAttr = xmlDoc.CreateAttribute("name");
                            string displayName = _fileManager.GetDisplayName(dirName, ShowFullPath);

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
                        }

                        // Add the node to its parent
                        currentNode.AppendChild(directoryNode);

                        // Set the current node to this directory
                        currentNode = directoryNode;
                        break;
                    }
                    case StoredResourceType.File:
                    {
                        if (!ShowFiles) break;

                        // File found
                        var fileName = fileEl.elementName;
                        string displayName = _fileManager.GetDisplayName(fileName, ShowFullPath);

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
                    default:
                        throw new ApplicationException("Bad selection");
                }
            }

            // Return the XML document
            return xmlDoc;
        }

        /// <summary>
        /// Add the file and directories to a XML document
        /// </summary>
        XmlDocument BuildXmlData()
        {
            // Create a XML document to act as a data source
            var xmlDoc = new XmlDocument();

            // Create a root node for the physical files
            var rootNode = xmlDoc.CreateElement("directories");
            var rootNodeNameAttr = xmlDoc.CreateAttribute("name");
            rootNodeNameAttr.Value = String.Format("<i class=\"fa fa-database\"></i> {0}", Bll.Resources.Controls.StorageDevices);
            rootNode.Attributes.Append(rootNodeNameAttr);
            xmlDoc.AppendChild(rootNode);

            // Should we show the physical files?
            if (ShowPhysicalFiles)
            {
                // Phsycial files storage
                var physFilesRootNode = xmlDoc.CreateElement("storage");
                var physFilesRootNodeNameAttr = xmlDoc.CreateAttribute("name");
                physFilesRootNodeNameAttr.Value = String.Format("<i class=\"fa fa-floppy-o\"></i> {0}", Bll.Resources.Controls.PhysicalFiles);
                physFilesRootNode.Attributes.Append(physFilesRootNodeNameAttr);
                rootNode.AppendChild(physFilesRootNode);

                // Add the physical files
                xmlDoc = AddPhysicalFiles(xmlDoc, physFilesRootNode);
            }

            // Should we show the virtual files?
            if (ShowVirtualFiles)
            {
                // Virtual files storage
                var virtFilesRootNode = xmlDoc.CreateElement("storage");
                var virtFilesRootNodeNameAttr = xmlDoc.CreateAttribute("name");
                virtFilesRootNodeNameAttr.Value = String.Format("<i class=\"fa fa-files-o\"></i> {0}", Bll.Resources.Controls.VirtualFiles);
                virtFilesRootNode.Attributes.Append(virtFilesRootNodeNameAttr);
                rootNode.AppendChild(virtFilesRootNode);

                // Add the virtual files
                xmlDoc = AddVirtualFiles(xmlDoc, virtFilesRootNode, SiteMap.Provider.RootNode);
            }

            // Return the XML document with all the file & directory data
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

        /// <summary>
        /// Called when the user selects a node in the treeview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void TreeView1_SelectedNodeChanged(object sender, EventArgs e)
        {
            // normalize path seperators
            var fullPath = TreeView1.SelectedNode.Value.Replace('\\', '/');
            fullPath = fullPath.Replace("//", "/");

            // Make sure the path is absolute
            if (!fullPath.StartsWith("/")) fullPath = "/" + fullPath;

            // Seperate the filename from the path
            var lastIdx = fullPath.LastIndexOf('/') + 1;
            var filename = fullPath.Substring(lastIdx);
            txtFilename.Text = filename;

            // Get the path only
            var path = fullPath.Substring(0, lastIdx);
            txtFullPath.Text = path;

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            pFile.Visible = ShowFiles;
            pPath.Visible = ShowPath;
        }

        /// <summary>
        /// Called when the user clicks the browse button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void butBrowse_Click(object sender, EventArgs e)
        {
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
            txtResult.Text = txtFullPath.Text + txtFilename.Text;

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
    }
}