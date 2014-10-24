<%@ Page Title="Edit account information" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EditUser.aspx.cs" 
    Inherits="VeraWAF.WebPages.Admin.EditUser" ValidateRequest="false" %>
<%@ Register src="~/Controls/FormNotification.ascx" tagname="FormNotification" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="AsideContent" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        <i class="fa fa-user"></i> Edit account
    </h1>

    <menu>
        <asp:Button ID="butSave" runat="server" onclick="butSave_Click" Text="Save" />
        <asp:Button ID="butDelete" runat="server" onclick="butDelete_Click" Text="Delete" />
        <asp:CheckBox runat="server" ID="chkBan" Text="Ban user" />
    </menu>

    <div class="clear"></div>

    <uc1:FormNotification id="notifications" runat="server"></uc1:FormNotification>

    <fieldset style="width:50em;">
        <legend>User account information</legend>
        <p>
            <asp:Label ID="lblId" runat="server" AssociatedControlID="litId" 
                Text="Internal ID:"></asp:Label>
            <asp:Literal ID="litId" runat="server"></asp:Literal>
        </p>
        <p>
            <asp:CheckBox runat="server" ID="chkApproved" Text="E-mail confirmed" />
            <asp:CheckBox runat="server" ID="chkLocked" Text="Account locked out" />
        </p>
        <p>
            <asp:Label ID="lblUsername" runat="server" AssociatedControlID="txtUsername" 
                Text="Username:"></asp:Label>
            <asp:TextBox ID="txtUsername" Width="98%" runat="server" TextMode="SingleLine"></asp:TextBox>
        </p>
        <p>
            <asp:Label ID="lblFullName" runat="server" AssociatedControlID="txtFullName" 
                Text="Full name:"></asp:Label>
            <asp:TextBox ID="txtFullName" Width="98%" runat="server" TextMode="SingleLine"></asp:TextBox>
        </p>
        <p>
            <asp:Label ID="lblEmail" runat="server" AssociatedControlID="txtEmail" 
                Text="E-mail:"></asp:Label>
            <asp:TextBox ID="txtEmail" Width="98%" runat="server" TextMode="SingleLine"></asp:TextBox>
        </p>
        <p>
            <asp:Label ID="lblProfile" runat="server" AssociatedControlID="litProfile" 
                Text="Account profile:"></asp:Label>
            <asp:Literal ID="litProfile" runat="server"></asp:Literal>
        </p>
        <p>
            <asp:CheckBox runat="server" ID="chkContact" Text="Allow direct messages" />
            <asp:CheckBox runat="server" ID="chkNewsletter" Text="Send newsletter" />
        </p>
        <p>
            <asp:Label ID="lblCreated" runat="server" AssociatedControlID="litCreated" 
                Text="Created date:"></asp:Label>
            <asp:Literal ID="litCreated" runat="server"></asp:Literal>
        </p>
        <p>
            <asp:Label ID="lblLastActive" runat="server" AssociatedControlID="litLastActive" 
                Text="Last active date:"></asp:Label>
            <asp:Literal ID="litLastActive" runat="server"></asp:Literal>
        </p>
        <p>
            <asp:Label ID="lblBirthday" runat="server" AssociatedControlID="litBirthday" 
                Text="Birthday:"></asp:Label>
            <asp:Literal ID="litBirthday" runat="server"></asp:Literal>
        </p>
        <p>
            <asp:Label ID="lblComment" runat="server" AssociatedControlID="txtComment" 
                Text="User's own comment:"></asp:Label>
            <asp:TextBox ID="txtComment" Width="98%" runat="server" TextMode="MultiLine" Rows="5"></asp:TextBox>
        </p>
        <p>
            <asp:Label ID="lblRoles" runat="server" AssociatedControlID="txtRoles" 
                Text="Role memberships:"></asp:Label>
            <asp:TextBox ID="txtRoles" Width="98%" runat="server" TextMode="MultiLine" Rows="5" ReadOnly="true"></asp:TextBox>
        </p>

        <fieldset class="fill" style="width: 25em" runat="server">
            <legend>
                OAuth security data for API access
            </legend>
            <p>
                <asp:Label ID="lblOAuthConsumerKey" runat="server" AssociatedControlID="txtOAuthConsumerKey">Consumer key:</asp:Label>
                <asp:TextBox ID="txtOAuthConsumerKey" runat="server"></asp:TextBox>
            </p>
            <p>
                <asp:Label ID="lblOAuthConsumerSecret" runat="server" AssociatedControlID="txtOAuthConsumerSecret">Consumer secret:</asp:Label>
                <asp:TextBox ID="txtOAuthConsumerSecret" runat="server"></asp:TextBox>
                <br />
                <menu>
                    <asp:Button id="butRegenerateKey" OnClick="butRegenerateKey_Click" runat="server" Text="New key" />
                    <asp:Button id="butRegenerateSecret" OnClick="butRegenerateSecret_Click" runat="server" Text="New secret" />
                </menu>
            </p>
        </fieldset>
    </fieldset>
</asp:Content>
