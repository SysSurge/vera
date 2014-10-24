using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web.UI;
using VeraWAF.AzureTableStorage;
using VeraWAF.Core.Templates;
using VeraWAF.WebPages.Bll;
using VeraWAF.WebPages.Controls;
using VeraWAF.WebPages.Dal;

namespace VeraWAF.WebPages.Edit {
    public partial class SendNewsLetter : PageTemplateBase
    {
        /// <summary>
        /// Fill the controls with data from the database
        /// </summary>
        void InitControls()
        {
            var dataSource = new AzureTableStorageDataSource();
            var users =
                dataSource.GetUsers(ConfigurationManager.AppSettings["ApplicationName"]).Where(user => user.Newsletter);

            // Prevent duplicate e-mail addresses in the list by using a key/value dictionary
            var emails = new Dictionary<string, string>();

            foreach (var user in users.Where(user => !user.Email.Contains(";")).Where(user => !emails.ContainsKey(user.Email)))
                emails.Add(user.Email, String.Empty);

            foreach (var email in emails.Keys)
                txtTo.Text += email + ";";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
                InitControls();
        }

        /// <summary>
        /// Send newsletter as an e-mail.
        /// </summary>
        /// <param name="page">Page entity data</param>
        /// <param name="emailAddress">E-mail address to send to</param>
        void SendEmail(PageEntity page, string emailAddress) {
            // Now lets create an email message
            var emailMessage = new StringBuilder();
            var applicationName = ConfigurationManager.AppSettings["ApplicationName"];
            var header = VeraWAF.WebPages.Bll.Resources.Email.EmailHeader.Replace("{0}", page.Title);
            emailMessage.Append(header);
            emailMessage.AppendFormat(VeraWAF.WebPages.Bll.Resources.Email.NewsletterBody, page.VirtualPath, applicationName);
            emailMessage.Append(page.MainContent);
            var footer = String.Format(VeraWAF.WebPages.Bll.Resources.Email.EmailFooter, 
                String.Format(VeraWAF.WebPages.Bll.Resources.Email.NewsletterReason, applicationName));
            emailMessage.Append(footer);
            var fromEmail = ConfigurationManager.AppSettings["fromEmail"];

            // Send the email
            var messagingClient = new MessagingClient();
            messagingClient.SendEmail(fromEmail, emailAddress, page.Title, emailMessage.ToString());
        }

        /// <summary>
        /// Called when the user clicks the send button
        /// </summary>
        /// <remarks>
        /// The e-mails are sent to one subscriber at the time to prevent e-mail address disclosure and mail server blocking
        /// because of large e-mail lists. The e-mail are processed asynchronously by the e-mail worker process and may not be 
        /// sent immidiately. 
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void butSend_Click(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(txtVirtualPath.Text) || txtVirtualPath.Text.Contains(':'))
                    throw new ApplicationException(Bll.Resources.Email.NewsletterBadPath);

                var page = new PageCache().GetPageByVirtualPath(txtVirtualPath.Text);
                if (page == null) throw new ApplicationException(Bll.Resources.Email.NewsletterPageNotFound);

                if (String.IsNullOrWhiteSpace(txtTo.Text))
                    throw new ApplicationException(Bll.Resources.Email.NewsletterNoSubscribers);

                var emails = txtTo.Text.Split(';');

                foreach (var emailAddress in emails.Where(email => !String.IsNullOrWhiteSpace(email)))
                    SendEmail(page, emailAddress.Trim());
            }
            catch(Exception ex)
            {
                notifications.AddMessage(ex.Message);
                return;
            }

            notifications.AddMessage(Bll.Resources.Email.NewsletterSuccess1, FormNotification.NotificationType.Information);
            notifications.AddMessage(Bll.Resources.Email.NewsletterSuccess2, FormNotification.NotificationType.Information);
        }
    }
}