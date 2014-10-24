<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContentField.ascx.cs" Inherits="VeraWAF.WebPages.Controls.ContentField" EnableViewState="true" %>
<%@ Register src="~/Controls/InputBox.ascx" tagname="InputBox" tagprefix="uc1" %>
<asp:Literal ID="litMarkup" runat="server"></asp:Literal>
<uc1:InputBox id="inputBox" visible="false" TableName="VeraPages" EntityType="PageEntity" runat="server"></uc1:InputBox>