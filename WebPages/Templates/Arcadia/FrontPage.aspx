<%@ Page Title="Front page" Language="C#" AutoEventWireup="true" Inherits="VeraWAF.Core.Templates.PageTemplateBase" ValidateRequest="false" %>
<%@ Register src="~/Controls/Footer.ascx" tagname="Footer" tagprefix="uc1" %>
<%@ Register src="~/Controls/UserRoleContextMenu.ascx" tagname="UserRoleContextMenu" tagprefix="uc1" %>
<%@ Register src="~/Controls/DropdownMenu.ascx" tagname="DropdownMenu" tagprefix="uc1" %>
<%@ Register src="~/Controls/SiteTracker.ascx" tagname="SiteTracker" tagprefix="uc1" %>
<%@ Register src="~/Controls/JailImage.ascx" tagname="JailImage" tagprefix="uc1" %>
<%@ Register TagPrefix="uc1" TagName="ContentField" Src="~/Controls/ContentField.ascx" %><!DOCTYPE html>
<html version="HTML" runat="server">
<head profile="http://www.w3.org/1999/xhtml/vocab" runat="server">

    <CompressScripts CompressedFile="frontpage.min.js" runat="server">
        jquery-1.11.1.js
        jail.0.9.5.min.js
        jquery.preventDoubleSubmission.js
        dal.interfaces.js
        dal.interchange.js
        pl.searchBox.js
        pl.inputBox.js
        pl.pageLoad.js
        bll.email.js
        pl.email.js
        pl.submit.js
        jquery.animate-colors-min.js
        pl.frontpage.js
    </CompressScripts>

    <link href="https://fonts.googleapis.com/css?family=Open+Sans:400,300" rel="stylesheet" type="text/css" />
    <link href="/Styles/Shared/font-awesome-4.2.0/css/font-awesome.min.css" rel="stylesheet" type="text/css" />

    <CompressStyles CompressedFile="frontpage.min.css" runat="server">
        base.css
        devices.css
        form.css
        tools.css
        logo.css
        signin.css
        level1menu.css
        searchbox.css
        inputbox.css
        footer.css
        sociallinks.css
        rssicon.css
        beta.css
        email.css
        submit.css
        frontpage.css
    </CompressStyles>
     
    <uc1:SiteTracker runat="server"></uc1:SiteTracker>
</head>
<body>
    <form runat="server" property="sc:Article">

       <aside class="loginDisplay">
            <div class="loginDisplayNav">
                <a href="/"><uc1:JailImage ID="logo" runat="server" src="/Images/Logo_77x77.png" css="logo" alt="" width="77" height="77" /></a>
                <div class="loginNav">
                    <uc1:UserRoleContextMenu runat="server"></uc1:UserRoleContextMenu>
                </div>
            </div>
        </aside>

	    <header id="top">
		    <div id="content-container"> 
		        <div id="content">
                    <uc1:ContentField PropertyName="HeaderContent" EditHelpText="Enter page header markup" TextBoxMode="HtmlEditor" 
                        EditWidth="80em" EditHeight="20em" runat="server"></uc1:ContentField>
		        </div>
		    </div>
	    </header>

        <uc1:DropdownMenu runat="server"></uc1:DropdownMenu>

        <main>
	        <aside id="splash">
                <uc1:ContentField PropertyName="AsideContent" EditHelpText="Enter page aside text" TextBoxMode="SingleLine"
                        EditWidth="40em" runat="server"></uc1:ContentField>
	        </aside>

            <article id="main" property="sc:articleBody">
                <uc1:ContentField PropertyName="MainContent" EditHelpText="Enter main page markup" TextBoxMode="HtmlEditor" 
                        EditWidth="80em" EditHeight="20em" runat="server"></uc1:ContentField>
            </article>
        </main>

        <div class="footer clear">
            <div class="footerSectionContainer">
                <uc1:Footer runat="server" ViewStateMode="Disabled"></uc1:Footer>
            </div>
        </div>
    </form>
</body>
</html>
