<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Testimonial.aspx.cs" Inherits="BeautyTruth.Store1.Testimonial"
    MasterPageFile="~/Controls/Front.Master" %>

<%@ Register TagPrefix="uc" TagName="Footer" Src="~/Controls/Footer.ascx" %>
<%@ Register TagPrefix="uc" TagName="CategoryMenu" Src="~/Controls/CategoryMenu.ascx" %>
<%@ Register TagPrefix="uc" TagName="ProductReviews" Src="~/Controls/ProductReviews.ascx" %>
<asp:Content ContentPlaceHolderID="cphTitle" runat="server">
    Beauty &amp; Truth: Anti-Aging Wrinkle Creams & Skin Care for Face and Eye</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <div class="container_12">
        <uc:CategoryMenu runat="server" />
        <uc:ProductReviews runat="server" />
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphFooter" runat="server">
    <uc:Footer ID="Footer1" WithSignIn="true" runat="server" />
</asp:Content>
