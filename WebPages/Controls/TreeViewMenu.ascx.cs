using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace VeraWAF.WebPages.Controls {
    public partial class Level3Menu : UserControl {

        /// <summary>
        /// Level to start showing the menu; 0 means at the root level.
        /// Default value is 0.
        /// </summary>
        public int MenuLevel { get; set; }

        /// <summary>
        /// Number of node levels to expand.
        /// Default value is 2.
        /// </summary>
        public int ExpandDepth { get; set; }

        /// <summary>
        /// Should the menu show the starting node?
        /// Default value is true.
        /// </summary>
        public bool ShowStartingNode { get; set; }

        /// <summary>
        /// Should the menu start from the current node?
        /// Default value is false.
        /// Will override MenuLevel if set to true.
        /// </summary>
        public bool StartFromCurrentNode { get; set; }

        /// <summary>
        /// Starting node offset
        /// Default value is 0.
        /// </summary>
        public int StartingNodeOffset { get; set; }

        /// <summary>
        /// Should the menu show expand/collapse icons before a parent node?
        /// Default value is true.
        /// </summary>
        public bool ShowExpandCollapse { get; set; }

        /// <summary>
        /// Should the menu show connecting lines between nodes?
        /// Default value is true.
        /// </summary>
        public bool ShowLines { get; set; }

        /// <summary>
        /// Should the menu show checkboxes before each node?
        /// Default value is TreeNodeTypes.None.
        /// </summary>
        public TreeNodeTypes ShowCheckBoxes { get; set; }
        
        /// <summary>
        /// Class contructor
        /// </summary>
        public Level3Menu()
        {
            // Show menu from the top level as default
            MenuLevel = 0;

            // Expand two levels as default
            ExpandDepth = 2;

            ShowStartingNode = true;
            StartFromCurrentNode = false;

            StartingNodeOffset = 0;

            ShowCheckBoxes = TreeNodeTypes.None;
            ShowExpandCollapse = true;
            ShowLines = true;
        }

        /// <summary>
        /// Get all the parent menu nodes from the sitemap provider
        /// </summary>
        /// <param name="startNode">Start node</param>
        /// <returns>Parent nodes as a queue</returns>
        Queue<SiteMapNode> GetParentNodes(SiteMapNode startNode) {
            var queue = new Queue<SiteMapNode>();
            queue.Enqueue(startNode);

            var currentNode = startNode;

            while (currentNode.ParentNode != null) {
                currentNode = currentNode.ParentNode;
                queue.Enqueue(currentNode);
            }

            return queue;
        }

        /// <summary>
        /// Expands a parent node from the sitemap provider
        /// </summary>
        /// <param name="startNode">Child node</param>
        void ExpandParentNodes(TreeNode startNode) {
            if (startNode == null) return;

            var currentNode = startNode.Parent;

            while (currentNode != null) {
                currentNode.Expand();
                currentNode = currentNode.Parent;
            }                
        }

        protected void Page_Load(object sender, EventArgs e) {
            if (SiteMap.CurrentNode != null) {
                var nodeStructure = GetParentNodes(SiteMap.CurrentNode);

                // Calculate at what level to start showing the menu
                int index = 0;
                if (MenuLevel < 0)
                {
                    index = 0;
                }
                else if (MenuLevel >= nodeStructure.Count)
                {
                    index = nodeStructure.Count - 1;
                }

                siteMapDataSource.ShowStartingNode = ShowStartingNode;
                siteMapDataSource.StartingNodeOffset = StartingNodeOffset;

                // Start from current node or a specific level?
                if (StartFromCurrentNode == false)
                    siteMapDataSource.StartingNodeUrl = nodeStructure.ToArray()[index].Url;
                siteMapDataSource.StartFromCurrentNode = StartFromCurrentNode;

                mnuTreeView.ExpandDepth = ExpandDepth;
                mnuTreeView.ShowCheckBoxes = ShowCheckBoxes;
                mnuTreeView.ShowExpandCollapse = ShowExpandCollapse;
                mnuTreeView.ShowLines = ShowLines;

                // Expand the currently selcted node in the treeview menu
                ExpandParentNodes(mnuTreeView.SelectedNode);
            }
        }
    }
}