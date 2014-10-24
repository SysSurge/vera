<%@ Page Title="Unlock User Account" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="UnlockAccount.aspx.cs" Inherits="VeraWAF.WebPages.Account.UnlockAccount" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <header>
        <h1>
            Unlock your account
        </h1>
    </header>
    <div class="activateAccount">
        <asp:panel Visible="false" runat="server" ID="success">
            <fieldset class="information">
                <legend>
                    Thank you!
                </legend>
                <p>
                    Your <%=ConfigurationManager.AppSettings["ApplicationName"]%> account is now unlocked.
                    <a href="Login.aspx">Click here</a> to sign in.
                </p>
            </fieldset>
        </asp:panel>
        <asp:panel Visible="false" runat="server" ID="alreadyUnlocked">
            <fieldset class="errorMessage">
                <legend>
                    User account unlocking failed!
                </legend>
                <p>
                    This account has is not locked.
                </p>
            </fieldset>
        </asp:panel>
        <asp:panel Visible="false" runat="server" ID="notFound">
            <fieldset class="errorMessage">
                <legend>
                    User account unlocking failed!
                </legend>
                <p>
                    The account was not found.
                </p>
            </fieldset>
        </asp:panel>
        <asp:panel Visible="false" runat="server" ID="badConfirmationId">
            <fieldset class="errorMessage">
                <legend>
                    User account unlocking failed!
                </legend>
                <p>
                    Bad link. Make sure that the whole link is entered into the browser address bar and try again.
                </p>
            </fieldset>
        </asp:panel>
    </div>
</asp:Content>
