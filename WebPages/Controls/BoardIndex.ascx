<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BoardIndex.ascx.cs" Inherits="VeraWAF.WebPages.Controls.BoardIndex" %>
<link href="/Styles/board.css" rel="stylesheet" type="text/css" />
<nav>
    <div class="boardIndexMenu">
        <asp:SiteMapDataSource runat="server" ID="siteMapDataSource" ShowStartingNode="false"  />
        <asp:Menu DataSourceID="siteMapDataSource" ID="NavigationMenu" StaticDisplayLevels="4" runat="server" 
            StaticMenuItemStyle-ItemSpacing="3" DynamicHorizontalOffset="30"
            SkipLinkText="" CssClass="menu" StaticSelectedStyle-CssClass="menuitem-selected" IncludeStyleBlock="false" 
            Orientation="Horizontal" MaximumDynamicDisplayLevels="0" StaticSubMenuIndent="40" ShowExpandCollapse="False">
        </asp:Menu>
    </div>
</nav>
