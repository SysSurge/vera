using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Browser;

namespace AzureSilverlightFileUploaderPlugIn
{
    /// <summary>
    /// Represents a collection of files being uploaded
    /// </summary>
    [ScriptableType]
    public class FileCollection : ObservableCollection<UserFile>
    {
        /// <summary>
        /// Number of bytes uploaded in total
        /// </summary>
        double _bytesUploaded;

        /// <summary>
        /// Average percentage uploaded
        /// </summary>
        int _percentage;

        /// <summary>
        /// Number of files currently being uploaded
        /// </summary>
        int CurrentUpload;

        /// <summary>
        /// Parameters
        /// </summary>
        string _customParams;

        /// <summary>
        /// Maximum number of files concurrently being uploaded
        /// </summary>
        readonly int MaxUpload;

        /// <summary>
        /// Total files uploaded
        /// </summary>
        int _totalUploadedFiles;

        /// <summary>
        /// Number of bytes uploaded in total.
        /// Fires OnPropertyChanged()
        /// </summary>
        public double BytesUploaded
        {
            get { return _bytesUploaded; }
            set
            {
                _bytesUploaded = value;

                OnPropertyChanged(new PropertyChangedEventArgs("BytesUploaded"));
            }
        }

        /// <summary>
        /// Parameters.
        /// Fires OnPropertyChanged()
        /// </summary>
        [ScriptableMember]
        public string CustomParams
        {
            get { return _customParams; }
            set
            {
                _customParams = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CustomParams"));
            }
        }

        /// <summary>
        /// Number of selected files.
        /// </summary>
        [ScriptableMember]
        public int TotalFilesSelected
        {
            get { return Items.Count; }
        }

        /// <summary>
        /// Current average upload percentage.
        /// Fires OnPropertyChanged()
        /// </summary>
        [ScriptableMember]
        public int Percentage
        {
            get { return _percentage; }
            set
            {
                _percentage = value;

                OnPropertyChanged(new PropertyChangedEventArgs("Percentage"));

                //Notify Javascript of percentage change
                if (TotalPercentageChanged != null)
                    TotalPercentageChanged(this, null);
            }
        }

        /// <summary>
        /// Total number of uploaded files.
        /// Fires OnPropertyChanged()
        /// </summary>
        [ScriptableMember]
        public int TotalUploadedFiles
        {
            get { return _totalUploadedFiles; }
            set
            {
                _totalUploadedFiles = value;

                OnPropertyChanged(new PropertyChangedEventArgs("TotalUploadedFiles"));
            }
        }

        /// <summary>
        /// Files in the file list
        /// </summary>
        [ScriptableMember]
        public IList<UserFile> FileList
        {
            get { return Items; }
        }

        [ScriptableMember]
        public event EventHandler SingleFileUploadFinished;

        [ScriptableMember]
        public event EventHandler AllFilesFinished;

        [ScriptableMember]
        public event EventHandler ErrorOccurred;

        [ScriptableMember]
        public event EventHandler FileAdded;

        [ScriptableMember]
        public event EventHandler FileRemoved;

        [ScriptableMember]
        public event EventHandler StateChanged;

        [ScriptableMember]
        public event EventHandler TotalPercentageChanged;

        /// <summary>
        /// FileCollection constructor
        /// </summary>
        /// <param name="customParams"></param>
        /// <param name="maxUploads"></param>
        public FileCollection(string customParams, int maxUploads)
        {
            _customParams = customParams;
            MaxUpload = maxUploads;

            CollectionChanged += FileCollection_CollectionChanged;
        }

        /// <summary>
        /// Add a new file to the file collection
        /// </summary>
        /// <param name="item"></param>
        public new void Add(UserFile item)
        {
            var results = from c in Items where c.FileName == item.FileName select c;
            if (results.Count() > 0) return;

            //Listen to the property changed for each added item
            item.PropertyChanged += item_PropertyChanged;

            base.Add(item);

            if (FileAdded != null) FileAdded(this, null);
        }

        /// <summary>
        /// Removed an existing user file to the collection
        /// </summary>
        /// <param name="item"></param>
        public new void Remove(UserFile item)
        {
            base.Remove(item);

            if (FileRemoved != null) FileRemoved(this, null);
        }

        /// <summary>
        /// Clears the complete list
        /// </summary>
        public new void Clear()
        {
            base.Clear();

            if (FileRemoved != null) FileRemoved(this, null);
        }

        public void UploadFiles()
        {
            UploadFiles(null);
        }

        /// <summary>
        /// Start uploading files
        /// </summary>
        public void UploadFiles(string uploadContainerUrl)
        {
            lock (this)
            {
                foreach (var file in
                    this.Where(file => !file.IsDeleted && file.State == Constants.FileStates.Pending && CurrentUpload < MaxUpload))
                {
                    file.Upload(_customParams, uploadContainerUrl);
                    CurrentUpload++;
                }
            }
        }

        /// <summary>
        /// Recount statistics
        /// </summary>
        void RecountTotal()
        {
            //Recount total
            double totalSize = 0;
            double totalSizeDone = 0;

            foreach (var file in this)
            {
                totalSize += file.FileSize;
                totalSizeDone += file.BytesUploaded;
            }

            double percentage = 0;

            if (totalSize > 0) percentage = 100 * totalSizeDone / totalSize;

            BytesUploaded = totalSizeDone;

            Percentage = (int)percentage;
        }

        /// <summary>
        /// Check if all files are finished uploading
        /// </summary>
        void AreAllFilesFinished()
        {
            if (Percentage == 100 && AllFilesFinished != null) AllFilesFinished(this, null);
        }

        /// <summary>
        /// The collection changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void FileCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //Recount total when the collection changed (items added or deleted)
            RecountTotal();
        }

        /// <summary>
        /// Property of individual item changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //Check if deleted property is changed
            switch (e.PropertyName)
            {
                case "IsDeleted":
                    {
                        var file = (UserFile)sender;

                        if (file.IsDeleted)
                        {
                            if (file.State == Constants.FileStates.Uploading)
                            {
                                CurrentUpload--;
                                UploadFiles();
                            }

                            Remove(file);

                            file = null;
                        }
                    }
                    break;
                case "State":
                    {
                        var file = (UserFile)sender;
                        switch (file.State)
                        {
                            case Constants.FileStates.Finished:
                                CurrentUpload--;
                                TotalUploadedFiles++;
                                UploadFiles();
                                if (SingleFileUploadFinished != null)
                                    SingleFileUploadFinished(this, null);
                                break;
                            case Constants.FileStates.Error:
                                CurrentUpload--;
                                UploadFiles();
                                if (ErrorOccurred != null)
                                    ErrorOccurred(this, null);
                                break;
                        }

                        if (StateChanged != null) StateChanged(this, null);

                        AreAllFilesFinished();
                    }
                    break;
                case "BytesUploaded":
                    RecountTotal();
                    break;
            }
        }
    }
}
