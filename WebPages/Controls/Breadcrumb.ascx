<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Breadcrumb.ascx.cs" Inherits="VeraWAF.WebPages.Controls.Breadcrumb" %>
<div id="breadcrumbContainer" runat="server" class="breadcrumb" property="sc:breadcrumb">
    <aside class="breadcrumb-container">
        <nav>
            <asp:literal runat="server" ID="litBreadcrumb"></asp:literal>
        </nav>
    </aside>
</div>
