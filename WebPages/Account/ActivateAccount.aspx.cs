using System;
using System.Configuration;
using System.Text;
using System.Web.Security;
using VeraWAF.Core.Templates;
using VeraWAF.WebPages.Bll;
using VeraWAF.WebPages.Bll.Cloud;
using VeraWAF.WebPages.Dal;
using VeraWAF.CrossCuttingConcerns;

namespace VeraWAF.WebPages.Account
{
    public partial class ActivateAccount : PageTemplateBase
    {

        static void SendConfirmationEmail(string userName, string emailAddress)
        {
            // Now lets create an email message
            var emailMessage = new StringBuilder();
            var applicationName = ConfigurationManager.AppSettings["ApplicationName"];
            var header = Bll.Resources.Email.EmailHeader.Replace("{0}",
                String.Format(Bll.Resources.Email.ActivatedHeader, applicationName));
            emailMessage.Append(header);
            emailMessage.AppendFormat(Bll.Resources.Email.ActivatedBody, userName, applicationName);
            var footer = String.Format(Bll.Resources.Email.EmailFooter, String.Format(Bll.Resources.Email.ActivatedReason,
                applicationName));
            emailMessage.Append(footer);
            var fromEmail = ConfigurationManager.AppSettings["fromEmail"];

            // Send the email
            var messagingClient = new MessagingClient();
            messagingClient.SendEmail(fromEmail, emailAddress, String.Format(Bll.Resources.Email.ActivatedSubject,
                applicationName), emailMessage.ToString());
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
                    if (user.IsApproved) alreadyActivated.Visible = true;
                    else
                    {
                        success.Visible = user.IsApproved = true;

                        // Update the user activation
                        Membership.UpdateUser(user);

                        new Dal.UserCache().Clear2();

                        // Update all the other cloud instances by flushing their user caches
                        new Bll.Cloud.CloudCommandClient().SendCommand("ClearUserCache", true);

                        SendConfirmationEmail(user.UserName, user.Email);

                        new LogEvent().AddEvent(ELogEventTypes.Info, 
                            string.Format("User \"{0}\" has activated his account", user.UserName),
                            ConfigurationManager.AppSettings["ApplicationName"]);
                    }
                }
                else notFound.Visible = true;
            }
            else badConfirmationId.Visible = false;
        }

    }
}