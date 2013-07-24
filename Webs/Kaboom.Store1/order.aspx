<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/Front.Master" AutoEventWireup="true" CodeBehind="order.aspx.cs" Inherits="Kaboom.Store1.cart" EnableViewState="true" %>
<%@ Register TagPrefix="uc" TagName="ShoppingCart" Src="~/Controls/ShoppingCart.ascx" %>
<%@ Register TagPrefix="uc" TagName="BillingInfo" Src="~/Controls/BillingInfo.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">Kaboom For Men: Secure Checkout</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphScript" runat="server">
<script type="text/javascript" language="javascript" src="js/jquery.validationEngine.validationRules.js"></script>
<script type="text/javascript" language="javascript" src="js/jquery.validationEngine.js"></script>
<script type="text/javascript" language="javascript" src="js/mod10.js"></script>
<script type="text/javascript" language="javascript" src="js/ajax.login.js"></script>
<script type="text/javascript">
    $(document).ready(function() {
        $("#products").validationEngine({
            promptPosition: "topRight",
            inline: true
        });
    });
    $(document).ready(function() {
        $("#customer").validationEngine({
            promptPosition: "topRight",
            inline: true
        });
    });
    function validateProducts() {
        return $("#products").validationEngine({
            returnIsValid: true,
            promptPosition: "topRight",
            inline: true
        });
    }
    function validateBilling() {
        return $("#customer").validationEngine({
            returnIsValid: true,
            promptPosition: "topRight",
            inline: true
        });
    }
    function completeOrder() {
        if (validateProducts() && validateBilling())
        {
            //$("#<%# lbCompleteOrder.ClientID %>").removeAttr('onclick').click(function() { return false; });
            return true;
        }
        return false;
    }
    <asp:PlaceHolder runat="server" id="phScrollToBilling" Visible='<%# CurrentState == CartState.Checkout %>'>
    $(document).ready(function() { 
        var pos = ($("#error").length > 0) ? $("#error").offset().top : $("#customer").offset().top;
        $('html,body').animate({ scrollTop: pos }, 1100); 
    });
    </asp:PlaceHolder>
</script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
<form runat="server" ID="form1">
<asp:PlaceHolder runat="server" ID="phSecureBanner" Visible='<%# CurrentState == CartState.Checkout %>'>
  <div style="text-align:center; font-size:20px; font-weight:bold; margin:-20px 0 0 0; color:#2c4762;">
    Secure Checkout&nbsp;&nbsp;<img src="images/godaddy.png" width="115" height="60" align="absmiddle">&nbsp;&nbsp;128-Bit Encryption
  </div>
</asp:PlaceHolder>
<div id="cart">
  <asp:PlaceHolder runat="server" ID="phProductsError">
  <div id="error">
    <asp:Literal runat="server" ID="lProductsError" />
  </div>
  <br />
  </asp:PlaceHolder>
  <img src="images/form-step2.jpg" width="950" height="45" />
  <uc:ShoppingCart runat="server" ID="ShoppingCart1" OnProductsChanged="ShoppingCart1_ProductsChanged" OnCouponChanged="ShoppingCart1_CouponChanged" OnRefererCodeChanged="ShoppingCart1_RefererCodeChanged" />
  <asp:PlaceHolder runat="server" ID="phBillingError">
  <div id="error">
    <asp:Literal runat="server" ID="lBillingError" />
  </div>
  </asp:PlaceHolder>
  <uc:BillingInfo runat="server" ID="BillingInfo1" Visible='<%# CurrentState == CartState.Checkout %>' />
  <div id="checkout">
    <table width="100%" border="0" cellpadding="0" cellspacing="0" class="noborder" >
      <tr>
        <asp:PlaceHolder runat="server" ID="phButtonsCart" Visible='<%# CurrentState == CartState.Cart %>'>
          <td><span class="buttons back"><a href="<%# KeepShoppingUrl %>">&laquo; Keep Shopping</a></span></td>
          <td align="right"><span class="buttons checkout">
            <asp:LinkButton runat="server" ID="lbCheckout" Text="Checkout &raquo;" OnClick="lbCheckout_Click"
                OnClientClick="return validateProducts();" TabIndex="17"></asp:LinkButton></span>
          </td>
        </asp:PlaceHolder>
        <asp:PlaceHolder runat="server" ID="phButtonsCheckout" Visible='<%# CurrentState == CartState.Checkout %>'>
          <td><span class="buttons back"><a href="default.aspx">&laquo; Change Order</a></span></td>
          <td align="right"><span class="buttons checkout">
          <asp:LinkButton runat="server" ID="lbCompleteOrder" Text="Complete Order" OnClick="lbCompleteOrder_Click"
            OnClientClick="return completeOrder();" TabIndex="17"></asp:LinkButton></span>
          </td>        
        </asp:PlaceHolder>
      </tr>
    </table>
  </div>
</div>
</form>
</asp:Content>
