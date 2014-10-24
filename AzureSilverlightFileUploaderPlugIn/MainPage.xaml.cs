using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace AzureSilverlightFileUploaderPlugIn
{
    /// <summary>
    /// Layout of the Azure Silverlight File Uploader.
    /// Draws a icon that allows for drag'n drop and file browsing on the local disk.
    /// </summary>
    [ScriptableType]
    public partial class MainPage : UserControl
    {
        /// <summary>
        /// Maximum allowed file size.
        /// </summary>
        long MaxFileSize = long.MaxValue;

        /// <summary>
        /// Files currently being uploaded
        /// </summary>
        readonly FileCollection _files;

        /// <summary>
        /// Max concurrent uploads
        /// </summary>
        int MaxUpload = 2;

        /// <summary>
        /// Parameters
        /// </summary>
        string CustomParams;

        /// <summary>
        /// File name filter
        /// </summary>
        string FileFilter;

        /// <summary>
        /// File upload handle name
        /// </summary>
        string UploadHandlerName;

        /// <summary>
        /// Azure blob container
        /// </summary>
        string UploadContainerUrl;

        /// <summary>
        /// Total progress percentage
        /// </summary>
        int TotalProgress;

        /// <summary>
        /// Time when the upload started
        /// </summary>
        DateTime UploadStartTime;

        /// <summary>
        /// If true then multiple async uploads are allowed
        /// </summary>
        bool AllowMultipleFileUpload;

        #region JavaScript events

        /// <summary>
        /// Name of JavaScript event when the total percentage changes
        /// </summary>
        string TotalPercentageChangedEvent;
        
        /// <summary>
        /// Name of JavaScript event when all files have been uploaded
        /// </summary>
        string AllFilesFinishedEvent;
        
        /// <summary>
        /// Name of JavaScript event when the file list has been cleared
        /// </summary>
        string FileListEmptyEvent;

        /// <summary>
        /// Name of JavaScript event when the file list changes
        /// </summary>
        string CollectionChangedEvent;

        #endregion

        /// <summary>
        /// Default viewport width
        /// </summary>
        const double DEFAULT_WIDTH = 220.0;

        /// <summary>
        /// Default viewport height
        /// </summary>
        const double DEFAULT_HEIGHT = 300.0;

        /// <summary>
        /// Default line thickness
        /// </summary>
        const double DEFAULT_STROKE_THICKNESS = 2.0;

        /// <summary>
        /// Get the events from the parameters
        /// </summary>
        /// <param name="initParams"></param>
        void GetEvents(IDictionary<string, string> initParams)
        {
            if (initParams.ContainsKey("TotalPercentageChanged_event") && !string.IsNullOrEmpty(initParams["TotalPercentageChanged_event"]))
                TotalPercentageChangedEvent = initParams["TotalPercentageChanged_event"];

            if (initParams.ContainsKey("AllFilesFinished_event") && !string.IsNullOrEmpty(initParams["AllFilesFinished_event"]))
                AllFilesFinishedEvent = initParams["AllFilesFinished_event"];
            
            if (initParams.ContainsKey("FileListEmpty_event") && !string.IsNullOrEmpty(initParams["FileListEmpty_event"]))
                FileListEmptyEvent = initParams["FileListEmpty_event"];
            
            if (initParams.ContainsKey("CollectionChanged_event") && !string.IsNullOrEmpty(initParams["CollectionChanged_event"]))
                CollectionChangedEvent = initParams["CollectionChanged_event"];
        }

        #region Upload box

        /// <summary>
        /// Resizes a container of polygons
        /// </summary>
        /// <param name="container">Container</param>
        /// <param name="xFactor">Scale factor (X)</param>
        /// <param name="yFactor">Scale factor (Y)</param>
        /// <param name="strokeThickness">New line width</param>
        void ResizePolygons(Panel container, double xFactor, double yFactor, double strokeThickness)
        {
            foreach (var polygon in container.Children.OfType<Polygon>())
                for (var i = 0; i < polygon.Points.Count; i++)
                {
                    polygon.Points[i] =
                        new Point(polygon.Points[i].X * xFactor,
                                  polygon.Points[i].Y * yFactor);

                    polygon.StrokeThickness = strokeThickness;
                }
        }

        /// <summary>
        /// Get the scale factor (X)
        /// </summary>
        /// <param name="width">Current width</param>
        /// <returns>New width</returns>
        double GetXfactor(double width)
        {
            return width / DEFAULT_WIDTH;
        }

        /// <summary>
        /// Get the scale factor (Y)
        /// </summary>
        /// <param name="height">Current height</param>
        /// <returns>New height</returns>
        double GetYfactor(double height)
        {
            return height / DEFAULT_HEIGHT;
        }

        /// <summary>
        /// Get the line width
        /// </summary>
        /// <param name="height">Current line width</param>
        /// <returns>New line width</returns>
        double GetStrokeThickness(double width)
        {
            return (width / DEFAULT_WIDTH) * DEFAULT_STROKE_THICKNESS;
        }

        /// <summary>
        /// Resizes the icon
        /// </summary>
        /// <param name="width">Current width</param>
        /// <param name="height">Current height</param>
        void ResizeButtonContents(double width, double height)
        {
            var xFactor = GetXfactor(width);
            var yFactor = GetYfactor(height);
            var strokeThickness = GetStrokeThickness(width);

            ResizePolygons(BoxContainer, xFactor, yFactor, strokeThickness);
            ResizePolygons(ArrowContainer, xFactor, yFactor, strokeThickness);
        }

        /// <summary>
        /// Init the arrow animation
        /// </summary>
        /// <param name="height"></param>
        void InitArrowAnimation(double height)
        {
            // Scale the animation to the height of the object
            ((DoubleAnimation)ArrowStoryboard.Children[0]).To = height / 2.0;
        }

        #endregion

        /// <summary>
        /// Draws the graphics
        /// </summary>
        /// <param name="initParams">Parameters</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        public MainPage(IDictionary<string, string> initParams, double width, double height)
        {
            InitializeComponent();

            LoadConfiguration(initParams);
            GetEvents(initParams);

            _files = new FileCollection(CustomParams, MaxUpload);

            HtmlPage.RegisterScriptableObject("Files", _files);
            HtmlPage.RegisterScriptableObject("MainPage", this);

            Loaded += Page_Loaded;
            _files.CollectionChanged += _files_CollectionChanged;
            _files.AllFilesFinished += _files_AllFilesFinished;
            _files.TotalPercentageChanged += _files_TotalPercentageChanged;

            SelectFilesButton.Drop += SelectFilesButton_Drop;
            SelectFilesButton.DragEnter += SelectFilesButton_DragEnter;
            SelectFilesButton.DragLeave += SelectFilesButton_DragLeave;
            SelectFilesButton.MouseEnter += SelectFilesButton_MouseEnter;
            SelectFilesButton.MouseLeave += SelectFilesButton_MouseLeave;

            try
            {
                //ResizeButtonContents(width, height);
                //InitArrowAnimation(height);
            }
            catch
            {
                // Resizing fails on some browsers, so ignore error
            }
        }

        /// <summary>
        /// Event that is fired when the 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SelectFilesButton_DragLeave(object sender, DragEventArgs e)
        {
            AddIcon.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Event that is fired when the the user drags a file over the icon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SelectFilesButton_DragEnter(object sender, DragEventArgs e)
        {
            AddIcon.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Event that is fired when the mouse pointer is over the icon.
        /// Starts the arrow animation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SelectFilesButton_MouseEnter(object sender, MouseEventArgs e)
        {
            ArrowStoryboard.Begin();
        }

        /// <summary>
        /// Event that is fired when the mouse pointer leaves the icon
        /// Stops the arrow animation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SelectFilesButton_MouseLeave(object sender, MouseEventArgs e)
        {
            ArrowStoryboard.Stop();
        }

        /// <summary>
        /// Event that is fired when the user has drag'ed and drop'ed a file(s) on the icon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SelectFilesButton_Drop(object sender, DragEventArgs e)
        {
            var files = e.Data.GetData(DataFormats.FileDrop) as FileInfo[];

            foreach (var file in files)
            {
                _files.Add(new UserFile
                {
                    FileName = file.Name,
                    FileStream = file.OpenRead(),
                    UploadHandlerName = UploadHandlerName,
                    UploadContainerUrl = UploadContainerUrl,
                    UiDispatcher = Dispatcher
                });
            }
        }

        /// <summary>
        /// Calculates the time taken since the upload started.
        /// </summary>
        /// <returns>Time taken since the upload started</returns>
        TimeSpan CalculateTimeTaken()
        {
            return DateTime.Now - UploadStartTime;
        }

        /// <summary>
        /// Calculates the remaining time for all the uploads to complete
        /// </summary>
        /// <returns>The remaining time for all the uploads to complete</returns>
        TimeSpan CalculateRemainingTime(int currentPercentage)
        {
            var timeSpentSoFar = CalculateTimeTaken();
            var factor = timeSpentSoFar.Ticks/currentPercentage;
            var totalEstimatedTime = new TimeSpan(factor);
            return totalEstimatedTime - timeSpentSoFar;
        }

        /// <summary>
        /// Temp value for storing percentages since last event.
        /// </summary>
        DateTime? TotalPercentageChangedEventLastCalled;

        bool TotalPercentageChangedEventDone;

        /// <summary>
        /// Use trotteling to prevent SL from flooding the browser with event calls
        /// </summary>
        /// <returns></returns>
        bool TotalPercentageChangedEventTrotteling(int totalProgress)
        {
            bool result;

            if (TotalPercentageChangedEventLastCalled.HasValue)
            {
                result = DateTime.Now.Second - TotalPercentageChangedEventLastCalled.Value.Second > 1;

                // Make sure that the 100 % event is called, but make sure it is only called once
                if (!result && totalProgress == 100 && !TotalPercentageChangedEventDone)
                {
                    result = true;
                    TotalPercentageChangedEventDone = true;
                }

            }
            else result = true;

            if (result) TotalPercentageChangedEventLastCalled = DateTime.Now;

            return result;
        }

        /// <summary>
        /// Event that is fired when the total percentage has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _files_TotalPercentageChanged(object sender, EventArgs e)
        {
            long millisecondsLeft;
            if (_files.Percentage < 5) millisecondsLeft = -1;   // Don't try to calculate time left before at least 5% has been uploaded
            else millisecondsLeft = CalculateRemainingTime(_files.Percentage).Milliseconds;

            TotalProgress = _files.Percentage;

            if (!String.IsNullOrEmpty(TotalPercentageChangedEvent)
                && (TotalPercentageChangedEventTrotteling(TotalProgress)))
                HtmlPage.Window.Invoke(TotalPercentageChangedEvent, TotalProgress, millisecondsLeft);

        }

        /// <summary>
        /// Event that is fired when all the files have been uploaded 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _files_AllFilesFinished(object sender, EventArgs e)
        {
            VisualStateManager.GoToState(this, "Finished", true);
            
            if (!String.IsNullOrEmpty(AllFilesFinishedEvent))
                HtmlPage.Window.Invoke(AllFilesFinishedEvent, CalculateTimeTaken().Milliseconds, _files.Count);
        }

        void Page_Loaded(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Empty", false);

            if (!String.IsNullOrEmpty(FileListEmptyEvent)) HtmlPage.Window.Invoke(FileListEmptyEvent);
        }

        /// <summary>
        /// Event that is fired when the the file list changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _files_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (_files.Count == 0)
            {
                VisualStateManager.GoToState(this, "Empty", true);

                if (!String.IsNullOrEmpty(FileListEmptyEvent)) HtmlPage.Window.Invoke(FileListEmptyEvent);            
            }
            else
            {
                if (_files.FirstOrDefault(f => f.State == Constants.FileStates.Uploading) != null)
                {
                    VisualStateManager.GoToState(this, "Uploading", true);

                    if (!String.IsNullOrEmpty(CollectionChangedEvent)) HtmlPage.Window.Invoke(CollectionChangedEvent, "Uploading", _files.Count);
                }
                else if (_files.FirstOrDefault(f => f.State == Constants.FileStates.Finished) != null)
                {
                    VisualStateManager.GoToState(this, "Finished", true);

                    if (!String.IsNullOrEmpty(CollectionChangedEvent)) HtmlPage.Window.Invoke(CollectionChangedEvent, "Finished", _files.Count);
                }
                else
                {
                    VisualStateManager.GoToState(this, "Selected", true);
                    
                    if (!String.IsNullOrEmpty(CollectionChangedEvent)) HtmlPage.Window.Invoke(CollectionChangedEvent, "Selected", _files.Count);
                }
            }

        
        }

        /// <summary>
        /// Clears the list
        /// </summary>
        [ScriptableMember]
        public void ClearList()
        {
            _files.Clear();
        }

        [ScriptableMember]
        public event EventHandler MaximumFileSizeReached;

        /// <summary>
        /// Load configuration first from initParams, then from .Config file
        /// </summary>
        /// <param name="initParams">Parameters</param>
        void LoadConfiguration(IDictionary<string, string> initParams)
        {
            //Load Custom Config String
            if (initParams.ContainsKey("CustomParam") && !string.IsNullOrEmpty(initParams["CustomParam"]))
                CustomParams = initParams["CustomParam"];

            if (initParams.ContainsKey("MaxUploads") && !string.IsNullOrEmpty(initParams["MaxUploads"]))
                int.TryParse(initParams["MaxUploads"], out MaxUpload);

            if (initParams.ContainsKey("MaxFileSizeKB") && !string.IsNullOrEmpty(initParams["MaxFileSizeKB"]))
                if (long.TryParse(initParams["MaxFileSizeKB"], out MaxFileSize))
                    MaxFileSize = MaxFileSize * 1024;

            if (initParams.ContainsKey("FileFilter") && !string.IsNullOrEmpty(initParams["FileFilter"]))
                FileFilter = initParams["FileFilter"];

            if (initParams.ContainsKey("UploadHandlerName") && !string.IsNullOrEmpty(initParams["UploadHandlerName"]))
                UploadHandlerName = initParams["UploadHandlerName"];

            if (initParams.ContainsKey("UploadContainerUrl") && !string.IsNullOrEmpty(initParams["UploadContainerUrl"]))
                UploadContainerUrl = initParams["UploadContainerUrl"];

            if (initParams.ContainsKey("AllowMultipleFileUpload") && !string.IsNullOrEmpty(initParams["AllowMultipleFileUpload"]))
                if (bool.TryParse(initParams["AllowMultipleFileUpload"], out AllowMultipleFileUpload))
                    AllowMultipleFileUpload = bool.Parse(initParams["AllowMultipleFileUpload"]);
        }

        /// <summary>
        /// Select files button click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SelectFilesButton_Click(object sender, RoutedEventArgs e)
        {
            SelectUserFiles();
        }

        /// <summary>
        /// Open the select file dialog
        /// </summary>
        //[ScriptableMember]
        public void SelectUserFiles()
        {
            var ofd = new OpenFileDialog {Multiselect = AllowMultipleFileUpload};

            try
            {
                // Check the file filter (filter is used to filter file extensions to select, for example only .jpg files)
                if (!string.IsNullOrEmpty(FileFilter)) ofd.Filter = FileFilter;
            }
            catch (ArgumentException ex)
            {
                //User supplied a wrong configuration file
                throw new Exception("Wrong file filter configuration.", ex);
            }

            if (ofd.ShowDialog() == true)
            {
                foreach (var userFile in ofd.Files.Select(file => new UserFile
                                                                      {
                                                                          FileName = file.Name, 
                                                                          FileStream = file.OpenRead(), 
                                                                          UploadHandlerName = UploadHandlerName, 
                                                                          UploadContainerUrl = UploadContainerUrl,
                                                                          UiDispatcher = Dispatcher
                                                                      }))
                {

                    //Check for the file size limit (configurable)
                    if (userFile.FileStream.Length <= MaxFileSize)
                    {
                        //Add to the list
                        _files.Add(userFile);
                    }
                    else
                    {
                        HtmlPage.Window.Alert("Maximum file size is: " + (MaxFileSize / 1024) + " KB.");

                        if (MaximumFileSizeReached != null) MaximumFileSizeReached(this, null);

                    }
                }
            }
        }
        /// <summary>
        /// Start uploading files
        /// </summary>
        [ScriptableMember]
        public void UploadFiles()
        {
            UploadFiles(null);
        }

        /// <summary>
        /// Start uploading files
        /// </summary>
        /// <param name="uploadContainerUrl">Azure container to upload to</param>
        [ScriptableMember]
        public void UploadFiles(string uploadContainerUrl)
        {
            if (_files.Count == 0) HtmlPage.Window.Alert("No files to upload. Please select one or more files first.");
            else
            {
                TotalPercentageChangedEventDone = false;

                UploadStartTime = DateTime.Now;

                //Tell the collection to start uploading
                _files.UploadFiles(uploadContainerUrl);
            }
        }

        /// <summary>
        /// Clear the file list
        /// </summary>
        [ScriptableMember]
        public void ClearFilesList()
        {
            _files.Clear();
        }

        /// <summary>
        /// Cancels a file upload
        /// </summary>
        /// <param name="id">File ID</param>
        [ScriptableMember]
        public void CancelUpload(string id)
        {
            foreach (var file in _files.Where(file => file.Id.Contains(id)))
            {
                file.CancelUpload();
                _files.Remove(file);
                break;
            }
        }
    }
}
