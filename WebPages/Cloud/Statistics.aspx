<%@ Page Title="Statistics" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="Statistics.aspx.cs" Inherits="VeraWAF.WebPages.Admin.Statistics" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script src="/Scripts/Highcharts-2.2.0/js/highcharts.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <header>
        <h1>
            <i class="fa fa-bar-chart"></i> Statistics
        </h1>
    </header>
    <div class="info">
        <fieldset class="info">
            <legend>Users</legend>
            <p>
                <asp:Label ID="NumberOfUsersLabel" runat="server" AssociatedControlID="NumberOfUsers">Registered users #:</asp:Label>
                <asp:Literal ID="NumberOfUsers" runat="server"></asp:Literal>
            </p>
            <p>
                <asp:Label ID="NumberOfUsersOnlineLabel" runat="server" AssociatedControlID="NumberOfUsersOnline">Users online #:</asp:Label>
                <asp:Literal ID="NumberOfUsersOnline" runat="server"></asp:Literal>
            </p>
        </fieldset>
    </div>
</asp:Content>
