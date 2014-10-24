using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace VeraWAF.WebPages.Controls
{
    /// <summary>
    /// Combo box that lets the user select files within a specified directory
    /// </summary>
    public partial class RecursiveDirComboBox : UserControl
    {
        /// <summary>
        /// File element types
        /// </summary>
        public enum RecursiveDirComboBoxElType
        {
            File, Directory
        }

        /// <summary>
        /// Default item text
        /// </summary>
        public string StaticItemText { get; set; }

        /// <summary>
        /// Default item value
        /// </summary>
        public string StaticItemValue { get; set; }


        /// <summary>
        /// File element type
        /// </summary>
        public struct RecursiveDirComboBoxInfo
        {
            /// <summary>
            /// Struct constructor
            /// </summary>
            /// <param name="elName"></param>
            /// <param name="elType"></param>
            public RecursiveDirComboBoxInfo(string elName, RecursiveDirComboBoxElType elType, int inDepth)
            {
                elementName = elName;
                elementType = elType;
                depth = inDepth;
            }

            /// <summary>
            /// File element name
            /// </summary>
            public string elementName;

            /// <summary>
            /// File element type
            /// </summary>
            public RecursiveDirComboBoxElType elementType;

            /// <summary>
            /// The depth within the file structure that the element was found.
            /// </summary>
            /// <remarks>Starts at 1</remarks>
            public int depth;
        }

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
        /// User to filter on file last name, ex. to only show ".aspx" files
        /// </summary>
        public string FilenameFilter { get; set; }

        /// <summary>
        /// Set if something should be selected by default
        /// </summary>
        public string SelectionMatch { get; set; }

        /// <summary>
        /// Set if something should be ignored
        /// </summary>
        public string IgnoreMatch { get; set; }

        /// <summary>
        /// Set to True to show depth.
        /// Default is False.
        /// </summary>
        public bool ShowDepth { get; set; }

        /// <summary>
        /// Get or set the selected value
        /// </summary>
        public string SelectedValue
        {
            get 
            {
                return cmbDirectories.SelectedValue;
            }
            set
            {
                // De-select the old selection, if any
                var listItem = cmbDirectories.SelectedItem;
                if (listItem != null)
                    listItem.Selected = false;

                // Select the new item
                listItem = cmbDirectories.Items.FindByValue(value);
                if (listItem != null)
                    listItem.Selected = true;
            }
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        public RecursiveDirComboBox()
        {
            // Set the default path to the web directory
            Path = "/";

            // Show the directories
            ShowDirectories = true;

            // Show the files
            ShowFiles = true;

            // Enable directory selection in the list
            EnableDirectories = true;
        }

        /// <summary>
        /// Does a recursive search to find all files and directories
        /// </summary>
        /// <param name="sDir">Start directory</param>
        /// <param name="elements">All found files and directories are added here</param>
        void DirSearch(string sDir, ref List<RecursiveDirComboBoxInfo> elements, bool removeBasePath, string basePath, int depth = 1)
        {
            var basePathLen = basePath.Length;
            try
            {
                foreach (var d in Directory.GetDirectories(sDir))
                {
                    // Should we ignore this?
                    if (!String.IsNullOrEmpty(IgnoreMatch) && d.Contains(IgnoreMatch))
                        continue;

                    // Remove the base path if requested
                    var dirText = removeBasePath ? d.Substring(basePathLen) : d;

                    elements.Add(new RecursiveDirComboBoxInfo(dirText, RecursiveDirComboBoxElType.Directory, depth));

                    foreach (var f in Directory.GetFiles(d))
                    {
                        // Should we ignore this?
                        if (!String.IsNullOrEmpty(IgnoreMatch) && f.Contains(IgnoreMatch))
                            continue;

                        // Should we filter out some of the files?
                        if (!(!String.IsNullOrEmpty(FilenameFilter) && f.EndsWith(FilenameFilter)))
                            continue;   // Skip file as it does not match the filter

                        // Remove the base path if requested
                        var fileText = removeBasePath ? f.Substring(basePathLen) : f;

                        elements.Add(new RecursiveDirComboBoxInfo(fileText, RecursiveDirComboBoxElType.File, depth));
                    }

                    DirSearch(d, ref elements, removeBasePath, basePath, depth + 1);
                }
            }
            catch (Exception)
            {
                // Skip directories/files where we have no access
            }
        }

        /// <summary>
        /// Get all the file elements
        /// </summary>
        /// <returns>File elements in an array</returns>
        List<RecursiveDirComboBoxInfo> GetFileElementNames()
        {
            var found = new List<RecursiveDirComboBoxInfo>();
            var basePath = HttpContext.Current.Server.MapPath("~") + Path;
            basePath = basePath.Replace('/', '\\');
            DirSearch(basePath, ref found, RemoveBasePath, basePath);
            return found;
        }

        /// <summary>
        /// Add the file and directories to a combo box
        /// </summary>
        /// <param name="dropDownControl">Combo box</param>
        void AddDropdownListItems(DropDownList dropDownControl)
        {
            if (!String.IsNullOrEmpty(StaticItemText))
            {
                // Add a static item
                dropDownControl.Items.Add(new ListItem(StaticItemText, StaticItemValue));
            }

            var fileElements = GetFileElementNames();

            foreach (var fileEl in fileElements)
            {
                ListItem dlItem;
                
                switch(fileEl.elementType)
                {
                    case RecursiveDirComboBoxElType.Directory:
                        var dirName = fileEl.elementName;

                        // Add some spaces to indicate depth
                        if (ShowDepth)
                            for (var i = 1; i < fileEl.depth; i++)
                                dirName = "--" + dirName;

                        dlItem = new ListItem(dirName, fileEl.elementName, ShowDirectories);

                        // Disable directories?
                        if (!EnableDirectories) dlItem.Attributes.Add("disabled", "disabled");

                        break;
                    case RecursiveDirComboBoxElType.File:
                        var fileName = fileEl.elementName;

                        // Add some spaces to indicate depth
                        if (ShowDepth)
                            for (var i = 0; i < fileEl.depth; i++)
                                fileName = "--" + fileName;

                        dlItem = new ListItem(fileName, fileEl.elementName, ShowFiles);
                        break;
                    default:
                        throw new ApplicationException("Bad selection");
                }

                // Should this item be selected by default?
                dlItem.Selected = !String.IsNullOrEmpty(SelectionMatch) && SelectionMatch == dlItem.Text;

                dropDownControl.Items.Add(dlItem);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // Add all the items to the combo box
            if (!Page.IsPostBack)
                AddDropdownListItems(cmbDirectories);
        }
    }
}