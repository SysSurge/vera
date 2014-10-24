<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PagePublishDate.ascx.cs" Inherits="VeraWAF.WebPages.Controls.PagePublishDate" EnableViewState="false" %>
<asp:PlaceHolder ID="phDate" Visible="false" runat="server">
    Published <time property="dc:date dc:created" id="timePublishedDate" runat="server" datetime="" pubdate="pubdate"></time>
</asp:PlaceHolder>
