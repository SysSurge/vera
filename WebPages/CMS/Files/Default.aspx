<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="VeraWAF.WebPages.Edit.Files.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <CompressStyles CompressedFile="edit.min.css" runat="server">
        edit.css
    </CompressStyles>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="AsideContent" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MainContent" runat="server">
    <header>
        <h1>
            <i class="fa fa-cloud-download"></i> File library panel
        </h1>        
    </header>
    <p>Administrative tasks for files.</p>
    <div>
        <section>
            <h2>Files</h2>
            <p>
                <asp:HyperLink runat="server" NavigateUrl="~/CMS/Files/EditFile.aspx">Upload or edit single file</asp:HyperLink>
            </p>        
            <p>
                <asp:HyperLink runat="server" NavigateUrl="~/CMS/Files/UploadMultipleFiles.aspx">Upload multiple and/or large files</asp:HyperLink>
            </p>        
            <p>
                <asp:HyperLink runat="server" NavigateUrl="~/CMS/Files/ViewFiles.aspx">View files</asp:HyperLink>
            </p>
        </section>
    </div>
</asp:Content>
