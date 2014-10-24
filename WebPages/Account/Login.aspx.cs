using System;
using System.Configuration;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using VeraWAF.Core.Templates;
using VeraWAF.WebPages.Bll;
using VeraWAF.CrossCuttingConcerns;

namespace VeraWAF.WebPages.Account
{
    public partial class Login : PageTemplateBase
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ViewStateUserKey = new ServerTools().GetClientIpAddress();
        }

        Control GetControl(string id)
        {
            return LoginUser.FindControl(id);
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            RegisterHyperLink.NavigateUrl = "Register.aspx?ReturnUrl=" + HttpUtility.UrlEncode(Request.QueryString["ReturnUrl"]);
        }

        protected void ValidateIfLockedOut(object source, ServerValidateEventArgs args)
        {
            try
            {
                var user = Membership.GetUser(LoginUser.UserName, false);
                if (user == null) return;
                args.IsValid = !user.IsLockedOut;
            }
            catch (Exception)
            {
                args.IsValid = true;
            }
        }

        void SendValidatationEmail(MembershipUser user)
        {
            var applicationName = ConfigurationManager.AppSettings["ApplicationName"];
            var userName = user.UserName;
            var emailAddress = user.Email;

            // Lets get the user's id
            var userId = (Guid)Membership.GetUser(userName).ProviderUserKey;

            // Now lets create an email message
            var emailMessage = new StringBuilder();
            var header = Bll.Resources.Email.EmailHeader.Replace("{0}", String.Format(Bll.Resources.Email.NewAccountHeader, applicationName));
            emailMessage.Append(header);
            emailMessage.AppendFormat(Bll.Resources.Email.NewAccountBody1, userName, applicationName);
            var url = String.Format("http://{1}/Account/ActivateAccount.aspx?Id={0}", userId, applicationName);
            emailMessage.AppendFormat(Bll.Resources.Email.NewAccountBody2, url);
            var footer = String.Format(Bll.Resources.Email.EmailFooter, String.Format(Bll.Resources.Email.NewAccountReason, applicationName));
            emailMessage.Append(footer);
            var fromEmail = ConfigurationManager.AppSettings["fromEmail"];

            // Send the email
            var messagingClient = new MessagingClient();
            messagingClient.SendEmail(fromEmail, emailAddress, String.Format(Bll.Resources.Email.NewAccountSubject, applicationName), 
                emailMessage.ToString());
        }

        protected void ValidateIfApproved(object source, ServerValidateEventArgs args)
        {
            try
            {
                var user = Membership.GetUser(LoginUser.UserName, false);
                if (user == null) return;
                if (!user.IsApproved)
                {
                    SendValidatationEmail(user);

                    new LogEvent().AddEvent(ELogEventTypes.Info, 
                        string.Format("Sent validation e-mail to user \"{0}\"", LoginUser.UserName),
                        ConfigurationManager.AppSettings["ApplicationName"]);
                }
                args.IsValid = user.IsApproved;
            }
            catch (Exception)
            {
                args.IsValid = true;
            }
        }

        protected void LoginUser_OnLoggingIn(object sender, EventArgs e)
        {
            var maxNumberOfFailedPasswordsPerHour = long.Parse(ConfigurationManager.AppSettings["MaxNumberOfFailedPasswordsPerHour"]);
            if (new HammerProtection(maxNumberOfFailedPasswordsPerHour, HammeringMode.Hours,
                HammerTypes.PasswordHammering).HostIsHammering(new ServerTools().GetClientIpAddress())) {

                new LogEvent().AddEvent(ELogEventTypes.Info, 
                    string.Format("The user \"{0}\" was caught password hammering", LoginUser.UserName),
                    ConfigurationManager.AppSettings["ApplicationName"]);

                // The user is hammering, so don't waste processing power on him
                HttpContext.Current.Response.Redirect("/ErrorPages/password-hammering.aspx", true);
            }
        }

        void ResetPasswordHammering()
        {
            var maxNumberOfFailedPasswordsPerHour = long.Parse(ConfigurationManager.AppSettings["MaxNumberOfFailedPasswordsPerHour"]);
            new HammerProtection(maxNumberOfFailedPasswordsPerHour,
                HammeringMode.Hours,
                HammerTypes.PasswordHammering).Reset(new ServerTools().GetClientIpAddress());
        }

        protected void LoginUser_OnLoggedIn(object sender, EventArgs e)
        {
            new LogEvent().AddEvent(ELogEventTypes.Info, string.Format("User \"{0}\" has signed in", LoginUser.UserName),
                ConfigurationManager.AppSettings["ApplicationName"]);

            ResetPasswordHammering();
        }

        protected void LoginUser_OnLoginError(object sender, EventArgs e)
        {
            LoginUser.FindControl("SignInFailed").Visible = true;
        }
    }
}
