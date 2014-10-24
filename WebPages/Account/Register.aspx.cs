using System;
using System.Configuration;
using System.Text;
using System.Web.Profile;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using VeraWAF.Core.Templates;
using VeraWAF.CrossCuttingConcerns;
using VeraWAF.WebPages;
using VeraWAF.WebPages.Bll;
using VeraWAF.WebPages.Bll.Cloud;
using VeraWAF.WebPages.Controls;

namespace VeraWAF.WebPages.Account
{
    public partial class Register : PageTemplateBase
    {
        const string DefaultPortraitUrl = "";
        string _janrainApiKey;
        string _janrainBaseUrl;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ViewStateUserKey = new ServerTools().GetClientIpAddress();
        }

        XmlElement AuthenticateUserWithJanrein(string token)
        {
            XmlElement authenticationResult;
            
            try
            {
                var janrainClient = new JanRain(_janrainApiKey, _janrainBaseUrl);
                authenticationResult = janrainClient.AuthInfo(token);
            }
            catch (Exception)
            {
                authenticationResult = null;
            }

            return authenticationResult;            
        }

        void InitJanrein()
        {
            _janrainApiKey = ConfigurationManager.AppSettings["JanrainApiKey"];
            _janrainBaseUrl = ConfigurationManager.AppSettings["JanrainBaseUrl"];
        }

        string GetJanreinValue(XmlElement authenticationResult, string xpath)
        {
            string value;

            try
            {
                var selectSingleNode = authenticationResult.SelectSingleNode(xpath);
                value = selectSingleNode != null ? selectSingleNode.InnerText : null;
            }
            catch (Exception)
            {
                value = null;
            }

            return value;
        }

        Control GetControl(string id)
        {
            return RegisterUser.CreateUserStep.ContentTemplateContainer.FindControl(id) ??
                          RegisterUser.CompleteStep.ContentTemplateContainer.FindControl(id);
        }

        void DisableWebControl(string id)
        {
            var control = GetControl(id);
            if (control != null)
                ((WebControl)control).Enabled = false;
        }

        void RemoveIrrelevantValidators()
        {
            DisableWebControl("BadUserName1");
            DisableWebControl("BadUserName2");
        }

        void FillUserNameControl(string userName)
        {
            var userNameControl = (TextBox)GetControl("UserName");
            userNameControl.Text = userName;
            userNameControl.Enabled = false;

            ((HtmlGenericControl)GetControl("pUserName")).Style.Add("display", "none");
        }

        void FillEmailControls(XmlElement authenticationResult)
        {
            var verifiedEmailAddress = GetJanreinValue(authenticationResult, "profile/verifiedEmail");
            
            var emailAddress = String.IsNullOrWhiteSpace(verifiedEmailAddress) ? 
                GetJanreinValue(authenticationResult, "profile/email") : verifiedEmailAddress;

            if (!String.IsNullOrWhiteSpace(emailAddress))
            {
                var emailControl = (TextBox)GetControl("Email");
                var confirmEmailControl = (TextBox)GetControl("ConfirmEmail");

                emailControl.Text = confirmEmailControl.Text = emailAddress;
                emailControl.Enabled = confirmEmailControl.Enabled = false;

                ((HtmlGenericControl)GetControl("fsAccountInfo")).Style.Add("display", "none");
            }
        }

        void FillSignInProviderNameControl(XmlElement authenticationResult)
        {
            var providerName = GetJanreinValue(authenticationResult, "profile/providerName");
            ((TextBox) GetControl("txtSignInProviderName")).Text = String.IsNullOrWhiteSpace(providerName) ? "Unknown" : providerName;
        }

        void FillEmailIsValidatedControl(XmlElement authenticationResult)
        {
            var verifiedEmailAddress = GetJanreinValue(authenticationResult, "profile/verifiedEmail");
            ((CheckBox)GetControl("chkEmailIsValidated")).Checked = !String.IsNullOrWhiteSpace(verifiedEmailAddress);
        }

