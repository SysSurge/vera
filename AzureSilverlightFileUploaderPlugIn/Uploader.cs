using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Browser;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace AzureSilverlightFileUploaderPlugIn
{
    /// <summary>
    /// File uploader logic
    /// </summary>
    public class Uploader : IFileUploader
    {
        /// <summary>
        /// File
        /// </summary>
        readonly UserFile File;

        /// <summary>
        /// Number of bytes in total
        /// </summary>
        readonly long DataLength;

        /// <summary>
        /// Number of bytes uploaded
        /// </summary>
        long DataSent;

        /// <summary>
        /// Chunk size in number of bytes.
        /// File is uploaded in chunks.
        /// </summary>
        const long CHUNK_SIZE = 4194304;

        /// <summary>
        /// Upload URL
        /// </summary>
        readonly string UploadUrl;

        /// <summary>
        /// If thru then chunking is used in the upload process
        /// </summary>
        readonly bool UseBlocks;

        /// <summary>
        /// Current chunk identifier
        /// </summary>
        string CurrentBlockId;

        /// <summary>
        /// Chunks
        /// </summary>
        readonly List<string> BlockIds = new List<string>();

        HttpWebRequest WebRequest;
        
        bool AbortRequested;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="file">File object</param>
        /// <param name="uploadContainerUrl">Upload url</param>
        public Uploader(UserFile file, string uploadContainerUrl)
        {
            File = file;

            DataLength = File.FileStream.Length;
            DataSent = 0;

            // upload the blob in smaller blocks if it's a "big" file
            UseBlocks = DataLength > CHUNK_SIZE;

            // uploadContainerUrl has a Shared Access Signature already
            var uriBuilder = new UriBuilder(uploadContainerUrl);
            uriBuilder.Path += String.Format("/{0}", File.FileName);
            UploadUrl = uriBuilder.Uri.AbsoluteUri;
        }

        /// <summary>
        /// Cancel the file upload
        /// </summary>
        public void CancelUpload()
        {
            AbortRequested = true;
            if (WebRequest != null) WebRequest.Abort();
        }

        public event EventHandler UploadFinished;

        /// <summary>
        /// Event that is fired when a HTTP response is recieved from the server during an upload process
        /// </summary>
        /// <param name="asynchronousResult"></param>
        void BlockListReadHttpResponseCallback(IAsyncResult asynchronousResult)
        {
            var error = false;

            try
            {
                var webRequest = (HttpWebRequest)asynchronousResult.AsyncState;
                var webResponse = (HttpWebResponse)webRequest.EndGetResponse(asynchronousResult);
                var reader = new StreamReader(webResponse.GetResponseStream());

                reader.ReadToEnd();
                reader.Close();
            }
            catch
            {
                error = true;

                File.UiDispatcher.BeginInvoke(
                    delegate
                    {
                        File.State = Constants.FileStates.Error;
                    }
                );
            }

            if (!error)
                File.UiDispatcher.BeginInvoke(
                    delegate
                    {
                        if (UploadFinished != null)
                            UploadFinished(this, null);
                    }
                );
        }

        /// <summary>
        /// Callback function when uploading
        /// </summary>
        /// <param name="asynchronousResult"></param>
        void BlockListWriteToStreamCallback(IAsyncResult asynchronousResult)
        {
            var webRequest = (HttpWebRequest)asynchronousResult.AsyncState;
            var requestStream = webRequest.EndGetRequestStream(asynchronousResult);

            var document = new XDocument(
                new XElement("BlockList",
                    from blockId in BlockIds
                    select new XElement("Uncommitted", blockId)));

            var writer = XmlWriter.Create(requestStream, new XmlWriterSettings { Encoding = Encoding.UTF8 });
            document.Save(writer);
            writer.Flush();

            requestStream.Close();

            if (!AbortRequested) webRequest.BeginGetResponse(BlockListReadHttpResponseCallback, webRequest);
        }

        /// <summary>
        /// Adds a a PUT block list to the HTTP request
        /// </summary>
        void PutBlockList()
        {
            var webRequest = (HttpWebRequest)WebRequestCreator.ClientHttp.Create(
                new Uri(string.Format("{0}&comp=blocklist", UploadUrl)));

            webRequest.Method = "PUT";
            webRequest.Headers["x-ms-version"] = "2009-09-19"; // x-ms-version is required for put block list!
            webRequest.BeginGetRequestStream(BlockListWriteToStreamCallback, webRequest);
        }

        /// <summary>
        /// Callback function for HTTP responses
        /// </summary>
        /// <param name="asynchronousResult"></param>
        void ReadHttpResponseCallback(IAsyncResult asynchronousResult)
        {
            var error = false;

            try
            {
                WebRequest = (HttpWebRequest)asynchronousResult.AsyncState;
                var webResponse = (HttpWebResponse)WebRequest.EndGetResponse(asynchronousResult);
                var reader = new StreamReader(webResponse.GetResponseStream());
                reader.ReadToEnd();
                reader.Close();
            }
            catch (Exception ex)
            {
                error = true;

                File.UiDispatcher.BeginInvoke(
                    delegate
                    {
                        File.State = Constants.FileStates.Error;
                    }
                );
            }

            if (!error) BlockIds.Add(CurrentBlockId);

            // if there's more data, send another request
            if (DataSent < DataLength)
            {
                if (File.State != Constants.FileStates.Error && !error) StartUpload();
            }
            else // all done
            {
                File.FileStream.Close();
                File.FileStream.Dispose();

                if (UseBlocks) PutBlockList(); // commit the blocks into the blob
                else File.UiDispatcher.BeginInvoke(
                    delegate
                    {
                        if (UploadFinished != null) UploadFinished(this, null);
                    }
                );
            }
        }

        /// <summary>
        /// Event that is fired when the upload progress changess
        /// </summary>
        void OnProgressChanged()
        {
            File.BytesUploaded = DataSent;
        }

        /// <summary>
        /// Write up to ChunkSize of data to the web request
        /// </summary>
        /// <param name="asynchronousResult"></param>
        void WriteToStreamCallback(IAsyncResult asynchronousResult)
        {
            var webRequest = (HttpWebRequest)asynchronousResult.AsyncState;
            var requestStream = webRequest.EndGetRequestStream(asynchronousResult);
            var buffer = new Byte[4096];
            int bytesRead;
            var tempTotal = 0;

            File.FileStream.Position = DataSent;

            while ((bytesRead = File.FileStream.Read(buffer, 0, buffer.Length)) != 0
                && tempTotal + bytesRead < CHUNK_SIZE 
                && !File.IsDeleted 
                && File.State != Constants.FileStates.Error)
            {
                requestStream.Write(buffer, 0, bytesRead);
                requestStream.Flush();

                DataSent += bytesRead;
                tempTotal += bytesRead;

                File.UiDispatcher.BeginInvoke(OnProgressChanged);
            }

            requestStream.Close();

            if (!AbortRequested) webRequest.BeginGetResponse(ReadHttpResponseCallback, webRequest);
        }

        /// <summary>
        /// Start the upload
        /// </summary>
        void StartUpload()
        {
            var uriBuilder = new UriBuilder(UploadUrl);

            if (UseBlocks)
            {
                // encode the block name and add it to the query string
                CurrentBlockId = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()));
                uriBuilder.Query = uriBuilder.Query.TrimStart('?') + string.Format("&comp=block&blockid={0}", CurrentBlockId);
            }

            // with or without using blocks, we'll make a PUT request with the data
            var webRequest = (HttpWebRequest)WebRequestCreator.ClientHttp.Create(uriBuilder.Uri);
            webRequest.Method = "PUT";
            webRequest.BeginGetRequestStream(WriteToStreamCallback, webRequest);
        }

        /// <summary>
        /// Start the file upload
        /// </summary>
        /// <param name="initParams"></param>
        public void StartUpload(string initParams)
        {
            StartUpload();
        }      
    }
}
