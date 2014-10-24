using System;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.Profile;
using System.Web.UI;
using VeraWAF.AzureTableStorage;
using VeraWAF.Core.Templates;
using VeraWAF.WebPages.Bll;

namespace VeraWAF.WebPages.Controls
{
    /// <summary>
    /// Shows the full user name or the accound name
    /// </summary>
    public partial class UserName : UserFieldControlBase
    {
        /// <summary>
        /// Vera base page 
        /// </summary>
        PageTemplateBase _page;

        /// <summary>
        /// Show the user's full name or user name
        /// </summary>
        /// <remarks>
        /// Called by VeraWAF.WebPages.Bll.UserFieldControlBase after having loaded the user info.
        /// </remarks>
        /// <param name="user">User</param>
        protected override void FillControlFields(MembershipUser user)
        {
            var displayName = new UserUtilities().GetDisplayName(user);
            litMarkup.Text = ProcessFieldData(displayName);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Get the parent page
            _page = Page as PageTemplateBase;
            if (_page == null)
                throw new ApplicationException("The page does not inherit from the VeraWAF.Core.Templates.PageTemplateBase page template.");

            // Get page entity data
            var pageEntity = _page.GetPageEntity();

            // Process user info and fill fields
            ProcessField(pageEntity);
        }
    }
}