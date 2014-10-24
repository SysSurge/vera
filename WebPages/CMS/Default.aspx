<%@ Page Title="Edit" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="VeraWAF.WebPages.Edit.Default" %>
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
            <i class="fa fa-sitemap"></i> Web Content Management System panel
        </h1>        
    </header>
    <p>Administrative tasks for the content managment system.</p>
    <div class="adminPanel">
        <section>
            <h2>Content pages</h2>
            <p>
                <asp:HyperLink ID="lnkEditArticle" runat="server" NavigateUrl="~/CMS/EditPage.aspx">New page</asp:HyperLink>
            </p>        
            <p>
                <asp:HyperLink ID="lnkViewPages" runat="server" NavigateUrl="~/CMS/ViewPages.aspx">View pages</asp:HyperLink>
            </p>
            <p>
                <asp:HyperLink ID="lnkViewComments" runat="server" NavigateUrl="~/CMS/ViewComments.aspx">View comments</asp:HyperLink>
            </p>
        </section>
        <section>
            <h2>Search</h2>
            <p>
                <asp:LinkButton ID="lnkRebuildSearchIndex" runat="server" Text="Rebuild search index" OnClick="lnkRebuildSearchIndex_Click"></asp:LinkButton>
            </p>
        </section>
        <section>
            <h2>Web application</h2>
            <p>
                <asp:LinkButton ID="lnkRestart" runat="server" Text="Restart web application" OnClick="lnkRestart_Click"></asp:LinkButton>
            </p>
            <p>
                <asp:LinkButton ID="lnkRebuildXmlSitemap" runat="server" Text="Rebuild sitemap.xml file" OnClick="lnkRebuildXmlSitemap_Click"></asp:LinkButton>
            </p>
            <p>
                <asp:LinkButton ID="lnkClearPageCache" runat="server" Text="Clear cache" OnClick="lnkClearPageCache_Click"></asp:LinkButton>
            </p>
        </section>
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
        <section>
            <h2>Marketing</h2>
            <p>
                <asp:HyperLink runat="server" NavigateUrl="~/CMS/SendNewsLetter.aspx">Send newsletter</asp:HyperLink>
            </p>        
            <p>
                <asp:HyperLink runat="server" NavigateUrl="~/CMS/Statistics.aspx">Statistics</asp:HyperLink>
            </p>        
        </section>
    </div>
</asp:Content>
