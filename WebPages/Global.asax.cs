using VeraWAF.AzureTableStorage;
using VeraWAF.CrossCuttingConcerns;
using System;
using System.Configuration;
using System.Net;
using System.Web;
using System.Web.Hosting;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure.ServiceRuntime;
using VeraWAF.WebPages.Bll;
using VeraWAF.WebPages.Bll.Cloud;
using VeraWAF.WebPages.Bll.Search;
using VeraWAF.WebPages.Bll.VirtualPathProvider;

namespace VeraWAF.WebPages
{
    public class Global : HttpApplication
    {
        /// <summary>
        /// Set the maximum allowed number of concurrent connections
        /// </summary>
        void SetDefaultConnectionLimit()
        {
            ServicePointManager.DefaultConnectionLimit = int.Parse(ConfigurationManager.AppSettings["DefaultConnectionLimit"]);
        }

        /// <summary>
        /// Setup CloudStorageAccount Configuration Setting Publisher
        /// </summary>
        void SetCloudPublisher()
        {
            CloudStorageAccount.SetConfigurationSettingPublisher(
                (configName, configSettingPublisher) =>
                {
                    var connectionString =
                        RoleEnvironment.GetConfigurationSettingValue(configName);
                    configSettingPublisher(connectionString);
                }
                );
        }

        /// <summary>
        /// Setup Custom Virtual Path provider
        /// </summary>
        void SetVirtualPathProvider()
        {
            HostingEnvironment.RegisterVirtualPathProvider(new CustomVirtualPathProvider());
        }

        /// <summary>
        /// Initiate search index
        /// </summary>
        void InitSearchIndex()
        {
            try
            {
                /*
                We should always regenerate the search index when the role starts in case this is 
                a new cloud instance. If we don't the search index file may not exist and we may
                get an error later on when someone tries to do a search
                */
                var _dataSource = new AzureTableStorageDataSource();
                var _applicationName = ConfigurationManager.AppSettings["ApplicationName"];
                var articles = _dataSource.GetPages(_applicationName);
                new LuceneClient().BuildIndex(articles);

                // Log that a new search index was rebuilt
                var roleId = "[Unknown]";
                if (RoleEnvironment.CurrentRoleInstance != null && RoleEnvironment.CurrentRoleInstance.Id != null)
                    roleId = RoleEnvironment.CurrentRoleInstance.Id;
                new LogEvent().AddEvent(ELogEventTypes.Info, string.Format("Rebuildt search index on role {0}", roleId),
                    _applicationName);

            }
            catch (Exception)
            {
                // Intentionally empty            
            }
        }

        /// <summary>
        /// Initiate XML sitemap
        /// </summary>
        void InitXmlSiteMap()
        {
            try
            {
                /*
                We should always regenerate the site map when the role starts in case this is 
                a new cloud instance. If we don't the site map file may not exist and we may
                get an error later on from bots that crawls the web site
                */
                var baseUrl = ConfigurationManager.AppSettings["BaseUrl"];
                new XmlSitemapGenerator().GenerateFile(baseUrl);

                // Log that a new sitemap.xml file was generated
                var roleId = "[Unknown]";
                if (RoleEnvironment.CurrentRoleInstance != null && RoleEnvironment.CurrentRoleInstance.Id != null)
                    roleId = RoleEnvironment.CurrentRoleInstance.Id;
                new LogEvent().AddEvent(ELogEventTypes.Info,
                    string.Format("Generated sitemap.xml file on role {0}", roleId),
                    ConfigurationManager.AppSettings["ApplicationName"]);
            }
            catch (Exception)
            {
                // Intentionally empty            
            }
        }

        /// <summary>
        /// Log that the role is starting
        /// </summary>
        void LogRoleStarting()
        {
            try
            {
                var roleId = "[Unknown]";
                if (RoleEnvironment.CurrentRoleInstance != null && RoleEnvironment.CurrentRoleInstance.Id != null)
                    roleId = RoleEnvironment.CurrentRoleInstance.Id;

                new LogEvent().AddEvent(ELogEventTypes.Info, String.Format("Starting WWW service on role {0}", roleId),
                    ConfigurationManager.AppSettings["ApplicationName"]);
            }
            catch (Exception)
            {
                // Intentionally empty
            }
        }

        /// <summary>
        /// Upload a clientaccesspolicy.xml file to the Azure Blob $root folder.
        /// Having this file allows SilverLight applications to access the Azure Blob storage
        /// </summary>
        void InitBlob()
        {
            try
            {
                // Setup the connection to Windows Azure Storage
                var storageAccount = CloudStorageAccount.FromConfigurationSetting("DataConnectionString");
                var blobClient = storageAccount.CreateCloudBlobClient();

                // Upload the clientaccesspolicy.xml file to the Azure Blob root
                new CloudUtils().CreateSilverlightPolicy(blobClient);
            }
            catch (Exception)
            {
                // Intentionally empty
            }
        }

