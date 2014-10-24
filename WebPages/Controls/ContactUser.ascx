<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContactUser.ascx.cs" Inherits="VeraWAF.WebPages.Controls.ContactUser" EnableViewState="true" ViewStateMode="Enabled" %>
<%@ Register TagPrefix="uc1" TagName="JailImage" Src="~/Controls/JailImage.ascx" %>
<fieldset class="contactUser">
    <legend><asp:Literal ID="legendText" runat="server"></asp:Literal></legend>
    <p class="fromUser" ID="fromField" runat="server" Visible="False">
        <asp:Label ID="lblFromEmail" runat="server" AssociatedControlID="txtFromEmail">Your e-mail:</asp:Label>
        <asp:TextBox Columns="38" ID="txtFromEmail" runat="server"></asp:TextBox>
    </p>
    <div ID="currentUserPortrait" class="currentUserPortrait" runat="server">
        <figure>
            <uc1:JailImage id="imgPortrait" runat="server" css="userPortrait64x64" alt="User portrait" width="64" height="64" />
            <br />
            <figcaption><asp:Literal ID="litPortraitCaption" runat="server"></asp:Literal></figcaption>
        </figure>
    </div>
    <div>
        <asp:TextBox TextMode="MultiLine" Columns="40" Rows="6" ID="txtMessage" runat="server"></asp:TextBox>
    </div>
    <p>
        <asp:Button id="butSendMessage" runat="server" onclick="butSendMessage_Click" Text="Send message" />     
        <asp:Literal ID="litConfirmMessageSent" runat="server" Text="Message sent." Visible="False"></asp:Literal>
    </p>
</fieldset>