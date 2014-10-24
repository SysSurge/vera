using System;
using System.Configuration;
using System.Diagnostics;
using System.Net.Mail;
using VeraWAF.AzureQueue;
using Microsoft.WindowsAzure.StorageClient;
using VeraWAF.ThreadedWorkerRoleLib;
using System.IO;
using System.Text;
using System.Net.Mime;
using VeraWAF.CrossCuttingConcerns;

namespace VeraWAF.MultiThreadedWorkerRole
{
    public class EmailWorker : WorkerEntryPoint
    {
        readonly int NumberOfSendEmailRetries;

        public EmailWorker()
        {
            NumberOfSendEmailRetries = int.Parse(ConfigurationManager.AppSettings["NumberOfSendEmailRetries"]);
        }

        void SendEmail(EmailEntity email)
        {
            Debug.WriteLine("Sending e-mail to '{0}', subject={1}, body={2}", email.To, email.Subject, email.Body);

            var mailMessage = new MailMessage { From = new MailAddress(email.From) };
            mailMessage.To.Add(new MailAddress(email.To));
            mailMessage.Subject = email.Subject;

            //Create two views; one plaintext and one HTML.
            var htmlView = AlternateView.CreateAlternateViewFromString(email.Body, null, "text/html");

            // Attach background image #1 
            var bgcontent = new LinkedResource("Resources\\bgcontent.gif", MediaTypeNames.Image.Gif);
            bgcontent.ContentId = "bgcontent";
            bgcontent.TransferEncoding = TransferEncoding.Base64;
            htmlView.LinkedResources.Add(bgcontent);

            // Attach background image #2
            var bgfooter = new LinkedResource("Resources\\bgfooter.gif", MediaTypeNames.Image.Gif);
            bgfooter.ContentId = "bgfooter";
            bgfooter.TransferEncoding = TransferEncoding.Base64;
            htmlView.LinkedResources.Add(bgfooter);

            // Attach background image #3
            var bgheader = new LinkedResource("Resources\\bgheader.gif", MediaTypeNames.Image.Gif);
            bgheader.ContentId = "bgheader";
            bgheader.TransferEncoding = TransferEncoding.Base64;
            htmlView.LinkedResources.Add(bgheader);

            // Attach Facebook icon
            var facebook = new LinkedResource("Resources\\facebook.gif", MediaTypeNames.Image.Gif);
            facebook.ContentId = "facebook";
            facebook.TransferEncoding = TransferEncoding.Base64;
            htmlView.LinkedResources.Add(facebook);

            // Attach LinkedIn icon
            var linkedin = new LinkedResource("Resources\\linkedin.gif", MediaTypeNames.Image.Gif);
            linkedin.ContentId = "linkedin";
            linkedin.TransferEncoding = TransferEncoding.Base64;
            htmlView.LinkedResources.Add(linkedin);

            // Attach Twitter icon
            var twitter = new LinkedResource("Resources\\twitter.gif", MediaTypeNames.Image.Gif);
            twitter.ContentId = "twitter";
            twitter.TransferEncoding = TransferEncoding.Base64;
            htmlView.LinkedResources.Add(twitter);

            // Attach Recycle icon
            var recycle = new LinkedResource("Resources\\recycle.gif", MediaTypeNames.Image.Gif);
            recycle.ContentId = "recycle";
            recycle.TransferEncoding = TransferEncoding.Base64;
            htmlView.LinkedResources.Add(recycle);

            //Add two views to message.
            mailMessage.AlternateViews.Add(htmlView);

            // Send the email
            new SmtpClient().Send(mailMessage);
        }

        void ProcessEmail(EmailEntity email, CloudQueueMessage queueMessage)
        {
            SendEmail(email);
            QueuDataSource.DeleteEmail(queueMessage);            
        }

        bool GetNextEmailFromQueue(out EmailEntity email, out CloudQueueMessage queueMessage)
        {
            // Try to send the e-mail NumberOfSendEmailRetries times, if all attempts fails then leave the e-mail in the queue for auditing reasons
            return QueuDataSource.GetEmail(out email, out queueMessage) 
                && queueMessage.DequeueCount < NumberOfSendEmailRetries
            ;
        }

        /// <summary>
        /// Processes the next item in the queue
        /// </summary>
        /// <returns>Returns true if an item was processed</returns>
        bool ProcessItem()
        {
            Debug.WriteLine("Starting to process a single e-mail message");
            Debug.Indent();

            EmailEntity email;
            CloudQueueMessage queueMessage;
            var result = GetNextEmailFromQueue(out email, out queueMessage);

            if (result) ProcessEmail(email, queueMessage);

            try
            {
                new LogEvent().AddEvent(ELogEventTypes.Info, 
                    String.Format("Sent e-mail to \"{0}\" with result equal #{1}.", email.To, result),
                    ConfigurationManager.AppSettings["ApplicationName"]
                    );
            }
            catch(Exception)
            {
                // Intentionally empty
            }

            Debug.Unindent();
            Debug.WriteLine("Done processing a single e-mail message with, result={0}", result);

            return result;
        }

        protected override int ProcessItems()
        {
            Debug.WriteLine("Starting to process the e-mail queue");
            Debug.Indent();

            var numberOfProcessedEmails = 0;
            var itemsInTheQueue = true ;

            do
                try
                {
                    itemsInTheQueue = ProcessItem();
                    numberOfProcessedEmails += Convert.ToInt32(itemsInTheQueue);
                }
                catch (Exception exception)
                {
                    new LogEvent().AddEvent(ELogEventTypes.Error, 
                        String.Format("E-mail queue processing exception: \"{0}\" ", exception.ToString()),
                        ConfigurationManager.AppSettings["ApplicationName"]);
                } 
            while (itemsInTheQueue);

            Debug.Unindent();
            Debug.WriteLine("Done to processing the e-mail queue, {0} e-mail's was sent", numberOfProcessedEmails);

            return numberOfProcessedEmails;
        }

    }
}
