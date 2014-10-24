using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Web.Security;
using System.Web.UI;
using VeraWAF.WebPages.Bll;

namespace VeraWAF.WebPages.Controls
{
    public partial class ImageUpload : UserControl
    {
        private readonly FileManager _fileManager;

        public string Destination { 
            get { return txtDestination.Text; } set { txtDestination.Text = value; } 
        }

        public ImageUpload()
        {
            _fileManager = new FileManager();
        }

        public string ImageUrl
        {
            get { return FigureImage.ImageUrl; }
            set { lnkFigureUrl.NavigateUrl = lnkFigureUrl.Text = FigureImage.ImageUrl = value; }
        }

        public string Caption
        {
            get { return txtFigureCaption.Text; }
            set { txtFigureCaption.Text = value; }
        }

        public int MaxWidth
        {
            get { return int.Parse(txtMaxWidth.Text); }
            set { txtMaxWidth.Text = value.ToString(CultureInfo.InvariantCulture); }
        }

        public int MaxHeight {
            get { return int.Parse(txtMaxHeight.Text); }
            set { txtMaxHeight.Text = value.ToString(CultureInfo.InvariantCulture); }
        }

        public bool ScaleDownOnly
        {
            get { return chkScaleDownOnly.Checked; }
            set { chkScaleDownOnly.Checked = value; }
        }

        public bool KeepAspectRatio {
            get { return chkKeepAspectRatio.Checked; }
            set { chkKeepAspectRatio.Checked = value; }
        }

        public bool ConvertToJpeg {
            get { return chkConvertToJpeg.Checked; }
            set { chkConvertToJpeg.Checked = value; }
        }

        public bool ShowCaption
        {
            set { figureCaptionContainer.Visible = lblFigureCaption.Visible = txtFigureCaption.Visible = value; }
        }

        void GetFigure()
        {
            var blobAddressUri = lnkFigureUrl.NavigateUrl;
            if (String.IsNullOrWhiteSpace(blobAddressUri)) return;

            FigureImage.ImageUrl = blobAddressUri;
            FigureImage.AlternateText = txtFigureCaption.Text;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            GetFigure();
        }

        ImageFormat GetImageFormat(string fileName)
        {
            ImageFormat format;
            var extension = Path.GetExtension(fileName);

            if (!String.IsNullOrWhiteSpace(extension))
            {
                switch (extension.ToLower())
                {
                    case ".gif":
                        format = ImageFormat.Gif;
                        break;
                    case ".png":
                        format = ImageFormat.Png;
                        break;
                    default:
                        format = ImageFormat.Jpeg;
                        break;
                }
            }
            else format = ImageFormat.Jpeg;

            return format;
        }

        Dictionary<string, string> GetMetaData(int width, int height) {
            var userName = Membership.GetUser().UserName;

            return new Dictionary<string, string>
                               {
                                   {"FileName", FigureUploadControl.FileName},
                                   {"Owner", userName},
                                   {"Description", txtFigureCaption.Text},
                                   {"Type", "Figure"},
                                   {"Width", width.ToString(CultureInfo.InvariantCulture)},
                                   {"Height", height.ToString(CultureInfo.InvariantCulture)},
                               };
        }

        string GetFileName()
        {
            var fileName = Path.GetFileName(FigureUploadControl.FileName);
            if (chkConvertToJpeg.Checked)
                fileName = fileName.Replace(Path.GetExtension(fileName), ".jpg");

            return fileName;
        }

        ImageFormat GetFileFormat(string fileName)
        {
            ImageFormat fileFormat;
            if (chkConvertToJpeg.Checked) {
                fileFormat = ImageFormat.Jpeg;
            } else fileFormat = GetImageFormat(fileName);
            return fileFormat;
        }

        protected void FigureSubmitButton_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(FigureUploadControl.FileName)) {

                // Make a unique blob name
                var fileName = GetFileName();
                var fileFormat = GetFileFormat(fileName);

                int newWidth;
                int newHeight;

                var graphics = new GraphicUtilities();

                var cloudBlob = _fileManager.AddFile(
                    fileName, 
                    txtDestination.Text, 
                    graphics.ResizeImage(FigureUploadControl.FileContent, chkKeepAspectRatio.Checked,
                        int.Parse(txtMaxWidth.Text), int.Parse(txtMaxHeight.Text),
                        out newWidth, out newHeight, fileFormat,
                        int.Parse(txtJpegCompression.Text), chkScaleDownOnly.Checked), 
                    txtFigureCaption.Text);

                var metaData = GetMetaData(newWidth, newHeight);
                _fileManager.SetFileMetaData(cloudBlob, metaData, FigureUploadControl.PostedFile.ContentType);

                lnkFigureUrl.NavigateUrl = lnkFigureUrl.Text = cloudBlob.Uri.ToString();
            }

            // Show the Figure
            GetFigure();
        }

        protected void ClearButton_Click(object sender, EventArgs e)
        {
            lnkFigureUrl.NavigateUrl = lnkFigureUrl.Text = FigureImage.ImageUrl = Caption = String.Empty;
        }
    }
}