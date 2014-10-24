using System;
using System.Windows;

namespace AzureSilverlightFileUploaderPlugIn
{
    /// <summary>
    /// Silverlight application for the Azure Silverlight File Uploader
    /// </summary>
    public partial class App
    {
        /// <summary>
        /// Class constructor
        /// </summary>
        public App()
        {
            // Add application events
            Startup += Application_Startup;
            Exit += Application_Exit;
            UnhandledException += Application_UnhandledException;

            InitializeComponent();
        }

        /// <summary>
        /// Event fired when the Silverlight application starts
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Application_Startup(object sender, StartupEventArgs e)
        {
            var width = Host.Content.ActualWidth;
            var height = Host.Content.ActualHeight;

            RootVisual = new MainPage(e.InitParams, width, height);
        }

        /// <summary>
        /// Event fired when the Silverlight application stops
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Application_Exit(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Event fired when the Silverlight application encounters an unhandled exception
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            // If the app is running outside of the debugger then report the exception using
            // the browser's exception mechanism. On IE this will display it a yellow alert 
            // icon in the status bar and Firefox will display a script error.
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                // NOTE: This will allow the application to continue running after an exception has been thrown
                // but not handled. 
                // For production applications this error handling should be replaced with something that will 
                // report the error to the website and stop the application.
                e.Handled = true;
                Deployment.Current.Dispatcher.BeginInvoke(() => ReportErrorToDOM(e));
            }
        }

        /// <summary>
        /// Throws a JavaScript exception
        /// </summary>
        /// <param name="e"></param>
        void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
        {
            try
            {
                var errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
                errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

                System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(\"Unhandled error in Silverlight File Uploader Application " + errorMsg + "\");");
            }
            catch (Exception)
            {
            }
        }
    }
}
