<%@ Page Title="Users" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="Users.aspx.cs" Inherits="VeraWAF.WebPages.AccessControl.Users" ValidateRequest="false" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <header>
        <h1>
            <i class="fa fa-user"></i> Users
        </h1>
    </header>
    <div class="info">
        <fieldset class="info">
            <legend>General  info</legend>
            <p>
                <asp:Label ID="NumberOfUsersLabel" runat="server" AssociatedControlID="NumberOfUsers">Total number of users:</asp:Label>
                <asp:Literal ID="NumberOfUsers" runat="server"></asp:Literal>
            </p>
            <p>
                <asp:Label ID="NumberOfUsersOnlineLabel" runat="server" AssociatedControlID="NumberOfUsersOnline">Number of users currently online:</asp:Label>
                <asp:Literal ID="NumberOfUsersOnline" runat="server"></asp:Literal>
            </p>
        </fieldset>

        <fieldset class="info" style="display:block;">
            <legend>User list</legend>

            <asp:Panel id="NavigationPanel" Visible="false" runat="server">
                <table border="0" cellpadding="3" cellspacing="3">
                    <tr>
                        <td>
                            <asp:Label ID="lblFilter" runat="server" AssociatedControlID="txtQueryFilter">Filter:</asp:Label>
                            <asp:TextBox ID="txtQueryFilter" runat="server"></asp:TextBox>
                            <asp:Button UseSubmitBehavior="true" runat="server" Text="Submit" CssClass="navigationPanel" />
                        </td>
                        <td>Page <asp:Label id="CurrentPageLabel" runat="server" /> of <asp:Label id="TotalPagesLabel" runat="server" /></td>
                        <td><asp:LinkButton id="PreviousButton" Text="< Previous page" OnClick="PreviousButton_OnClick" runat="server" /></td>
                        <td><asp:LinkButton id="NextButton" Text="Next page >" OnClick="NextButton_OnClick" runat="server" /></td>
                    </tr>
                </table>            
            </asp:Panel>

            <asp:DataGrid id="UserGrid" runat="server" CellPadding="2" CellSpacing="1" Gridlines="Both" OnEditCommand="UsersGrid_Edit"
                AutoGenerateColumns="false">
                
                <HeaderStyle BackColor="darkblue" ForeColor="white" />
                <EditItemStyle BackColor="yellow" />

                <Columns>
                    <asp:BoundColumn HeaderText="ID" DataField="PartitionKey" HeaderStyle-CssClass="hide" ItemStyle-CssClass="hide" />

                    <asp:TemplateColumn HeaderText="Username">
                        <ItemTemplate>
                            <%# ClipString((string)Eval("Username"), 20)%>
                        </ItemTemplate>
                    </asp:TemplateColumn>

                    <asp:TemplateColumn HeaderText="Full name">
                        <ItemTemplate>
                            <%# ClipString((string)Eval("FullName"), 30)%>
                        </ItemTemplate>
                    </asp:TemplateColumn>

                    <asp:BoundColumn HeaderText="Registered date" DataField="Timestamp" DataFormatString="{0:yyyy-MM-ddTHH:mm:ssZ}" />
                    <asp:BoundColumn HeaderText="Last active date" DataField="LastActivityDate" DataFormatString="{0:yyyy-MM-ddTHH:mm:ssZ}" />

                    <asp:TemplateColumn HeaderText="E-mail">
                        <ItemTemplate>
                            <%# FormatEmail((string)Eval("Email"))%>
                        </ItemTemplate>
                    </asp:TemplateColumn>

                    <asp:BoundColumn HeaderText="Deleted" DataField="IsDeleted" />
                    <asp:BoundColumn HeaderText="Validated e-mail" DataField="IsApproved" />
                    <asp:BoundColumn HeaderText="Locked out" DataField="IsLockedOut" />
                    <asp:BoundColumn HeaderText="Social points" DataField="SocialPoints" />

                    <asp:EditCommandColumn EditText="Edit" CancelText="Cancel" UpdateText="Update" HeaderText="Edit">
                        <ItemStyle Wrap="false"></ItemStyle>
                        <HeaderStyle Wrap="false"></HeaderStyle>
                    </asp:EditCommandColumn>
                </Columns>

            </asp:DataGrid>


        </fieldset>
    </div>
</asp:Content>
