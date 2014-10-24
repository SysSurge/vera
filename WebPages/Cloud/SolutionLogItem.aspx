<%@ Page Title="Solution log entry" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SolutionLogItem.aspx.cs" 
    Inherits="VeraWAF.WebPages.Cloud.SolutionLogItem" ValidateRequest="false" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="AsideContent" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        <i class="fa fa-archive"></i> Solution log entry
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
            <asp:Label ID="lblDescription" runat="server" AssociatedControlID="txtDescription" 
                Text="Description:"></asp:Label>
            <asp:TextBox ID="txtDescription" runat="server" Width="98%" Rows="10" TextMode="MultiLine" 
                ReadOnly="true"></asp:TextBox>
        </p>
    </fieldset>
</asp:Content>
