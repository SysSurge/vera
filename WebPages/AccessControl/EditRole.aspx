<%@ Page Title="Edit role information" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EditRole.aspx.cs" 
    Inherits="VeraWAF.WebPages.Admin.EditRole" ValidateRequest="false" %>
<%@ Register src="~/Controls/FormNotification.ascx" tagname="FormNotification" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="AsideContent" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        <i class="fa fa-users"></i> Edit role
    </h1>

    <menu>
        <asp:Button ID="butSave" runat="server" OnClick="butSave_Click" Text="Save" />
        <asp:Button ID="butDelete" runat="server" OnClick="butDelete_Click" Text="Delete" />
    </menu>

    <div class="clear"></div>

    <uc1:FormNotification id="notifications" runat="server"></uc1:FormNotification>

    <fieldset style="width:50em;">
        <legend>Role information</legend>
        <p>
            <asp:Label ID="lblId" runat="server" AssociatedControlID="litId" 
                Text="Internal ID:"></asp:Label>
            <asp:Literal ID="litId" runat="server"></asp:Literal>
        </p>
        <p>
            <asp:Label ID="lblRolename" runat="server" AssociatedControlID="txtRolename" 
                Text="Role name:"></asp:Label>
            <asp:TextBox ID="txtRolename" Width="98%" runat="server" TextMode="SingleLine"></asp:TextBox>
        </p>
        <p>
            <asp:Label ID="lblCreated" runat="server" AssociatedControlID="litCreated" 
                Text="Created date:"></asp:Label>
            <asp:Literal ID="litCreated" runat="server"></asp:Literal>
        </p>
        <p>
            <asp:Label ID="lblModified" runat="server" AssociatedControlID="litModified" 
                Text="Modified date:"></asp:Label>
            <asp:Literal ID="litModified" runat="server"></asp:Literal>
        </p>
        <p>
            <asp:Label ID="lblComment" runat="server" AssociatedControlID="txtComment" 
                Text="Description:"></asp:Label>
            <asp:TextBox ID="txtComment" Width="98%" runat="server" TextMode="MultiLine" Rows="5"></asp:TextBox>
        </p>

        <fieldset>
            <legend>Membership management</legend>
            <p>
                <asp:Label ID="lblUsers" runat="server" AssociatedControlID="txtUsers" 
                    Text="Role members:"></asp:Label>
                <asp:TextBox ID="txtUsers" Width="98%" runat="server" TextMode="MultiLine" Rows="5" ReadOnly="true"></asp:TextBox>
            </p>
            <p>
                <asp:Label ID="lblUserName" runat="server" AssociatedControlID="txtUserName" 
                    Text="Username:"></asp:Label>
                <asp:TextBox ID="txtUserName" Width="98%" runat="server"></asp:TextBox>
            </p>
            <menu>
                <asp:Button runat="server" ID="btnAddUser" Text="Add user" OnClick="btnAddUser_Click" />
                <asp:Button runat="server" ID="btnRemoveUser" Text="Remove user" OnClick="btnRemoveUser_Click" />
            </menu>
        </fieldset>

    </fieldset>
</asp:Content>
