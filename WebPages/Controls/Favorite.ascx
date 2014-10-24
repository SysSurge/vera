<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Favorite.ascx.cs" Inherits="VeraWAF.WebPages.Controls.Favorite" %>
<div class="favoriteContainer" title="Favorite this">
    Favorite
    <br />
    <asp:Button ID="btnFavorite" CssClass="favorite" OnClick="btnFavorite_OnClick" Text="☆" runat="server" />
    <var class="center" ID="numFavorites" runat="server"></var>
</div>