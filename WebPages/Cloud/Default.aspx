<%@ Page Title="Cloud Panel" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="VeraWAF.WebPages.Cloud.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <CompressStyles CompressedFile="edit.min.css" runat="server">
        edit.css
    </CompressStyles>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="AsideContent" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MainContent" runat="server">
    <header>
        <h1>
            <i class="fa fa-cloud"></i> Cloud panel
        </h1>        
    </header>
    <p>Administrative tasks for the cloud solution.</p>
    <div class="adminPanel">
        <section>
            <h2>Solution</h2>
            <p>
                <asp:HyperLink ID="lnkCommands" runat="server" NavigateUrl="~/Cloud/CloudCommand.aspx">Commands</asp:HyperLink>
            </p>        
            <p>
                <asp:HyperLink ID="lnkViewPages" runat="server" NavigateUrl="~/Cloud/Settings.aspx">Settings</asp:HyperLink>
            </p>
            <p>
                <asp:HyperLink ID="lnkViewComments" runat="server" NavigateUrl="~/Cloud/Statistics.aspx">Online users</asp:HyperLink>
            </p>
            <p>
                <asp:HyperLink ID="lnkRoles" runat="server" NavigateUrl="~/Cloud/Roles.aspx">Cloud roles</asp:HyperLink>
            </p>
        </section>
        <section>
            <h2>Logs</h2>
            <p>
                <asp:HyperLink ID="lnkCloudLog" runat="server" NavigateUrl="~/Cloud/SolutionLog.aspx">Solution Log</asp:HyperLink>
            </p>
            <p>
                <asp:HyperLink ID="lnkEventLog" runat="server" NavigateUrl="~/Cloud/WinEventLog.aspx">Windows Event Log</asp:HyperLink>
            </p>
        </section>
        <section>
            <h2>Real-time monitoring</h2>
            <p>
                <asp:HyperLink ID="lnkPerformance" runat="server" NavigateUrl="~/Cloud/Performance.aspx">Performance monitor</asp:HyperLink>
            </p>
        </section>
    </div>
</asp:Content>
