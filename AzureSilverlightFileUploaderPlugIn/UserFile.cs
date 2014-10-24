using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Browser;
using System.Windows.Threading;

namespace AzureSilverlightFileUploaderPlugIn
{
    /// <summary>
    /// Handles logic for a file
    /// </summary>
    public class UserFile : INotifyPropertyChanged
    {
        /// <summary>
        /// Filename
        /// </summary>
        string _fileName;

        /// <summary>
        /// If true then the file is deleted
        /// </summary>
        bool _isDeleted;

        /// <summary>
        /// File stream
        /// </summary>
        Stream _fileStream;

        /// <summary>
        /// State engine
        /// </summary>
        Constants.FileStates _state = Constants.FileStates.Pending;

        /// <summary>
        /// Bytes uploded
        /// </summary>
        double _bytesUploaded;

        /// <summary>
        /// Percentage uploaded
        /// </summary>
        int _percentage;

        /// <summary>
        /// File uploaded interface
        /// </summary>
        IFileUploader FileUploader;

        /// <summary>
        /// Class constructor
        /// </summary>
        public UserFile()
        {
            FileSize = 0;
        }

        public Dispatcher UiDispatcher { get; set; }
        public bool HttpUploader { get; set; }
        public string UploadHandlerName { get; set; }

        public string UploadContainerUrl { get; set; }

        [ScriptableMember]
        public string FileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
                Id = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(value));
                NotifyPropertyChanged("FileName");
            }
        }

        public Constants.FileStates State
        {
            get { return _state; }
            set
            {
                _state = value;
                NotifyPropertyChanged("State");
            }
        }

        [ScriptableMember]
        public string StateString
        {
            get { return _state.ToString(); }
        }

        public bool IsDeleted
        {
            get { return _isDeleted; }
            set
            {
                _isDeleted = value;

                if (_isDeleted) CancelUpload();

                NotifyPropertyChanged("IsDeleted");
            }
        }

        public Stream FileStream
        {
            get { return _fileStream; }
            set
            {
                _fileStream = value;

                if (_fileStream != null)
                    FileSize = _fileStream.Length;
            }
        }

        [ScriptableMember]
        public string Id { get; private set; }

        [ScriptableMember]
        public double FileSize { get; private set; }

        [ScriptableMember]
        public double BytesUploaded
        {
            get { return _bytesUploaded; }
            set
            {
                _bytesUploaded = value;

                NotifyPropertyChanged("BytesUploaded");

                Percentage = (int)((value * 100) / FileSize);
            }
        }

        [ScriptableMember]
        public int Percentage
        {
            get { return _percentage; }
            set
            {
                _percentage = value;
                NotifyPropertyChanged("Percentage");
            }
        }

        public string ErrorMessage { get; set; }

        public void Upload(string initParams, string uploadContainerUrl)
        {
            State = Constants.FileStates.Uploading;

            // Allow the user to ovverride the upload container on a idividual upload call, this is useful if there are many upload calls
            if (!String.IsNullOrEmpty(uploadContainerUrl)) UploadContainerUrl = uploadContainerUrl;

            if (String.IsNullOrEmpty(UploadContainerUrl)) throw new ArgumentNullException("uploadContainerUrl");

            FileUploader = new Uploader(this, UploadContainerUrl);
            FileUploader.StartUpload(initParams);
            FileUploader.UploadFinished += fileUploader_UploadFinished;
        }

        [ScriptableMember]
        public void CancelUpload()
        {
            if (FileUploader != null) // && State == Constants.FileStates.Uploading)
                FileUploader.CancelUpload();
        }

        private void fileUploader_UploadFinished(object sender, EventArgs e)
        {
            FileUploader = null;

            State = Constants.FileStates.Finished;
        }


        #region INotifyPropertyChanged Members

        private void NotifyPropertyChanged(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
