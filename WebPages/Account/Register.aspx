<%@ Page Title="Register" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Register.aspx.cs" Inherits="VeraWAF.WebPages.Account.Register" %>
<%@ Register src="../Controls/MarketingResearch.ascx" tagname="MarketingResearch" tagprefix="uc1" %>
<%@ Register src="~/Controls/SocialSignIn.ascx" tagname="ThirdPartySignIn" tagprefix="uc1" %>
<%@ Register src="~/Controls/Submit.ascx" tagname="SubmitForm" tagprefix="uc1" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <uc1:ThirdPartySignIn runat="server"></uc1:ThirdPartySignIn>

    <CompressStyles runat="server">
        register.css
        account.css
        marketingform.css
    </CompressStyles>

    <script>
        $(document).ready(function () {
            $('#MainContent_RegisterUser_CreateUserStepContainer_CreateUserButton').attr('disabled', !$('#confirm').attr("checked"));
            $('#confirm').click(function () {
                $('#MainContent_RegisterUser_CreateUserStepContainer_CreateUserButton').attr('disabled',
                !$('#confirm').attr("checked") );
            });

            $('form').attr('autocomplete', 'off');
            $('form').preventDoubleSubmission();

            $('input#MainContent_RegisterUser_CreateUserStepContainer_PasswordLabel').attr('autocomplete', 'off');
            $('input#MainContent_RegisterUser_CreateUserStepContainer_ConfirmPassword').attr('autocomplete', 'off');            
        });
    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div class="hideDisabled">
        <asp:CreateUserWizard  ID="RegisterUser" runat="server" EnableViewState="true" ViewStateMode="Enabled" OnCreatedUser="RegisterUser_CreatedUser">
            <LayoutTemplate>
                <asp:PlaceHolder ID="wizardStepPlaceholder" runat="server"></asp:PlaceHolder>
                <asp:PlaceHolder ID="navigationPlaceholder" runat="server"></asp:PlaceHolder>
            </LayoutTemplate>
            <WizardSteps>
                <asp:CreateUserWizardStep ID="RegisterUserWizardStep" runat="server">
                    <ContentTemplate>
                        <header>
                            <h1>
                                <span class="fa-stack fa-lg">
                                    <i style="margin-left:-0.5em" class="fa fa-user fa-stack-1x"></i>
                                    <i style="color:#2b96fe; font-size:0.8em" class="fa fa-magic fa-stack-1x"></i>
                                </span>
                                <span style="margin-left:-0.5em">Create a new account</span>
                            </h1>
                            <p>
                                Use the form below to create your new user account.
                            </p>
                        </header>
                        <span class="failureNotification">
                            <asp:Literal ID="ErrorMessage" runat="server"></asp:Literal>
                        </span>
                        <asp:ValidationSummary ID="RegisterUserValidationSummary" runat="server" CssClass="failureNotification" 
                             ValidationGroup="RegisterUserValidationGroup"/>
                        <div class="accountInfo">

                           <div class="accountInfo-container">

                               <div class="local-account">
                                    <fieldset ID="fsAccountInfo" class="register" runat="server">
                                        <legend>Account Information</legend>
                                        <p ID="pUserName" runat="server">
                                            <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName">User name:</asp:Label>
                                            <asp:TextBox AutoCompleteType="DisplayName" ID="UserName" runat="server" CssClass="textEntry"></asp:TextBox>
                                            <asp:RequiredFieldValidator ControlToValidate="UserName" CssClass="failureNotification" Display="Dynamic" 
                                                 ErrorMessage="User name is required." ID="UserNameRequired" runat="server" 
                                                 ToolTip="User name is required." ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
                                            <asp:RegularExpressionValidator id="BadUserName1" 
                                                                 ControlToValidate="UserName"
                                                                 ValidationExpression="^[^<^>^'^\x22^:^\x3f^=]*$"
                                                                 CssClass="failureNotification"
                                                                 ErrorMessage="User name cannot contain ?.':=<>&#34;"
                                                                 ToolTip="Username is required." 
                                                                 runat="server"
                                                                 ValidationGroup="RegisterUserValidationGroup">*</asp:RegularExpressionValidator>
                                            <asp:RegularExpressionValidator id="BadUserName2" 
                                                                 ControlToValidate="UserName"
                                                                 ValidationExpression="^[\s\S]{4,30}$"
                                                                 CssClass="failureNotification"
                                                                 ErrorMessage="User name must be between 4 and 30 character long."
                                                                 ToolTip="Username is required." 
                                                                 runat="server"
                                                                 ValidationGroup="RegisterUserValidationGroup">*</asp:RegularExpressionValidator>

                                        </p>
                                        <p>
                                            <asp:Label ID="EmailLabel" runat="server" AssociatedControlID="Email">E-mail:</asp:Label>
                                            <asp:TextBox AutoCompleteType="Email" ID="Email" runat="server" CssClass="textEntry"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="EmailRequired" runat="server" ControlToValidate="Email" 
                                                 CssClass="failureNotification" ErrorMessage="E-mail is required." ToolTip="E-mail is required." 
                                                 ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
                                            <asp:RegularExpressionValidator id="BadEmail" 
                                                                 ControlToValidate="Email"
                                                                 ValidationExpression="^([\w\!\#$\%\&\'\*\+\-\/\=\?\^\`{\|\}\~]+\.)*[\w\!\#$\%\&\'\*\+\-\/\=\?\^\`{\|\}\~]+@((((([a-zA-Z0-9]{1}[a-zA-Z0-9\-]{0,62}[a-zA-Z0-9]{1})|[a-zA-Z])\.)+[a-zA-Z]{2,6})|(\d{1,3}\.){3}\d{1,3}(\:\d{1,5})?)$"
                                                                 CssClass="failureNotification"
                                                                 ErrorMessage="Bad e-mail address."
                                                                 ToolTip="Bad e-mail." 
                                                                 runat="server"
                                                                 ValidationGroup="RegisterUserValidationGroup">*</asp:RegularExpressionValidator>                            
                                        </p>
                                        <p>
                                            <asp:Label ID="ConfirmEmailLabel" runat="server" AssociatedControlID="ConfirmEmail">Confirm e-mail:</asp:Label>
                                            <asp:TextBox AutoCompleteType="Disabled" ID="ConfirmEmail" runat="server" CssClass="textEntry"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="ConfirmEmailRequired" runat="server" ControlToValidate="ConfirmEmail" 
                                                 CssClass="failureNotification" ErrorMessage="Confirm e-mail field is required." ToolTip="Confirm e-mail field is required." 
                                                 ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
                                            <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="Email" ControlToValidate="ConfirmEmail" 
                                                 CssClass="failureNotification" Display="Dynamic" ErrorMessage="The e-mail addresses does not match."
                                                 ValidationGroup="RegisterUserValidationGroup">*</asp:CompareValidator>
                                            <asp:Label ID="ContactEmailLabel" runat="server" AssociatedControlID="ContactEmail" CssClass="labelEntryHp">Secondary e-mail:</asp:Label>
                                            <asp:TextBox AutoCompleteType="Email" ID="ContactEmail" runat="server" CssClass="textEntryHp"></asp:TextBox>
                                        </p>
                                        <asp:Panel ID="panPassword" runat="server">
                                            <p>
                                                <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password">Password:</asp:Label>
                                                <asp:TextBox AutoCompleteType="Disabled" ID="Password" runat="server" CssClass="passwordEntry" TextMode="Password"></asp:TextBox>
                                                <asp:RegularExpressionValidator id="PasswordTooShort" 
                                                                     ControlToValidate="Password"
                                                                     ValidationExpression="^[\s\S]{6,50}$"
                                                                     CssClass="failureNotification"
                                                                     ErrorMessage="Password must be a minimum of 6 characters in length."
                                                                     ToolTip="Password is too short." 
                                                                     runat="server"
                                                                     ValidationGroup="RegisterUserValidationGroup">*</asp:RegularExpressionValidator>

                                                <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password" 
                                                     CssClass="failureNotification" ErrorMessage="Password is required." ToolTip="Password is required." 
                                                     ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>

                                            </p>
                                            <p>
                                                <asp:Label ID="ConfirmPasswordLabel" runat="server" AssociatedControlID="ConfirmPassword">Confirm password:</asp:Label>
                                                <asp:TextBox AutoCompleteType="Disabled" ID="ConfirmPassword" runat="server" CssClass="passwordEntry" TextMode="Password"></asp:TextBox>
                                                <asp:RequiredFieldValidator ControlToValidate="ConfirmPassword" CssClass="failureNotification" Display="Dynamic" 
                                                     ErrorMessage="Confirm Password is required." ID="ConfirmPasswordRequired" runat="server" 
                                                     ToolTip="Confirm Password is required." ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
                                                <asp:CompareValidator ID="PasswordCompare" runat="server" ControlToCompare="Password" ControlToValidate="ConfirmPassword" 
                                                     CssClass="failureNotification" Display="Dynamic" ErrorMessage="The Password and Confirmation Password must match."
                                                     ValidationGroup="RegisterUserValidationGroup">*</asp:CompareValidator>
                                            </p>
                                        </asp:Panel>
                                    </fieldset>
                        
                                    <fieldset ID="fsPasswordRecovery" class="register clear" style="margin:1em 0 2em" runat="server">
                                        <legend>For password recovery purposes</legend>

                                        <p>
                                            <asp:Label ID="QuestionLabel" runat="server" AssociatedControlID="Question">Security question:</asp:Label>
                                            <asp:TextBox AutoCompleteType="Disabled" ID="Question" runat="server" CssClass="textEntry" Text="What was the name of your first pet?"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="QuestionRequired" runat="server" ControlToValidate="Question"
                                                CssClass="failureNotification" ErrorMessage="Security question is required." ToolTip="Security question is required."
                                                ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
                                        </p>
                                        <p>
                                            <asp:Label ID="AnswerLabel" runat="server" AssociatedControlID="Answer">Security answer:</asp:Label>
                                            <asp:TextBox AutoCompleteType="Disabled" ID="Answer" runat="server" CssClass="textEntry"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="AnswerRequired" runat="server" ControlToValidate="Answer"
                                                CssClass="failureNotification" ErrorMessage="Security answer is required." ToolTip="Security answer is required."
                                                ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>                            
                                        </p>
                                    </fieldset>
                            
                                    <p class="newsletter clear">
                                        <asp:CheckBox runat="server" ID="Newsletter" Checked="true" />
                                        <asp:Label ID="NewsletterLabel" runat="server" AssociatedControlID="Newsletter">Subscribe to newsletter</asp:Label>
                                    </p>
                            
                                    <p class="confirm clear">
                                        <input type="checkbox" id="confirm" />
                                        <label for="confirm">I agree with the <a href="/Terms.aspx" target="_blank">Terms of Service</a> and <a href="/Privacy.aspx" target="_blank">Privacy Policies</a>.</label>
                                    </p>
                                    <br />
                                    <p class="submitButton">
                                        <asp:Button ID="CreateUserButton" runat="server" CommandName="MoveNext" Text="Create Account" 
                                        ValidationGroup="RegisterUserValidationGroup"/>
                                    </p>
                                    <div class="hide">
                                        <asp:TextBox ID="txtSignInProviderName" runat="server" Enabled="False"></asp:TextBox>
                                        <asp:CheckBox ID="chkEmailIsValidated" runat="server" Enabled="False" />
                                        <asp:TextBox ID="txtGender" runat="server" Enabled="False"></asp:TextBox>
                                        <asp:TextBox ID="txtFullName" runat="server" Enabled="False"></asp:TextBox>
                                        <asp:TextBox ID="txtPhoto" runat="server" Enabled="False"></asp:TextBox>
                                    </div>
                                </div>

                                <asp:Panel ID="panThirdPartyRegister" CssClass="thirdPartyRegister" runat="server">
                                    <span style="font-weight:bold;display:inline-block;vertical-align: top; margin: 3em 1em 0 0">OR</span>
                                    <div style="display:inline-block;vertical-align: top;margin-top: 2.3em" id="janrainEngageEmbed"></div>
                                    <noscript>
                                        <div style="padding-top:7em;width:300px;text-align:center;float:left;font-style:italic">You must enable Javascript in your web browser to register using Facebook, Google, Twitter, OpenID, LinkedIn, or Yahoo.</div>
                                    </noscript>
                                </asp:Panel>

                            </div>
                        </div>
                    </ContentTemplate>
                    <CustomNavigationTemplate>
                    </CustomNavigationTemplate>
                </asp:CreateUserWizardStep>
                <asp:CompleteWizardStep ID="CompleteWizardStep1" runat="server">
                    <ContentTemplate>
                        <div class="accountInfo">
                            <fieldset class="register" style="width: 67em">
                                <legend>Successful registration</legend>
                                <p ID="pSuccess" runat="server">
                                    You have successfully created a new <%=ConfigurationManager.AppSettings["ApplicationName"]%> user account.
                                </p>
                                <asp:Panel ID="panEmailSentMessage" runat="server">
                                    <p>
                                        Please confirm your identity by visiting the link that was just e-mailed to 
                                        <em style="color:#000000">
                                            <asp:Literal ID="litUserEmail" runat="server"></asp:Literal>
                                        </em>.
                                    </p>
                                    <p>
                                        This little security check helps keep <%=ConfigurationManager.AppSettings["ApplicationName"]%> a community of real people using real identities.
                                    </p>
                                </asp:Panel>
                            </fieldset>

                            <div class="clear">
                                <uc1:MarketingResearch ID="MarketingResearch1" runat="server" />
                            </div>
                            
                            <div class="clear">
                               <asp:Button ID="butContinue" Text="Continue > " runat="server" CssClass="clear" OnClick="Continue_OnClick" />
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:CompleteWizardStep>
            </WizardSteps>
        </asp:CreateUserWizard>
    </div>
</asp:Content>
