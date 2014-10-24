<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MarketingResearch.ascx.cs" Inherits="VeraWAF.WebPages.Controls.MarketingResearch" %>

<fieldset class="market clear" style="width: 67em">
    <legend>For market research (optional)</legend>
    <div class="leftContainer">
        <p>
            <asp:Label ID="IndustryLabel" runat="server" AssociatedControlID="IndustryCombo">What is your industry?</asp:Label>
            <asp:DropDownList id="IndustryCombo" runat="server" EnableViewState="true" ViewStateMode="Enabled" AppendDataBoundItems="true">
                <asp:ListItem Text="-- Select one --" Value=""></asp:ListItem>
                <asp:ListItem Text="-- Other" Value="Other"></asp:ListItem>
                <asp:ListItem Text="-- N/A" Value="N/A"></asp:ListItem>
            </asp:DropDownList>
        </p>
        <p>
            <asp:Label ID="JobCategoryLabel" runat="server" AssociatedControlID="JobCategoryCombo">What is your job category?</asp:Label>
            <asp:DropDownList id="JobCategoryCombo" runat="server" EnableViewState="true" ViewStateMode="Enabled" AppendDataBoundItems="true">
                <asp:ListItem Text="-- Select one --" Value=""></asp:ListItem>
                <asp:ListItem Text="-- Other" Value="Other"></asp:ListItem>
                <asp:ListItem Text="-- N/A" Value="N/A"></asp:ListItem>
            </asp:DropDownList>
        </p>
    </div>
    <div class="rightContainer">
        <p>
            <asp:Label ID="CompanySizeLabel" runat="server" AssociatedControlID="CompanySizeCombo">What is the size of your employeer?</asp:Label>
            <asp:DropDownList id="CompanySizeCombo" runat="server" EnableViewState="true" ViewStateMode="Enabled" AppendDataBoundItems="true">
                <asp:ListItem Text="-- Select one --" Value=""></asp:ListItem>
                <asp:ListItem Text="-- Other" Value="Other"></asp:ListItem>
                <asp:ListItem Text="-- N/A" Value="N/A"></asp:ListItem>
                <asp:ListItem Text="Nonemployer" Value="Nonemployer"></asp:ListItem>
            </asp:DropDownList>
        </p>
        <p>
            <asp:Label ID="CountryLabel" runat="server" AssociatedControlID="CountryCombo">What country are you from?</asp:Label>
            <asp:DropDownList id="CountryCombo" runat="server" EnableViewState="true" ViewStateMode="Enabled" AppendDataBoundItems="true">
                <asp:ListItem Text="-- Select one --" Value=""></asp:ListItem>
            </asp:DropDownList>
        </p>
    </div>
</fieldset>
