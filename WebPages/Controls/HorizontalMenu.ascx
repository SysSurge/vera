<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HorizontalMenu.ascx.cs" Inherits="VeraWAF.WebPages.Controls.Level2Menu" %>
<nav class="level2Menu">
    <div class="level2Menu">
        <asp:SiteMapDataSource runat="server" ID="siteMapDataSource" ShowStartingNode="true" />
        <asp:Menu DataSourceID="siteMapDataSource" ID="NavigationMenu" StaticDisplayLevels="2" runat="server" SkipLinkText="" 
            CssClass="menu" StaticSelectedStyle-CssClass="menuitem-selected" IncludeStyleBlock="false" Orientation="Horizontal" 
            MaximumDynamicDisplayLevels="0">
        </asp:Menu>
    </div>
</nav>
