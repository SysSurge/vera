<%@ Page Title="Forum page editor" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EditForumPage.aspx.cs" Inherits="VeraWAF.WebPages.Templates.EditForumPage" ValidateRequest="false" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <CompressScripts CompressedFile="wysiwyg-editor.min.js" runat="server">
        wysiwyg-editor/htmlbox.colors.js
        wysiwyg-editor/htmlbox.styles.js
        wysiwyg-editor/htmlbox.syntax.js
        wysiwyg-editor/xhtml.js
        wysiwyg-editor/htmlbox.min.js
    </CompressScripts>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeaderContent" runat="server"></asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="AsideContent" runat="server"></asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MainContent" runat="server">
    <div class="pageEditor">
        <header>
            <h1>
                Forum page editor
            </h1>
        </header>

        <menu>
            <asp:Button ID="butSave" runat="server" onclick="butSave_Click" Text="Save" />
            <asp:Button ID="butDelete" runat="server" onclick="butDelete_Click" Text="Delete" />
            <asp:Button ID="butClear" runat="server" onclick="butClear_Click" Text="Clear" />
            <asp:CheckBox runat="server" ID="chkPublish" Text="Publish"/>
        </menu>

        <fieldset class="clear" style="width:98%">
            <legend>Page content</legend>
            <p>
                <asp:Label ID="lblTitle" runat="server" AssociatedControlID="txtTitle" 
                    Text="Title:"></asp:Label>
                <asp:TextBox ID="txtTitle" runat="server" Width="98%"></asp:TextBox>
            </p>
            <p>
                <asp:Label ID="lblMainContent" runat="server" 
                    AssociatedControlID="txtMainContent" Text="Main content:"></asp:Label>
                <br />
                <asp:TextBox ID="txtMainContent" runat="server" Height="500px" TextMode="MultiLine" 
                    Width="98%"></asp:TextBox>
            </p>
            <asp:TextBox ID="txtVirtualPath" runat="server" CssClass="hide" Enabled="False"></asp:TextBox>
        </fieldset>
      
    </div>
</asp:Content>
