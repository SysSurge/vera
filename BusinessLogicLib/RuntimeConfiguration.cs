using System;
using System.Configuration;
using System.Web.Configuration;

namespace VeraWAF.WebPages.Bll
{
    public class RuntimeConfiguration
    {
        HttpRuntimeSection _httpRuntimeSection = (HttpRuntimeSection)ConfigurationManager.GetSection("system.web/httpRuntime");
        ProcessModelSection _processModelSection = (ProcessModelSection)ConfigurationManager.GetSection("system.web/processModel");

        #region Process model

        public int ProcessModelMaxIoThreads
        {
            get { return _processModelSection.MaxIOThreads; }
        }

        public int ProcessModelRequestQueueLimit 
        {
            get { return _processModelSection.RequestQueueLimit; }
        }

        public DateTime GetSiteLaunchDate() {
            return DateTime.Parse(ConfigurationManager.AppSettings["SiteLaunchDate"]);
        }


        public TimeSpan GetVirtualFileCacheSlidingExpiration() {
            var virtualFileCacheSlidingExpiration = ConfigurationManager.AppSettings["VirtualFileCacheSlidingExpiration"];
            return TimeSpan.ParseExact(virtualFileCacheSlidingExpiration, "c", null);
        }
       
        #endregion

    }
}