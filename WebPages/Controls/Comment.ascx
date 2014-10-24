<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Comment.ascx.cs" Inherits="VeraWAF.WebPages.Controls.Comment" ClassName="Commment" %>
<%@ Register src="~/Controls/Vote.ascx" tagname="Vote" tagprefix="uc1" %>
<%@ Register TagPrefix="uc1" TagName="JailImage" Src="~/Controls/JailImage.ascx" %>

<div class="comment clear" property="sc:comment">
    <article>
        <div class="commentFooter">
            <footer>
                <a ID="lnkAnchor" name="" runat="server" property="sc:URL"></a>
                Posted <time ID="timePubDate" class="timeAgo" pubdate="pubdate" datetime="" runat="server" property="dc:date dc:created"></time> 
                by <a ID="lnkUser" href="" runat="server" property="sc:accountablePerson sc:name sc:URL"></a>:
                <a ID="lnkCommentLink" class="commentLink" href="" runat="server"><i class="fa fa-link"></i>Link to comment</a>
            </footer>
        </div>
        <div class="userInfo">
            <a ID="lnkUserPortrait" class="userPortrait" href="" runat="server">
                <figure property="sc:author sc:accountablePerson">
                    <uc1:JailImage ID="jailUserPortrait" runat="server" src="" css="userPortrait64x64 center" alt="" width="64" height="64" />
                    <figcaption property="sc:name"><asp:Literal ID="figCaption" runat="server"></asp:Literal></figcaption>
                </figure>
            </a>
            <uc1:Vote ID="vote" runat="server"></uc1:Vote>
            <asp:Panel ID="panUserMetaData" runat="server">
                <br />
                <p class="score clear">◊ <var ID="score" class="score" runat="server" property="sc:aggregateRating"></var> points</p>
                <span class="joinedDate clear">Joined <time ID="joinedDate" class="timeAgo" datetime="" runat="server"></time></span>
                <span class="lastActiveDate clear">Seen <time ID="lastActiveDate" class="timeAgo" datetime="" runat="server"></time></span>
                <span ID="userRoles" class="userRoles clear" runat="server"></span>
            </asp:Panel>
        </div>
        <span property="sc:text"><asp:Literal ID="litMainContent" runat="server"></asp:Literal></span>
        <div ID="divSignature" runat="server" class="signature clear">
            Signature: <asp:Literal ID="signature" runat="server"></asp:Literal>
        </div>
    </article>
</div>