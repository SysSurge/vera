<%@ Page Title="Activate User Account" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="ActivateAccount.aspx.cs" Inherits="VeraWAF.WebPages.Account.ActivateAccount" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <header>
        <h1>
            Account activation
        </h1>
    </header>
    <div class="activateAccount">
        <asp:panel Visible="false" runat="server" ID="success">
            <fieldset class="information">
                <legend>
                    Thank you!
                </legend>
                <p>
                    Your <%=ConfigurationManager.AppSettings["ApplicationName"]%> account is now activated.
                    <a href="Login.aspx">Click here</a> to sign in.
                </p>
            </fieldset>
        </asp:panel>
        <asp:panel Visible="false" runat="server" ID="alreadyActivated">
            <fieldset class="errorMessage">
                <legend>
                    User account activation failed!
                </legend>
                <p>
                    This account has already been activated.
                </p>
                <p>
                    If you are having problems signing in then <a href="ResetPassword.aspx">click here</a> to reset your password.
                </p>
            </fieldset>
        </asp:panel>
        <asp:panel Visible="false" runat="server" ID="notFound">
            <fieldset class="errorMessage">
                <legend>
                    User account activation failed!
                </legend>
                <p>
                    The account was not found.
                </p>
            </fieldset>
        </asp:panel>
        <asp:panel Visible="false" runat="server" ID="badConfirmationId">
            <fieldset class="errorMessage">
                <legend>
                    User account activation failed!
                </legend>
                <p>
                    Bad confirmation ID. Make sure that the whole link is entered into the browser address bar and try again.
                </p>
            </fieldset>
        </asp:panel>
    </div>
</asp:Content>
