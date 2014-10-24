<%@ Page Title="View comments" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ViewComments.aspx.cs" Inherits="VeraWAF.WebPages.Edit.ViewComments" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h1>
        <i class="fa fa-comments-o"></i> View comments
    </h1>
    <div class="info">
        <fieldset class="info">
            <legend>Comment list</legend>
            <asp:Panel ID="NavigationPanel" Visible="false" runat="server">
                <table border="0" cellpadding="3" cellspacing="3">
                    <tr>
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
                GridLines="Both" OnEditCommand="CommentsGrid_Edit" AutoGenerateColumns="false">
                <HeaderStyle BackColor="darkblue" ForeColor="white" />
                <EditItemStyle BackColor="yellow" />
                <Columns>
                    <asp:BoundColumn HeaderText="Virtual Path" DataField="VirtualPath" />
                    <asp:BoundColumn HeaderText="Row Key" DataField="RowKey" Visible="False" />
                    <asp:BoundColumn HeaderText="Author" DataField="Author" />
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
