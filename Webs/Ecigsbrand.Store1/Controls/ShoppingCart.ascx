<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ShoppingCart.ascx.cs" Inherits="Ecigsbrand.Store1.Controls.ShoppingCart_" %>
<%@ Import Namespace="Ecigsbrand.Store1.Logic" %>
<%@ Register TagPrefix="uc" TagName="ProductDescription" Src="~/Controls/ProductDescription.ascx" %>
<%@ Register TagPrefix="uc" TagName="ProductImg" Src="~/Controls/ProductImg.ascx" %>
<asp:PlaceHolder runat="server" ID="phScripts">
<script type="text/javascript">
    function validateCoupon() {
        return !($.validationEngine.loadValidation("#<%# tbCoupon.ClientID %>"));
    }
    function validateRefererCode() {
        return !($.validationEngine.loadValidation("#<%# tbRefererCode.ClientID %>"));
    }
</script>
</asp:PlaceHolder>
<table width="100%" border="0" cellspacing="0" cellpadding="5">
    <tbody id="products">
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
        <asp:TextBox runat="server" ID="tbProductCount" CssClass="quantity validate[custom[Quantity]]" MaxLength="2"
            Text='<%# DataBinder.Eval(Container.DataItem, "Value") %>'>
        </asp:TextBox>        
        <asp:HiddenField runat="server" ID="hdnProductID" Value='<%# (int)ShoppingCart.GetProductNumber((ShoppingCartProduct)DataBinder.Eval(Container.DataItem, "Key")) %>' />
      <td class="last" align="right"><%# FormatPrice((decimal)DataBinder.Eval(Container.DataItem, "Key.Price")) %></td>
    </tr>
    </ItemTemplate>
</asp:Repeater>
    </tbody>
    <tr class="item update">
      <td colspan="4" align="right"><asp:Button runat="server" ID="bUpdateQuantities" Text="Update Quantities" OnClick="bUpdateQuantities_Click" OnClientClick="return validateProducts();" CssClass="button" /></td>
      <td class="last" align="right"></td>
    </tr>
    <tr class="total">
      <td colspan="3">
        Coupon Code: 
        <asp:PlaceHolder runat="server" ID="phEnterCoupon" Visible='<%# true %>'>
            <asp:TextBox runat="server" ID="tbCoupon" MaxLength="15" Text="" style="width:125px;" CssClass="validate[custom[Coupon]]"></asp:TextBox>&nbsp;
            <asp:Button runat="server" ID="bApplyCoupon" Text="Apply" OnClick="bApplyCoupon_Click" OnClientClick="return validateCoupon();" CssClass="button" />
            <asp:PlaceHolder runat="server" ID="phCouponError">
                <div class="localError">We're sorry, this coupon code is not valid, please verify code and try again</div>                
            </asp:PlaceHolder>
            <asp:PlaceHolder runat="server" ID="phCouponError2">
                <div class="localError">This appears to be a gift card.  Would you like to use it?</div>                
            </asp:PlaceHolder>
        </asp:PlaceHolder>
        <asp:PlaceHolder runat="server" ID="phCouponApplied" Visible='<%# Coupon != null %>'>            
            <br/>Coupon Applied: 
            <%# (Coupon != null) ? Coupon.CouponCode : string.Empty %>
            <%# (Coupon != null && Coupon.Discount != null) ? string.Format(", {0:p2} discount applied", Coupon.Discount.Value) : string.Empty %>
        </asp:PlaceHolder>
      </td>
      <td align="right">
        Shipping
      </td>
      <td class="last" align="right">FREE</td>
    </tr>    
    <tr class="total">
      <td colspan="3">
        Referer Code: 
        <asp:PlaceHolder runat="server" ID="phRefererCode" Visible='<%# RefererCode == null %>'>
            <asp:TextBox runat="server" ID="tbRefererCode" MaxLength="50" Text="" style="width:125px;" CssClass="validate[custom[RefererCode]]"></asp:TextBox>&nbsp;
            <asp:Button runat="server" ID="bApplyRefererCode" Text="Apply" OnClick="bApplyRefererCode_Click" OnClientClick="return validateRefererCode();" CssClass="button" />
            <asp:PlaceHolder runat="server" ID="phRefererCodeError">
                <div class="localError">We're sorry, this referer code is not valid, please verify code and try again</div>                
            </asp:PlaceHolder>
        </asp:PlaceHolder>
        <asp:PlaceHolder runat="server" ID="phRefererCodeApplied" Visible='<%# RefererCode != null %>'>
            <%# RefererCode %>
            <%# (RefererCode != null) ? string.Format(", {0:p2} discount applied", RefererDiscount) : string.Empty%>
        </asp:PlaceHolder>
      </td>
      <td align="right"></td>
      <td class="last" align="right"></td>
    </tr>
    <asp:PlaceHolder runat="server" ID="phEcigBucks" Visible='<%# Ecigsbrand.Store1.Logic.ShoppingCart.Current.EcigBucksRedeem > 0M %>'>
        <tr class="total">
          <td colspan="4" align="right">
            E-Cigs Dollars Applied
            (<asp:LinkButton runat="server" ID="lbRemoveEcigBucks" OnClick="lbRemoveEcigBucks_Click">Remove</asp:LinkButton>)
          </td>
          <td class="last" align="right"><%# FormatPrice(0M - Ecigsbrand.Store1.Logic.ShoppingCart.Current.EcigBucksRedeem) %></td>
        </tr>
    </asp:PlaceHolder>
    <asp:Repeater runat="server" ID="rGiftCertificateList" OnItemCommand="rGiftCertificateList_ItemCommand">
        <ItemTemplate>
        <tr class="total">
          <td colspan="4" align="right">
            Gift Certificate <%# Eval("GiftNumber")%> Applied
            (<asp:LinkButton runat="server" ID="lbRemoveGiftCertificate" CommandName="Remove" CommandArgument='<%# Eval("GiftNumber")%>'>Remove</asp:LinkButton>)
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
