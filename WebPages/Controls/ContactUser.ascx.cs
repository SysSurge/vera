using System;
using System.Configuration;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using VeraWAF.Core.Templates;
using VeraWAF.WebPages;
using VeraWAF.WebPages.Bll;

namespace VeraWAF.WebPages.Controls {

    /// <summary>
    /// Allows a user to send an e-mail to another user without disclosing any e-mail addresses.
    /// </summary>
    public partial class ContactUser : UserControl {
        /// <summary>
        /// Vera base page
        /// </summary>
        PageTemplateBase _page;

        /// <summary>
        /// Class contructor
        /// </summary>
        public ContactUser()
        {
            Legend = VeraWAF.WebPages.Bll.Resources.Template.ContactForm;
        }

        /// <summary>
        /// E-mail address to send to.
        /// </summary>
        public string ToUserEmail { get; set; }

        /// <summary>
        /// Legend text.
        /// </summary>
        public string Legend { get; set; }

        /// <summary>
        /// Show the contact portrait
        /// </summary>
        /// <param name="currentUser">Sender</param>
        void InitPortrait(MembershipUser currentUser) {
            if (currentUser != null)
            {
                var userUtilities = new UserUtilities();
                var displayName = userUtilities.GetDisplayName(currentUser);
                var portrait = HttpUtility.HtmlEncode(userUtilities.GetPortraitUrl());
                imgPortrait.src = portrait;
                litPortraitCaption.Text = HttpUtility.HtmlEncode(displayName);
            } else currentUserPortrait.Visible = false;

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Get the parent page
            _page = Page as PageTemplateBase;
            if (_page != null)
            {
                // Get page entity data
                var pageEntity = _page.GetPageEntity();
                if (pageEntity != null)
                {
                    // Does the age entity data say anything about showing the control?
                    Visible = pageEntity.ShowContactControl;
                }
            }

            // Get the sender- ie the current user or anonymous
            var currentUser = Membership.GetUser();
            var isLoggedIn = currentUser != null;

            fromField.Visible = !isLoggedIn;

            if (isLoggedIn)
                txtMessage.Attributes["style"] = "width: 20em";
            else txtMessage.Attributes["style"] = "width: 27em";


            legendText.Text = Legend;

            InitPortrait(currentUser);
        }

        /// <summary>
        /// Send the actual e-mail.
        /// Does not disclose any e-mail addresses.
        /// </summary>
        /// <param name="toEmail">E-mail address to send to</param>
        /// <param name="fromUserDisplayName">Name of sender</param>
        /// <param name="fromUserId">Internal user ID of sender</param>
        void SendEmail(string toEmail, string fromUserDisplayName, object fromUserId) {
            fromUserDisplayName = HttpUtility.HtmlEncode(fromUserDisplayName);

            // Now lets create an email message
            var emailMessage = new StringBuilder();
            var applicationName = ConfigurationManager.AppSettings["ApplicationName"];
            var header = VeraWAF.WebPages.Bll.Resources.Email.EmailHeader.Replace("{0}",
                String.Format(VeraWAF.WebPages.Bll.Resources.Email.InternalMessageHeader,
                fromUserDisplayName, applicationName));

            emailMessage.Append(header);

            emailMessage.AppendFormat(VeraWAF.WebPages.Bll.Resources.Email.InternalMessageBody1, fromUserDisplayName, 
                HttpUtility.HtmlEncode(txtMessage.Text));

            if (!fromField.Visible) {
                var url = String.Format("http://{1}/Account/?id={0}", fromUserId, applicationName);
                emailMessage.AppendFormat(VeraWAF.WebPages.Bll.Resources.Email.InternalMessageBody2, url, fromUserDisplayName);
            }

            var footer = String.Format(VeraWAF.WebPages.Bll.Resources.Email.EmailFooter, 
                String.Format(VeraWAF.WebPages.Bll.Resources.Email.InternalMessageReason, applicationName));
            emailMessage.Append(footer);

            // Send the email
            var fromEmail = ConfigurationManager.AppSettings["fromEmail"];
            var messagingClient = new MessagingClient();
            messagingClient.SendEmail(fromEmail, toEmail,
                String.Format(VeraWAF.WebPages.Bll.Resources.Email.InternalMessageSubject, applicationName), 
                emailMessage.ToString());
        }

        /// <summary>
        /// Called when the uesr clicks the send button.
        /// Sends the e-mail.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void butSendMessage_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(ToUserEmail) && !String.IsNullOrWhiteSpace(txtMessage.Text))
            {
                object fromUserId = String.Empty;
                var fromUserDisplayName = fromField.Visible ? txtFromEmail.Text : null;

                if (String.IsNullOrWhiteSpace(fromUserDisplayName))
                {
                    var currentUser = Membership.GetUser();
                    if (currentUser == null)
                    {
                        FormsAuthentication.RedirectToLoginPage();
                        return;
                    }

                    fromUserId = currentUser.ProviderUserKey;
                    fromUserDisplayName = new UserUtilities().GetDisplayName(currentUser);
                }

                SendEmail(ToUserEmail, fromUserDisplayName, fromUserId);

                txtMessage.Text = String.Empty;
                litConfirmMessageSent.Visible = true;                
            }

        }
    }
}