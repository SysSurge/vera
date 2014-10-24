<%@ Page Title="User Profile Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="VeraWAF.WebPages.Account.UserProfile" ValidateRequest="false" %>
<%@ Register src="~/Controls/ContactUser.ascx" tagname="ContactUser" tagprefix="uc1" %>
<%@ Register src="~/Controls/Submit.ascx" tagname="SubmitForm" tagprefix="uc1" %>
<%@ Register src="~/Controls/UserProfileField.ascx" tagname="UserProfileField" tagprefix="uc1" %>
<%@ Register src="~/Controls/UserMemberSinceDate.ascx" tagname="UserMemberSinceDate" tagprefix="uc1" %>
<%@ Register src="~/Controls/UserLastActiveDate.ascx" tagname="UserLastActiveDate" tagprefix="uc1" %>
<%@ Register src="~/Controls/UserRoles.ascx" tagname="UserRoles" tagprefix="uc1" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <CompressStyles runat="server">
        profile.css
        table.css
    </CompressStyles>
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <header>
        <h1>
            <i class="fa fa-male"></i> <uc1:UserProfileField UserIdFromRequestParam="id" PropertyName="Username" HtmlEncode="true" runat="server"></uc1:UserProfileField>'s Profile Page
        </h1>
    </header>
    <div class="profile">
        <div class="clear">
            <menu>
                <asp:Button CssClass="submitButton" ID="SubmitButton" runat="server" OnClick="SubmitButton_Click" Text="Update profile" />
                <asp:HyperLink ID="lnkPublicProfile" CssClass="buttonFace publicProfile" runat="server" Visible="False"><i style="margin-right:1em" class="fa fa-chain"></i>See your public profile</asp:HyperLink>
            </menu>
        </div>

        <div class="profileMetaData clear">
            <span class="socialScore clear">
                Social score is <var><uc1:UserProfileField UserIdFromRequestParam="id" PropertyName="SocialPoints" runat="server"></uc1:UserProfileField></var> points.
            </span>
            <br />
            <br />
            <span>
                Member since <uc1:UserMemberSinceDate UserIdFromRequestParam="id" runat="server"></uc1:UserMemberSinceDate>.
            </span>
            <br/>
            <span>
                Last activity was <uc1:UserLastActiveDate UserIdFromRequestParam="id" runat="server"></uc1:UserLastActiveDate>.
            </span>
            <br/>
            Roles: <uc1:UserRoles UserIdFromRequestParam="id" DisplayMode="Csv" runat="server"></uc1:UserRoles>.
        </div>

        <fieldset class="information">
            <legend>
                User profile
            </legend>
            
            <img ID="imgPortrait" src="" alt="" runat="server" style="float:left;margin:0 1em 1em" />

            <fieldset ID="fsPortrait" class="portrait" runat="server">
                <legend>
                    Portrait
                </legend>
                <p>
                    <asp:Image ID="PortraitImage" runat="server" CssClass="userPortrait64x64" ImageUrl="~/Images/none.png"></asp:Image>
                </p>
                <asp:Panel runat="server" ID="UploadPortrait">
                    <p>
                        <asp:Label ID="PortraitUploadControlLabel" runat="server" AssociatedControlID="PortraitUploadControl">Change your portrait:</asp:Label>
                        <asp:FileUpload ID="PortraitUploadControl" runat="server" />
                        <asp:Button id="PortraitSubmitButton" Text="Upload image" OnClick="PortraitSubmitButton_Click" runat="server"/>
                    </p>
                </asp:Panel>
            </fieldset>

            <span ID="pUserName" runat="server">
                <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName">User name:</asp:Label>
                <uc1:UserProfileField ID="UserName" UserIdFromRequestParam="id" PropertyName="Username" HtmlEncode="true" runat="server"></uc1:UserProfileField>
            </span>
            <span ID="pFullName" runat="server">
                <asp:Label ID="FullNameLabel" runat="server" AssociatedControlID="FullName">Full name:</asp:Label>
                <asp:Literal ID="FullNameLiteral" runat="server"></asp:Literal>
                <asp:TextBox ID="FullName" runat="server"></asp:TextBox>
            </span>
            <span ID="pEmail" runat="server">
                <asp:Label ID="EmailLabel" runat="server" AssociatedControlID="Email">E-mail:</asp:Label>
                <asp:TextBox ID="Email" runat="server" ReadOnly="true"></asp:TextBox>
            </span>
            <span ID="pEmployer" runat="server">
                <asp:Label ID="EmployerLabel" runat="server" AssociatedControlID="Employer">Employer:</asp:Label>
                <uc1:UserProfileField UserIdFromRequestParam="id" PropertyName="Employer" HtmlEncode="true" runat="server"></uc1:UserProfileField>
                <asp:TextBox ID="Employer" runat="server"></asp:TextBox>
            </span>
            <div ID="pGender" runat="server">
                <asp:Label ID="GenderLabel" runat="server" AssociatedControlID="PublicGender">Gender:</asp:Label>
                <asp:Literal ID="PublicGender" runat="server"></asp:Literal>
                <asp:DropDownList id="PrivateGender" runat="server" EnableViewState="true" ViewStateMode="Enabled"></asp:DropDownList>
            </div>
            <span ID="pDescription" runat="server">
                <asp:Label ID="DescriptionLabel" runat="server" AssociatedControlID="Description">About me:</asp:Label>
                <uc1:UserProfileField ID="Description1" PropertyName="Description" HtmlEncode="true" runat="server"></uc1:UserProfileField>
                <asp:TextBox TextMode="MultiLine" Columns="40" Rows="6" ID="Description" runat="server"></asp:TextBox>
            </span>
            
        </fieldset>

        <fieldset ID="fsOauth" class="information" style="width: 25em" visible="false" runat="server">
            <legend>
                OAuth security data for API access
            </legend>
            <p>
                <asp:Label ID="lblOAuthConsumerKey" runat="server" AssociatedControlID="OAuthConsumerKey">Consumer key:</asp:Label>
                <uc1:UserProfileField ID="OAuthConsumerKey" PropertyName="OAuthConsumerKey" runat="server"></uc1:UserProfileField>
            </p>
            <p>
                <asp:Label ID="lblOAuthConsumerSecret" runat="server" AssociatedControlID="OAuthConsumerSecret">Consumer secret:</asp:Label>
                <asp:Literal ID="hiddenSecret" runat="server" Text="********-****-****-****-************<br/>"></asp:Literal>
                
                <asp:Button id="butShowConsumerSecret" OnClick="butShowConsumerSecret_Click" runat="server" Text="Show" />
                <uc1:UserProfileField ID="OAuthConsumerSecret" PropertyName="OAuthConsumerSecret" runat="server"></uc1:UserProfileField>
            </p>
        </fieldset>
        
        <div ID="divUserOptions" runat="server">
            <p class="newsletter">
                <asp:CheckBox runat="server" ID="Newsletter" Checked="true" />
                <asp:Label ID="NewsletterLabel" runat="server" AssociatedControlID="Newsletter">I would like to recieve the <asp:Literal ID="litCompanyName1" runat="server"></asp:Literal> e-mail newsletter.</asp:Label>
            </p>

            <p class="allowContactForm">
                <asp:CheckBox runat="server" ID="AllowContactForm" Checked="true" />
                <asp:Label ID="AllowContactFormLabel" runat="server" AssociatedControlID="AllowContactForm">Allow other users to message me.</asp:Label>
            </p>
            <p><i class="fa fa-ban" style="color:#ff0000"></i> <a href="DeleteAccount.aspx">Delete account</a></p>
        </div>

        <uc1:ContactUser ID="ContactUser1" runat="server" />

        <asp:Panel ID="NavigationPanel" Visible="false" runat="server">
            <div class="forumPostList">                
                <table border="0" cellpadding="3" cellspacing="3">
                    <tr>
                        <td>
                            Showing page
                            <asp:Label ID="CurrentPageLabel" runat="server" />
                            of
                            <asp:Label ID="TotalPagesLabel" runat="server" />
                            .
                        </td>
                        <td>
                            <asp:LinkButton ID="PreviousButton" Text="< Previous page" OnClick="PreviousButton_OnClick"
                                            runat="server" />
                        </td>
                        <td>
                            <asp:LinkButton ID="NextButton" Text="Next page >" OnClick="NextButton_OnClick" runat="server" />
                        </td>
                    </tr>
                </table>
                <asp:DataGrid CssClass="blueHeader" ID="ContentPagesGrid" runat="server" CellPadding="0" CellSpacing="0" BorderWidth="0"
                          GridLines="None" AutoGenerateColumns="false">
                    <EditItemStyle BackColor="yellow" />
                    <Columns>
                        <asp:TemplateColumn HeaderText="My latest forum posts">
                            <ItemTemplate>
                                <%# Docals(Convert.ToString(DataBinder.Eval(Container.DataItem, "VirtualPath")))%>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                    </Columns>
                </asp:DataGrid>
            </div> 
        </asp:Panel>

        <asp:Panel ID="NavigationPanel2" Visible="false" runat="server">
            <div class="forumPostList clear">
                <table border="0" cellpadding="3" cellspacing="3">
                    <tr>
                        <td>
                            Showing page
                            <asp:Label ID="CurrentPageLabel2" runat="server" />
                            of
                            <asp:Label ID="TotalPagesLabel2" runat="server" />
                            .
                        </td>
                        <td>
                            <asp:LinkButton ID="PreviousButton2" Text="< Previous page" OnClick="PreviousButton2_OnClick"
                                            runat="server" />
                        </td>
                        <td>
                            <asp:LinkButton ID="NextButton2" Text="Next page >" OnClick="NextButton2_OnClick" runat="server" />
                        </td>
                    </tr>
                </table>
                <asp:DataGrid CssClass="blueHeader" ID="ContentPagesGrid2" runat="server" CellPadding="0" CellSpacing="0" BorderWidth="0"
                          GridLines="None" AutoGenerateColumns="false">
                    <EditItemStyle BackColor="yellow" />
                    <Columns>
                        <asp:TemplateColumn HeaderText="↶ Latest replies">
                            <ItemTemplate>
                                <%# Docals(Convert.ToString(DataBinder.Eval(Container.DataItem, "VirtualPath")))%>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                    </Columns>
                </asp:DataGrid>
            </div>
        </asp:Panel>
        
        <asp:Panel ID="NavigationPanel3" Visible="false" runat="server">
            <div class="forumPostList clear" property="sc:UserLikes">
                <table border="0" cellpadding="3" cellspacing="3">
                    <tr>
                        <td>
                            Showing page
                            <asp:Label ID="CurrentPageLabel3" runat="server" />
                            of
                            <asp:Label ID="TotalPagesLabel3" runat="server" />
                            .
                        </td>
                        <td>
                            <asp:LinkButton ID="PreviousButton3" Text="< Previous page" OnClick="PreviousButton3_OnClick"
                                            runat="server" />
                        </td>
                        <td>
                            <asp:LinkButton ID="NextButton3" Text="Next page >" OnClick="NextButton3_OnClick" runat="server" />
                        </td>
                    </tr>
                </table>
                <asp:DataGrid CssClass="blueHeader" ID="ContentPagesGrid3" runat="server" CellPadding="0" CellSpacing="0" BorderWidth="0"
                          GridLines="None" AutoGenerateColumns="false">
                    <EditItemStyle BackColor="yellow" />
                    <Columns>
                        <asp:TemplateColumn HeaderText="&#9733; Favorites">
                            <ItemTemplate>
                                <%# Docals2(Convert.ToString(DataBinder.Eval(Container.DataItem, "PartitionKey")))%>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                    </Columns>
                </asp:DataGrid>
            </div>
        </asp:Panel>
        
    </div>
</asp:Content>
