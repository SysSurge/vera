using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
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
    public partial class EditUser : PageTemplateBase
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
        /// User partition key
        /// </summary>
        string _partitionKey;

        /// <summary>
        /// User entity data
        /// </summary>
        UserEntity _userEntity;

        /// <summary>
        /// Class constructor
        /// </summary>
        public EditUser()
        {
            _datasource = new AzureTableStorageDataSource();
            _applicationName = ConfigurationManager.AppSettings["ApplicationName"];
        }

        /// <summary>
        /// Clear the user cache
        /// </summary>
        void ClearCache()
        {
            new CloudCommand().Execute("ClearUserCache");
            new CloudCommandClient().SendCommand("ClearUserCache", true);
        }

        /// <summary>
        /// Get all the roles the user is a member of
        /// </summary>
        /// <returns>Returns all the roles the user is a member of as a CSV</returns>
        string GetRoles()
        {
            var roles = _datasource.GetRolesForUser(_partitionKey, _applicationName);

            var sb = new StringBuilder();

            bool isFirst = true;
            foreach(RoleEntity role in roles)
            {
                if (!isFirst) sb.Append(", ");
                else isFirst = false;

                sb.Append(role.RoleName);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Fill form with user entity data
        /// </summary>
        void PopulateFormFieldsFromUserEntityData()
        {
            var dateFormatRule = "{0:" + ConfigurationManager.AppSettings["DateFormat"] + "}";

            // Make sure we have a display name
            var displayName = _userEntity.Username.Trim().Length == 0 ? "[?]" : Server.HtmlEncode(_userEntity.Username);
            litProfile.Text = String.Format("<a href=\"/Account/?id={0}\">{1}</a>", _userEntity.PartitionKey,
                displayName);

            litId.Text = _userEntity.PartitionKey;
            litCreated.Text = String.Format(dateFormatRule, _userEntity.CreationDate);
            litBirthday.Text = String.Format(dateFormatRule, _userEntity.Birthday);
            litLastActive.Text = String.Format(dateFormatRule, _userEntity.LastActivityDate);

            txtUsername.Text = Server.HtmlEncode(_userEntity.Username);
            txtFullName.Text = Server.HtmlEncode(_userEntity.FullName);
            txtEmail.Text = Server.HtmlEncode(_userEntity.Email);
            chkContact.Checked = _userEntity.AllowContactForm;

            chkLocked.Checked = _userEntity.IsLockedOut;
            chkApproved.Checked = _userEntity.IsApproved;
            chkNewsletter.Checked = _userEntity.Newsletter;
            chkBan.Checked = _userEntity.IsDeleted;

            txtComment.Text = Server.HtmlEncode(_userEntity.Comment);
            txtRoles.Text = GetRoles();

            // OAuth
            txtOAuthConsumerKey.Text = _userEntity.OAuthConsumerKey;
            txtOAuthConsumerSecret.Text = _userEntity.OAuthConsumerSecret;
        }

        /// <summary>
        /// Save the user entity data
        /// </summary>
        void SaveUser()
        {
            _userEntity.Username = txtUsername.Text;
            _userEntity.FullName = txtFullName.Text;
            _userEntity.Email = txtEmail.Text;
            _userEntity.AllowContactForm = chkContact.Checked;

            _userEntity.IsLockedOut = chkLocked.Checked;
            _userEntity.IsApproved = chkApproved.Checked;
            _userEntity.Newsletter = chkNewsletter.Checked;
            _userEntity.IsDeleted = chkBan.Checked;

            _userEntity.Comment = txtComment.Text;

            // OAuth
            _userEntity.OAuthConsumerKey = txtOAuthConsumerKey.Text;
            _userEntity.OAuthConsumerSecret = txtOAuthConsumerSecret.Text;

            // Prevent accidental banning of the system administrator
            if (_userEntity.Username == ConfigurationManager.AppSettings["AdminName"]
                && _userEntity.IsDeleted)
                throw new ApplicationException("You are not allowed to ban the system administrator");
   
            _datasource.Update(_userEntity);
        }

        /// <summary>
        /// Called when the user clicks the save button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void butSave_Click(object sender, EventArgs e)
        {
            try
            {
                SaveUser();

                // Make sure that all the cloud nodes are updated with the changes
                ClearCache();
            }
            catch(Exception ex)
            {
                notifications.AddMessage(ex.Message);
                return;
            }

            notifications.AddMessage("Saved user data successfully", FormNotification.NotificationType.Information);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _partitionKey = Request["id"];
            if (String.IsNullOrEmpty(_partitionKey)) return;

            _userEntity = _datasource.GetUser(_partitionKey, _applicationName);

            if (!Page.IsPostBack)
                PopulateFormFieldsFromUserEntityData();
        }

        /// <summary>
        /// Deletes the user
        /// </summary>
        void DeleteUser()
        {
            // Prevent accidental deletion of the system administrator
            if (_userEntity.Username == ConfigurationManager.AppSettings["AdminName"])
                throw new ApplicationException("You are not allowed to delete the system administrator");

            _datasource.Delete(_userEntity);
        }

        /// <summary>
        /// Caleld when the user clicks the delete button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void butDelete_Click(object sender, EventArgs e)
        {
            try
            {
                DeleteUser();
            }
            catch(Exception ex)
            {
                notifications.AddMessage(ex.Message);
                return;
            }

            // Make sure that all the cloud nodes are updated with the changes
            ClearCache();

            Response.Redirect("/Admin/Users.aspx");
        }

        /// <summary>
        /// Called when the user clicks the regenerate OAuth key button.
        /// Creates a new OAuth consumer key.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void butRegenerateKey_Click(object sender, EventArgs e)
        {
            txtOAuthConsumerKey.Text = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Called when the user clicks the regenerate OAuth secret button.
        /// Creates a new OAuth consumer secret.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void butRegenerateSecret_Click(object sender, EventArgs e)
        {
            txtOAuthConsumerSecret.Text = Guid.NewGuid().ToString();
        }

    }
}