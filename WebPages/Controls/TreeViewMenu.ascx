<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TreeViewMenu.ascx.cs" Inherits="VeraWAF.WebPages.Controls.Level3Menu" %>
<div class="level3Menu">
    <nav>
        <asp:SiteMapDataSource runat="server" ID="siteMapDataSource" ShowStartingNode="false" />
        <asp:TreeView ID="mnuTreeView" SelectedNodeStyle-ForeColor="Black" ExpandDepth="2" DataSourceID="siteMapDataSource" 
            CssClass="treeview" ShowLines="true" ShowCheckBoxes="None" ShowExpandCollapse="True" SkipLinkText=""  Runat="server">
            
            <LevelStyles>
                <asp:TreeNodeStyle ChildNodesPadding="10" 
                    Font-Size="1.2em"/>
                <asp:TreeNodeStyle ChildNodesPadding="8" 
                    Font-Size="1em"/>
                <asp:TreeNodeStyle ChildNodesPadding="6" 
                    Font-Size="0.9em"/>
                <asp:TreeNodeStyle ChildNodesPadding="5" 
                    Font-Size="0.8em"/>
            </LevelStyles>

        </asp:TreeView>
    </nav>
</div>
