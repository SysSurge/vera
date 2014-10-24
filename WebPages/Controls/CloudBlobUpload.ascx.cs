using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using VeraWAF.Core.Templates;
using VeraWAF.WebPages.Bll;
using VeraWAF.WebPages.Bll.Cloud;

namespace VeraWAF.WebPages.Controls
{
    /// <summary>
    /// Allows the user to upload files to the Azure Blob Storage
    /// </summary>
    public partial class CloudBlobUpload : UserControl
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // Create a Azure Blob upload url with Shared Access Signature
            var sasUploadUrl = new CloudUtils().GetBlobUploadUrl("publicfiles", Request["folder"]);
            UploadFiles.Attributes.Add("uploadUrl", sasUploadUrl);
        }
    }
}