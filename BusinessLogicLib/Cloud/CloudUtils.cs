using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using System;
using System.Collections.Generic;
using System.ServiceModel.Activation;

namespace VeraWAF.WebPages.Bll.Cloud
{
    /// <summary>
    /// Cloud utility class
    /// </summary>
    public class CloudUtils
    {
        /// <summary>
        /// Returns a list of all the nodes in the cloud
        /// </summary>
        /// <returns>Array containing all the IP addresses of the nodes endponts in the cloud</returns>
        public List<string> GetNodes()
        {
            var allNodes = new List<string>();

            foreach (RoleInstance roleInst in RoleEnvironment.CurrentRoleInstance.Role.Instances)
            {
                if (roleInst.Role.Name == "Www")
                {
                    foreach (RoleInstanceEndpoint roleInstEndpoint in roleInst.InstanceEndpoints.Values)
                    {
                        // Get endpoint address using the internal endpoint's IP address
                        if (roleInstEndpoint.Protocol == "http")
                            allNodes.Add(roleInstEndpoint.IPEndpoint.Address.ToString() + ":" + roleInstEndpoint.IPEndpoint.Port);
                    }
                }
            }

            return allNodes;
        }

        /// <summary>
        /// Creates a Shared Access Signature (SAS) that will allow the user to download a file from the Azure blob
        /// or list resources.
        /// </summary>
        /// <param name="containerAddress">Blob container address. Ex. "publicfiles"</param>
        /// <returns>Download URL with a Shared Access Signature</returns>
        /// <remarks>
        /// The SAS is valid for one hour and will not work afterwards. Downloads that exceed the one hour window
        /// are ok as long as they start downloading before the window is shut.
        /// 
        /// It is usually not required to call this function to access resources on public blob containers like 
        /// "publicfiles".
        /// </remarks>
        public string GetBlobDownloaddUrl(string containerAddress, string folder)
        {
            // Setup the connection to Windows Azure Storage
            var storageAccount = CloudStorageAccount.FromConfigurationSetting("DataConnectionString");
            var blobClient = storageAccount.CreateCloudBlobClient();

            // Get and create the container
            var blobContainer = blobClient.GetContainerReference(containerAddress);

            /*
             * Create a Shared Access Signature that will allow the user to download a file to the Azure blob.
             */

            // Set the allowed window in number of minutes
            const int allowedWindowMinutes = 60;

            // Create the SAS
            var sasWithIdentifier = blobContainer.GetSharedAccessSignature(
                    new SharedAccessPolicy
                    {
                        Permissions = SharedAccessPermissions.Read | SharedAccessPermissions.List,
                        SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(allowedWindowMinutes)
                    }
                );

            return blobContainer.Uri.AbsoluteUri + "/" + folder + sasWithIdentifier;
        }

        /// <summary>
        /// Creates a Shared Access Signature (SAS) that will allow the user to upload a resource to the Azure blob
        /// or delete a resource.
        /// </summary>
        /// <param name="containerAddress">Blob container address. Ex. "publicfiles"</param>
        /// <returns>Upload URL with a Shared Access Signature</returns>
        /// <remarks>
        /// The SAS is valid for one hour and will not work afterwards. Uploads that exceed the one hour window
        /// are ok as long as they start uploading before the window is shut.
        /// </remarks>
        public string GetBlobUploadUrl(string containerAddress, string folder)
        {
            // Setup the connection to Windows Azure Storage
            var storageAccount = CloudStorageAccount.FromConfigurationSetting("DataConnectionString");
            var blobClient = storageAccount.CreateCloudBlobClient();

            // Get and create the container
            var blobContainer = blobClient.GetContainerReference(containerAddress);

            /*
             * Create a Shared Access Signature that will allow the user to upload a file to the Azure blob.
             */

            // Set the allowed window in number of minutes
            const int allowedWindowMinutes = 60;

            // Create the SAS
            var sasWithIdentifier = blobContainer.GetSharedAccessSignature(
                    new SharedAccessPolicy
                    {
                        Permissions = SharedAccessPermissions.Write | SharedAccessPermissions.Delete,
                        SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(allowedWindowMinutes)
                    }
                );

            return blobContainer.Uri.AbsoluteUri + "/" + folder + sasWithIdentifier;
        }

        /// <summary>
        /// Creates a new Azure Blob Container called $root and creates a clientaccesspolicy.xml
        /// file in it. A clientaccesspolicy.xml file is required by Silverlight for any kind of
        /// I/O agains a REST API like the Azure Blob Storage.
        /// </summary>
        /// <param name="blobs">Azure Blob client</param>
        /// <remarks>
        /// The developer environments Visual Studio Azure Storage Emulator there is no manner to
        /// access the required root directory of the Blob Storage. Hense, if you wish to use 
        /// Silverlight REST API functionality against the Azure Storage Emulator you must download
        /// Fiddler and add a custom rule on the bottom of its OnBeforeRequest() function:
        /// 
        /// if (oSession.url == "127.0.0.1:10000/clientaccesspolicy.xml") { oSession.url = "127.0.0.1:10000/devstoreaccount1/clientaccesspolicy.xml"; }
        /// 
        /// This will allow you to use Silverlight against the Azure Storage Emulator like it was
        /// in the actual Azure Cloud, and causes Fiddler to redirect all clientaccesspolicy.xml
        /// access to where the file is actually stored. You must have Fiddler running to make this
        /// work.
        /// </remarks>
        public void CreateSilverlightPolicy(CloudBlobClient blobs)
        {
            blobs.GetContainerReference("$root").CreateIfNotExist();
            blobs.GetContainerReference("$root").SetPermissions(
                new BlobContainerPermissions()
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                });
            var blob = blobs.GetBlobReference("clientaccesspolicy.xml");
            blob.Properties.ContentType = "text/xml";
            blob.UploadText(Resources.Solution.clientaccesspolicy_xml);
        }

        /// <summary>
        /// Returns a list of all the blob containers in the Azure Blob Storage
        /// </summary>
        /// <param name="blobs">Azure Blob client</param>
        /// <returns>Array containing the name and url's of the blob containers in the Azure Blob Storage</returns>
        public Dictionary<string, string> GetBlobContainers(CloudBlobClient blobs)
        {
            var allBlobContainers = new Dictionary<string, string>();

            foreach (var blobContainer in blobs.ListContainers())
                allBlobContainers.Add(blobContainer.Name, blobContainer.Uri.AbsoluteUri);

            return allBlobContainers;
        }
    }
}
