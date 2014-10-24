<%@ Page Language="C#" MasterPageFile="~/Sub.Master" AutoEventWireup="true" CodeBehind="NewsPage.aspx.cs" Inherits="VeraWAF.WebPages.Templates.NewsPage" %>
<%@ Register TagPrefix="uc1" TagName="Share" Src="~/Controls/Share.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Map" Src="~/Controls/Map.ascx" %>
<%@ Register TagPrefix="uc1" TagName="ContactUser" Src="~/Controls/ContactUser.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Comments" Src="~/Controls/Comments.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server" EnableViewState="false">
    <link ID="metaMade" rel="made" href="mailto:info@example.com" runat="server" enableviewstate="false" />
    <meta ID="metaRdfaCreated" property="dc:date dc:created" content="2010-11-11T13:00:00" runat="server" enableviewstate="false" />
    <meta ID="metaRdfaModified" property="dc:date dc:modified" content="2010-11-11T13:00:00" runat="server" enableviewstate="false" />
    <meta ID="metaRdfaCreator" property="dc:creator" content="Admin" runat="server" enableviewstate="false" />
    <meta ID="metaDescription" name="description" content="Description" runat="server" enableviewstate="false" />
    <meta ID="metaAuthor" name="author" content="Admin" runat="server" enableviewstate="false" />
    <asp:Literal ID="litRdfaSubjects" runat="server" EnableViewState="false"></asp:Literal>
    <asp:Literal ID="litExternalResources" runat="server" EnableViewState="false"></asp:Literal>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeaderContent" runat="server" EnableViewState="false">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="AsideContent" runat="server" EnableViewState="false">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="TitleContent" runat="server" EnableViewState="false">
    <div class="pageHeader">
        <header>
            <hgroup>
                <h1 property="dc:title"><asp:Literal ID="litTitle" runat="server" EnableViewState="false"></asp:Literal></h1>
                <h2><asp:Literal ID="litIngress" runat="server" EnableViewState="false"></asp:Literal></h2>
            </hgroup>
            <div class="pageHeaderFigure clear">
                <asp:Literal ID="litFigure" runat="server" EnableViewState="false"></asp:Literal>
            </div>
        </header>    
    </div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="MainContent" runat="server">
    <section>
        <uc1:Map ID="Map" runat="server" Visible="False" EnableViewState="false"></uc1:Map>
    </section>
    <div class="maintext" property="sc:Article sc:articleBody">
        <asp:Literal ID="litBody" runat="server" EnableViewState="false"></asp:Literal>
    </div>
    <div class="pageFooter clear">
        <footer>
            <small>Published <time property="dc:date dc:created" id="timePublishedDate" runat="server" datetime="" pubdate="pubdate" enableviewstate="false"></time> &bull; Modified <time property="dc:date dc:modified" id="timeModifiedDate" runat="server" datetime="" enableviewstate="false"></time></small>
            <div class="section">
                <section>
                    <uc1:Share ID="Share1" runat="server"></uc1:Share>
                </section>
            </div>
        </footer>
    </div>
    <div class="clear"></div>
    <section>
        <uc1:ContactUser ID="contactUser1" runat="server" ToUserEmail="info@example.com" />
    </section>
    <section>
        <uc1:Comments VirtualFiles="VirtualPages" ID="comments1" runat="server"></uc1:Comments>
    </section>
</asp:Content>
