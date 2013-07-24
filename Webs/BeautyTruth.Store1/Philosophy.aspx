<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Philosophy.aspx.cs" Inherits="BeautyTruth.Store1.Philosophy"
    MasterPageFile="~/Controls/Front.Master" %>

<%@ Register TagPrefix="uc" TagName="Footer" Src="~/Controls/Footer.ascx" %>
<%@ Register TagPrefix="uc" TagName="CategoryMenu" Src="~/Controls/CategoryMenu.ascx" %>
<asp:Content ContentPlaceHolderID="cphTitle" runat="server">
    Beauty &amp; Truth: Anti-Aging Wrinkle Creams & Skin Care for Face and Eye</asp:Content>
<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <div class="container_12">
        <uc:CategoryMenu ID="CategoryMenu" runat="server" />
        <div class="grid_9 generic">
            <h1>
                Beauty &amp; Truth Philosophy</h1>
            <p>
                At Beauty & Truth, we believe that true beauty radiates from a place inside. We
                believe in delving beneath the surface and nourishing from the inside out. We believe
                that when it comes to skincare, serious science reveals radiant results. No false
                promises, just potent products to restore your gorgeous glow!
            </p>
            <p>
                Beauty & Truth products work with your skin, addressing skin woes on a cellular
                level to help your skin recover from the stresses of environmental toxins and irritants.
                We’ve developed our exceptional skincare system to restore your skin’s natural luminosity
                and rewind the signs of time.
            </p>
            <p>
                Discover the rejuvenation that comes from happy, healthy, holistic skincare!
            </p>
            <h1>
                Our Ingredients</h1>
            <p>
                We’re proud of our ingredients, and source only the purest and most hard-working.
                All of our products are (and always have been) paraben-free. Our line is formulated
                to be gentle and effective on all skin types. We have not tested our products on
                animals, nor have we asked others to do so.
            </p>
            <p>
                For more information on our key ingredients, check out the “Ingredients” tab on
                any one of our product pages. This glossary allows you to expand your product knowledge
                and understand why Beauty & Truth products are special!
            </p>
            <h1>
                Our Customer Service</h1>
            <p>
                We’re passionate about beauty and are eager to share that passion with you! Our
                satisfaction guarantee ensures you can shop with confidence. We’re also happy to
                field questions and comments about any of our products.
            </p>
            <p>
                We encourage you to first explore our FAQ (Frequently Asked Questions) page. If
                you still can’t find what you seek, don’t hesitate to give us a call. Our customer
                service lines are open Monday to Saturday, 8AM – 6PM EST, at 1-800-333-3333. Or,
                if you prefer the written word, drop us a line at <a href="mailto:support@findbeautyandtruth.com">support@findbeautyandtruth.com</a>
            </p>
        </div>
    </div>
</asp:Content>
<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <uc:Footer ID="Footer1" WithSignIn="true" runat="server" />
</asp:Content>
