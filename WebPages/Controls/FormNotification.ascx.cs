using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace VeraWAF.WebPages.Controls
{
    /// <summary>
    /// Shows messages to notify the user after some action on a form.
    /// </summary>
    public partial class FormNotification : UserControl
    {
        /// <summary>
        /// Notification types
        /// </summary>
        public enum NotificationType
        {
            Information, Failure
        }

        /// <summary>
        /// Is True if there are recorded failures, or False if there are no failures
        /// </summary>
        bool HasFailures { get; set; }

        /// <summary>
        /// Class constructor
        /// </summary>
        public FormNotification()
        {
            HasFailures = false;
        }

        /// <summary>
        /// Add a new message
        /// </summary>
        /// <param name="message"></param>
        public void AddMessage(string message, NotificationType notificationType = NotificationType.Failure)
        {
            // We have recorded failures
            HasFailures = true;

            switch(notificationType)
            {
                case NotificationType.Information:
                    infoMessages.InnerHtml += String.Format("<li>{0}</li>", message);
                    infoMessages.Visible = true;
                    break;
                case NotificationType.Failure:
                    failureMessages.InnerHtml += String.Format("<li>{0}</li>", message);
                    failureMessages.Visible = true;
                    break;
            }
        }
    }
}