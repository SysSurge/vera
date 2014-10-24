<%@ Page Title="Sign Out" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Logout.aspx.cs" Inherits="VeraWAF.WebPages.Account.Logout" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <CompressStyles CompressedFile="account.min.css" runat="server">
        account.css
    </CompressStyles>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <header>
        <h1>
            Sign Out
        </h1>
    </header>
    <div class="accountInfo">
        <fieldset class="changePassword">
            <legend>Sign out confirmation</legend>
                <p>
                    You have successfully signed out.
                </p>
        </fieldset>
        <p class="submitButton">
            <asp:Button ID="ContinueButton" runat="server" CausesValidation="False" CommandName="SignIn" PostBackUrl="~/Account/Login.aspx" Text="Sign In" />
        </p>
    </div>
</asp:Content>
