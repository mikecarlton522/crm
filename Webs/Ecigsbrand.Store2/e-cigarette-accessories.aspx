<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/Front.Master" AutoEventWireup="true" CodeBehind="e-cigarette-accessories.aspx.cs" Inherits="Ecigsbrand.Store2.e_cigarette_accessories" %>
<%@ Register TagPrefix="uc" TagName="RefillCartridges" Src="~/Controls/RefillCartridges.ascx" %>
<%@ Import Namespace="Ecigsbrand.Store2.Logic" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">E-Cigs Electronic Cigarette: E-Cigarette Accessories, Car Chargers, Wall Chargers</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
<div id="content">
  <div class="left">
    <h1>Accessories</h1>
    <img src="images/box-car-charger.png" width="610" height="200" style="margin-top:15px;">
    <table width="100%" border="0" cellspacing="0" cellpadding="8" style="margin-top:10px;">
      <tr id="product-el-1">
        <td width="44%"><strong id="product-id-1" value='<%# (int)KnownProduct.CarCharger %>'>E-Cigs Car Charger</strong><br>
          <span class="small">Converts USB to standard car outlet.</span></td>
        <td width="13%"><span class="bold green">In Stock!</span></td>
        <td width="14%">Qty:
          <input name="quantity" id="product-count-1" type="text" value="1" size="2" maxlength="2" class="quantity" /></td>
        <td width="9%"><strong><%# FormatPrice(ShoppingCart.KnownProducts[KnownProduct.CarCharger].Price)%></strong></td>
        <td width="20%" align="right"><a href="#" id="add-product-1"><img src="images/button-add-to-cart.png" width="111" height="19" border="0"></a></td>
      </tr>
    </table>
    <img src="images/box-wall-charger.png" width="610" height="200" style="margin-top:40px;">
    <table width="100%" border="0" cellspacing="0" cellpadding="8" style="margin-top:10px;">
      <tr id="product-el-2">
        <td width="44%"><strong id="product-id-2" value='<%# (int)KnownProduct.WallCharger %>'>E-Cigs Wall Charger</strong><br>
          <span class="small">Converts USB to standard wall outlet.</span></td>
        <td width="13%"><span class="bold green">In Stock!</span></td>
        <td width="14%">Qty:
          <input name="quantity" id="product-count-2" type="text" value="1" size="2" maxlength="2" class="quantity" /></td>
        <td width="9%"><strong><%# FormatPrice(ShoppingCart.KnownProducts[KnownProduct.WallCharger].Price)%></strong></td>
        <td width="20%" align="right"><a href="#" id="add-product-2"><img src="images/button-add-to-cart.png" width="111" height="19" border="0"></a></td>
      </tr>
    </table>
    <img src="images/box-battery.png" width="610" height="200" style="margin-top:40px;">
    <table width="100%" border="0" cellspacing="0" cellpadding="8" style="margin-top:10px;">
      <tr id="product-el-3">
        <td width="44%"><strong id="product-id-3" value='<%# (int)KnownProduct.Battery %>'>E-Cigs Battery</strong><br>
          <span class="small">High-Capacity Lithium-Ion Battery.</span></td>
        <td width="13%"><span class="bold green">In Stock!</span></td>
        <td width="14%">Qty:
          <input name="quantity" id="product-count-3" type="text" value="1" size="2" maxlength="2" class="quantity" /></td>
        <td width="9%"><strong><%# FormatPrice(ShoppingCart.KnownProducts[KnownProduct.Battery].Price)%></strong></td>
        <td width="20%" align="right"><a href="#" id="add-product-3"><img src="images/button-add-to-cart.png" width="111" height="19" border="0"></a></td>
      </tr>
    </table>
  </div>
  <div class="right">
    <uc:RefillCartridges ID="cRefillCartridges" runat="server" />
  </div>
  <div style="clear:both;"></div>
</div>
</asp:Content>
