<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Comments.ascx.cs" Inherits="VeraWAF.WebPages.Controls.Comments" %>
<%@ Register TagPrefix="uc1" TagName="JailImage" Src="~/Controls/JailImage.ascx" %>
<%@ Reference Control="~/Controls/Comment.ascx" %> 

<div class="commentSection clear">
    <a name="comments"></a>
    <fieldset class="commentsMenu">
        <legend>Comment</legend>
        <nav ID="navSignIn" runat="server">
            <div class="signInToComment" runat="server" id="signInToCommentContainer">
                <span class="fa-stack fa-lg">
                    <i style="margin-left:-0.5em" class="fa fa-user fa-stack-1x"></i>
                    <i style="color:#2b96fe; font-size:0.8em; margin-left: 0.1em" class="fa fa-sign-in fa-stack-1x"></i>
                </span>
                <a runat="server" id="lnkSignIn" href="/Account/Login.aspx">Sign in to comment</a>        
            </div>
        </nav>
        <asp:Panel ID="panMenu" runat="server">
            <div ID="currentUserPortrait" class="currentUserPortrait" runat="server" property="sc:accountablePerson">
                <figure>
                    <uc1:JailImage id="imgPortrait" runat="server" css="userPortrait64x64" alt="User portrait" width="64" height="64" />
                    <br />
                    <figcaption property="sc:name"><asp:Literal ID="litPortraitCaption" runat="server"></asp:Literal></figcaption>
                </figure>
            </div>
            <asp:TextBox ID="txtComment" runat="server" TextMode="MultiLine" Columns="40" Rows="6"></asp:TextBox>
            <menu>
                <asp:Button CssClass="clear" ID="butSubmit" runat="server" OnClick="butSumbit_Click" Text="Add comment" />
                <a class="bbCode" href="/Help/Commenting.aspx" target="_blank" title="Click here for help with BBCodes">Help</a>
            </menu>
        </asp:Panel>
    </fieldset>
    <asp:Panel ID="panComments" runat="server"></asp:Panel>
</div>