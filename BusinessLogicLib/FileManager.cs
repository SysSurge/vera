using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Services.Client;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using VeraWAF.AzureTableStorage;
using VeraWAF.CrossCuttingConcerns;
using VeraWAF.WebPages.Bll.Cloud;

namespace VeraWAF.WebPages.Bll {

    /// <summary>
    /// File element types
    /// </summary>
    public enum StoredResourceType {
        File, Directory
    }

    /// <summary>
    /// File element type
    /// </summary>
    public struct StoredResourceInfo
    {
        /// <summary>
        /// Struct constructor
        /// </summary>
        /// <param name="elName"></param>
        /// <param name="elType"></param>
        public StoredResourceInfo(string elName, StoredResourceType elType, int inDepth)
        {
            elementName = elName;
            elementType = elType;
            depth = inDepth;
        }

        /// <summary>
        /// Resource name
        /// </summary>
        public string elementName;

        /// <summary>
        /// Resource type
        /// </summary>
        public StoredResourceType elementType;

        /// <summary>
        /// The depth within the file structure that the resource was found
        /// </summary>
        /// <remarks>Starts at 1</remarks>
        public int depth;
    }

    public class FileManager
    {

        private readonly string _applicationName;

        public FileManager()
        {
            _applicationName = ConfigurationManager.AppSettings["ApplicationName"];
        }

        string CleanPath(string path) {
            // Some validators, like RSS 2.0, does not accept '.'s in the url unless its in the file name
            return path.Trim().Replace('.', '_').Replace(' ', '-');
        }

        private Uri GetFullFileUrl(string destinationFolder, string fileName)
        {
            var leftPart = _applicationName + destinationFolder;
            leftPart = CleanPath(leftPart);

            var fullUrl = String.Format("{0}/{1}", leftPart, fileName);
            while (fullUrl.Contains("//")) fullUrl = fullUrl.Replace("//", "/");

            return new Uri(fullUrl, UriKind.RelativeOrAbsolute);
        }

        private string GetPartitionKey(string fileUrl)
        {
            return new StringUtilities().ConvertToHex(new Uri(fileUrl, UriKind.RelativeOrAbsolute).AbsolutePath);
        }

        public void AddFileInfoToTableStorage(string fileUrl, FileEntity fileEntity)
        {
            var datasource = new AzureTableStorageDataSource();
            var partitionKey = GetPartitionKey(fileUrl);

            bool fileExists;

            try {
                fileExists = datasource.FileExists(partitionKey, _applicationName);
            } catch (DataServiceQueryException) {
                fileExists = false;
            }

            if (fileExists)
            {
                var exisitingFile = datasource.GetFile(partitionKey, _applicationName);
                datasource.Delete(exisitingFile);
            }

            datasource.Insert(fileEntity);            
        }

        CloudBlobClient GetBlobClient()
        {
            var storageAccount = CloudStorageAccount.FromConfigurationSetting("DataConnectionString");
            return storageAccount.CreateCloudBlobClient();            
        }

        CloudBlobContainer GetBlobContainer(string containerAddress = "publicfiles") {
            return GetBlobClient().GetContainerReference(containerAddress);
        }

        public CloudBlob UploadToBackupStorage(string filename, string path, string source) {

            var storageAccount = CloudStorageAccount.FromConfigurationSetting("DataConnectionString");

            var container = storageAccount
                .CreateCloudBlobClient()
                .GetContainerReference("backup");

            var blob = container.GetBlobReference(filename);

            // Get shared access signature
            var sas = blob.GetSharedAccessSignature(new SharedAccessPolicy {
                Permissions = SharedAccessPermissions.Read
                                | SharedAccessPermissions.Write,
                SharedAccessExpiryTime = DateTime.UtcNow + TimeSpan.FromMinutes(5)
            });

            var sasCreds = new StorageCredentialsSharedAccessSignature(sas);

            var sasBlob = new CloudBlobClient(storageAccount.BlobEndpoint, sasCreds).GetBlobReference("backup/" + filename);

            sasBlob.UploadText(source);

            return blob;
        }
        
        public CloudBlob UploadFileToBlobStorage(string file, string path, Stream source) {
            var cleanPath = CleanPath(path);

            // Make a unique blob name
            var fileName = Path.GetFileName(file);

            // Create the Blob and upload the file
            var blobAddressUri = GetFullFileUrl(cleanPath, fileName);

            var blob = GetBlobContainer().GetBlobReference(blobAddressUri.ToString());
            blob.UploadFromStream(source);

            return blob;
        }

