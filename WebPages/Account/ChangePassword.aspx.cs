using System;
using System.Configuration;
using System.Text;
using System.Web.Security;
using VeraWAF.AzureQueue;
using VeraWAF.Core.Templates;
using VeraWAF.WebPages.Bll;
using VeraWAF.WebPages.Bll.Cloud;
using VeraWAF.WebPages.Dal;
using VeraWAF.CrossCuttingConcerns;

namespace VeraWAF.WebPages.Account
{
    public partial class ChangePassword : PageTemplateBase
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ViewStateUserKey = new ServerTools().GetClientIpAddress() + Membership.GetUser().ProviderUserKey;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        void SendConfirmationEmail()
        {
            var applicationName = ConfigurationManager.AppSettings["ApplicationName"];
            var userName = Membership.GetUser().UserName;
            var newPassword = ChangeUserPassword.NewPassword;
            var emailAddress = Membership.GetUser().Email;
            var fromEmail = ConfigurationManager.AppSettings["fromEmail"];
            var subject = String.Format(Bll.Resources.Email.ChangePasswordSubject, applicationName);

            // Now lets create an email message
            var emailMessage = new StringBuilder();
            var header = Bll.Resources.Email.EmailHeader.Replace("{0}", String.Format(Bll.Resources.Email.ChangePasswordHeader, applicationName));
            emailMessage.Append(header);
            emailMessage.AppendFormat(Bll.Resources.Email.ChangePasswordBody, userName, newPassword);
            var footer = String.Format(Bll.Resources.Email.EmailFooter, String.Format(Bll.Resources.Email.ChangePasswordReason, applicationName));
            emailMessage.Append(footer);

            var messagingClient = new MessagingClient();
            messagingClient.SendEmail(fromEmail, emailAddress, subject, emailMessage.ToString());
        }

        protected void ChangeUserPassword_ChangedPassword(object sender, EventArgs e)
        {
            // Clear the local user cache
            new CloudCommand().Execute("ClearUserCache");

            // Update all the other cloud instances by flushing their user caches
            new CloudCommandClient().SendCommand("ClearUserCache", true);

            SendConfirmationEmail();

            new LogEvent().AddEvent(ELogEventTypes.Info,
                string.Format("User \"{0}\" has changed his password", Membership.GetUser().UserName),
                ConfigurationManager.AppSettings["ApplicationName"]);

        }
    }
}
