<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SendNewsLetter.aspx.cs" Inherits="VeraWAF.WebPages.Edit.SendNewsLetter" %>
<%@ Register src="~/Controls/FormNotification.ascx" tagname="FormNotification" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <CompressStyles CompressedFile="newsletter.min.css" runat="server">
        newsletter.css
    </CompressStyles>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="AsideContent" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MainContent" runat="server">
<div class="newsletterForm">
    <header>
        <h1>
            <i class="fa fa-send-o"></i> Send newsletter
        </h1>
    </header>


    <menu>
        <asp:Button ID="butSend" runat="server" onclick="butSend_Click" Text="Send" />
    </menu>

    <uc1:FormNotification id="notifications" runat="server"></uc1:FormNotification>

    <fieldset>
        <legend>Newsletter</legend>
        <p>
            <asp:Label ID="lblVirtualPath" runat="server" 
                AssociatedControlID="txtVirtualPath" Text="Page virtual path:"></asp:Label>
            <asp:TextBox ID="txtVirtualPath" runat="server" Width="98%"></asp:TextBox>
        </p>
        <p>
            <asp:Label ID="lblTo" runat="server" 
                AssociatedControlID="txtTo" Text="Newletter subscriber e-mail addresses:"></asp:Label>
            <br />
            <asp:TextBox ID="txtTo" runat="server" Height="300px" TextMode="MultiLine" 
                Width="98%"></asp:TextBox>
        </p>
    </fieldset>
</div>
</asp:Content>