        private FileEntity CreateFileEntity(string fileUrl, string description) {
            return new FileEntity {
                PartitionKey = GetPartitionKey(fileUrl),
                RowKey = String.Empty,
                ApplicationName = _applicationName,
                Description = description,
                Url = new CdnUtilities().GetCdnUrl(fileUrl)
            };
        }

        public CloudBlob AddFile(string file, string path, Stream source, string description)
        {
            var blob = UploadFileToBlobStorage(file, path, source);

            var fileEntity = CreateFileEntity(blob.Uri.ToString(), description);

            AddFileInfoToTableStorage(blob.Uri.ToString(), fileEntity);

            return blob;
        }

        public void SetFileMetaData(CloudBlob blob, Dictionary<string, string> metaData, string contentType)
        {
            foreach (var keyValuePair in metaData.Where(keyValuePair => !String.IsNullOrWhiteSpace(keyValuePair.Value)))
                blob.Metadata[keyValuePair.Key] = keyValuePair.Value;

            blob.SetMetadata();

            // Set the properties
            blob.Properties.ContentType = contentType;
            blob.SetProperties();
        }

        void DeleteFileFromTableStorage(string fileUrl)
        {
            var datasource = new AzureTableStorageDataSource();
            var partitionKey = GetPartitionKey(fileUrl);

            while (datasource.FileExists(partitionKey, _applicationName))
            {
                var file = datasource.GetFile(partitionKey, _applicationName);
                datasource.Delete(file);
            }                
        }

        void DeleteFileFromBlob(string fileUrl, bool deleteSnapshots)
        {
            var blob = GetBlobContainer().GetBlobReference(fileUrl);
            if (deleteSnapshots)
            {
                var options = new BlobRequestOptions { DeleteSnapshotsOption = DeleteSnapshotsOption.IncludeSnapshots };
                blob.DeleteIfExists(options);
            }
            else blob.DeleteIfExists();
        }

        public void DeleteFile(string fileUrl, bool deleteSnapshots = true)
        {
            DeleteFileFromTableStorage(fileUrl);

            DeleteFileFromBlob(fileUrl, deleteSnapshots);
        }

        /// <summary>
        /// Returns the number of bytes in use
        /// </summary>
        /// <returns></returns>
        public long GetStorageUsageByteSize()
        {
            var blobClient = GetBlobClient();
            return (from container in blobClient.ListContainers()
                              select
                              (from CloudBlob blob in
                                   container.ListBlobs(new BlobRequestOptions { UseFlatBlobListing = true })
                               select blob.Properties.Length
                              ).Sum()
                             ).Sum();            
        }

        public long GetFileSize(string fileUrl)
        {
            var blob = GetBlobContainer().GetBlobReference(fileUrl);
            blob.FetchAttributes();

            return blob.Properties.Length;
        }

        /// <summary>
        /// Get a matching FontAwesome icon for the file type
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <returns>FontAwesome icon</returns>
        public string GetFileIcon(string fileName)
        {
            var suffixPos = fileName.IndexOf('.');
            if (suffixPos == -1) return "fa-file";
            var suffix = fileName.Substring(suffixPos);

            if (Regex.IsMatch(suffix, ConfigurationManager.AppSettings["AudioFilesRegEx"], RegexOptions.IgnoreCase))
                return "fa-file-audio-o";

            if (Regex.IsMatch(suffix, ConfigurationManager.AppSettings["CodeFilesRegEx"], RegexOptions.IgnoreCase))
                return "fa-file-code-o";

            if (Regex.IsMatch(suffix, ConfigurationManager.AppSettings["ExcelFilesRegEx"], RegexOptions.IgnoreCase))
                return "fa-file-excel-o";

            if (Regex.IsMatch(suffix, ConfigurationManager.AppSettings["ImageFilesRegEx"], RegexOptions.IgnoreCase))
                return "fa-file-image-o";

            if (Regex.IsMatch(suffix, ConfigurationManager.AppSettings["MovieFilesRegEx"], RegexOptions.IgnoreCase))
                return "fa-file-movie-o";

            if (Regex.IsMatch(suffix, ConfigurationManager.AppSettings["PdfFilesRegEx"], RegexOptions.IgnoreCase))
                return "fa-file-pdf-o";

            if (Regex.IsMatch(suffix, ConfigurationManager.AppSettings["PowerpointFilesRegEx"], RegexOptions.IgnoreCase))
                return "fa-file-powerpoint-o";

            if (Regex.IsMatch(suffix, ConfigurationManager.AppSettings["TextFilesRegEx"], RegexOptions.IgnoreCase))
                return "fa-file-text-o";

            if (Regex.IsMatch(suffix, ConfigurationManager.AppSettings["WordFilesRegEx"], RegexOptions.IgnoreCase))
                return "fa-file-word-o";

            if (Regex.IsMatch(suffix, ConfigurationManager.AppSettings["ZipFilesRegEx"], RegexOptions.IgnoreCase))
                return "fa-file-zip-o";

            if (Regex.IsMatch(suffix, ConfigurationManager.AppSettings["TextFilesRegEx"], RegexOptions.IgnoreCase))
                return "fa-file-text-o";

            if (Regex.IsMatch(suffix, ConfigurationManager.AppSettings["TextFilesRegEx"], RegexOptions.IgnoreCase))
                return "fa-file-text-o";

            return "fa-file-o";
        }

