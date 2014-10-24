<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ForumPage.aspx.cs" Inherits="VeraWAF.WebPages.Templates.ForumPage" ValidateRequest="false" %>
<%@ Register TagPrefix="uc1" TagName="Share" Src="~/Controls/Share.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Comments" Src="~/Controls/Comments.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Vote" Src="~/Controls/Vote.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Favorite" Src="~/Controls/Favorite.ascx" %>
<%@ Register TagPrefix="uc1" TagName="ContentField" Src="~/Controls/ContentField.ascx" %>
<%@ Register TagPrefix="uc1" TagName="PagePublishDate" Src="~/Controls/PagePublishDate.ascx" %>
<%@ Register TagPrefix="uc1" TagName="PageModifiedDate" Src="~/Controls/PageModifiedDate.ascx" %>
<%@ Register TagPrefix="uc1" TagName="PageAuthor" Src="~/Controls/PageAuthor.ascx" %>
<%@ Register TagPrefix="uc1" TagName="UserPortrait" Src="~/Controls/UserPortrait.ascx" %>
<%@ Register src="~/Controls/UserProfileField.ascx" tagname="UserProfileField" tagprefix="uc1" %>
<%@ Register src="~/Controls/UserMemberSinceDate.ascx" tagname="UserMemberSinceDate" tagprefix="uc1" %>
<%@ Register src="~/Controls/UserLastActiveDate.ascx" tagname="UserLastActiveDate" tagprefix="uc1" %>
<%@ Register src="~/Controls/UserRoles.ascx" tagname="UserRoles" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server" EnableViewState="false">
    <CompressStyles CompressedFile="forumpage.min.css" runat="server">
        favorite.css
        board.css
        vote.css
    </CompressStyles>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeaderContent" runat="server" EnableViewState="false">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="AsideContent" runat="server" EnableViewState="false">
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="MainContent" runat="server">
    <div class="forumPage" property="sc:Article">
        <div class="pageHeader">
            <header>
                <h1 property="dc:title">
                    <uc1:ContentField PropertyName="Title" HtmlEncode="true" runat="server"></uc1:ContentField>
                </h1>
            </header>    
        </div>
        
        <div class="content clear">
            <div class="userInfo" property="sc:author sc:accountablePerson">

                <uc1:UserPortrait Format="Portrait of {0}" runat="server"></uc1:UserPortrait>

                <uc1:Favorite ID="favorite" runat="server" />

                <uc1:Vote ID="vote" runat="server" />

                <br />

                <p class="score clear">&loz; 
                    <var><uc1:UserProfileField UserIdFromRequestParam="id" PropertyName="SocialPoints" runat="server" Format="{0:n0}"></uc1:UserProfileField></var> points
                </p>

                <span class="joinedDate clear">Joined <uc1:UserMemberSinceDate UserIdFromRequestParam="id" runat="server"></uc1:UserMemberSinceDate></span>
                <span class="lastActiveDate clear">Last active <uc1:UserLastActiveDate UserIdFromRequestParam="id" runat="server"></uc1:UserLastActiveDate></span>
                <span class="userRoles clear">Roles: <uc1:UserRoles UserIdFromRequestParam="id" DisplayMode="Csv" runat="server"></uc1:UserRoles></span>

                <p class="signature clear">
                    Signature: <uc1:UserProfileField ID="Description1" PropertyName="Description" HtmlEncode="true" runat="server"></uc1:UserProfileField>
                </p>
            </div>        
            <span property="sc:articleBody">
                <uc1:ContentField PropertyName="MainContent" HtmlEncode="true" EnableBbCodes="true" runat="server"></uc1:ContentField>
            </span>
        </div>
        <div class="pageFooter clear">
            <footer>
                <small>
                    <asp:Panel ID="panEditPost" runat="server"><span style="float:left">
                        <i class="fa fa-edit"></i> <a ID="lnkEditPost" href="Edit.aspx" runat="server">Edit post</a></span>
                    </asp:Panel>

                    <uc1:PagePublishDate runat="server"></uc1:PagePublishDate> 
                    &bull; 
                    <uc1:PageModifiedDate runat="server"></uc1:PageModifiedDate>
                     &bull; 
                    <uc1:PageAuthor runat="server"></uc1:PageAuthor>
                </small>
                <div class="section">
                    <section>
                        <uc1:Share ID="Share1" runat="server"></uc1:Share>
                    </section>
                </div>
            </footer>
        </div>

        <section>
            <uc1:Comments ID="comments1" runat="server" VirtualFiles="VirtualForumPages" AllowVoting="True" ShowSignature="True" ShowUserMetaData="True"></uc1:Comments>
        </section>

    </div>
</asp:Content>
