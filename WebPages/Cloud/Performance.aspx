<%@ Page Title="Real-time performance" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="Performance.aspx.cs" Inherits="VeraWAF.WebPages.Cloud.Performance" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <CompressScripts runat="server">
        pl.performance.js
    </CompressScripts>
    <style>
        canvas.performance {
            background-color: grey;
            border: 1px solid black;
        }
    </style>
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <header>
        <h1>
            <i class="fa fa-tachometer"></i> Real-time performance overview
        </h1>
    </header>
    <p></p>
    <span id="cloudPassword" style="display:none" runat="server"></span>
    <span id="nodeList" style="display:none" runat="server"></span>
    <div class="info">
        <canvas class="performance"></canvas>
    </div>
</asp:Content>
