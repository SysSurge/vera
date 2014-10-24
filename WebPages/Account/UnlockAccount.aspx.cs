using System;
using System.Configuration;
using System.Text;
using System.Web.Security;
using VeraWAF.AzureQueue;
using VeraWAF.Core.Templates;
using VeraWAF.WebPages.Bll;
using VeraWAF.WebPages.Bll.Cloud;
using VeraWAF.CrossCuttingConcerns;
using VeraWAF.WebPages.Dal;

namespace VeraWAF.WebPages.Account
{
    public partial class UnlockAccount : PageTemplateBase
    {
        void SendConfirmationEmail(string userName, string emailAddress)
        {
            // Now lets create an email message
            var emailMessage = new StringBuilder();
            var applicationName = ConfigurationManager.AppSettings["ApplicationName"];
            var header = VeraWAF.WebPages.Bll.Resources.Email.EmailHeader.Replace("{0}", 
                String.Format(VeraWAF.WebPages.Bll.Resources.Email.UnlockAccountHeader, applicationName));
            emailMessage.Append(header);
            emailMessage.AppendFormat(VeraWAF.WebPages.Bll.Resources.Email.UnlockAccountBody, userName, applicationName);
            var footer = String.Format(VeraWAF.WebPages.Bll.Resources.Email.EmailFooter, 
                String.Format(VeraWAF.WebPages.Bll.Resources.Email.UnlockAccountReason, applicationName));
            emailMessage.Append(footer);
            var fromEmail = ConfigurationManager.AppSettings["fromEmail"];

            // Send the email
            var messagingClient = new MessagingClient();
            messagingClient.SendEmail(fromEmail, emailAddress, 
                String.Format(VeraWAF.WebPages.Bll.Resources.Email.UnlockAccountSubject, applicationName), 
                emailMessage.ToString());
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Lets Activate the User
            var userId = new Guid();    // Initialized to avoid compiler error

            if (!String.IsNullOrEmpty(Request.Params["Id"]) && Guid.TryParse(Request.Params["Id"], out userId))
            {
                var user = Membership.GetUser(userId);

                // Activate the user
                if (user != null)
                {
                    if (!user.IsLockedOut) alreadyUnlocked.Visible = true;
                    else
                    {
                        user.UnlockUser();

                        success.Visible = true;

                        // Update the user
                        Membership.UpdateUser(user);

                        new UserCache().Clear2();

                        // Update all the other cloud instances by flushing their user caches
                        new CloudCommandClient().SendCommand("ClearUserCache", true);

                        SendConfirmationEmail(user.UserName, user.Email);

                        new LogEvent().AddEvent(ELogEventTypes.Info, 
                            string.Format("User \"{0}\" has unlocked his account", user.UserName),
                            ConfigurationManager.AppSettings["ApplicationName"]);
                    }
                }
                else notFound.Visible = true;
            }
            else badConfirmationId.Visible = true;

        }
    }
}