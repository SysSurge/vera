using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Configuration;
using System.Web;
using VeraWAF.Core.Templates;
using VeraWAF.CrossCuttingConcerns;
using VeraWAF.WebPages.Bll;
using VeraWAF.WebPages.Controls;

namespace VeraWAF.WebPages.Cloud
{
    /*
    * Non-MVC interface to the ASP.NET process
    * We've used a ASP.NET interface as standard MVC does not have access to the ASP.NET process
    */
    public partial class CloudCommand : PageTemplateBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var command = HttpContext.Current.Request["command"];
            if (String.IsNullOrWhiteSpace(command))
                return; // Nothing to do

            try
            {
                // Execute the cloud command locally
                new Bll.Cloud.CloudCommand().Execute(command);

                // Execute the cloud command on all the other nodes
                new Bll.Cloud.CloudCommandClient().SendCommand(command, true);

#if DEBUG
                // Log the command
                new LogEvent().AddEvent(ELogEventTypes.Info, String.Format("Role {0} recieved a \"{1}\" command from {2}. Full url is \"{3}\"",
                    RoleEnvironment.CurrentRoleInstance.Id, command, new ServerTools().GetClientIpAddress(), Request.Url.AbsoluteUri), 
                    ConfigurationManager.AppSettings["ApplicationName"]);
#endif
            }
            catch(Exception ex)
            {
                notifications.AddMessage(ex.Message);
                return;
            }

            notifications.AddMessage(String.Format("\"{0}\" command sent", command), FormNotification.NotificationType.Information);
        }
    }
}