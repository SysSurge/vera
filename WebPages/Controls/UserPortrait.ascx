<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserPortrait.ascx.cs" Inherits="VeraWAF.WebPages.Controls.UserPortrait" EnableViewState="false" %>
<%@ Register TagPrefix="uc1" TagName="JailImage" Src="~/Controls/JailImage.ascx" %>
<a ID="lnkUserPortrait" class="userPortrait" href="" runat="server">
    <figure>
        <uc1:JailImage ID="jailUserPortrait" runat="server" src="" css="userPortrait64x64 center" alt="" width="64" height="64" />
        <figcaption property="sc:name"><asp:Literal ID="figCaption" runat="server"></asp:Literal></figcaption>
    </figure>
</a>