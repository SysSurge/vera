<%@ Page Title="Latest news & press releases" Language="C#" MasterPageFile="~/Sub.Master" AutoEventWireup="true" CodeBehind="NewsSection.aspx.cs" Inherits="VeraWAF.WebPages.Templates.NewsSection" EnableViewState="false" %>
<%@ Register src="~/Controls/Articles.ascx" tagname="Articles" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link ID="metaMade" rel="made" href="mailto:info@example.com" runat="server" />
    <meta ID="metaRdfaCreated" property="dc:date dc:created" content="2010-11-11T13:00:00" runat="server" />
    <meta ID="metaRdfaModified" property="dc:date dc:modified" content="2010-11-11T13:00:00" runat="server" />
    <meta ID="metaRdfaCreator" property="dc:creator" content="Admin" runat="server" />
    <meta ID="metaDescription" name="description" content="Description" runat="server" />
    <meta ID="metaAuthor" name="author" content="Admin" runat="server" />
    <asp:Literal ID="litRdfaSubjects" runat="server"></asp:Literal>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeaderContent" runat="server" EnableViewState="false">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="AsideContent" runat="server" EnableViewState="false">
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="TitleContent" runat="server" EnableViewState="false">
    <header>
        <hgroup>
            <h1 property="dc:title"><asp:Literal ID="litTitle" runat="server"></asp:Literal></h1>
            <h2><asp:Literal ID="litIngress" runat="server"></asp:Literal></h2>
        </hgroup>
        <div class="pageHeaderFigure clear">
            <asp:Literal ID="litFigure" runat="server"></asp:Literal>
        </div>
    </header>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="MainContent" runat="server">
    <div class="maintext">
        <asp:Literal ID="litBody" runat="server"></asp:Literal>
    </div>
    <p>
        <uc1:Articles runat="server" Mode="1"></uc1:Articles>
    </p>
</asp:Content>