        void FillGenderControl(XmlElement authenticationResult) {
            var gender = GetJanreinValue(authenticationResult, "profile/gender");
            ((TextBox)GetControl("txtGender")).Text = String.IsNullOrWhiteSpace(gender) ? String.Empty : gender;
        }

        void FillUserPortraitControl(XmlElement authenticationResult) {
            var gender = GetJanreinValue(authenticationResult, "profile/photo");
            ((TextBox)GetControl("txtPhoto")).Text = String.IsNullOrWhiteSpace(gender) ? "/Images/none.png" : gender;
        }

        String GetUserFullName(XmlElement authenticationResult)
        {
            var fullName = GetJanreinValue(authenticationResult, "profile/displayName");

            if (String.IsNullOrWhiteSpace(fullName))
                fullName = GetJanreinValue(authenticationResult, "profile/name/formatted");

            if (String.IsNullOrWhiteSpace(fullName))
            {
                var givenName = GetJanreinValue(authenticationResult, "profile/name/givenName");
                var familyName = GetJanreinValue(authenticationResult, "profile/name/familyName");

                fullName = !String.IsNullOrWhiteSpace(givenName) ? givenName : String.Empty;

                if (!String.IsNullOrWhiteSpace(familyName))
                    fullName += " " + familyName;
            }

            if (String.IsNullOrWhiteSpace(fullName))
                fullName = GetJanreinValue(authenticationResult, "profile/preferredUsername");

            return fullName;
        }

        string GenerateUserFullName()
        {
            return "User " + DateTime.UtcNow.Ticks;
        }

        void FillFullNameControl(XmlElement authenticationResult)
        {
            var fullName = GetUserFullName(authenticationResult);

            if (String.IsNullOrWhiteSpace(fullName))
                fullName = GenerateUserFullName();

            ((TextBox)GetControl("txtFullName")).Text = fullName;
        }

        void FillFormWithUserInfoFromJanrein(XmlElement authenticationResult, string identifier)
        {
            FillUserNameControl(identifier);
            FillEmailControls(authenticationResult);

            FillSignInProviderNameControl(authenticationResult);
            FillEmailIsValidatedControl(authenticationResult);
            FillGenderControl(authenticationResult);
            FillFullNameControl(authenticationResult);

            FillUserPortraitControl(authenticationResult);
        }

        string GetReturnUrl()
        {
            return Request["ReturnUrl"] == null || Request["ReturnUrl"].Contains(":")  // Avoid hack redirect to some other site
                ? "/Account/" : Request.QueryString["ReturnUrl"];
        }

        void HideIrrelevantControls()
        {
            GetControl("panPassword").Visible = false;
            GetControl("panThirdPartyRegister").Visible = false;
            GetControl("fsPasswordRecovery").Visible = false;
            GetControl("pPasswordLengthText").Visible = false;         
        }

        void UseThirdPartyToSignInOrRegisterUser()
        {
            var token = Request["token"];
            if (!String.IsNullOrWhiteSpace(token)
                && !token.Contains("&") // Simple querystring injection hack prevention
                ) {

                InitJanrein();
                
                var authenticationResult = AuthenticateUserWithJanrein(token);
                if (authenticationResult != null) {
                    var identifier = GetJanreinValue(authenticationResult, "profile/identifier");
                    if (!String.IsNullOrWhiteSpace(identifier)) {
                        var user = Membership.GetUser(identifier);
                        if (user == null)
                        {
                            RegisterUser.AutoGeneratePassword = true;

                            // Remove user name validators since the identificator may contain unusual characters
                            RemoveIrrelevantValidators();

                            FillFormWithUserInfoFromJanrein(authenticationResult, identifier);

                            HideIrrelevantControls();
                        }
                        else
                        {
                            // User is already registered, so sign him/her in instead
                            FormsAuthentication.SetAuthCookie(identifier, true);
                            Response.Redirect(GetReturnUrl(), true);                            
                        }
                    }
                }
            }            
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack) return;

