<%@ Page Title="Site Map" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" %>
<%@ Register TagPrefix="uc1" TagName="TreeViewMenu" Src="~/Controls/TreeViewMenu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="AsideContent" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MainContent" runat="server">
    <div class="sitemapPage">
        <header>
            <hgroup>
                <h1>Site map</h1>
            </hgroup>
        </header>
        <main>
            <p>
                <uc1:TreeViewMenu ID="mnuTreeview" ExpandDepth="10" runat="server" ViewStateMode="Disabled"></uc1:TreeViewMenu>
            </p>
        </main>
   </div>
</asp:Content>
