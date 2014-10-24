<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Map.ascx.cs" Inherits="VeraWAF.WebPages.Controls.Map" EnableViewState="false" %>
<script type="text/javascript" src="https://maps.google.com/maps?file=api&v=2&sensor=false&key=AIzaSyCvlzjgpae21_gsM3-_8MHdyiEMkh9hOEU&hl=en"></script>
<script src="/Scripts/jquery.gmap-1.1.0-min.js"></script>
<div id="map1" property="sc:Map"></div>
<script>
    $(document).ready(function () {
        <asp:Literal ID="litMap" runat="server"></asp:Literal>
    });
</script>
