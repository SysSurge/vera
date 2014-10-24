using System;
using System.Configuration;
using System.Security;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using VeraWAF.Core.Templates;

namespace VeraWAF.WebPages.Account
{
    /// <summary>
    /// Allows a user to delete his or hers account
    /// </summary>
    public partial class DeleteAccount : PageTemplateBase
    {
        /// <summary>
        /// Clear the user authentication cookie
        /// </summary>
        void ClearAuthenticationCookie()
        {
            var cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, String.Empty) { Expires = DateTime.Now.AddYears(-1) };
            Response.Cookies.Add(cookie1);
        }

        /// <summary>
        /// Sign out user and clear authentication cookies
        /// </summary>
        void SignOutUser()
        {
            // The account has been deleted, so sign out
            ClearAuthenticationCookie();
            FormsAuthentication.SignOut();
        }

        /// <summary>
        /// Called when the user clicks the cancel button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CancelPushButton_Click(object sender, EventArgs e)
        {
            // Redirect to frontpage
            Response.Redirect("/");
        }

        /// <summary>
        /// Called when the user clicks the delete button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void butDeleteAccount_Click(object sender, EventArgs e)
        {
            if (!ValidatePassword.IsValid) return;

            // Get the current user
            var currentUser = Membership.GetUser();
            if (currentUser == null) throw new SecurityException("Not signed in");

            // Prevent the accidental deletion of the default admin
            if (currentUser.UserName == ConfigurationManager.AppSettings["AdminName"])
                throw new SecurityException("You are not allowed to delete the default system administrator");

            // Delete the user and all related data
            Membership.DeleteUser(currentUser.UserName, true);

            // Sign out the user and clear authentication cookies
            SignOutUser();

            // Redirect to frontpage
            Response.Redirect("/");
        }

        /// <summary>
        /// Custom form validator that asserts the account password
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void ValidatePassword_ServerValidate(object source, ServerValidateEventArgs args)
        {
            // Get the current user
            var currentUser = Membership.GetUser();
            if (currentUser == null) throw new SecurityException("Not signed in");

            // Assert the user password
            args.IsValid = Membership.ValidateUser(currentUser.UserName, txtCurrentPassword.Text);
        }

    }
}