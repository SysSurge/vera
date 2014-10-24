<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ImageUpload.ascx.cs" Inherits="VeraWAF.WebPages.Controls.ImageUpload" %>
<p>
    <asp:Image ID="FigureImage" runat="server" CssClass="userFigure64x64"></asp:Image>
</p>
<asp:Panel runat="server" ID="UploadFigure">
    <p class="clear">
        <asp:Label ID="lblDestination" runat="server" AssociatedControlID="txtDestination" 
            Text="Destination folder:"></asp:Label>
        <asp:TextBox ID="txtDestination" runat="server" Width="98%" Text="/"></asp:TextBox>
    </p>    
    <p class="clear">
        <asp:Button id="FigureSubmitButton" Text="Upload image" OnClick="FigureSubmitButton_Click" runat="server"/>
        <asp:Button id="ClearImage" Text="Clear image" OnClick="ClearButton_Click" runat="server"/>
        <asp:FileUpload ID="FigureUploadControl" runat="server" />
    </p>
    <p>
        <asp:Label ID="lblFigureUrl" runat="server" AssociatedControlID="lnkFigureUrl" Text="Upload url:"></asp:Label>
        <asp:HyperLink ID="lnkFigureUrl" runat="server" Target="_blank"></asp:HyperLink>
    </p>
    <div style="float:left;padding:1em">
        <asp:Label ID="lblJpegCompression" runat="server" AssociatedControlID="txtJpegCompression" Text="JPEG Compression:"></asp:Label>
        <asp:TextBox ID="txtJpegCompression" runat="server" type="range" min="0" max="100" value="75"></asp:TextBox>
    </div>
    <div style="float:left;padding:1em">
        <asp:Label ID="lblKeepAspectRatio" runat="server" AssociatedControlID="chkKeepAspectRatio" Text="Keep aspect ratio:"></asp:Label>
        <asp:CheckBox ID="chkKeepAspectRatio" runat="server" Checked="True"/>
    </div>
    <div style="float:left;padding:1em">
        <asp:Label ID="lblScaleDownOnly" runat="server" AssociatedControlID="chkScaleDownOnly" Text="Scale down only:"></asp:Label>
        <asp:CheckBox ID="chkScaleDownOnly" runat="server" Checked="True"/>
    </div>
    <div style="float:left;padding:1em">
        <asp:Label ID="lblConvertToJpeg" runat="server" AssociatedControlID="chkConvertToJpeg" Text="Convert to JPEG:"></asp:Label>
        <asp:CheckBox ID="chkConvertToJpeg" runat="server" Checked="False"/>
    </div>
    <div class="clear"></div>
    <div runat="server" ID="figureCaptionContainer" style="float:left;padding:1em">
        <asp:Label ID="lblFigureCaption" runat="server" AssociatedControlID="txtFigureCaption" Text="Caption:"></asp:Label>
        <asp:TextBox ID="txtFigureCaption" runat="server" Width="650"></asp:TextBox>
    </div>
    <div style="float:left;padding:1em">
        <asp:Label ID="lblMaxWidth" runat="server" AssociatedControlID="txtMaxWidth" Text="Max width:"></asp:Label>
        <asp:TextBox ID="txtMaxWidth" runat="server" Width="50"></asp:TextBox>
    </div>
    <div style="float:left;padding:1em">
        <asp:Label ID="lblMaxHeight" runat="server" AssociatedControlID="txtMaxHeight" Text="Max height:"></asp:Label>
        <asp:TextBox ID="txtMaxHeight" runat="server" Width="50"></asp:TextBox>
    </div>

</asp:Panel>
