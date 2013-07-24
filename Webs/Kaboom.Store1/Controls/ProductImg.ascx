<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProductImg.ascx.cs" Inherits="Kaboom.Store1.Controls.ProductImg" %>
<%@ Import Namespace="Kaboom.Store1.Logic" %>
<asp:PlaceHolder runat="server" ID="ph1" Visible='<%# ProductNumber == KnownProduct.KaboomCombo_1x2_30_Trial %>'><img src="images/product-combo.jpg" width="138" height="87"></asp:PlaceHolder>
<asp:PlaceHolder runat="server" ID="ph2" Visible='<%# ProductNumber == KnownProduct.KaboomCombo_1x12_60_Upsell %>'><img src="images/product-combo.jpg" width="138" height="87"></asp:PlaceHolder>
<asp:PlaceHolder runat="server" ID="ph3" Visible='<%# ProductNumber == KnownProduct.KaboomStrips_1x12_Upsell %>'><img src="images/product-action-strips.jpg" width="68" height="87"></asp:PlaceHolder>
<asp:PlaceHolder runat="server" ID="ph4" Visible='<%# ProductNumber == KnownProduct.KaboomDaily_1x60_Upsell %>'><img src="images/product-kaboom-daily.jpg" width="48" height="87"></asp:PlaceHolder>
<asp:PlaceHolder runat="server" ID="ph5" Visible='<%# ProductNumber == KnownProduct.KaboomDaily_1x30_Upsell %>'><img src="images/product-kaboom-daily.jpg" width="48" height="87"></asp:PlaceHolder>