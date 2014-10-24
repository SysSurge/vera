<%@ Page Language="C#" MasterPageFile="~/Sub2.Master" AutoEventWireup="true" CodeBehind="Documentation.aspx.cs" Inherits="VeraWAF.WebPages.Templates.Documentation" ValidateRequest="false" %>
<%@ Register TagPrefix="uc1" TagName="Share" Src="~/Controls/Share.ascx" %>
<%@ Register TagPrefix="uc1" TagName="TreeViewMenu" Src="~/Controls/TreeViewMenu.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Comments" Src="~/Controls/Comments.ascx" %>
<%@ Register TagPrefix="uc1" TagName="ContactUser" Src="~/Controls/ContactUser.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Map_1" Src="~/Controls/Map.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Codemirror" Src="~/Controls/Codemirror.ascx" %>
<%@ Register TagPrefix="uc1" TagName="BashSimulator" Src="~/Controls/BashSim.ascx" %>
<%@ Register TagPrefix="uc1" TagName="ContentField" Src="~/Controls/ContentField.ascx" %>
<%@ Register TagPrefix="uc1" TagName="PagePublishDate" Src="~/Controls/PagePublishDate.ascx" %>
<%@ Register TagPrefix="uc1" TagName="PageModifiedDate" Src="~/Controls/PageModifiedDate.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server" ViewStateMode="Disabled">

    <CompressStyles CompressedFile="documentation.min.css" runat="server">
        documentation.css
    </CompressStyles>

    <uc1:BashSimulator ID="BashSimulator" runat="server" ViewStateMode="Disabled"></uc1:BashSimulator>
    <uc1:Codemirror ID="Codemirror" runat="server" ViewStateMode="Disabled"></uc1:Codemirror>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="MainContent" runat="server">
    <div class="documentation" property="sc:Article">

        <uc1:TreeViewMenu ID="mnuTreeview" runat="server"></uc1:TreeViewMenu>

        <div class="top">
            <header>
                <h1 property="dc:title">
                    <uc1:ContentField PropertyName="Title" runat="server"></uc1:ContentField>
                </h1>
            </header>
            <div class="pageFooter">
                <footer>
                    <small>
                        <uc1:PagePublishDate runat="server"></uc1:PagePublishDate> 
                        &bull; 
                        <uc1:PageModifiedDate runat="server"></uc1:PageModifiedDate>
                    </small>
                    <div class="section clear">
                        <section>
                            <uc1:Share ID="Share1" runat="server"></uc1:Share>
                        </section>
                    </div>
                </footer>
            </div>
        </div>
        <section><uc1:Map_1 ID="Map" runat="server" Visible="False"></uc1:Map_1></section>
        <div class="maintext2" property="sc:articleBody">
            <uc1:ContentField PropertyName="MainContent" runat="server"></uc1:ContentField>
        </div>
        <section><uc1:Comments VirtualFiles="VirtualPages" ID="comments1" runat="server"></uc1:Comments></section>
        <section><uc1:ContactUser ID="contactUser1" runat="server" ToUserEmail="info@example.com" /></section>
    </div>
</asp:Content>