        /// <summary>
        /// Gets the display name of a path
        /// </summary>
        /// <param name="rawText">Path</param>
        /// <returns>Display name of path</returns>
        public string GetDisplayName(string rawText, bool showFullPath = false)
        {
            string displayName;

            if (showFullPath)
            {
                // Show the path
                displayName = rawText;
            }
            else
            {
                // Hide the path
                var lastIdx = rawText.LastIndexOf('\\');
                displayName = lastIdx == -1 ? rawText : rawText.Substring(lastIdx + 1);
            }

            return displayName;
        }

        /// <summary>
        /// Cleans up a directory name
        /// </summary>
        /// <param name="dir">Driectory name to be cleaned</param>
        /// <returns>Cleaned directory name</returns>
        public string CleanDirectoryName(bool removeBasePath, string dir, int basePathLen)
        {
            var dirText = removeBasePath ? dir.Substring(basePathLen) : dir;
            dirText = dirText.Replace('\\', '/');
            dirText.Replace("//", "/");
            if (!dirText.EndsWith("/")) dirText = dirText + '/';
            if (removeBasePath && !dirText.StartsWith("/")) dirText = '/' + dirText;
            return dirText;
        }

        /// <summary>
        /// Do a recursive search to find all files and directories
        /// </summary>
        /// <param name="outResources">All found files and directories are added here</param>
        /// <param name="sDir">Start directory</param>
        /// <param name="removeBasePath">Set to true to strip away the base paths</param>
        /// <param name="basePath">Base path</param>
        /// <param name="directoryMatchRegEx">Directories must match this regular expression</param>
        /// <param name="fileMatchRegEx">Files must match this regular expression</param>
        /// <param name="directoryIgnoreRegEx">Directories that matches this regular expression are ignored</param>
        /// <param name="fileIgnoreRegEx">Directories that matches this regular expression are ignored</param>
        /// <param name="depth">Current recursive depth. Iterates upwards starting at 1.</param>
        public void DirSearch(ref List<StoredResourceInfo> outResources, string sDir, bool removeBasePath, string basePath, 
            string directoryMatchRegEx = null, string fileMatchRegEx = null, string directoryIgnoreRegEx = null, string fileIgnoreRegEx = null, 
            int depth = 1)
        {
            var basePathLen = basePath.Length;

            try
            {
                foreach (var d in Directory.GetDirectories(sDir))
                {
                    // Should we match this directory?
                    if (!String.IsNullOrEmpty(directoryMatchRegEx) && !Regex.IsMatch(d, directoryMatchRegEx, RegexOptions.IgnoreCase))
                        continue;
                    // Should we ignore this?
                    if (!String.IsNullOrEmpty(directoryIgnoreRegEx) && Regex.IsMatch(d, directoryIgnoreRegEx, RegexOptions.IgnoreCase))
                        continue;

                    // Remove the base path if requested
                    var dirText = CleanDirectoryName(removeBasePath, d, basePathLen);

                    outResources.Add(new StoredResourceInfo(dirText, StoredResourceType.Directory, depth));

                    foreach (var f in Directory.GetFiles(d))
                    {
                        // Should we match this?
                        if (!String.IsNullOrEmpty(fileMatchRegEx) && !Regex.IsMatch(f, fileMatchRegEx, RegexOptions.IgnoreCase))
                            continue;
                        // Should we ignore this?
                        if (!String.IsNullOrEmpty(fileIgnoreRegEx) && Regex.IsMatch(f, fileIgnoreRegEx, RegexOptions.IgnoreCase))
                            continue;

                        // Remove the base path if requested
                        var fileText = removeBasePath ? f.Substring(basePathLen) : f;

                        outResources.Add(new StoredResourceInfo(fileText, StoredResourceType.File, depth));
                    }

                    // Call the sub directory
                    DirSearch(ref outResources, d, removeBasePath, basePath, directoryMatchRegEx, fileMatchRegEx, directoryIgnoreRegEx,
                        fileIgnoreRegEx, depth + 1);
                }
            }
            catch (Exception)
            {
                // Skip directories/files where don't have access
            }
        }

    }
}