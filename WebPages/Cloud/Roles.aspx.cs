using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Linq;
using System.Management;
using System.Text;
using System.Web.Hosting;
using VeraWAF.Core.Templates;
using VeraWAF.WebPages.Bll;

namespace VeraWAF.WebPages.Cloud
{
    public partial class Roles : PageTemplateBase
    {
        void ListCloudInstances()
        {
            var instances = new StringBuilder("");

            foreach (var role in RoleEnvironment.Roles)
            {
                instances.AppendFormat("<section><h2>Role: {0}</h2>", role.Value.Name);
                if (role.Value.Instances.Count == 0)
                    instances.AppendFormat("<small>Role <em>{0}</em> has no role instances</small><br/><br/>", role.Value.Name);

                foreach (var roleInstance in role.Value.Instances)
                {
                    var currentRoleMarker = RoleEnvironment.CurrentRoleInstance.Id == roleInstance.Id ? " *" : String.Empty;
                    instances.AppendFormat("<section><h3>Role <em>{2}</em> instance <em>{0}</em>{1}</h3><br/>",
                        roleInstance.Id, currentRoleMarker, roleInstance.Role.Name);

                    // List some metadata about the role instance
                    instances.AppendFormat("<p>Role instance fault domain: {0}</p>", roleInstance.FaultDomain);
                    instances.AppendFormat("<p>Role for the instance: {0}</p>", roleInstance.Role.Name);
                    instances.AppendFormat("<p>Role instance update domain: {0}</p><br/>", roleInstance.UpdateDomain);

                    // List the endpoints
                    instances.AppendFormat("<h4>Role <em>{1}</em> instance <em>{0}</em> endpoints</h4><br/>", roleInstance.Id,
                        roleInstance.Role.Name);

                    foreach (RoleInstanceEndpoint instanceEndpoint in roleInstance.InstanceEndpoints.Values)
                    {
                        if (roleInstance.Role.Name == "Www")
                            instances.AppendFormat("<p><a href=\"{0}://{1}/Admin/Settings.aspx\" target=\"_blank\">{1}</a></p>",
                                instanceEndpoint.Protocol, instanceEndpoint.IPEndpoint);
                        else instances.AppendFormat("<p>Instance endpoint IP address, port, and protocol : {0} {1}</p>",
                            instanceEndpoint.IPEndpoint, instanceEndpoint.Protocol);
                    }

                    instances.Append("</section>");

                }

                instances.Append("</section><hr />");
            }

            instances.AppendFormat("<small>* Current role instance is {0}</small>", RoleEnvironment.CurrentRoleInstance.Id);

            CloudHtml.Text = instances.ToString();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ListCloudInstances();
        }
    }
}