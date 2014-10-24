<%@ Page Language="C#" Title="View content pages" MasterPageFile="~/Site.master"
    AutoEventWireup="true" CodeBehind="ViewPages.aspx.cs" Inherits="VeraWAF.WebPages.Edit.ViewPages" ValidateRequest="false" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h1>
        View content pages
    </h1>
    <div class="info">
        <fieldset class="info">
            <legend>Content page list</legend>
            <asp:Panel ID="NavigationPanel" Visible="false" runat="server">
                <table border="0" cellpadding="3" cellspacing="3">
                    <tr>
                        <td>
                            <asp:Label ID="lblFilter" runat="server" AssociatedControlID="txtQueryFilter">Filter:</asp:Label>
                            <asp:TextBox ID="txtQueryFilter" runat="server"></asp:TextBox>
                            <asp:Button UseSubmitBehavior="true" runat="server" Text="Submit" CssClass="navigationPanel" />
                        </td>
                        <td>
                            Page
                            <asp:Label ID="CurrentPageLabel" runat="server" />
                            of
                            <asp:Label ID="TotalPagesLabel" runat="server" />
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
            </asp:Panel>
            <asp:DataGrid ID="ContentPagesGrid" runat="server" CellPadding="2" CellSpacing="1"
                GridLines="Both" OnEditCommand="ContentPagesGrid_Edit" AutoGenerateColumns="false">
                <HeaderStyle BackColor="darkblue" ForeColor="white" />
                <EditItemStyle BackColor="yellow" />
                <Columns>
                    <asp:BoundColumn HeaderText="Virtual Path" DataField="VirtualPath" />
                    <asp:BoundColumn HeaderText="Menu Item Name" DataField="MenuItemName" />
                    <asp:BoundColumn HeaderText="Menu Item Description" DataField="MenuItemDescription" />
                    <asp:BoundColumn HeaderText="Template" DataField="Template" />
                    <asp:BoundColumn HeaderText="Show in menu" DataField="ShowInMenu" />
                    <asp:BoundColumn HeaderText="Published" DataField="IsPublished" />
                    <asp:BoundColumn HeaderText="Timestamp" DataField="Timestamp" DataFormatString="{0:yyyy-MM-ddTHH:mm:ssZ}" />
                    <asp:EditCommandColumn EditText="Edit" CancelText="Cancel" UpdateText="Update" HeaderText="Edit">
                        <ItemStyle Wrap="false"></ItemStyle>
                        <HeaderStyle Wrap="false"></HeaderStyle>
                    </asp:EditCommandColumn>
                </Columns>
            </asp:DataGrid>
        </fieldset>
    </div>
</asp:Content>
