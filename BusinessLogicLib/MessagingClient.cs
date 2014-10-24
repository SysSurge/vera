using System.Configuration;
using System.Web;
using VeraWAF.AzureQueue;

namespace VeraWAF.WebPages.Bll {

    /// <summary>
    /// The Messaging client allows the user to add e-mail on the e-mail processing queue
    /// </summary>
    public class MessagingClient 
    {
        /// <summary>
        /// Send an e-mail.
        /// </summary>
        /// <remarks>
        /// The email is added to the e-mail queue and processed asynchronously by the Azure worker role at its discretion.
        /// Function will return immidiately and not wait for the e-mail to be sent.
        /// If the user is sending too many e-mail then he/she is blocked from sending any more e-mail for a while.
        /// </remarks>
        /// <example>
        /// // Create an email message
        /// var fromEmail = ConfigurationManager.AppSettings["fromEmail"];
        /// var toEmail = "someone@example.com;someone_else@example.com";
        /// 
        /// // Send the email
        /// var messagingClient = new MessagingClient();
        /// messagingClient.SendEmail(fromEmail, toEmail, "Eat a Silicon Burger today!", 
        ///    "<b>50% rebate today only!</b><p> Come with three of your friends and get a free soda!</p>");
        /// </example>
        /// <param name="from">From e-mail address(es)</param>
        /// <param name="to">To e-mail address(es)</param>
        /// <param name="subject">E-mail subject</param>
        /// <param name="body">HTML or plain text e-mail message body</param>
        /// <returns>Returns true if the e-mail was successfully added to the e-mail queue</returns>
        public bool SendEmail(string from, string to, string subject, string body)
        {
            var maxNumberOfRequestAMinute = long.Parse(ConfigurationManager.AppSettings["MaxNumberOfEmailsPerHour"]);
            if (new HammerProtection(maxNumberOfRequestAMinute, HammeringMode.Hours, HammerTypes.EmailHammering).HostIsHammering(
                new ServerTools().GetClientIpAddress())) {
                // The user is hammering, so don't waste processing power on him
                HttpContext.Current.Response.Redirect("/ErrorPages/EmailHammering.aspx", true);
            }

            var queueDataSource = new AzureQueueDataSource();
            return queueDataSource.SendEmail(from, to, subject, body);                
        }
    }
}