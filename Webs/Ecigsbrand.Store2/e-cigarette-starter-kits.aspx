<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/Front.Master" AutoEventWireup="true" CodeBehind="e-cigarette-starter-kits.aspx.cs" Inherits="Ecigsbrand.Store2.e_cigarette_starter_kits" %>
<%@ Register TagPrefix="uc" TagName="Accessories" Src="~/Controls/Accessories.ascx" %>
<%@ Import Namespace="Ecigsbrand.Store2.Logic" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphScript" runat="server">
<script>
  function setPrice(el) {
      var price = $(el).find("option:selected").attr("product-price");
      var product_price_id = $(el).attr("id").replace("-id", "-price");
      $("#" + product_price_id).html(price);
  }
  </script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">E-Cigs Electronic Cigarette: E-Cigarette Starter Kits</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
<div id="content">
  <div class="left">
    <h1>Starter Kits</h1>
    <img src="images/box-starter-kit.png" width="610" height="200" style="margin-top:15px;"><img src="images/how-kit-1.jpg" width="160" height="128" border="1" align="right" style="margin:20px 0 0 15px;">
    <h3 style="margin-bottom:0px;">In the box</h3>
    <ul>
      <li>1 Branded Hard Plastic Box (NEW!)</li>
      <li>1 Electronic Cigarette Unit with Lithium-Ion Battery</li>
      <li>2 Nicotine Cartridges<br>
        (Equal to 4 Packs or 80 Cigarettes)</li>
      <li>1 USB Charger</li>
      <li>30 Day Money Back Satisfaction Guarantee</li>
      <li>1 Year Parts Warranty</li>
    </ul>
    <table width="100%" border="0" cellspacing="0" cellpadding="10" style="margin-top:10px;">
      <tr id="product-el-1">
        <td width="27%"><strong>One Time Order<br>
          <span class="bold green">In Stock!</span> </strong></td>
        <td width="24%"><select name="offerQty3" id="product-id-1" onchange="setPrice(this)" style="width:115px;">
            <option value='<%# (int)KnownProduct.StarterKit_OneTimeOrder_OriginalFlavor %>' product-price='<%# FormatPrice(ShoppingCart.KnownProducts[KnownProduct.StarterKit_OneTimeOrder_OriginalFlavor].Price) %>' selected>Original Flavor</option>
            <!--
            <option value='<%# (int)KnownProduct.StarterKit_OneTimeOrder_MentholFlavor %>' product-price='<%# FormatPrice(ShoppingCart.KnownProducts[KnownProduct.StarterKit_OneTimeOrder_MentholFlavor].Price) %>'>Menthol Flavor</option>
            -->
          </select></td>
        <td width="18%">Qty:
          <input name="quantity" id="product-count-1" type="text" value="1" size="2" maxlength="2" class="quantity" /></td>
        <td width="10%"><strong id="product-price-1"><%# FormatPrice(ShoppingCart.KnownProducts[KnownProduct.StarterKit_OneTimeOrder_OriginalFlavor].Price) %></strong></td>
        <td width="21%" align="right"><a href="#" id="add-product-1"><img src="images/button-add-to-cart.png" width="111" height="19" border="0"></a></td>
      </tr>
      <tr id="product-el-2">
        <td><strong>15 Day Trial</strong><br>
          <span class="bold green">In Stock!</span></td>
        <td><select name="offerQty3" id="product-id-2" onchange="setPrice(this)" style="width:115px;">
            <option value='<%# (int)KnownProduct.StarterKit_TrialOrder_OriginalFlavor %>' product-price='<%# FormatPrice(ShoppingCart.KnownProducts[KnownProduct.StarterKit_TrialOrder_OriginalFlavor].Price) %>' selected>Original Flavor</option>
            <!--
            <option value='<%# (int)KnownProduct.StarterKit_TrialOrder_MentholFlavor %>' product-price='<%# FormatPrice(ShoppingCart.KnownProducts[KnownProduct.StarterKit_TrialOrder_MentholFlavor].Price) %>'>Menthol Flavor</option>
            -->
          </select></td>
        <td>Qty:
          <input name="quantity" id="product-count-2" product-limit="1" type="text" value="1" disabled="disabled" size="2" maxlength="2" class="quantity" /></td>
        <td><strong id="product-price-2"><%# FormatPrice(ShoppingCart.KnownProducts[KnownProduct.StarterKit_TrialOrder_OriginalFlavor].Price) %></strong></td>
        <td align="right"><a href="#" id="add-product-2"><img src="images/button-add-to-cart.png" width="111" height="19" border="0"></a></td>
      </tr>
      <tr>
        <td colspan="5" style="font-size:13px;">Menthol Starter Kits - Coming Soon!</td>
      </tr>
      <tr>
        <td colspan="5" style="font-size:13px;">15 Day Trial Offer: Unless you call to cancel, in 15 days and every 30 days thereafter, you will be sent 10 refill packs (equivalent to 20 packs of tobacco cigarettes) for only $79.99 plus S&H.
          To modify your order at anytime call 1-866-830-2464. Limit one per customer.</td>
    </table>
  </div>
  <div class="right">
    <uc:Accessories ID="cAccessories" runat="server" />
  </div>
  <div style="clear:both;"></div>
</div>
</asp:Content>
