<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PageModifiedDate.ascx.cs" Inherits="VeraWAF.WebPages.Controls.PageModifiedDate" EnableViewState="false" %>
<asp:PlaceHolder ID="phDate" Visible="false" runat="server">
    Modified <time property="dc:date dc:modified" id="timeModifiedDate" runat="server" datetime=""></time>
</asp:PlaceHolder>
