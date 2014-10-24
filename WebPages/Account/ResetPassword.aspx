<%@ Page Title="Reset Password" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="ResetPassword.aspx.cs" Inherits="VeraWAF.WebPages.Account.ResetPassword" ViewStateMode="Enabled" EnableViewState="true" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <CompressStyles CompressedFile="account.min.css" runat="server">
        account.css
    </CompressStyles>
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent" ViewStateMode="Enabled" EnableViewState="true">
    <asp:Wizard ID="ResetPasswordWizard" runat="server" DisplaySideBar="false" OnNextButtonClick="OnNextButtonClick" CancelButtonType="Link" CancelDestinationPageUrl="~/"
        OnFinishButtonClick="OnNextButtonClick" FinishCompleteButtonText="Submit" ViewStateMode="Enabled" EnableViewState="true">
        <WizardSteps>
            <asp:WizardStep ID="IdentifyUserWizardStep" runat="server" Title="Identify User" StepType="Start" EnableViewState="true">
                <header>
                    <h1>
                        Reset your password - Step 1 of 3
                    </h1>
                    <p>
                        Use the form below to reset your password, this is usefull if you have forgotten it. The new password is e-mailed to you.
                    </p>
                </header>
                <p>
                    Please enter your account name or your e-mail address to help identify your account.
                </p>
                <asp:ValidationSummary ID="ResetPasswordValidationSummary1" runat="server" CssClass="failureNotification" ValidationGroup="ResetPasswordValidationGroup1"/>
                <div class="resetPassword">
                    <fieldset class="resetPassword">
                        <legend>Identify user</legend>
                        <p>
                            <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserNameOrEmail">User Name or E-Mail:</asp:Label>
                            <asp:TextBox ID="UserNameOrEmail" runat="server" CssClass="textEntry" ViewStateMode="Enabled"></asp:TextBox>
                            <asp:RequiredFieldValidator ControlToValidate="UserNameOrEmail" CssClass="failureNotification" 
                                    ErrorMessage="User name or e-mail is required." ID="UserNameOrEmailRequired" runat="server" 
                                    ToolTip="User name or e-mail is required." ValidationGroup="ResetPasswordValidationGroup1">*</asp:RequiredFieldValidator>
                            <asp:CustomValidator ControlToValidate="UserNameOrEmail" CssClass="failureNotification" OnServerValidate="ValidateUserNameOrEmail" 
                                    ErrorMessage="No matching user name or e-mail was found." ID="UserNameOrEmailNotFound" runat="server" 
                                    ToolTip="No matching user name or e-mail." ValidationGroup="ResetPasswordValidationGroup1">*</asp:CustomValidator>
                        </p>
                    </fieldset>
                </div>
            </asp:WizardStep>
            <asp:WizardStep ID="AnswerQuestionWizardStep" runat="server" Title="Answer Question" StepType="Finish" EnableViewState="true">
                <header>
                    <h1>
                        Reset your password - Step 2 of 3
                    </h1>
                    <p>
                        Please answer the question to reset your password.
                    </p>
                </header>
                <asp:ValidationSummary ID="ResetPasswordValidationSummary2" runat="server" CssClass="failureNotification" ValidationGroup="ResetPasswordValidationGroup2"/>
                <div class="resetPassword">
                    <fieldset class="resetPassword">
                        <legend>Answer a question</legend>
                        <p>
                            <asp:Literal ID="PasswordQuestion" runat="server"></asp:Literal>?
                        </p>
                        <p>
                            <asp:Label ID="PasswordAnswerLabel" runat="server" AssociatedControlID="UserNameOrEmail">Answer:</asp:Label>
                            <asp:TextBox ID="PasswordAnswer" runat="server" CssClass="textEntry"></asp:TextBox>
                            <asp:RequiredFieldValidator ControlToValidate="PasswordAnswer" CssClass="failureNotification" 
                                    ErrorMessage="Answer is required." ID="PasswordAnswerRequired" runat="server" 
                                    ToolTip="Answer is required." ValidationGroup="ResetPasswordValidationGroup2">*</asp:RequiredFieldValidator>
                            <asp:CustomValidator ControlToValidate="PasswordAnswer" CssClass="failureNotification" 
                                    ErrorMessage="The answer was wrong." ID="RequiredFieldValidator1" runat="server" OnServerValidate="ValidateAnswer" 
                                    ToolTip="Wrong answer." ValidationGroup="ResetPasswordValidationGroup2">*</asp:CustomValidator>
                        </p>
                    </fieldset>
                </div>
            </asp:WizardStep>
            <asp:WizardStep ID="CompleteWizardStep" runat="server" Title="Confirmation" StepType="Complete" EnableViewState="true">
                <header>
                    <h1>
                        Reset your password - Step 3 of 3
                    </h1>
                </header>
                <div class="resetPassword">
                    <fieldset class="resetPassword">
                        <legend>Password was reset</legend>
                        <p>
                            Your account password was reset. An e-mail with your new password was sent to you.
                        </p>
                    </fieldset>
                    <p class="submitButton">
                        <asp:Button ID="ContinueButton" runat="server" CausesValidation="False" PostBackUrl="~/Account/Login.aspx" Text="Sign In" />
                    </p>
                </div>
            </asp:WizardStep>
        </WizardSteps>
    </asp:Wizard>
</asp:Content>
