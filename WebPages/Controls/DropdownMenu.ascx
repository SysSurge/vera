<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" %>
<%@ Register src="~/Controls/SearchBox.ascx" tagname="SearchBox" tagprefix="uc1" %>
<nav id="level1menu">
    <div class="level1MenuContainer">
        
        <div class="level1Menu">
            <asp:SiteMapDataSource runat="server" ID="siteMapDataSource" ShowStartingNode="false" />
            <asp:Menu DataSourceID="siteMapDataSource" ID="NavigationMenu" runat="server" SkipLinkText="" CssClass="menu" 
                StaticSelectedStyle-CssClass="menuitem-selected" IncludeStyleBlock="false" Orientation="Horizontal">
            </asp:Menu>
        </div>

        <uc1:SearchBox runat="server" Width="200px" />

        <div class="rssIconContainer">
            <a class="fa fa-rss fa-fw rssIcon" title="RSS feeds" href="/Feeds.aspx">
<!--[if lt IE 8]>
                <img src="/Images/rss_16x16.png" alt="RSS feeds" />
<![endif]-->
            </a>
        </div>

    </div>
</nav>
