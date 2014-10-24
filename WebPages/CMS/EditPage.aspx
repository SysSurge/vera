<%@ Page Title="Page editor" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EditPage.aspx.cs" Inherits="VeraWAF.WebPages.Edit.EditPage" ValidateRequest="false" %>
<%@ Register src="~/Controls/ImageUpload.ascx" tagname="ImageUpload" tagprefix="uc1" %>
<%@ Register TagPrefix="uc1" TagName="Map" Src="~/Controls/Map.ascx" %>
<%@ Register TagPrefix="uc1" TagName="RecursiveDirComboBox" Src="~/Controls/RecursiveDirComboBox.ascx" %>
<%@ Register Assembly="CKEditor.NET" Namespace="CKEditor.NET" TagPrefix="CKEditor" %>
<%@ Register src="~/Controls/FormNotification.ascx" tagname="FormNotification" tagprefix="uc1" %>
<%@ Register src="~/Controls/VirtualFileExplorer.ascx" tagname="VirtualFileExplorer" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <CompressStyles CompressedFile="editpage.min.css" runat="server">
        edit.css
        fileexplorer.css
    </CompressStyles>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="AsideContent" runat="server">

</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MainContent" runat="server">
    <div class="pageEditor">
        <header>            
            <h1>
                <i class="fa fa-edit"></i> Page editor
            </h1>
        </header>
        <menu>
            <asp:Button ID="butUpdate" runat="server" OnClick="butUpdate_Click" Text="Save" />
            <asp:Button ID="butDelete" runat="server" onclick="butDelete_Click" Text="Delete" />
            <asp:Button ID="butClear" runat="server" onclick="butClear_Click" Text="Clear" />
            <asp:CheckBox CssClass="menuCheck" runat="server" ID="chkPublish" Text="Publish page"/>
        </menu>

        <uc1:FormNotification id="notifications" runat="server"></uc1:FormNotification>

        <fieldset style="display:block">
            <legend>Virtual path</legend>

            <uc1:VirtualFileExplorer id="fileExplorer" RemoveBasePath="true" ShowFiles="true" ShowPath="true" ShowFullPath="false" 
                FileMatch="(.*?)\.(aspx|css|htm|html|js|xhtml|xml|gif|jif|jfif|jpeg|jpg|png|tif|tiff|lic|md|txt)$" 
                DirectoryIgnore="(App_Data|App_GlobalResources|App_Start|Batch|bin|Controls|obj|Properties|Templates)$" runat="server">
            </uc1:VirtualFileExplorer>

        </fieldset>

        <fieldset style="display: block">
            <legend>Page layout & styling</legend>
            <p>
                <asp:Label ID="lblTemplate" runat="server" AssociatedControlID="cmbTemplate" 
                    Text="Page layout template:"></asp:Label>
                <uc1:RecursiveDirComboBox ID="cmbTemplate" StaticItemText="None" StaticItemValue="" Path="/Templates/" 
                    RemoveBasePath="true" EnableDirectories="false" FilenameFilter=".aspx" runat="server"></uc1:RecursiveDirComboBox>

                <asp:Label ID="lblStyle" runat="server" AssociatedControlID="cmbStyle" 
                    Text="Page style:"></asp:Label>
                <uc1:RecursiveDirComboBox ID="cmbStyle" StaticItemText="None" StaticItemValue="" Path="/Styles/" 
                    RemoveBasePath="true" ShowFiles="false" IgnoreMatch="Shared" runat="server"></uc1:RecursiveDirComboBox>
            </p>
        </fieldset>    

        <fieldset>
            <legend>Menu options</legend>
            <div style="float:left;padding:1em">
                <asp:Label ID="lblMenuItemName" runat="server" AssociatedControlID="txtMenuItemName" 
                    Text="Name:"></asp:Label>
                <asp:TextBox ID="txtMenuItemName" runat="server" Width="200"></asp:TextBox>
            </div>    
            <div style="float:left;padding:1em">
                <asp:Label ID="lblMenuItemSortOrder" runat="server" AssociatedControlID="txtMenuItemSortOrder" 
                    Text="Sort order:"></asp:Label>
                <asp:TextBox ID="txtMenuItemSortOrder" runat="server" Width="3em" Text="0"></asp:TextBox>
            </div>
            <div style="float:left;padding:1em">
                <asp:Label ID="lblMenuItemDescription" runat="server" AssociatedControlID="txtMenuItemDescription" 
                    Text="Description:"></asp:Label>
                <asp:TextBox ID="txtMenuItemDescription" runat="server" Width="500"></asp:TextBox>
            </div>
            <div style="float:left;padding:1em">
                <asp:Label ID="lblShowInMenu" runat="server" AssociatedControlID="chkShowInMenu" 
                    Text="Show in menu:"></asp:Label>
                <asp:CheckBox ID="chkShowInMenu" runat="server" Checked="True"></asp:CheckBox>
            </div>
        </fieldset>

        <fieldset style="width:98%">
            <legend>Page content</legend>
            <p>
                <asp:Label ID="lblTitle" runat="server" AssociatedControlID="txtTitle" 
                    Text="Title:"></asp:Label>
                <asp:TextBox ID="txtTitle" runat="server" Width="98%"></asp:TextBox>
            </p>
            <p>
                <asp:Label ID="lblRollupText" runat="server" 
                    AssociatedControlID="txtRollupText" Text="Rollup text:"></asp:Label>
                <br />
                <asp:TextBox ID="txtRollupText" runat="server" Height="56px" TextMode="MultiLine" 
                    Width="98%"></asp:TextBox>
            </p>
            <p class="clear">
                <asp:Label ID="lblIngress" runat="server" AssociatedControlID="txtIngress" 
                    Text="Ingress:"></asp:Label>
                <br />
                <asp:TextBox ID="txtIngress" runat="server" Height="53px" TextMode="MultiLine" 
                    Width="98%"></asp:TextBox>
            </p>

            <p>
                <asp:Label ID="lblHeaderContent" runat="server" 
                    AssociatedControlID="txtHeaderContent" Text="Header content:"></asp:Label>
                <br />

                <CKEditor:CKEditorControl ID="txtHeaderContent" BasePath="/Scripts/ckeditor/" runat="server"></CKEditor:CKEditorControl>
            </p>

            <p>
                <asp:Label ID="lblMainContent" runat="server" 
                    AssociatedControlID="txtMainContent" Text="Main content:"></asp:Label>
                <br />
                <CKEditor:CKEditorControl ID="txtMainContent" BasePath="/Scripts/ckeditor/" runat="server"></CKEditor:CKEditorControl>
            </p>
            <p>
                <asp:Label ID="lblAsideContent" runat="server" 
                    AssociatedControlID="txtAsideContent" Text="Aside content:"></asp:Label>
                <br />
                <CKEditor:CKEditorControl ID="txtAsideContent" BasePath="/Scripts/ckeditor/" runat="server"></CKEditor:CKEditorControl>
            </p>
        </fieldset>

        <fieldset class="clear">
            <legend>
                Page images
            </legend>

            <fieldset class="clear">
                <legend>
                    Main page image
                </legend>
                <uc1:ImageUpload ID="iuFigure" runat="server" MaxWidth="960" MaxHeight="1000" ConvertToJpeg="False" 
                    KeepAspectRatio="True" ScaleDownOnly="True" ShowCaption="True" Destination="/images/pages/figure"></uc1:ImageUpload>
            </fieldset>    

            <fieldset class="clear">
                <legend>Rollup image</legend>
                <uc1:ImageUpload ID="iuRollupImage" runat="server" MaxWidth="64" MaxHeight="64" ConvertToJpeg="True" 
                    KeepAspectRatio="False" ScaleDownOnly="False" ShowCaption="False" Destination="/images/pages/rollup"></uc1:ImageUpload>
            </fieldset>
        </fieldset>

        <fieldset style="width:98%">
            <legend>Page options</legend>
            <p>

                <asp:CheckBox ID="chkIndex" runat="server" Checked="True" Text="Add to search index"></asp:CheckBox>
            </p>
            <p>
                <asp:Label ID="lblPageMoved" runat="server" AssociatedControlID="txtPageMoved" 
                    Text="Page moved to url:"></asp:Label>
                <asp:TextBox ID="txtPageMoved" runat="server" Width="98%"></asp:TextBox>
            </p>
        </fieldset>

        
        <fieldset class="clear">
            <legend>Widgets</legend>

            <fieldset>
                <legend>Google map</legend>
                <div style="float:left;padding:1em">
                    <asp:Label ID="lblLat" runat="server" AssociatedControlID="txtLat" Text="Latitude:"></asp:Label>
                    <asp:TextBox ID="txtLat" runat="server" Width="98%">0</asp:TextBox>
                </div>
                <div style="float:left;padding:1em">
                    <asp:Label ID="lblLong" runat="server" AssociatedControlID="txtLong" Text="Longitude:"></asp:Label>
                    <asp:TextBox ID="txtLong" runat="server" Width="98%">0</asp:TextBox>
                </div>
                <div style="float:left;padding:1em">
                    <asp:Label ID="lblZoom" runat="server" AssociatedControlID="txtZoom" Text="Zoom:"></asp:Label>
                    <asp:TextBox ID="txtZoom" runat="server" Width="98%">16</asp:TextBox>
                </div>
                <div class="clear" style="float:left;padding:1em">
                    <asp:Label ID="lblGeolocationText" runat="server" AssociatedControlID="txtGeolocationText" Text="Text:"></asp:Label>
                    <asp:TextBox ID="txtGeolocationText" runat="server" Width="300"></asp:TextBox>
                </div>
                <div class="clear"></div>
                <section>
                    <uc1:Map ID="Map" runat="server" Visible="False"></uc1:Map>
                </section>
            </fieldset>
            <br />
            <p class="clear">
                <asp:CheckBox ID="chkAllowForComments" runat="server" Checked="False" Text="Allow for page comments"></asp:CheckBox>
            </p>
            <p>
                <asp:CheckBox ID="chkShowContactControl" runat="server" Checked="False" Text="Show the contact widget" ></asp:CheckBox>
            </p>

        </fieldset>
        
        <fieldset class="clear" style="width: 98%">
            <legend>Metadata</legend>
            
            <p class="clear">
                <asp:Label ID="lblRdfSubjects" runat="server" AssociatedControlID="txtRdfSubjects" 
                    Text="RDF subjects:"></asp:Label>
                <br />
                <asp:TextBox ID="txtRdfSubjects" runat="server" Height="53px" TextMode="MultiLine" 
                    Width="98%"></asp:TextBox>
            </p>

        </fieldset>

        <fieldset class="clear" style="width: 98%">
            <legend>Inline resources</legend>
            
            <p class="clear">
                <asp:Label ID="lblCss" runat="server" AssociatedControlID="txtCss" 
                    Text="Page CSS files:"></asp:Label>
                <br />
                <asp:TextBox ID="txtCss" runat="server" Height="53px" TextMode="MultiLine" 
                    Width="98%"></asp:TextBox>
            </p>

            <p class="clear">
                <asp:Label ID="lblScripts" runat="server" AssociatedControlID="txtScripts" 
                    Text="Page script files:"></asp:Label>
                <br />
                <asp:TextBox ID="txtScripts" runat="server" Height="53px" TextMode="MultiLine" 
                    Width="98%"></asp:TextBox>
            </p>

        </fieldset>
    </div>
</asp:Content>
