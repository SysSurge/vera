<%@ Page Title="Access Control Panel" Language="C#" MasterPageFile="~/Site.Master"  Inherits="VeraWAF.Core.Templates.PageTemplateBase" AutoEventWireup="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="AsideContent" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MainContent" runat="server">
    <header>
        <h1>
            <i class="fa fa-shield"></i> Access Control Panel
        </h1>        
    </header>
    <p>Administrative tasks for the managing access to the solution.</p>
    <div class="adminPanel">
        <section>
            <h2>Accounts</h2>
            <p>
                <asp:HyperLink ID="lnkNewUser" runat="server" NavigateUrl="~/AccessControl/EditUser.aspx">New users</asp:HyperLink>
            </p>
            <p>
                <asp:HyperLink ID="lnkUsers" runat="server" NavigateUrl="~/AccessControl/Users.aspx">Users</asp:HyperLink>
            </p>
            <p>
                <asp:HyperLink ID="lnkRoles" runat="server" NavigateUrl="~/AccessControl/Roles.aspx">Roles</asp:HyperLink>
            </p>
        </section>
        <section>
            <h2>Permissions</h2>
            <p>
                <asp:HyperLink ID="lnkViewComments" runat="server" NavigateUrl="~/AccessControl/Acl.aspx">Access control list</asp:HyperLink>
            </p>
        </section>
    </div>
</asp:Content>
