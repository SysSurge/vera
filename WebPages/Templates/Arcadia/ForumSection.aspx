<%@ Page Title="" Language="C#" MasterPageFile="~/Sub2.Master" AutoEventWireup="true" CodeBehind="ForumSection.aspx.cs" Inherits="VeraWAF.WebPages.Templates.ForumSection" %>
<%@ Register TagPrefix="uc1" TagName="ContentField" Src="~/Controls/ContentField.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server" EnableViewState="false">
    <CompressStyles CompressedFile="forumsection.min.css" runat="server">
        table.css
        board.css
    </CompressStyles>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="MainContent" runat="server">
    <div class="boardIndex">

        <div class="pageHeader">
            <header>
                <hgroup>
                    <h1 property="dc:title">
                        <uc1:ContentField PropertyName="Title" runat="server"></uc1:ContentField>
                    </h1>
                    <h2>
                        <uc1:ContentField PropertyName="Ingress" runat="server"></uc1:ContentField>
                    </h2>
                </hgroup>
            </header>    
        </div>

        <div class="menu clear">
            <menu>
                <a href="Edit.aspx">New post</a>
            </menu>
        </div>

        <div class="content clear">
            <asp:Panel ID="NavigationPanel" Visible="false" runat="server">
                <table border="0" cellpadding="3" cellspacing="3">
                    <tr>
                        <td>
                            Page
                            <asp:Label ID="CurrentPageLabel" runat="server" EnableViewState="false" />
                            of
                            <asp:Label ID="TotalPagesLabel" runat="server" EnableViewState="false" />
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
            <asp:DataGrid CssClass="blueHeader" ID="ContentPagesGrid" runat="server" CellPadding="0" CellSpacing="0" BorderWidth="0"
                          GridLines="None" AutoGenerateColumns="false">
                <EditItemStyle BackColor="yellow" />
                <Columns>
                    <asp:TemplateColumn HeaderText="Latest posts">
                        <ItemTemplate>
                            <%# Docals(Convert.ToString(DataBinder.Eval(Container.DataItem, "VirtualPath")))%>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                </Columns>
            </asp:DataGrid>
        </div>

    </div>
</asp:Content>