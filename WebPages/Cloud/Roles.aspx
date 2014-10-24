<%@ Page Title="Cloud roles" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Roles.aspx.cs" Inherits="VeraWAF.WebPages.Cloud.Roles" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <header>
        <h1>
            <i class="fa fa-cog"></i> Cloud roles
        </h1>
    </header>
    <div class="info">
        <asp:Literal ID="CloudHtml" runat="server"></asp:Literal>
    </div>
</asp:Content>
