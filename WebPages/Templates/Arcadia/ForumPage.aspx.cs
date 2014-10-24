using System;
using System.Web;
using VeraWAF.AzureTableStorage;
using VeraWAF.Core.Templates;
using VeraWAF.WebPages.Bll;
using VeraWAF.WebPages.Bll.Security;

namespace VeraWAF.WebPages.Templates {

    /// <summary>
    /// Forum page
    /// </summary>
    public partial class ForumPage : PageTemplateBase 
    {
        /// <summary>
        /// Page entity data
        /// </summary>
        PageEntity _currentPage;

        /// <summary>
        /// Add a edit link if the user has the correct priviledges
        /// </summary>
        /// <param name="page">Page entity</param>
        void InitEditPageControls(PageEntity page)
        {
            var userHasEditPermissions = new AccessControlManager().UserHasEditPermissions(page);
            panEditPost.Visible = userHasEditPermissions;

            if (userHasEditPermissions)
                lnkEditPost.Attributes["href"] = "Edit.aspx?path=" + HttpUtility.UrlEncode(page.VirtualPath);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
             _currentPage = GetPageEntity();

             if (_currentPage != null)
                 InitEditPageControls(_currentPage);
        }

    }
}