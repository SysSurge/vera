<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Vote.ascx.cs" Inherits="VeraWAF.WebPages.Controls.Vote" %>
<div class="voteContainer" property="sc:AggregateRating">
    <asp:Button ID="butUpVote" CssClass="upArrow center" runat="server" OnClick="butUpVote_OnClick" ToolTip="I like this (Vote +5 points)"></asp:Button>
    <div ID="divVoteCounter" class="voteCount center" runat="server" property="sc:ratingValue sc:contentRating sc:Integer"><asp:Literal  ID="voteCounter" runat="server"></asp:Literal></div>
    <asp:Button ID="butDownVote" CssClass="downArrow center" runat="server" OnClick="butDownVote_OnClick" ToolTip="I don't like this (Vote -5 points)"></asp:Button>
</div>
