<%@ Page Language="C#" Title="View solution log" MasterPageFile="~/Site.master"
    AutoEventWireup="true" CodeBehind="SolutionLog.aspx.cs" Inherits="VeraWAF.WebPages.Cloud.SolutionLog" ValidateRequest="false"%>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h1>
        <i class="fa fa-archive"></i> Solution log
    </h1>
    <div class="info">
        <fieldset class="info">
            <legend>Log items</legend>

            <asp:Panel ID="NavigationPanel" runat="server">
                <table border="0" cellpadding="3" cellspacing="3">
                    <tr>
                        <td>
                            <asp:Label ID="lblTypeFilter" runat="server" AssociatedControlID="cmbTypeFilter">Type filter:</asp:Label>
                            <asp:DropDownList id="cmbTypeFilter" runat="server" EnableViewState="true" ViewStateMode="Enabled" 
                                AppendDataBoundItems="false" AutoPostBack="true" >
                                <asp:ListItem Text="-- No filter --" Value=""></asp:ListItem>
                                <asp:ListItem Text="Info" Value="Info"></asp:ListItem>
                                <asp:ListItem Text="Warning" Value="Warning"></asp:ListItem>
                                <asp:ListItem Text="Error" Value="Error"></asp:ListItem>
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
            
            <asp:DataGrid ID="SolutionLogGrid" runat="server" CellPadding="2" CellSpacing="1"
                GridLines="Both" OnEditCommand="SolutionLogGrid_Edit" AutoGenerateColumns="false">
                <HeaderStyle BackColor="darkblue" ForeColor="white" />
                <EditItemStyle BackColor="yellow" />
                <Columns>
                    <asp:BoundColumn HeaderText="ID" DataField="PartitionKey" HeaderStyle-CssClass="hide" ItemStyle-CssClass="hide" />
                    <asp:TemplateColumn HeaderText="Log item type">
                        <ItemTemplate>
                            <%# GetLogItemTypeColor((string)Eval("RowKey"))%>
                        </ItemTemplate>
                    </asp:TemplateColumn>

                    <asp:BoundColumn HeaderText="Timestamp" DataField="Timestamp" DataFormatString="{0:yyyy-MM-ddTHH:mm:ssZ}" />
                    <asp:TemplateColumn HeaderText="Description">
                        <ItemTemplate>
                            <%# Server.HtmlEncode(new VeraWAF.WebPages.Bll.TextUtilities().ClipLeft((string)Eval("Message"), 70))%>
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