            UseThirdPartyToSignInOrRegisterUser();
        }

        void SendActivationEmail()
        {
            var applicationName = ConfigurationManager.AppSettings["ApplicationName"];
            var userName = ((TextBox)GetControl("UserName")).Text;
            var user = Membership.GetUser(userName);
            if (user == null) return;

            var password = ((TextBox)GetControl("Password")).Text;
            var emailAddress = user.Email;

            // Lets get the user's id
            var userId = (Guid)user.ProviderUserKey;
            
            // Now lets create an email message
            var emailMessage = new StringBuilder();

            var header = Bll.Resources.Email.EmailHeader.Replace("{0}", String.Format(Bll.Resources.Email.RegisterHeader, applicationName));
            emailMessage.Append(header);
            emailMessage.AppendFormat(Bll.Resources.Email.RegisterBody1, applicationName);
            
            var url = String.Format("http://{1}/Account/ActivateAccount.aspx?Id={0}", userId, applicationName);
            emailMessage.AppendFormat(Bll.Resources.Email.RegisterBody2, url);

            var isUser3RdPartyAuthentication = !String.IsNullOrWhiteSpace(((TextBox) GetControl("txtSignInProviderName")).Text);
            if (!isUser3RdPartyAuthentication)
                emailMessage.AppendFormat(Bll.Resources.Email.RegisterHeader3rdPartyAuthentication, userName, password);

            var footer = String.Format(Bll.Resources.Email.EmailFooter, String.Format(Bll.Resources.Email.RegisterReason, applicationName));
            emailMessage.Append(footer);
            var fromEmail = ConfigurationManager.AppSettings["fromEmail"];

            // Send the email
            var messagingClient = new MessagingClient();
            messagingClient.SendEmail(fromEmail, emailAddress, String.Format(Bll.Resources.Email.RegisterSubject, applicationName), 
                emailMessage.ToString());
        }

        string GetWizardTextBoxValue(string textBoxId)
        {
            return ((TextBox)RegisterUser.CreateUserStep.ContentTemplateContainer.FindControl(textBoxId)).Text;
        }

        int GetGenderEnum(string gender)
        {
            int genderEnum;

            if (gender.Equals(Bll.Resources.Controls.MaleGender, StringComparison.InvariantCultureIgnoreCase))
                genderEnum = 1;
            else if (gender.Equals(Bll.Resources.Controls.FemaleGender, StringComparison.InvariantCultureIgnoreCase))
                genderEnum = 0;
            else genderEnum = -1;

            return genderEnum;
        }

        string GetPortraitUrl()
        {         
            var photoUrl = ((TextBox)GetControl("txtPhoto")).Text;

            return String.IsNullOrWhiteSpace(photoUrl) ? DefaultPortraitUrl : photoUrl;
        }

        void StoreCustomProfileFields()
        {
            var profile = ProfileBase.Create(GetWizardTextBoxValue("UserName"));

            profile.SetPropertyValue("ClientIpAddress", new ServerTools().GetClientIpAddress());

            profile.SetPropertyValue("Gender", GetGenderEnum(((TextBox)GetControl("txtGender")).Text));
            profile.SetPropertyValue("FullName", ((TextBox)GetControl("txtFullName")).Text);
            profile.SetPropertyValue("AuthProvider", ((TextBox)GetControl("txtSignInProviderName")).Text);

            profile.SetPropertyValue("PortraitBlobAddressUri", GetPortraitUrl());

            profile.SetPropertyValue("Newsletter", ((CheckBox)GetControl("Newsletter")).Checked);

            profile.Save();
        }

        /// <summary>
        /// Send an email to the webmaster to inform him/her that a new user has registered.
        /// Adds some of the available information about the user in the email.
        /// </summary>
        void SendNewUserNotficationEmail()
        {
            var applicationName = ConfigurationManager.AppSettings["ApplicationName"];
            var fullName = ((TextBox)GetControl("txtFullName")).Text;
            var userName = ((TextBox)GetControl("UserName")).Text;
            var displayName = String.IsNullOrWhiteSpace(fullName) ? userName : fullName;

            var emailAddress = ConfigurationManager.AppSettings["notificationsEmail"];

            // Now lets create an email message
            var emailMessage = new StringBuilder();
            var header = Bll.Resources.Email.EmailHeader.Replace("{0}", Bll.Resources.Email.NewUserNotifyHeader);
            emailMessage.Append(header);
            emailMessage.AppendFormat(Bll.Resources.Email.NewUserNotifyBody1, applicationName);
            emailMessage.AppendFormat(Bll.Resources.Email.NewUserNotifyBody2, userName);

            if (!String.IsNullOrWhiteSpace(fullName))
                emailMessage.AppendFormat(Bll.Resources.Email.NewUserNotifyBody3, fullName);

            emailMessage.AppendFormat(Bll.Resources.Email.NewUserNotifyBody4, ((TextBox)GetControl("Email")).Text);
            emailMessage.AppendFormat(Bll.Resources.Email.NewUserNotifyBody5, DateTime.UtcNow);
            emailMessage.AppendFormat(Bll.Resources.Email.NewUserNotifyBody6, new ServerTools().GetClientIpAddress());
            emailMessage.AppendFormat(Bll.Resources.Email.NewUserNotifyBody8, ((CheckBox)GetControl("Newsletter")).Checked);

            var footer = String.Format(Bll.Resources.Email.EmailFooter, String.Format(Bll.Resources.Email.NewUserNotifyReason, 
                applicationName));
            emailMessage.Append(footer);
            var fromEmail = ConfigurationManager.AppSettings["fromEmail"];

            // Send the email
            var messagingClient = new MessagingClient();
            var emailSubject = String.Format(Bll.Resources.Email.NewUserNotifySubject, displayName, applicationName);
            messagingClient.SendEmail(fromEmail, emailAddress, emailSubject, emailMessage.ToString());
        }

        protected void RegisterUser_CreatedUser(object sender, EventArgs e)
        {
            // Prevent spamming by using a honey pot field that is not to be filled by a human user
            if (((TextBox)GetControl("ContactEmail")).Text != "")
            {
                /*
                 * Honey pot field had some data, which means this is a form filling bot.
                 * Don't let the bot know we detected it by pretending a success
                 */
                GetControl("pSuccess").Visible = false;
                return;
            }

            StoreCustomProfileFields();

            var userName = ((TextBox)GetControl("UserName")).Text;
            var user = Membership.GetUser(userName);
            if (user == null) return;

            var userEmailIsValidated = ((CheckBox) GetControl("chkEmailIsValidated")).Checked;
            if (userEmailIsValidated)
            {
                GetControl("panEmailSentMessage").Visible = false;                

                user.IsApproved = true;
                Membership.UpdateUser(user);

                // Sign in user
                FormsAuthentication.SetAuthCookie(userName, true);
            }
            else
            {
                GetControl("pSuccess").Visible = false;
                ((Literal)GetControl("litUserEmail")).Text = user.Email;

                SendActivationEmail();
            }

            new LogEvent().AddEvent(ELogEventTypes.Info, 
                string.Format("New user \"{0}\" has registered", user.UserName),
                ConfigurationManager.AppSettings["ApplicationName"]);

            new Dal.UserCache().Clear2();

            // Update all the other cloud instances by flushing their user caches
            new CloudCommandClient().SendCommand("ClearUserCache", true);

            SendNewUserNotficationEmail();
        }

        protected void Continue_OnClick(object sender, EventArgs e)
        {
            var userName = ((TextBox) GetControl("UserName")).Text;
            var user = Membership.GetUser(userName);
            if (user != null)
            {
                var profile = ProfileBase.Create(userName);
                var marketingResearch = ((MarketingResearch)GetControl("MarketingResearch1"));

                marketingResearch.StoreFields(profile, true);
                marketingResearch.SendNotficationEmail(userName);

                new LogEvent().AddEvent(ELogEventTypes.Info, 
                    string.Format("User \"{0}\" has submitted the marketing research form", user.UserName),
                    ConfigurationManager.AppSettings["ApplicationName"]);
            }

            Response.Redirect(GetReturnUrl(), true);
        }
    }
}
