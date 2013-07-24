<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ShoppingCartView.ascx.cs" Inherits="Ecigsbrand.Store1.Controls.ShoppingCartView" %>
<%@ Import Namespace="Ecigsbrand.Store1.Logic" %>
<%@ Register TagPrefix="uc" TagName="ProductDescription" Src="~/Controls/ProductDescription.ascx" %>
<%@ Register TagPrefix="uc" TagName="ProductImg" Src="~/Controls/ProductImg.ascx" %>
<table width="100%" border="0" cellspacing="0" cellpadding="5">
    <tr class="header">
      <td width="13%" align="center">Product</td>
      <td width="63%">Description</td>
      <td width="8%" align="center">Availability</td>
      <td width="8%" align="center">Quantity</td>
      <td width="8%" align="right">Price</td>
    </tr>
<asp:Repeater runat="server" ID="rProducts">
    <ItemTemplate>
    <tr class="item">        
      <td align="center"><div class="cart picture"><uc:ProductImg runat="server" ID="ProductImg1" Product='<%# DataBinder.Eval(Container.DataItem, "Key") %>' /></div></td>
      <td class="bold"><uc:ProductDescription runat="server" ID="ProductDescription1" Product='<%# DataBinder.Eval(Container.DataItem, "Key") %>' /></td>
      <td align="center" class="bold green">In Stock!</td>
      <td align="center">
        <%# DataBinder.Eval(Container.DataItem, "Value") %>
      <td class="last" align="right"><%# FormatPrice((decimal)DataBinder.Eval(Container.DataItem, "Key.Price")) %></td>
    </tr>
    </ItemTemplate>
</asp:Repeater>
    <tr class="total">
      <td colspan="3">
        <asp:PlaceHolder runat="server" ID="phCouponApplied" Visible='<%# Coupon != null %>'>
            Coupon Code:
            <%# (Coupon != null) ? Coupon.CouponCode : string.Empty %>
            <%# (Coupon != null && Coupon.Discount != null) ? string.Format(", {0:p2} discount applied", Coupon.Discount.Value) : string.Empty %>
        </asp:PlaceHolder>
      </td>
      <td align="right">
        Shipping
      </td>
      <td class="last" align="right">FREE</td>
    </tr>
    <asp:PlaceHolder runat="server" ID="phEcigBucks" Visible='<%# EcigBucksAmountApplied > 0M %>'>
        <tr class="total">
          <td colspan="4" align="right">
            E-Cigs Dollars Applied
          </td>
          <td class="last" align="right"><%# FormatPrice(0M - EcigBucksAmountApplied)%></td>
        </tr>
    </asp:PlaceHolder>
    <asp:Repeater runat="server" ID="rGiftCertificateList">
        <ItemTemplate>
        <tr class="total">
          <td colspan="4" align="right">
            Gift Certificate <%# Eval("GiftNumber")%> Applied
          </td>
          <td class="last" align="right"><%# FormatPrice(0M - (decimal)Eval("Value"))%></td>
        </tr>
        </ItemTemplate>        
    </asp:Repeater>
    <tr class="total bottom bold">
      <td colspan="4" align="right">Total</td>
      <td class="last" align="right"><%# FormatPrice(TotalCost) %></td>
    </tr>
</table>
