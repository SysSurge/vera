<%@ Page Title="Cloud commands" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CloudCommand.aspx.cs" Inherits="VeraWAF.WebPages.Cloud.CloudCommand" %>
<%@ Register src="~/Controls/FormNotification.ascx" tagname="FormNotification" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="AsideContent" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MainContent" runat="server">
    <div class="commandPage">
        <header>
            <hgroup>
                <h1><i class="fa fa-rocket"></i> Cloud commands page</h1>
            </hgroup>
        </header>

        <uc1:FormNotification id="notifications" runat="server"></uc1:FormNotification>

        <p class="commandIngress">
            Select a cloud command:
        </p>
        <div class="commands">
            <section>
                <p /><a href="?command=ClearHostingEnvirionmentCache">Clear hosting environment cache</a>
                <p /><a href="?command=ClearPageCache">Clear page cache</a>
                <p /><a href="?command=ClearUserCache">Clear user cache</a>
                <p /><a href="?command=ForumPageCRUD">Call after forum page CRUD operation</a>
                <p /><a href="?command=FreeAllCaches">Clear all caches</a>
                <p /><a href="?command=PageCRUD">Call after page CRUD operation</a>
                <p /><a href="?command=RebuildSearchIndex">Rebuild search index</a>
                <p /><a href="?command=RebuildXmlSitemapFile">Rebuild sitemap.xml file</a>
                <p /><a href="?command=RestartServer">Restart cloud servers</a>
                <p /><a href="?command=ReloadSitemap">Reload sitemap</a>
                <p /><a href="?command=TouchVirtualPageCacheDependencyFiles">Touch virtual page cache dependency files</a>
                <p /><a href="?command=DeleteVirtualPageCacheDependencyFiles">Delete virtual page cache dependency files</a>
                <p /><a href="?command=VoteCRUD">Call after vote CRUD operation</a>
            </section>
        </div>
   </div>
</asp:Content>
