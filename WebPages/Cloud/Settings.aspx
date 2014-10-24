<%@ Page Title="Cloud settings" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Settings.aspx.cs" Inherits="VeraWAF.WebPages.Admin.Settings" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <header>
        <h1>
            <i class="fa fa-gears"></i> Cloud settings
        </h1>
    </header>
    <div class="info">
        <fieldset class="info">
            <legend>Process model configuration</legend>
            <p>
                <asp:Label ID="RequestQueueLimitLabel" runat="server" AssociatedControlID="RequestQueueLimit">Value indicating the number of requests allowed in the queue:</asp:Label>
                <asp:Literal ID="RequestQueueLimit" runat="server"></asp:Literal>
            </p>
            <p>
                <asp:Label ID="MaxIOThreadsLabel" runat="server" AssociatedControlID="MaxIOThreads">Maximum number of I/O threads per CPU in the CLR thread pool:</asp:Label>
                <asp:Literal ID="MaxIOThreads" runat="server"></asp:Literal>
            </p>
        </fieldset>

        <fieldset class="info">
            <legend>Application configuration</legend>
            <p>
                <asp:Label ID="MaxConcurrentRequestsPerCPULabel" runat="server" AssociatedControlID="MaxConcurrentRequestsPerCPU">Maximum concurrent requests per CPU:</asp:Label>
                <asp:Literal ID="MaxConcurrentRequestsPerCPU" runat="server"></asp:Literal>
            </p>
            <p>
                <asp:Label ID="MaxConcurrentThreadsPerCPULabel" runat="server" AssociatedControlID="MaxConcurrentThreadsPerCPU">Maximum concurrent threads per CPU:</asp:Label>
                <asp:Literal ID="MaxConcurrentThreadsPerCPU" runat="server"></asp:Literal>
            </p>
        </fieldset>

        <fieldset class="info">
            <legend>Connection management</legend>
            <p>
                <asp:Label ID="DefaultConnectionLimitLabel" runat="server" AssociatedControlID="DefaultConnectionLimit">Maximum number of concurrent connections allowed:</asp:Label>
                <asp:Literal ID="DefaultConnectionLimit" runat="server"></asp:Literal>
            </p>
            <p>
                <asp:Label ID="Expect100ContinueLabel" runat="server" AssociatedControlID="Expect100Continue">HTTP 100-Continue behavior is used:</asp:Label>
                <asp:Literal ID="Expect100Continue" runat="server"></asp:Literal>
            </p>
            <p>
                <asp:Label ID="UseNagleAlgorithmLabel" runat="server" AssociatedControlID="UseNagleAlgorithm">Nagle algorithm is used:</asp:Label>
                <asp:Literal ID="UseNagleAlgorithm" runat="server"></asp:Literal>
            </p>
        </fieldset>

        <fieldset class="info">
            <legend>Hardware</legend>
            <p>
                <asp:Label ID="NumberOfProcessorsLabel" runat="server" AssociatedControlID="NumberOfProcessors">Number of physical processors:</asp:Label>
                <asp:Literal ID="NumberOfProcessors" runat="server"></asp:Literal>
            </p>
            <p>
                <asp:Label ID="NumberOfCoresLabel" runat="server" AssociatedControlID="NumberOfCores">Number of cores:</asp:Label>
                <asp:Literal ID="NumberOfCores" runat="server"></asp:Literal>
            </p>
            <p>
                <asp:Label ID="ProcessorCountLabel" runat="server" AssociatedControlID="ProcessorCount">Number of logical processors:</asp:Label>
                <asp:Literal ID="ProcessorCount" runat="server"></asp:Literal>
            </p>
        </fieldset>

    </div>
</asp:Content>
