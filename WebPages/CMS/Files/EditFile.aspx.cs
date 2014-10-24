using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Security;
using System.Web.UI;
using VeraWAF.AzureTableStorage;
using VeraWAF.Core.Templates;
using VeraWAF.WebPages.Bll;
using VeraWAF.CrossCuttingConcerns;

namespace VeraWAF.WebPages.Edit.Files
{
    public partial class EditFile : PageTemplateBase
    {
        private readonly string _applicationName;
        private string _partitionKey;
        private readonly FileManager _fileManager;

        public EditFile()
        {
            _applicationName = ConfigurationManager.AppSettings["ApplicationName"];
            _fileManager = new FileManager();
        }

        private void PopulateFormFieldsFromFileEntityData(FileEntity file)
        {
            txtDescription.Text = file.Description;
            lnkUrl.NavigateUrl = lnkUrl.Text = file.Url;
            txtDestination.Text =
                new Uri(file.Url.Substring(0, file.Url.LastIndexOf('/')), UriKind.RelativeOrAbsolute).AbsolutePath;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _partitionKey = Request["file"];
            if (String.IsNullOrWhiteSpace(_partitionKey)) return;

            var datasource = new AzureTableStorageDataSource();

            var file = datasource.GetFile(_partitionKey, _applicationName);
            if (file != null && !Page.IsPostBack && !String.IsNullOrEmpty(_partitionKey))
                PopulateFormFieldsFromFileEntityData(file);
        }

        private string GetPartitionKey(string fileUrl)
        {
            return string.IsNullOrWhiteSpace(_partitionKey)
                       ? new StringUtilities().ConvertToHex(new Uri(fileUrl, UriKind.RelativeOrAbsolute).AbsolutePath)
                       : _partitionKey;
        }

        private FileEntity CreateFileEntityFromFormData()
        {
            return new FileEntity
                       {
                           PartitionKey = GetPartitionKey(lnkUrl.NavigateUrl),
                           RowKey = String.Empty,
                           ApplicationName = _applicationName,
                           Description = txtDescription.Text,
                           Url = lnkUrl.NavigateUrl
                       };
        }

        private void SaveFile()
        {
            var fileEntity = CreateFileEntityFromFormData();
            _fileManager.AddFileInfoToTableStorage(lnkUrl.NavigateUrl, fileEntity);
        }

        protected void butSave_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(lnkUrl.NavigateUrl))
                SaveFile();
        }

        private void ClearForm()
        {
            Response.Redirect("/CMS/Files/EditFile.aspx", true);
        }

        protected void butClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        protected void butDelete_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(lnkUrl.NavigateUrl))
                _fileManager.DeleteFile(lnkUrl.NavigateUrl);

            ClearForm();
        }

        Dictionary<string, string> GetMetaData()
        {
            var userName = Membership.GetUser().UserName;

            return new Dictionary<string, string>
                               {
                                   {"FileName", FileUploadControl.FileName},
                                   {"Owner", userName},
                                   {"Description", txtDescription.Text}
                               };
        }

        protected void FileSubmitButton_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(FileUploadControl.FileName))
            {
                var cloudBlob = _fileManager.AddFile(FileUploadControl.FileName, txtDestination.Text, FileUploadControl.FileContent, txtDescription.Text);

                var metaData = GetMetaData();
                _fileManager.SetFileMetaData(cloudBlob, metaData, FileUploadControl.PostedFile.ContentType);

                lnkUrl.NavigateUrl = lnkUrl.Text = cloudBlob.Uri.ToString();
            }
        }
    }
}