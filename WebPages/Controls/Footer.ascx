<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Footer.ascx.cs" Inherits="VeraWAF.WebPages.Controls.Footer" EnableViewState="false" %>
<%@ Register TagPrefix="uc1" TagName="JailImage" Src="~/Controls/JailImage.ascx" %>
<footer itemscope itemtype="http://schema.org/Corporation">
    <div class="footerSection">
        <section>
            <h3 itemprop="name"><asp:Literal ID="litCompanyName1" runat="server"></asp:Literal></h3>
            <p itemprop="address" itemscope itemtype="http://schema.org/PostalAddress">
                <span itemprop="streetAddress">
                    <asp:Literal ID="litCompanyStreet1" runat="server"></asp:Literal>
                    <asp:Literal ID="litCompanyStreet2" runat="server"></asp:Literal>
                </span>,
                <br />
                <span itemprop="postalCode"><asp:Literal ID="litCompanyZip" runat="server"></asp:Literal></span>
                <span itemprop="addressLocality"><asp:Literal ID="litCompanyCity" runat="server"></asp:Literal></span>,
                <span itemprop="addressCountry"><asp:Literal ID="litCompanyCountry" runat="server"></asp:Literal></span>.<br />
            </p>
            E-mail: <asp:Literal id="litCompanyEmail" runat="server"></asp:Literal><br />
            Phone: <span itemprop="telephone"><asp:Literal id="litCompanyPhone" runat="server"></asp:Literal></span><br />
        </section>
    </div>
    <div class="footerSection">
        <section>
            <h3>About</h3>
            <p itemprop="description"><asp:Literal id="litCompanyName2" runat="server"></asp:Literal> provides professional services for the examples industry.</p>
        </section>
    </div>
    <div class="footerSection">
        <section>
            <h3>Solutions</h3>
            <p><asp:Literal id="litCompanyName3" runat="server"></asp:Literal> is an example company that has example solutions.</p>
        </section>
    </div>
    <div class="footerSection rightmostfooterSection">
        <section>
            <a href="/"><uc1:JailImage ID="logo" runat="server" src="/Images/Logo_77x77.png" css="smallLogo" alt="" width="77" height="77" /></a>
            <br />
            <a href="/Privacy.aspx">Privacy policy</a>
            <br />
            <a href="/Terms.aspx">Terms and conditions</a>
            <br />
            <a href="/Feeds.aspx">RSS feeds</a>
            <br />
            <a href="/Trademarks.aspx">Trademarks</a>
            <br />
            <a href="/SiteMapOverview.aspx">Site map</a>
            <br />
        </section>
    </div>  
    <div class="footerSectionBottom clear">
        <span class="copyText">Copyright © <asp:Literal ID="litCopyrightYear" runat="server"></asp:Literal> <span itemprop="name"><asp:Literal id="litCompanyName4" runat="server"></asp:Literal></span>. 
            All rights reserved. Various trademarks held by their respective owners.</span>
        <ul class="socialList">
            <li>
                <a runat="server" id="twitter" title="Twitter" target="_blank" class="fa-stack fa-lg">
                    <i class="fa fa-circle fa-stack-2x"></i>
                    <i class="fa fa-twitter fa-stack-1x icon-top"></i>
                </a>
			</li>
            <li>
                <a runat="server"  id="facebook" title="Facebook" target="_blank" class="fa-stack fa-lg">
                    <i class="fa fa-circle fa-stack-2x"></i>
                    <i class="fa fa-facebook fa-stack-1x icon-top"></i>                
                </a>
			</li>
            <li>
                <a runat="server" id="linkedin" title="LinkedIn" target="_blank" class="fa-stack fa-lg">
                    <i class="fa fa-circle fa-stack-2x"></i>
                    <i class="fa fa-linkedin fa-stack-1x icon-top"></i>
                </a>
			</li>
        </ul>
    </div>
</footer>
