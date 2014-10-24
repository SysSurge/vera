using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;

namespace VeraWAF.WebPages.Controls
{
    public partial class Level2Menu : UserControl
    {
        /// <summary>
        /// The parent menu node location where to start showing the menu items.
        /// Default value is 0.
        /// </summary>
        public int ParentIndex { get; set; }

        /// <summary>
        /// Class constructor
        /// </summary>
        public Level2Menu()
        {
            ParentIndex = 0;
        }

        /// <summary>
        /// Get all the parents menu nodes as a queue
        /// </summary>
        /// <param name="startNode">Starting menu node</param>
        /// <returns>All startNode's parents menu nodes as a queue</returns>
        Queue<SiteMapNode> GetParentNodes(SiteMapNode startNode)
        {
            var queue = new Queue<SiteMapNode>();
            queue.Enqueue(startNode);

            var currentNode = startNode;

            while (currentNode.ParentNode != null)
            {
                currentNode = currentNode.ParentNode;
                queue.Enqueue(currentNode);
            }

            return queue;
        }

        /// <summary>
        /// Set the start node
        /// </summary>
        void SetStartNode(int parentIndex)
        {
            if (SiteMap.CurrentNode != null)
            {
                var nodeStructure = GetParentNodes(SiteMap.CurrentNode);

                var index = nodeStructure.Count - parentIndex;
                if (index < 0) index = 0;

                siteMapDataSource.StartingNodeUrl = nodeStructure.ToArray()[index].Url;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Start at parent node?
            if (ParentIndex > 0)
                SetStartNode(ParentIndex); // Yes, start at a parent node
        }
    }
}