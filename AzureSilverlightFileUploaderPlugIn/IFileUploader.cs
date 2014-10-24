using System;

namespace AzureSilverlightFileUploaderPlugIn
{
    /// <summary>
    /// Interface for different kind of file uploaders
    /// </summary>
    public interface IFileUploader
    {
        void StartUpload(string initParams);
        void CancelUpload();

        event EventHandler UploadFinished;
    }
}
