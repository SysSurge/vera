using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using VeraWAF.AzureTableStorage;
using VeraWAF.Core.Templates;
using VeraWAF.CrossCuttingConcerns;
using VeraWAF.WebPages.Bll;
using VeraWAF.WebPages.Bll.Cloud;
using VeraWAF.WebPages.Controls;

namespace VeraWAF.WebPages.Admin
{
    public partial class EditRole : PageTemplateBase
    {
        /// <summary>
        /// Azure table data source
        /// </summary>
        readonly AzureTableStorageDataSource _datasource;

        /// <summary>
        /// Application name
        /// </summary>
        readonly string _applicationName;

        /// <summary>
        /// Role partition key
        /// </summary>
        string _partitionKey;

        /// <summary>
        /// Role entity data
        /// </summary>
        RoleEntity _roleEntity;

        /// <summary>
        /// Class constructor
        /// </summary>
        public EditRole()
        {
            _datasource = new AzureTableStorageDataSource();
            _applicationName = ConfigurationManager.AppSettings["ApplicationName"];
        }

        /// <summary>
        /// Get all the members of a role
        /// </summary>
        /// <param name="roleName">Role name</param>
        /// <returns>Returns all the members of the role as a CSV</returns>        
        string GetMembers(string roleName)
        {
            var users = Roles.GetUsersInRole(roleName);
            return String.Join(", ", users);
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
        /// Fills the form with the role entity data
        /// </summary>
        void PopulateFormFieldsFromRoleEntityData()
        {
            var dateFormatRule = "{0:" + ConfigurationManager.AppSettings["DateFormat"] + "}";

            // Get th internal id
            litId.Text = _roleEntity.PartitionKey;

            // Get the date when the role was first created
            litCreated.Text = String.Format(dateFormatRule, _roleEntity.CreationDate);

            // Get the date when the role was last modifed
            litModified.Text = String.Format(dateFormatRule, _roleEntity.Timestamp);

            // Get the name of the role
            txtRolename.Text = Server.HtmlEncode(_roleEntity.RoleName);

            // Get the role description
            txtComment.Text = Server.HtmlEncode(_roleEntity.Description);

            // Get the role members
            txtUsers.Text = GetMembers(_roleEntity.RoleName);
        }

        /// <summary>
        /// Saves the role
        /// </summary>
        void SaveRole()
        {
            // Make sure that the built-in roles are not renamed by accident
            if (_roleEntity.RoleName != txtRolename.Text)
            {
                // Prevent accidental renaming of the system administrator
                if (_roleEntity.RoleName == ConfigurationManager.AppSettings["AdminRoleName"])
                    throw new ApplicationException("You are not allowed to rename the default system administrator role");

                // Prevent accidental renaming of the editors
                if (_roleEntity.RoleName == ConfigurationManager.AppSettings["EditorRoleName"])
                    throw new ApplicationException("You are not allowed to rename the default editor role");
            }

            // Save the role name
            _roleEntity.RoleName = txtRolename.Text;

            // Save the role description
            _roleEntity.Description = txtComment.Text;

            // Update the changes
            _datasource.Update(_roleEntity);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _partitionKey = Request["id"];
            if (String.IsNullOrEmpty(_partitionKey)) return;

            _roleEntity = _datasource.GetRole(_partitionKey, _applicationName);

            if (!Page.IsPostBack)
                PopulateFormFieldsFromRoleEntityData();
        }

        /// <summary>
        /// Called when the user clicks the save button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void butSave_Click(object sender, EventArgs e)
        {
            SaveRole();

            // Make sure that all the cloud nodes are updated with the changes
            ClearCache();
        }

        /// <summary>
        /// Deletes role
        /// </summary>
        void DeleteRole()
        {
            // Prevent accidental deletion of the system administrator
            if (_roleEntity.RoleName == ConfigurationManager.AppSettings["AdminRoleName"])
                throw new ApplicationException("You are not allowed to delete the default system administrator role");

            // Prevent accidental deletion of the editors
            if (_roleEntity.RoleName == ConfigurationManager.AppSettings["EditorRoleName"])
                throw new ApplicationException("You are not allowed to delete the default editor role");

            _datasource.Delete(_roleEntity);
        }

        /// <summary>
        /// Called when the user clicks the delete user button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void butDelete_Click(object sender, EventArgs e)
        {
            try
            {
                DeleteRole();
            }
            catch(Exception ex)
            {
                notifications.AddMessage(ex.Message);
                return;
            }

            // Make sure that all the cloud nodes are updated with the changes
            ClearCache();

            Response.Redirect("/Admin/Roles.aspx");
        }

        /// <summary>
        /// Called when the user clicks the add user button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnAddUser_Click(object sender, EventArgs e)
        {
            try
            {
                if (Membership.GetUser(txtUserName.Text) == null)
                    throw new ApplicationException("User don't exist");

                if (Roles.IsUserInRole(txtUserName.Text))
                    throw new ApplicationException("User is already a member of the role");

                // Add the user to the role
                Roles.AddUserToRole(txtUserName.Text, _roleEntity.RoleName);
            }
            catch (Exception ex)
            {
                notifications.AddMessage(ex.Message);
                return;
            }

            ClearCache();

            notifications.AddMessage(String.Format("Added \"{0}\" to role", txtUserName.Text), FormNotification.NotificationType.Information);
        }

        /// <summary>
        /// Called when the user clicks the remove user button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnRemoveUser_Click(object sender, EventArgs e)
        {
            try
            {
                // Prevent accidental removal of the system administrator from the admin role
                if (_roleEntity.RoleName == ConfigurationManager.AppSettings["AdminRoleName"]
                    && txtUserName.Text == ConfigurationManager.AppSettings["AdminName"])
                    throw new ApplicationException("You are not to remove the default administrator user from the system administrator role");

                // Prevent accidental deletion of the editors
                if (_roleEntity.RoleName == ConfigurationManager.AppSettings["EditorRoleName"]
                    && txtUserName.Text == ConfigurationManager.AppSettings["EditorName"])
                    throw new ApplicationException("You are not to remove the default editor user from the editor role");

                if (Membership.GetUser(txtUserName.Text) == null)
                    throw new ApplicationException("User don't exist");

                if (!Roles.IsUserInRole(txtUserName.Text))
                    throw new ApplicationException("User is not a member of the role");

                Roles.RemoveUserFromRole(txtUserName.Text, _roleEntity.RoleName);
            }
            catch(Exception ex)
            {
                notifications.AddMessage(ex.Message);
                return;
            }

            ClearCache();

            notifications.AddMessage(String.Format("Removed \"{0}\" from role", txtUserName.Text), FormNotification.NotificationType.Information);
        }
    }
}