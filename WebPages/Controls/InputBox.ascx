<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="InputBox.ascx.cs" Inherits="VeraWAF.WebPages.Controls.InputBox" %>
<%@ Register Assembly="CKEditor.NET" Namespace="CKEditor.NET" TagPrefix="CKEditor" %>
<div id="editContainer" class="inputBox" runat="server">
    <CKEditor:CKEditorControl ID="htmlInputField" CustomConfig="/Scripts/ckeditor/config.js" BasePath="/Scripts/ckeditor/" class="inputBox inputField" visible="false" runat="server"></CKEditor:CKEditorControl>
    <asp:PlaceHolder ID="plTextOnly" runat="server">
        <div id="helpText" style="display: block;"><asp:Literal ID="litHelpText" runat="server"></asp:Literal></div>
        <div id="inputBoxBackground" style="opacity: 0.5;"></div>
        <asp:TextBox ID="txtInputField" class="inputBox inputField" TextMode="MultiLine" runat="server"></asp:TextBox>
        <asp:HyperLink ID="saveButton" CssClass="saveIcon toolIcon" runat="server" ToolTip="Save input field"><i class="fa fa-save fa-fw"></i></asp:HyperLink>
        <div id="savingIcon" class="fa fa-refresh fa-spin" title="Saving, please wait..."></div>
        <div id="failIcon" class="fa fa-exclamation-circle" title="The operation failed"></div>
        <div id="successIcon" class="fa fa-check-circle-o" title="The operation was successful"></div>
        <asp:HyperLink ID="clearButton" CssClass="clearIcon toolIcon rotate-45" runat="server" ToolTip="Clear text"><i class="fa fa-plus-circle"></i></asp:HyperLink>
    </asp:PlaceHolder> 
</div>
 