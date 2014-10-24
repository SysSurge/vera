<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserRoleContextMenu.ascx.cs" Inherits="VeraWAF.WebPages.Controls.UserRoleContextMenu" %>
<%@ Register src="~/Controls/UserName.ascx" tagname="UserName" tagprefix="uc1" %>
<%@ Register src="~/Controls/UserPortrait.ascx" tagname="UserPortrait" tagprefix="uc1" %>
<nav>
    <asp:LoginView ID="HeadLoginView" runat="server" EnableViewState="true">
        <AnonymousTemplate>
            <a href="~/Account/Register.aspx" ID="A1" class="topRegisterButton" runat="server">Register</a>
            <a href="~/Account/Login.aspx" ID="HeadLoginStatus" class="topLoginButton" runat="server">Sign In</a>
        </AnonymousTemplate>
        <LoggedInTemplate>
            <uc1:UserPortrait UseCurrentUser="True" ShowUsername="False" runat="server"></uc1:UserPortrait>
            <div class="perzonalizedMenuRight">
                Welcome, <uc1:UserName UseCurrentUser="True" HtmlEncode="true" runat="server"></uc1:UserName>.
                (<asp:HyperLink ID="LogOut" Runat="server" NavigateUrl="~/Account/Logout.aspx">Not <uc1:UserName UseCurrentUser="True" HtmlEncode="true" runat="server"></uc1:UserName>?</asp:HyperLink>)
                <br />
                <i class="fa fa-male"></i><asp:HyperLink ID="Profile" Runat="server" NavigateUrl="~/Account/">My Profile</asp:HyperLink>
                |
                <i class="fa fa-lock"></i><asp:HyperLink ID="ChangePassword" Runat="server" NavigateUrl="~/Account/ChangePassword.aspx">Change Password</asp:HyperLink>
                |
                <i class="fa fa-sign-out"></i><asp:HyperLink ID="SignOut" Runat="server" NavigateUrl="~/Account/Logout.aspx">Sign Out</asp:HyperLink>
            </div>
        </LoggedInTemplate>
        <RoleGroups>
            <asp:RoleGroup Roles="Admins">
                <ContentTemplate>
                    <uc1:UserPortrait UseCurrentUser="True" ShowUsername="False" runat="server"></uc1:UserPortrait>
                    <div class="perzonalizedMenuRight">
                        Welcome, administrator: <uc1:UserName UseCurrentUser="True" HtmlEncode="true" runat="server"></uc1:UserName>.
                        (<asp:HyperLink ID="LogOut" Runat="server" NavigateUrl="~/Account/Logout.aspx">Not <uc1:UserName UseCurrentUser="True" HtmlEncode="true" runat="server"></uc1:UserName>?</asp:HyperLink>)
                        <br />
                        <asp:PlaceHolder ID="phAdminStartEdit" runat="server">
                            <i class="fa fa-edit"></i><a href="?edit=1">Edit page</a>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="phAdminStopEdit" Visible="false" runat="server">
                            <span class="fa-stack fa-lg">
                                <i class="fa fa-edit fa-stack-1x"></i>
                                <i class="fa fa-ban fa-stack-2x text-danger"></i>
                            </span>
                            <a href=".">Stop editing page</a>
                        </asp:PlaceHolder>
                        |
                        <i class="fa fa-male"></i><asp:HyperLink ID="Profile" Runat="server" NavigateUrl="~/Account/">My Profile</asp:HyperLink>
                        |
                        <i class="fa fa-lock"></i><asp:HyperLink ID="ChangePassword" Runat="server" NavigateUrl="~/Account/ChangePassword.aspx">Change Password</asp:HyperLink>
                        |
                        <i class="fa fa-shield"></i><asp:HyperLink ID="Users" Runat="server" NavigateUrl="~/AccessControl/Default.aspx">Access Control</asp:HyperLink>
                        |
                        <i class="fa fa-sitemap"></i><asp:HyperLink ID="Edit" Runat="server" NavigateUrl="~/CMS/">Web CMS</asp:HyperLink>
                        |
                        <i class="fa fa-cloud"></i><asp:HyperLink ID="Commands" Runat="server" NavigateUrl="~/Cloud/Default.aspx">Cloud</asp:HyperLink>
                        |
                        <i class="fa fa-sign-out"></i><asp:HyperLink ID="SignOut" Runat="server" NavigateUrl="~/Account/Logout.aspx">Sign Out</asp:HyperLink>
                    </div>
                </ContentTemplate>
            </asp:RoleGroup>
            <asp:RoleGroup Roles="Editors">
                <ContentTemplate>
                    <uc1:UserPortrait UseCurrentUser="True" ShowUsername="False" runat="server"></uc1:UserPortrait>
                    <div class="perzonalizedMenuRight">
                        Welcome, editor: <uc1:UserName UseCurrentUser="True" HtmlEncode="true" runat="server"></uc1:UserName>.
                        (<asp:HyperLink ID="LogOut" Runat="server" NavigateUrl="~/Account/Logout.aspx">Not <uc1:UserName UseCurrentUser="True" HtmlEncode="true" runat="server"></uc1:UserName>?</asp:HyperLink>)
                        <br />
                        <asp:PlaceHolder ID="phEditorStartEdit" runat="server">
                            <i class="fa fa-edit"></i><a href="?edit=1">Edit page</a>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="phEditorStopEdit" Visible="false" runat="server">
                            <span class="fa-stack fa-lg">
                                <i class="fa fa-edit fa-stack-1x"></i>
                                <i class="fa fa-ban fa-stack-2x text-danger"></i>
                            </span>
                            <a href=".">Stop editing page</a>
                        </asp:PlaceHolder>
                        |
                        <i class="fa fa-male"></i><asp:HyperLink ID="Profile" Runat="server" NavigateUrl="~/Account/">My Profile</asp:HyperLink>
                        |
                        <i class="fa fa-lock"></i><asp:HyperLink ID="ChangePassword" Runat="server" NavigateUrl="~/Account/ChangePassword.aspx">Change Password</asp:HyperLink>
                        |
                        <i class="fa fa-sitemap"></i></i><asp:HyperLink ID="Edit" Runat="server" NavigateUrl="~/CMS/">Web CMS</asp:HyperLink>
                        |
                        <i class="fa fa-sign-out"></i><asp:HyperLink ID="SignOut" Runat="server" NavigateUrl="~/Account/Logout.aspx">Sign Out</asp:HyperLink>
                    </div>
                </ContentTemplate>
            </asp:RoleGroup>
        </RoleGroups>
    </asp:LoginView>
</nav>
