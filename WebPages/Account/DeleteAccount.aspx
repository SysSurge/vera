<%@ Page Title="Delete your account" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="DeleteAccount.aspx.cs" Inherits="VeraWAF.WebPages.Account.DeleteAccount" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <CompressStyles CompressedFile="account.min.css" runat="server">
        account.css
    </CompressStyles>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <header>
        <h1>
            <i class="fa fa-ban" style="color:#ff0000"></i> Delete your account
        </h1>
        <p>
            Use the form below to delete your account.
        </p>
    </header>

    <asp:ValidationSummary ID="DeleteAccountValidationSummary" runat="server" CssClass="failureNotification" 
            ValidationGroup="DeleteAccountValidationGroup" />

    <div class="accountInfo">

        <fieldset class="deleteAccount">
            <legend>Account Information</legend>
            <p>
                <asp:Label ID="lblCurrentPasswordLabel" runat="server" AssociatedControlID="txtCurrentPassword">Password:</asp:Label>
                <asp:TextBox ID="txtCurrentPassword" runat="server" CssClass="passwordEntry" TextMode="Password"></asp:TextBox>
                <asp:RequiredFieldValidator ID="CurrentPasswordRequired" runat="server" ControlToValidate="txtCurrentPassword" 
                        CssClass="failureNotification" ErrorMessage="Password is required." ToolTip="Enter your password" 
                        ValidationGroup="DeleteAccountValidationGroup">*</asp:RequiredFieldValidator>
                <asp:CustomValidator ID="ValidatePassword" OnServerValidate="ValidatePassword_ServerValidate" ValidationGroup="DeleteAccountValidationGroup" 
                    ControlToValidate="txtCurrentPassword" Text="Bad password" runat="server" ValidateEmptyText="False" EnableClientScript="false"><span class="failureNotification">* Bad password</span></asp:CustomValidator>
            </p>
            <p>
                <asp:Label ID="lblConfirm" runat="server" AssociatedControlID="txtConfirm">Confirm account deletion by typing "DELETE MY ACCOUNT" in all capital letters without the quotes:</asp:Label>
                <asp:TextBox ID="txtConfirm" runat="server" CssClass="passwordEntry"></asp:TextBox>
                <asp:RequiredFieldValidator ID="ValidateIntentReq" runat="server" ControlToValidate="txtConfirm" 
                        CssClass="failureNotification" ErrorMessage="Intent text is required." ToolTip="Declare your intent" 
                        ValidationGroup="DeleteAccountValidationGroup">*</asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator id="ValidateIntent" 
                                        ControlToValidate="txtConfirm"
                                        ValidationExpression="^DELETE MY ACCOUNT$"
                                        CssClass="failureNotification"
                                        ErrorMessage="You must type 'DELETE MY ACCOUNT' in all capital letters without the quotes to declare your intent."
                                        ToolTip="Declare your intent by typing it out" 
                                        runat="server"
                                        ValidationGroup="DeleteAccountValidationGroup">*</asp:RegularExpressionValidator>
            </p>
        </fieldset>
        <menu>
            <p class="submitButton">
                <asp:Button ID="butDeleteAccount" OnClick="butDeleteAccount_Click" OnClientClick="if (!confirm('Are you sure you want delete your account?')) return false;" runat="server" 
                    Text="Delete account" ValidationGroup="DeleteAccountValidationGroup" CausesValidation="true" />
                <asp:Button ID="CancelPushButton" OnClick="CancelPushButton_Click" runat="server" CausesValidation="False" Text="Cancel" />
            </p>
        </menu>
    </div>

</asp:Content>
