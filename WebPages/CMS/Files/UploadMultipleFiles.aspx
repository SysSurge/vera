<%@ Page Title="Upload multiple or large files" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UploadMultipleFiles.aspx.cs" Inherits="VeraWAF.WebPages.Edit.Files.UploadMultipleFiles" ValidateRequest="false" %>
<%@ Register src="~/Controls/CloudBlobUpload.ascx" tagname="CloudBlobUpload" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <CompressStyles CompressedFile="editfile.min.css" runat="server">
        cloudBlobUpload.css
    </CompressStyles>

    <CompressScripts CompressedFile="editfile.min.js" runat="server">
        Silverlight.js
        pl.silverlightEvents.js
        pl.cloudBlobUpload.js
    </CompressScripts>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="AsideContent" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        <i class="fa fa-cloud-upload"></i> Upload multiple and/or large files
    </h1>
    <p>
        Upload large and/or multiple files to the Azure Blob storage. 
        You can drag'n drop files into the box icon or click the icon to open a file explorer dialog. 
        Maximum file size is 1 TB. 
    </p>
    <fieldset>
        <legend>File uploader</legend>
        <uc1:CloudBlobUpload ID="CloudBlobUpload" runat="server"></uc1:CloudBlobUpload>
    </fieldset>
</asp:Content>
