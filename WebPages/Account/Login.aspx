<%@ Page Title="Sign In" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Login.aspx.cs" Inherits="VeraWAF.WebPages.Account.Login" %>
<%@ Register src="~/Controls/SocialSignIn.ascx" tagname="ThirdPartySignIn" tagprefix="uc1" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <uc1:ThirdPartySignIn runat="server"></uc1:ThirdPartySignIn>
    <CompressStyles CompressedFile="account.min.css" runat="server">
        account.css
    </CompressStyles>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <header>
        <h1>
            <span class="fa-stack fa-lg">
                <i style="margin-left:-0.5em" class="fa fa-user fa-stack-1x"></i>
                <i style="color:#2b96fe; font-size:0.8em; margin-left: 0.1em" class="fa fa-sign-in fa-stack-1x"></i>
            </span>
            <span>Sign In</span>
        </h1>
        <p>
            Sign in with your existing user account or
            <asp:HyperLink ID="RegisterHyperLink" runat="server" EnableViewState="false">click here to register</asp:HyperLink>.
        </p>
    </header>

    <asp:Login ID="LoginUser" runat="server" EnableViewState="false" RenderOuterTable="false" OnLoggingIn="LoginUser_OnLoggingIn" OnLoggedIn="LoginUser_OnLoggedIn" OnLoginError="LoginUser_OnLoginError" >
        <LayoutTemplate>
            <span class="failureNotification">
                <asp:Literal ID="SignInFailed" runat="server" Visible="false">
                    <p style="text-decoration:underline">Your sign-in attempt was unsuccessful; note that usernames and passwords are case-sensitive. Please try again.</p>
                </asp:Literal>
            </span>
            <asp:ValidationSummary ID="LoginUserValidationSummary" runat="server" CssClass="failureNotification" 
                 ValidationGroup="LoginUserValidationGroup"/>
            <div class="accountInfo">
                <div class="accountInfo-container">
                    <fieldset class="login">
                        <legend>Account Information</legend>
                        <p>
                            <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName">Username:</asp:Label>
                            <asp:TextBox AutoCompleteType="DisplayName" ID="UserName" runat="server" CssClass="textEntry"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName" 
                                 CssClass="failureNotification" ErrorMessage="User Name is required." ToolTip="Username is required." 
                                 ValidationGroup="LoginUserValidationGroup">*</asp:RequiredFieldValidator>
                            <asp:CustomValidator ID="IsLockedOut" runat="server" ControlToValidate="UserName" 
                                 CssClass="failureNotification" ErrorMessage="This account is now locked out because of too many unsuccessful sign in attempts. Check your e-mail account for instruction on how to unlock your account again." 
                                 ToolTip="Account is locked out." ValidationGroup="LoginUserValidationGroup" OnServerValidate="ValidateIfLockedOut">*</asp:CustomValidator>
                            <asp:CustomValidator ID="IsApproved" runat="server" ControlToValidate="UserName" 
                                 CssClass="failureNotification" ErrorMessage="You must activate your account before signing in. To help you sign in we just sent the e-mail with the validation link to you again; please check your e-mail account." 
                                 ToolTip="Activate account first." ValidationGroup="LoginUserValidationGroup" OnServerValidate="ValidateIfApproved">*</asp:CustomValidator>
                        </p>
                        <p>
                            <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password">Password:</asp:Label>
                            <asp:TextBox AutoCompleteType="Disabled" ID="Password" runat="server" CssClass="passwordEntry" TextMode="Password"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password" 
                                 CssClass="failureNotification" ErrorMessage="Password is required." ToolTip="Password is required." 
                                 ValidationGroup="LoginUserValidationGroup">*</asp:RequiredFieldValidator>
                        </p>
                        <p>
                            <asp:CheckBox ID="RememberMe" runat="server"/>
                            <asp:Label ID="RememberMeLabel" runat="server" AssociatedControlID="RememberMe" CssClass="inline">Keep me signed in</asp:Label>
                        </p>
                        <p>
                            <a href="ResetPassword.aspx">Click here</a> if you have forgotten your password.
                        </p>
                        <p class="submitButton">
                            <asp:Button ID="LoginButton" runat="server" CommandName="Login" Text="Sign In" ValidationGroup="LoginUserValidationGroup" />
                        </p>
                    </fieldset>                
                    <span style="font-weight:bold;display:inline-block;vertical-align: top; margin: 3em 1em 0 0">&nbsp;</span>                
                    <div style="display:inline-block;vertical-align: top;margin-top: 2.3em" id="janrainEngageEmbed"></div>
                    <noscript>
                        <div style="padding-top: 11em;width:450px;text-align:center;float:left;font-style:italic">
                            You must enable JavaScript to sign in using Facebook, Google, Twitter, OpenID, LinkedIn, or Yahoo.
                        </div>
                    </noscript>
                </div>
            </div>
            

        </LayoutTemplate>
    </asp:Login>
</asp:Content>
