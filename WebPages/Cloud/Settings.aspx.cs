using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Linq;
using System.Management;
using System.Text;
using System.Web.Hosting;
using VeraWAF.Core.Templates;
using VeraWAF.WebPages.Bll;

namespace VeraWAF.WebPages.Admin
{
    public partial class Settings : PageTemplateBase
    {
        void ShowProcessModelConfigurationInfo(RuntimeConfiguration runtimeConfiguration)
        {
            RequestQueueLimit.Text = String.Format("{0:N0} (Recommended = {1:N0})", runtimeConfiguration.ProcessModelRequestQueueLimit, 5000 * Environment.ProcessorCount);
            MaxIOThreads.Text = String.Format("{0:N0} (Recommended = {1:N0})", runtimeConfiguration.ProcessModelMaxIoThreads, 100 * Environment.ProcessorCount);
        }

        void ShowApplicationConfigurationInfo()
        {
            MaxConcurrentRequestsPerCPU.Text = String.Format("{0:N0} (Recommended = {1:N0})", HostingEnvironment.MaxConcurrentRequestsPerCPU, 5000);
            MaxConcurrentThreadsPerCPU.Text = String.Format("{0:N0} (Recommended = 0)", HostingEnvironment.MaxConcurrentThreadsPerCPU);
        }

        void ShowConnectionManagementConfigurationInfo()
        {
            DefaultConnectionLimit.Text = String.Format("{0:N0}", System.Net.ServicePointManager.DefaultConnectionLimit);
            Expect100Continue.Text = String.Format("{0} (Recommended = False)", System.Net.ServicePointManager.Expect100Continue);
            UseNagleAlgorithm.Text = String.Format("{0} (Recommended = False)", System.Net.ServicePointManager.UseNagleAlgorithm);
        }

        string GetNumberOfProcessors()
        {
            return String.Join(",", (from ManagementBaseObject item in new ManagementObjectSearcher("Select * from Win32_ComputerSystem").Get()
                                     select item["NumberOfProcessors"].ToString()).ToArray());
        }

        string GetNumberOfCores()
        {
            return String.Join(",", (from ManagementBaseObject item in new ManagementObjectSearcher("Select * from Win32_Processor").Get()
                                     select item["NumberOfCores"].ToString()).ToArray());
        }

        void ShowHardwareInfo()
        {
            NumberOfProcessors.Text = GetNumberOfProcessors();
            NumberOfCores.Text = GetNumberOfCores();
            ProcessorCount.Text = Environment.ProcessorCount.ToString();
        }



        void ShowConfigurationInfo(RuntimeConfiguration runtimeConfiguration)
        {
            ShowProcessModelConfigurationInfo(runtimeConfiguration);
            ShowApplicationConfigurationInfo();
            ShowConnectionManagementConfigurationInfo();
            ShowHardwareInfo();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ShowConfigurationInfo(new RuntimeConfiguration());
        }
    }
}