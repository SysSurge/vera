<%@ Page Title="Roles" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="Roles.aspx.cs" Inherits="VeraWAF.WebPages.AccessControl.Roles_" %>
<%@ Register src="~/Controls/FormNotification.ascx" tagname="FormNotification" tagprefix="uc1" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <header>
        <h1>
            <i class="fa fa-users"></i> Roles
        </h1>
    </header>

    <uc1:FormNotification id="notifications" runat="server"></uc1:FormNotification>

    <div class="info">
        <fieldset class="info">
            <legend>Roles list</legend>
              <asp:Panel id="NavigationPanel" Visible="false" runat="server">

                <table border="0" cellpadding="3" cellspacing="3">
                  <tr>
                    <td>Page <asp:Label id="CurrentPageLabel" runat="server" /> of <asp:Label id="TotalPagesLabel" runat="server" /></td>
                    <td><asp:LinkButton id="PreviousButton" Text="< Previous page" OnClick="PreviousButton_OnClick" runat="server" /></td>
                    <td><asp:LinkButton id="NextButton" Text="Next page >" OnClick="NextButton_OnClick" runat="server" /></td>
                  </tr>
                </table>
              </asp:Panel>
              <asp:DataGrid id="RolesGrid" runat="server" CellPadding="2" CellSpacing="1" Gridlines="Both" AutoGenerateColumns="false"
                  oneditcommand="RolesGrid_EditCommand">
                <HeaderStyle BackColor="darkblue" ForeColor="white" />

                <Columns>
                    <asp:BoundColumn HeaderText="ID" DataField="PartitionKey" HeaderStyle-CssClass="hide" ItemStyle-CssClass="hide" />

                    <asp:BoundColumn HeaderText="Role name" DataField="RoleName" />

                    <asp:TemplateColumn HeaderText="Members">
                        <ItemTemplate>
                            <%# ClipString(GetMembers((string)Eval("RoleName")), 250)%>
                        </ItemTemplate>
                    </asp:TemplateColumn>

                    <asp:BoundColumn HeaderText="Created date" DataField="CreationDate" DataFormatString="{0:yyyy-MM-ddTHH:mm:ssZ}" />
                    <asp:BoundColumn HeaderText="Modifed date" DataField="Timestamp" DataFormatString="{0:yyyy-MM-ddTHH:mm:ssZ}" />

                    <asp:EditCommandColumn EditText="Edit" CancelText="Cancel" UpdateText="Update" HeaderText="Edit">
                        <ItemStyle Wrap="false"></ItemStyle>
                        <HeaderStyle Wrap="false"></HeaderStyle>
                    </asp:EditCommandColumn>
                </Columns>
              </asp:DataGrid>
            
            <fieldset>
                <legend>Role management</legend>
                <p>
                    <asp:Label ID="lblRoleName" runat="server" AssociatedControlID="txtRoleName" 
                        Text="Role name:"></asp:Label>
                    <asp:TextBox ID="txtRoleName" Width="98%" runat="server"></asp:TextBox>
                </p>
                <menu>
                    <asp:Button runat="server" ID="btnAddRole" Text="Add role" OnClick="btnAddRole_Click"/>
                    <asp:Button runat="server" ID="btnRemoveRole" Text="Remove role" OnClick="btnRemoveRole_Click" />
                </menu>
            </fieldset>

        </fieldset>
    </div>
</asp:Content>
