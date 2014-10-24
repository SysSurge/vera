namespace AzureSilverlightFileUploaderPlugIn
{
    public static class Constants
    {
        /// <summary>
        /// Possible file upload states.
        /// Simple state engine.
        /// </summary>
        public enum FileStates
        {
            Pending = 0,
            Uploading = 1,
            Finished = 2,
            Deleted = 3,
            Error = 4
        }
    }
}
