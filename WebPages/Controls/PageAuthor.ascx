<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PageAuthor.ascx.cs" Inherits="VeraWAF.WebPages.Controls.PageAuthor" EnableViewState="false" %>
<asp:PlaceHolder ID="phDate" Visible="false" runat="server">
    <span>Written by </span><asp:HyperLink ID="lnkAuthor" runat="server" property="dc:creator"></asp:HyperLink>
</asp:PlaceHolder>