        /// <summary>
        /// Touch the default root file to update the ASP.NET page cache dependency
        /// </summary>
        void UpdateFileCacheDependency()
        {
            try
            {
                const string rootPage = "~/Default.aspx";
                var fileName = Server.MapPath(rootPage);
                System.IO.File.SetLastWriteTimeUtc(fileName, DateTime.UtcNow);
            }
            catch (Exception)
            {
                // Intentionally empty
            }
        }

        /// <summary>
        /// Event that is fired when the Web application starts
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Application_Start(object sender, EventArgs e)
        {
            SetDefaultConnectionLimit();

            SetCloudPublisher();

            SetVirtualPathProvider();

            LogRoleStarting();

            InitSearchIndex();

            InitXmlSiteMap();

            InitBlob();

            UpdateFileCacheDependency();
        }

        /// <summary>
        /// Event that is fired when the Web application closes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown
            try
            {
                var roleId = "[Unknown]";
                if (RoleEnvironment.CurrentRoleInstance != null && RoleEnvironment.CurrentRoleInstance.Id != null)
                    roleId = RoleEnvironment.CurrentRoleInstance.Id;

                new LogEvent().AddEvent(ELogEventTypes.Info,
                    String.Format("Stopping WWW service on role {0}", roleId),
                    ConfigurationManager.AppSettings["ApplicationName"]);
            }
            catch (Exception)
            {
                // Intentionally empty
            }
        }

        /// <summary>
        /// Event that is fired for each HTTP request
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Application_BeginRequest(object sender, EventArgs e)
        {
            // Should we force the user to a secure connection?
            bool forceSecure;
            if (bool.TryParse(ConfigurationManager.AppSettings["ForceHTTPS"], out forceSecure) && forceSecure
                && HttpContext.Current.Request.IsSecureConnection.Equals(false) && HttpContext.Current.Request.IsLocal.Equals(false))
            {
                // Force over to HTTPS
                var secureUrl = String.Format("https://{0}{1}", Request.ServerVariables["HTTP_HOST"], HttpContext.Current.Request.RawUrl);
                Response.Redirect(secureUrl, true);
            }

            // Should we prevent HTTP request hammering?
            bool enableReqHamProt;
            if (bool.TryParse(ConfigurationManager.AppSettings["EnableReqHamProt"], out enableReqHamProt) && enableReqHamProt)
            {
                // Prevent request hammering
                var maxNumberOfRequestAMinute = long.Parse(ConfigurationManager.AppSettings["MaxNumberOfRequestAMinute"]);
                if (new HammerProtection(maxNumberOfRequestAMinute).HostIsHammering(new ServerTools().GetClientIpAddress()))
                {
                    // The user has been caught HTTP request hammering, so we don't waste any more processing power on him
                    Response.StatusCode = 304;
                    Response.StatusDescription = "Not Modified";
                    Response.End();
                }
            }
        }

        /// <summary>
        /// Event that is fired when an unhandled error is caught
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Application_Error(object sender, EventArgs e)
        {
            try
            {
                // Get exception
                var exception = Server.GetLastError();

                // Ignore all "404 Page not found" exceptions - those are handled later on
                var httpEx = exception as HttpException;
                if (httpEx == null || httpEx.GetHttpCode() != 404)
                {
                    var url = "[Unknown]";
                    if (HttpContext.Current != null && HttpContext.Current.Request != null && HttpContext.Current.Request.Url != null)
                        url = HttpContext.Current.Request.Url.AbsoluteUri;

                    var roleId = "[Unknown]";
                    if (RoleEnvironment.CurrentRoleInstance != null && RoleEnvironment.CurrentRoleInstance.Id != null)
                        roleId = RoleEnvironment.CurrentRoleInstance.Id;

                    new LogEvent().AddEvent(ELogEventTypes.Error, 
                        String.Format("Role \"{0}\" caught an unhandled exception in page \"{1}\": {2}",
                            roleId, url, exception == null ? "[Unknown]" : exception.ToString()), 
                        ConfigurationManager.AppSettings["ApplicationName"]);
                }
            }
            catch (Exception)
            {
                // Intentionally empty
            }
        }

        /// <summary>
        /// Code that runs when a new session is started
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// Vera has sessions disabled by default, you can enable it in the Web.config.
        /// </remarks>
        void Session_Start(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Code that runs when a session ends.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// The Session_End event is raised only when the sessionstate mode is set to InProc in the Web.config file. If 
        /// session mode is set to StateServer or SQLServer, then this event is not raised.
        /// </remarks>
        void Session_End(object sender, EventArgs e)
        {
        }
    }
}