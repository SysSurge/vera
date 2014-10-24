<%@ Page Title="Windows Event log entry" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="WinEventLogItem.aspx.cs" 
    Inherits="VeraWAF.WebPages.Cloud.WinEventLogItem" ValidateRequest="false" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="AsideContent" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        <i class="fa fa-archive"></i> Windows Event log entry
    </h1>
    <fieldset style="width:50em;">
        <legend>Log item</legend>
        <p>
            <asp:Label ID="lblType" runat="server" AssociatedControlID="litType" 
                Text="Log entry type:"></asp:Label>
            <asp:Literal ID="litType" runat="server"></asp:Literal>
        </p>
        <p>
            <asp:Label ID="lblInternalId" runat="server" AssociatedControlID="litInternalId" 
                Text="Log item ID:"></asp:Label>
            <asp:Literal ID="litInternalId" runat="server"></asp:Literal>
        </p>
        <p>
            <asp:Label ID="lblTimestamp" runat="server" AssociatedControlID="litTimestamp" 
                Text="Timestamp:"></asp:Label>
            <asp:Literal ID="litTimestamp" runat="server"></asp:Literal>
        </p>
        <p>
            <asp:Label ID="lblEventId" runat="server" AssociatedControlID="litEventId" 
                Text="Event ID:"></asp:Label>
            <asp:Literal ID="litEventId" runat="server"></asp:Literal>
        </p>
        <p>
            <asp:Label ID="lblRole" runat="server" AssociatedControlID="litRole" 
                Text="Role:"></asp:Label>
            <asp:Literal ID="litRole" runat="server"></asp:Literal>
        </p>
        <p>
            <asp:Label ID="lblRoleInstance" runat="server" AssociatedControlID="litRoleInstance" 
                Text="Role instance:"></asp:Label>
            <asp:Literal ID="litRoleInstance" runat="server"></asp:Literal>
        </p>
        <p>
            <asp:Label ID="lblDeploymentId" runat="server" AssociatedControlID="litDeploymentId" 
                Text="Deployment ID:"></asp:Label>
            <asp:Literal ID="litDeploymentId" runat="server"></asp:Literal>
        </p>
        <p>
            <asp:Label ID="lblChannel" runat="server" AssociatedControlID="litChannel" 
                Text="Channel:"></asp:Label>
            <asp:Literal ID="litChannel" runat="server"></asp:Literal>
        </p>
        <p>
            <asp:Label ID="lblProviderName" runat="server" AssociatedControlID="litProviderName" 
                Text="Provider name:"></asp:Label>
            <asp:Literal ID="litProviderName" runat="server"></asp:Literal>
        </p>
        <p>
            <asp:Label ID="lblPid" runat="server" AssociatedControlID="litPid" 
                Text="Process ID:"></asp:Label>
            <asp:Literal ID="litPid" runat="server"></asp:Literal>
        </p>
        <p>
            <asp:Label ID="lblTid" runat="server" AssociatedControlID="litTid" 
                Text="Thread ID:"></asp:Label>
            <asp:Literal ID="litTid" runat="server"></asp:Literal>
        </p>
        <p>
            <asp:Label ID="lblDescription" runat="server" AssociatedControlID="txtDescription" 
                Text="Description:"></asp:Label>
            <asp:TextBox ID="txtDescription" runat="server" Width="98%" Rows="10" TextMode="MultiLine" 
                ReadOnly="true"></asp:TextBox>
        </p>
        <p>
            <asp:Label ID="lblRawXml" runat="server" AssociatedControlID="txtRawXml" 
                Text="Raw XML:"></asp:Label>
            <asp:TextBox ID="txtRawXml" runat="server" Width="98%" Rows="10" TextMode="MultiLine" 
                ReadOnly="true"></asp:TextBox>
        </p>

    </fieldset>
</asp:Content>
