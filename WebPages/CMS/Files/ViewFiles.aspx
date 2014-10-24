<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ViewFiles.aspx.cs" Inherits="VeraWAF.WebPages.Edit.Files.ViewFiles" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <CompressStyles CompressedFile="files.min.css" runat="server">
        files.css
    </CompressStyles>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="AsideContent" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        <i class="fa fa-cloud-download"></i> View files
    </h1>
    <div class="fileList">
        <fieldset>
            <legend>File list</legend>
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
            <asp:DataGrid CssClass="filesGrid" ID="FilesGrid" runat="server" CellPadding="2" CellSpacing="1"
                GridLines="Both" OnEditCommand="FilesGrid_Edit" AutoGenerateColumns="false">
                <HeaderStyle BackColor="darkblue" ForeColor="white" />
                <EditItemStyle BackColor="yellow" />
                <Columns>
                    <asp:BoundColumn Visible="False" HeaderText="Partition key" DataField="PartitionKey"  />                    
                    <asp:TemplateColumn HeaderText="Url">
                        <ItemTemplate>
                            <%# Docals(Convert.ToString(DataBinder.Eval(Container.DataItem, "Url")))%>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:BoundColumn HeaderText="Description" DataField="Description" />
                    <asp:EditCommandColumn EditText="Edit" CancelText="Cancel" UpdateText="Update" HeaderText="Edit">
                        <ItemStyle Wrap="false"></ItemStyle>
                        <HeaderStyle Wrap="false"></HeaderStyle>
                    </asp:EditCommandColumn>
                </Columns>
            </asp:DataGrid>
        </fieldset>
        <p class="clear">
            Blob storage usage: <asp:Literal ID="litByteCount" runat="server"></asp:Literal> MB.
        </p>
    </div>
</asp:Content>

