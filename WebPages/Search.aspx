<%@ Page Title="Search website" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="VeraWAF.WebPages.Search" ValidateRequest="false" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="/Styles/search.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="AsideContent" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MainContent" runat="server">
    <div class="searchPage" property="sc:SearchResultsPage">
        <header>
            <hgroup>
                <h2 ID="h2PageNotFound" class="error" runat="server">The page you wanted to visit was not found. Please try searching for it instead.</h2>
                <h1>Search example.com</h1>
            </hgroup>
        </header>
        <p class="queryIngress">
            Enter a query and click the search button to search example.com.
        </p>
        <p>
            <asp:Label ID="lblSearch" runat="server" AssociatedControlID="txtSearch" CssClass="labelForQueryField" >Enter query terms:</asp:Label>
            <asp:TextBox ID="txtSearch" runat="server" CssClass="queryField"></asp:TextBox>
            <asp:Button ID="butSubmit" runat="server" OnClick="butSubmit_Click" Text="Search" CssClass="queryFieldSubmit" />
            <a class="queryHelp" href="/Help/search-query-syntax.aspx">Query syntax guide</a>
        </p>
        <div class="queryResults">
            <section>
                <p>
                    <asp:Literal ID="litSearchResults" runat="server"></asp:Literal>
                </p>
            </section>
        </div>
   </div>
</asp:Content>
