<%@ Page Title="Statistics" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="Statistics.aspx.cs" Inherits="VeraWAF.WebPages.Edit.Statistics" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script src="/Scripts/Highcharts-4.0.1/js/highcharts-all.js" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <header>
        <h1>
            <i class="fa fa-pie-chart"></i> Statistics & charts
        </h1>
    </header>
    <div class="info">
        <h2>Market research charts</h2>
        <p>
            <asp:Literal ID="litIndustry" runat="server"></asp:Literal>
        </p>
        <br/>
        <p>
            <asp:Literal ID="litJobCategory" runat="server"></asp:Literal>
        </p>
        <br/>
        <p>
            <asp:Literal ID="litCompanySizes" runat="server"></asp:Literal>
        </p>
        <br/>
        <p>
            <asp:Literal ID="litCountries" runat="server"></asp:Literal>
        </p>
    </div>
</asp:Content>
