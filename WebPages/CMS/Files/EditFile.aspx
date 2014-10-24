<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EditFile.aspx.cs" Inherits="VeraWAF.WebPages.Edit.Files.EditFile" ValidateRequest="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="AsideContent" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        <i class="fa fa-cloud-upload"></i> File editor
    </h1>
    <p>
        <asp:Button ID="butSave" runat="server" onclick="butSave_Click" Text="Save" />
        <asp:Button ID="butDelete" runat="server" onclick="butDelete_Click" Text="Delete" />
        <asp:Button ID="butClear" runat="server" onclick="butClear_Click" Text="Clear" />
    </p>
    <fieldset>
        <legend>File data</legend>
        <div>
            <asp:Label ID="lblDescription" runat="server" AssociatedControlID="txtDescription" 
                Text="Description:"></asp:Label>
            <asp:TextBox ID="txtDescription" runat="server" Width="98%" Rows="5" TextMode="MultiLine"></asp:TextBox>
        </div>
        <div class="clear">
            <asp:Label ID="lblUrl" runat="server" AssociatedControlID="lnkUrl" 
                Text="Url:"></asp:Label>
            <asp:HyperLink ID="lnkUrl" runat="server" Target="_blank"></asp:HyperLink>
        </div>
        <div class="clear">
            <asp:Label ID="lblDestination" runat="server" AssociatedControlID="txtDestination" 
                Text="Destination folder:"></asp:Label>
            <asp:TextBox ID="txtDestination" runat="server" Width="98%" Text="/"></asp:TextBox>
        </div>
        <div class="clear">
            <asp:Button id="FileSubmitButton" Text="Upload file" OnClick="FileSubmitButton_Click" runat="server"/>
            <asp:FileUpload ID="FileUploadControl" runat="server" />
        </div>
    </fieldset>
</asp:Content>
