<%@ Page Title="Access control lists" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="Acl.aspx.cs" Inherits="VeraWAF.WebPages.AccessControl.Acl" %>
<%@ Register src="~/Controls/FormNotification.ascx" tagname="FormNotification" tagprefix="uc1" %>
<%@ Reference Control="~/Controls/RulePermissions.ascx" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <CompressStyles CompressedFile="acl.min.css" runat="server">
        edit.css
        fileexplorer.css
    </CompressStyles>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <header>
        <h1>
            <span class="fa-stack fa-lg">
                <i class="fa fa-cube fa-stack-2x"></i><i style="color:#1e610f" class="fa fa-shield fa-stack-1x"></i>
            </span> Access control lists
        </h1>
    </header>

    <uc1:FormNotification id="notifications" runat="server"></uc1:FormNotification>

    <div class="info">
        <section id="fileExplorerDialog" class="fileexplorer fileexplorerContainer" runat="server">
            <div id="fileexplorerBg" class="fileexplorerBg" runat="server"></div>
            <fieldset id="fsDialog" class="fileexplorer" visible="false" runat="server">
                <legend>Rule explorer</legend>

                <asp:Label AssociatedControlID="litResource" Text="Resource:" runat="server"></asp:Label>
                <asp:Literal ID="litResource" Text="Resource" runat="server"></asp:Literal>
                <p></p>

                <div class="fileexplorer">

                    <asp:PlaceHolder ID="phRules" runat="server"></asp:PlaceHolder>

                </div>

                <asp:Panel runat="server" ID="pFile">
                </asp:Panel>

                <fieldset>
                    <legend>Add new rule</legend>
                    <asp:Label AssociatedControlID="txtUserOrRole" Text="User/role name:" CssClass="addButton" runat="server"></asp:Label>
                    <br />
                    <asp:TextBox ID="txtUserOrRole" runat="server"></asp:TextBox>
                    <asp:Button ID="butAdd" runat="server" OnClientClick="" OnClick="butAdd_Click" Text="Add rule" />
                </fieldset>

                <p></p>

                <menu>
                    <asp:Button ID="butSelect" runat="server" OnClientClick="" OnClick="butSelect_Click" Text="Close" />
                </menu>
            </fieldset>

            <asp:XmlDataSource ID="treeViewXmlDataSource" runat="server"></asp:XmlDataSource>
            <asp:TreeView id="TreeView1" runat="server" datasourceid="treeViewXmlDataSource" ExpandDepth="1" 
                SelectedNodeStyle-CssClass="selectedNode" OnSelectedNodeChanged="TreeView1_SelectedNodeChanged">
                <databindings>
                    <asp:treenodebinding datamember="resources" textfield="name" Value="" SelectAction="None" />
                    <asp:treenodebinding datamember="database" textfield="name" Value="" SelectAction="None" />
                    <asp:treenodebinding datamember="tables" textfield="name" ValueField="path" />
                    <asp:treenodebinding datamember="table" textfield="name" ValueField="path" />
                    <asp:treenodebinding datamember="file" textfield="name" ValueField="path" />

                    <asp:treenodebinding datamember="directories" textfield="name" Value="/" />
                    <asp:treenodebinding datamember="storage" textfield="name" Value="/" />
                    <asp:treenodebinding datamember="directory" textfield="name" ValueField="path" />
                </databindings>
            </asp:TreeView>

        </section>

    </div>
</asp:Content>
