<%@ Control Language="C#" ClassName="RulePermissions" AutoEventWireup="true" CodeBehind="RulePermissions.ascx.cs" Inherits="VeraWAF.WebPages.Controls.RulePermissions" %>
<section class="aclRule">
    
    <menu>
        <table border="0">
            <tr><td><span class="aclRuleOwner"><asp:Literal ID="litUserOrRoleName" Text="Username" runat="server"></asp:Literal></span></td>
                <td><asp:CheckBox ID="chkAllowRead" Text="Allow read" runat="server" /></td>
                <td><asp:CheckBox ID="chkAllowWrite" Text="Allow write" runat="server" /></td>
                <td><asp:CheckBox ID="chkAllowDelete" Text="Allow delete" runat="server" /></td>
            </tr>
            <tr>
                <td><asp:LinkButton ID="butRemove" OnClick="butRemove_Click" Runat="server"><i class="fa fa-trash-o fa-lg"></i> Remove</asp:LinkButton></td>
                <td><asp:CheckBox ID="chkDenyRead" Text="Deny read" runat="server" /></td>
                <td><asp:CheckBox ID="chkDenyWrite" Text="Deny write" runat="server" /></td>
                <td><asp:CheckBox ID="chkDenyDelete" Text="Deny delete" runat="server" /></td>
            </tr>
        </table>
    </menu>
</section>
