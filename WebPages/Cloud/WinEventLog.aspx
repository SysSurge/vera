<%@ Page Language="C#" Title="View Windows Event log" MasterPageFile="~/Site.master"
    AutoEventWireup="true" CodeBehind="WinEventLog.aspx.cs" Inherits="VeraWAF.WebPages.Cloud.WinEventLog" ValidateRequest="false"%>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h1>
        <i class="fa fa-archive"></i> Windows Event log
    </h1>
    <div class="info">
        <fieldset class="info">
            <legend>Log items</legend>

            <asp:Panel ID="NavigationPanel" runat="server">
                <table border="0" cellpadding="3" cellspacing="3">
                    <tr>
                        <td>
                            <asp:Label ID="lblTypeFilter" runat="server" AssociatedControlID="cmbTypeFilter">Level type filter:</asp:Label>
                            <asp:DropDownList id="cmbTypeFilter" runat="server" EnableViewState="true" ViewStateMode="Enabled" 
                                AppendDataBoundItems="false" AutoPostBack="true" >
                                <asp:ListItem Text="-- No filter --" Value=""></asp:ListItem>
                                <asp:ListItem Text="Critical" Value="1"></asp:ListItem>
                                <asp:ListItem Text="Error" Value="2"></asp:ListItem>
                                <asp:ListItem Text="Warning" Value="3"></asp:ListItem>
                                <asp:ListItem Text="Informational" Value="4"></asp:ListItem>
                                <asp:ListItem Text="Verbose" Value="5"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Button ID="clearLog" OnClick="clearLog_Click" runat="server" Text="Clear log" />
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
            
            <asp:DataGrid ID="WinEventLogGrid" runat="server" CellPadding="2" CellSpacing="1"
                GridLines="Both" OnEditCommand="WinEventLogGrid_Edit" AutoGenerateColumns="false">
                <HeaderStyle BackColor="darkblue" ForeColor="white" />
                <EditItemStyle BackColor="yellow" />
                <Columns>
                    <asp:BoundColumn HeaderText="ID" DataField="PartitionKey" HeaderStyle-CssClass="hide" ItemStyle-CssClass="hide" />

                    <asp:TemplateColumn HeaderText="Log item type">
                        <ItemTemplate>
                            <%# GetLogItemType((System.Int32)Eval("Level"))%>
                        </ItemTemplate>
                    </asp:TemplateColumn>

                    <asp:BoundColumn HeaderText="Timestamp" DataField="Timestamp" DataFormatString="{0:yyyy-MM-ddTHH:mm:ssZ}" />

                    <asp:BoundColumn HeaderText="Role" DataField="Role" />
                    <asp:BoundColumn HeaderText="Role instance" DataField="RoleInstance" />
                    <asp:BoundColumn HeaderText="Provider name" DataField="ProviderName" />
                    <asp:BoundColumn HeaderText="Channel" DataField="Channel" />

                    <asp:TemplateColumn HeaderText="Description">
                        <ItemTemplate>
                            <%# Server.HtmlEncode(new VeraWAF.WebPages.Bll.TextUtilities().ClipLeft((string)Eval("Description"), 70))%>
                        </ItemTemplate>
                    </asp:TemplateColumn>

                    <asp:EditCommandColumn EditText="View" CancelText="Cancel" UpdateText="Update" HeaderText="View">
                        <ItemStyle Wrap="false"></ItemStyle>
                        <HeaderStyle Wrap="false"></HeaderStyle>
                    </asp:EditCommandColumn>
                </Columns>
            </asp:DataGrid>
        </fieldset>
    </div>
</asp:Content>
