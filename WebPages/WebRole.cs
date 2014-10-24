using System.Linq;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Diagnostics;
using VeraWAF.WebPages.Bll;
using System;
using VeraWAF.CrossCuttingConcerns;
using System.Configuration;

namespace VeraWAF.WebPages
{
    public class WebRole : RoleEntryPoint
    {
        public override bool OnStart()
        {
            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.
            RoleEnvironment.Changing += RoleEnvironmentChanging;

            try
            {
                new LogEvent().AddEvent(ELogEventTypes.Info, 
                    string.Format("Role {0} is starting", RoleEnvironment.CurrentRoleInstance.Id),
                    ConfigurationManager.AppSettings["ApplicationName"]);
            }
            catch(Exception)
            {
                // Intentionally empty
            }

            return base.OnStart();
        }


        public override void OnStop()
        {
            try
            {
                new LogEvent().AddEvent(ELogEventTypes.Info, 
                    string.Format("Role {0} is stopping", RoleEnvironment.CurrentRoleInstance.Id),
                    ConfigurationManager.AppSettings["ApplicationName"]);
            }
            catch (Exception)
            {
                // Intentionally empty
            }

            // Good form to call base method
            base.OnStop();
        }

        private void RoleEnvironmentChanging(object sender, RoleEnvironmentChangingEventArgs e)
        {
            // If a configuration setting is changing
            if (e.Changes.Any(change => change is RoleEnvironmentConfigurationSettingChange))
            {
                // Set e.Cancel to true to restart this role instance
                e.Cancel = true;
            }
        }
    }
}
