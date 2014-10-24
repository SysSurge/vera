using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Collections;
using System.Web.Security;
using System.Web.UI.WebControls;
using VeraWAF.AzureTableStorage;
using VeraWAF.Core.Templates;
using VeraWAF.WebPages.Bll;
using VeraWAF.WebPages.Bll.Cloud;
using VeraWAF.WebPages.Controls;

namespace VeraWAF.WebPages.AccessControl
{
    public partial class Roles_ : PageTemplateBase
    {
        /// <summary>
        /// Max number of items per page
        /// </summary>
        const int pageSize = 25;

        /// <summary>
        /// Total number of roles
        /// </summary>
        int totalRoles;

        /// <summary>
        /// Total number of pages
        /// </summary>
        int totalPages;

        /// <summary>
        /// Current page number
        /// </summary>
        int currentPage = 1;

        /// <summary>
        /// Azure table data source
        /// </summary>
        readonly AzureTableStorageDataSource _datasource;

        /// <summary>
        /// Applciation name
        /// </summary>
        readonly string _applicationName;

        /// <summary>
        /// Text utilities
        /// </summary>
        TextUtilities _textUtils;

        /// <summary>
        /// Class constructor
        /// </summary>
        public Roles_()
        {
            _datasource = new AzureTableStorageDataSource();
            _applicationName = ConfigurationManager.AppSettings["ApplicationName"];

            _textUtils = new TextUtilities();
        }

        /// <summary>
        /// Reloads the current page
        /// </summary>
        void ReloadPage()
        {
            Response.Redirect(Request.Path, true);
        }

        /// <summary>
        /// Get all the members of a role
        /// </summary>
        /// <param name="roleName">Role name</param>
        /// <returns>Returns all the members of the role as a CSV</returns>
        protected string GetMembers(string roleName)
        {
            var users = Roles.GetUsersInRole(roleName);
            return String.Join(", ", users);
        }

        /// <summary>
        /// Clips a string.
        /// Also does HTML encoding.
        /// </summary>
        /// <param name="inString">Input string</param>
        /// <param name="maxLen">Maximum number of characters allowed</param>
        /// <returns>Clipped and HTML encoded string</returns>
        protected string ClipString(string inString, int maxLen)
        {
            if (String.IsNullOrEmpty(inString)) return inString;

            return Server.HtmlEncode(_textUtils.ClipLeft(inString, maxLen));
        }

        /// <summary>
        /// Load all the roles
        /// </summary>
        void GetRoles()
        {
            var allRoles = new List<RoleEntity>(_datasource.GetAllRoles(_applicationName));
            RolesGrid.DataSource = allRoles;

            totalRoles = allRoles.Count;
            totalPages = ((totalRoles - 1) / pageSize) + 1;

            // Ensure that we do not navigate past the last page of users.
            if (currentPage > totalPages)
            {
                currentPage = totalPages;
                GetRoles();
                return;
            }

            RolesGrid.DataBind();
            CurrentPageLabel.Text = currentPage.ToString();
            TotalPagesLabel.Text = totalPages.ToString();

            NextButton.Visible = currentPage != totalPages;

            PreviousButton.Visible = currentPage != 1;

            NavigationPanel.Visible = totalRoles > 0;
        }

        /// <summary>
        /// Called when the user clicks the next button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void NextButton_OnClick(object sender, EventArgs args)
        {
            currentPage = Convert.ToInt32(CurrentPageLabel.Text);
            currentPage++;
            GetRoles();
        }

        /// <summary>
        /// Called when the user clicks the previous button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void PreviousButton_OnClick(object sender, EventArgs args)
        {
            currentPage = Convert.ToInt32(CurrentPageLabel.Text);
            currentPage--;
            GetRoles();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            GetRoles();
        }

        /// <summary>
        /// Called when the user clicks a role edit link
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void RolesGrid_EditCommand(object source, DataGridCommandEventArgs e)
        {
            // Go to a more detailed view of the log item entry
            var partitionKey = ((DataGrid)source).Items[e.Item.ItemIndex].Cells[0].Text;
            Response.Redirect(String.Format("/Admin/EditRole.aspx?id={0}",
                partitionKey), true);
        }

        /// <summary>
        /// Clears the user cache
        /// </summary>
        void ClearCache()
        {
            new CloudCommand().Execute("ClearUserCache");
            new CloudCommandClient().SendCommand("ClearUserCache", true);
        }

        /// <summary>
        /// Called when the user clicks the add role button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnAddRole_Click(object sender, EventArgs e)
        {
            try
            {
                if (Roles.RoleExists(txtRoleName.Text))
                    throw new ApplicationException("Role already exists");

                // Add the user to the role
                Roles.CreateRole(txtRoleName.Text);
            }
            catch (Exception ex)
            {
                notifications.AddMessage(ex.Message);
                return;
            }

            ClearCache();

            // Refresh page to reflect changes
            ReloadPage();
        }

        /// <summary>
        /// Called when the user clicks the remove role button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnRemoveRole_Click(object sender, EventArgs e)
        {
            try
            {
                // Prevent accidental removal of the admin role
                if (txtRoleName.Text == ConfigurationManager.AppSettings["AdminRoleName"])
                    throw new ApplicationException("You are not to remove the default system administrator role");

                // Prevent accidental deletion of the editors
                if (txtRoleName.Text == ConfigurationManager.AppSettings["EditorRoleName"])
                    throw new ApplicationException("You are not to remove the default editor role");

                if (!Roles.RoleExists(txtRoleName.Text))
                    throw new ApplicationException("Role don't exist");

                var members = Roles.GetUsersInRole(txtRoleName.Text);
                Roles.RemoveUsersFromRole(members, txtRoleName.Text);
                Roles.DeleteRole(txtRoleName.Text);
            }
            catch (Exception ex)
            {
                notifications.AddMessage(ex.Message);
                return;
            }

            ClearCache();

            // Refresh page to reflect changes
            ReloadPage();
        }
    }
}