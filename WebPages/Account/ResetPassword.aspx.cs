using System;
using System.Configuration;
using System.Text;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using VeraWAF.AzureQueue;
using VeraWAF.Core.Templates;
using VeraWAF.WebPages;
using VeraWAF.WebPages.Bll;
using VeraWAF.WebPages.Bll.Cloud;
using VeraWAF.CrossCuttingConcerns;

namespace VeraWAF.WebPages.Account
{
    public partial class ResetPassword : PageTemplateBase
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ViewStateUserKey = new ServerTools().GetClientIpAddress();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// TODO: Lots of calls to GetUser(), look at a way to only do this once by using viewstate or something
        /// </summary>
        /// <returns></returns>
        MembershipUser GetUser()
        {
            var userNameOrEmail = UserNameOrEmail.Text;
            var user = Membership.GetUser(userNameOrEmail, false);
            if (user == null)
            {
                userNameOrEmail = Membership.GetUserNameByEmail(userNameOrEmail);
                user = Membership.GetUser(userNameOrEmail, false);
            }
            return user;
        }

        protected void ValidateUserNameOrEmail(object source, ServerValidateEventArgs args)
        {
            try
            {
                args.IsValid = (GetUser() != null);
            }
            catch (Exception)
            {
                args.IsValid = false;
            }
        }

        void SendConfirmationEmail(string userName, string newPassword, string emailAddress)
        {
            // Now lets create an email message
            var emailMessage = new StringBuilder();
            var applicationName = ConfigurationManager.AppSettings["ApplicationName"];
            var header = VeraWAF.WebPages.Bll.Resources.Email.EmailHeader.Replace("{0}", 
                String.Format(VeraWAF.WebPages.Bll.Resources.Email.ResetPasswordHeader, applicationName));
            emailMessage.Append(header);
            emailMessage.AppendFormat(VeraWAF.WebPages.Bll.Resources.Email.ResetPasswordBody, userName, newPassword);
            var footer = String.Format(VeraWAF.WebPages.Bll.Resources.Email.EmailFooter, 
                String.Format(VeraWAF.WebPages.Bll.Resources.Email.ResetPasswordReason, applicationName));
            emailMessage.Append(footer);
            var fromEmail = ConfigurationManager.AppSettings["fromEmail"];

            // Send the email
            var messagingClient = new MessagingClient();
            messagingClient.SendEmail(fromEmail, emailAddress, 
                String.Format(VeraWAF.WebPages.Bll.Resources.Email.ResetPasswordSubject, applicationName), 
                emailMessage.ToString());
        }

        void ResetPasswordHammering()
        {
            var maxNumberOfFailedPasswordsPerHour = 
                long.Parse(ConfigurationManager.AppSettings["MaxNumberOfFailedPasswordsPerHour"]);
            
            new HammerProtection(maxNumberOfFailedPasswordsPerHour,
                HammeringMode.Hours,
                HammerTypes.PasswordHammering).Reset(new ServerTools().GetClientIpAddress());             
        }

        protected void ValidateAnswer(object source, ServerValidateEventArgs args)
        {
            try
            {
                var user = GetUser();
                var newPassword = user.ResetPassword(PasswordAnswer.Text);

                var isValid = !String.IsNullOrEmpty(newPassword);
                if (isValid)
                {
                    new VeraWAF.WebPages.Dal.UserCache().Clear2();

                    // Update all the other cloud instances by flushing their user caches
                    new CloudCommandClient().SendCommand("ClearUserCache", true);

                    SendConfirmationEmail(user.UserName, newPassword, user.Email);

                    ResetPasswordHammering();

                    new LogEvent().AddEvent(ELogEventTypes.Info, 
                        string.Format("User \"{0}\" has reset his password", user.UserName),
                        ConfigurationManager.AppSettings["ApplicationName"]);
                }

                args.IsValid = isValid;
            }
            catch (Exception)
            {
                args.IsValid = false;
            }
        }

        bool AssertValidationGroup(string name)
        {
            var validators = Page.GetValidators(name);
            foreach (IValidator validator in validators)
            {
                validator.Validate();
                if (!validator.IsValid) return false;
            }
            return true;
        }

        protected void OnNextButtonClick(object sender, WizardNavigationEventArgs e)
        {
            switch (ResetPasswordWizard.ActiveStep.Title)
            {
                case "Identify User":
                    e.Cancel = !AssertValidationGroup("ResetPasswordValidationGroup1");
                    if (!e.Cancel) PasswordQuestion.Text = GetUser().PasswordQuestion;
                    break;
                case "Answer Question":
                    e.Cancel = !AssertValidationGroup("ResetPasswordValidationGroup2");
                    break;
            }
        }

    }
}