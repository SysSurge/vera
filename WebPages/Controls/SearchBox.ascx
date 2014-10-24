<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchBox.ascx.cs" Inherits="VeraWAF.WebPages.Controls.SearchBox" EnableViewState="false" %>
<div id="searchBox" class="searchBox">
    <div id="helpText">Search example.com</div>
    <div id="searchBoxBackground"></div>
    <asp:TextBox ID="txtSearchbox" CssClass="searchBox" runat="server" spellcheck="false" Text="" accesskey="s" autocomplete="off"></asp:TextBox>
    <div id="searchIcon" class="fa fa-search fa-fw searchIcon" title="Click to sumbit search"></div>
    <div id="clearIcon" class="searchIcon rotate-45 fa fa-plus-circle" title="Clear text"></div>
    <div id="loadingIcon" class="searchIcon fa fa-refresh fa-spin" title="Searching, please wait..."></div>
    <div id="searchResult"></div>
    <noscript>
        <asp:Button ID="butSearch" runat="server" OnClick="butSearch_Click" Text="Search" CssClass="searchBoxSubmit" />
    </noscript>
</div>