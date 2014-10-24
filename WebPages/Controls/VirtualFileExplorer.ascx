<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VirtualFileExplorer.ascx.cs" Inherits="VeraWAF.WebPages.Controls.VirtualFileExplorer" %>
<section id="fileExplorerDialog" class="fileexplorer" runat="server">
    <div id="fileexplorerBg" class="fileexplorerBg" runat="server"></div>
    <fieldset id="fsDialog" class="fileexplorer" visible="false" runat="server">
        <legend>File explorer</legend>
        <asp:Panel runat="server" ID="pPath">
            <asp:Label AssociatedControlID="txtFullPath" Text="Path:" runat="server"></asp:Label>
            <asp:TextBox ID="txtFullPath" runat="server"></asp:TextBox>
        </asp:Panel>    
        <div class="fileexplorer">
            <asp:XmlDataSource ID="treeViewXmlDataSource" runat="server"></asp:XmlDataSource>
            <asp:TreeView id="TreeView1" runat="server" datasourceid="treeViewXmlDataSource" ExpandDepth="1" SelectedNodeStyle-CssClass="selectedNode" 
                OnSelectedNodeChanged="TreeView1_SelectedNodeChanged">
                <databindings>
                    <asp:treenodebinding datamember="directories" textfield="name" Value="/" />
                    <asp:treenodebinding datamember="storage" textfield="name" Value="/" />
                    <asp:treenodebinding datamember="directory" textfield="name" ValueField="path" />
                    <asp:treenodebinding datamember="file" textfield="name" ValueField="path" />
                </databindings>
            </asp:TreeView>
        </div>
        <asp:Panel runat="server" ID="pFile">
            <asp:Label AssociatedControlID="txtFilename" Text="Filename:" runat="server"></asp:Label>
            <asp:TextBox ID="txtFilename" runat="server"></asp:TextBox>
        </asp:Panel>
        <menu>
            <asp:Button ID="butSelect" runat="server" OnClientClick="" OnClick="butSelect_Click" Text="Ok" />
            <asp:Button ID="butCancel" runat="server" OnClientClick="" OnClick="butCancel_Click" Text="Cancel" />
        </menu>
    </fieldset>
    <p></p>
    <asp:Label ID="lblResult" AssociatedControlID="txtResult" Text="Filename:" runat="server"></asp:Label>
    <asp:TextBox ID="txtResult" CssClass="result" runat="server"></asp:TextBox>
    <asp:Button ID="butBrowse" Text="..." ToolTip="Browse files" runat="server" OnClick="butBrowse_Click" OnClientClick="return $(this).openFileExplorer();" />
</section>
