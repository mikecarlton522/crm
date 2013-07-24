<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/Front.Master" AutoEventWireup="true" CodeBehind="e-cigarette-refill-cartridges.aspx.cs" Inherits="Ecigsbrand.Store2.e_cigarette_refill_cartridges" %>
<%@ Register TagPrefix="uc" TagName="Accessories" Src="~/Controls/Accessories.ascx" %>
<%@ Import Namespace="Ecigsbrand.Store2.Logic" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">E-Cigs Electronic Cigarette: E-Cigarette Refill Nicotine Cartridges</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
<div id="content">
  <div class="left">
    <h1>Refill Cartridges</h1>
    <img src="images/box-cartridge-tobacco.png" width="610" height="200" style="margin-top:15px;">
    <table width="100%" border="0" cellspacing="0" cellpadding="8" style="margin-top:10px;">
      <tr id="product-el-1">
        <td width="44%"><strong id="product-id-1" value='<%# (int)KnownProduct.OriginalFlavor_StandardNicotine %>'>Standard Nicotine</strong><br>
          <span class="small">Box of 10 Cartridges</span></td>
        <td width="13%"><span class="bold green">In Stock!</span></td>
        <td width="14%">Qty:
          <input name="quantity" id="product-count-1" type="text" value="1" size="2" maxlength="2" class="quantity" /></td>
        <td width="9%"><strong><%# FormatPrice(ShoppingCart.KnownProducts[KnownProduct.OriginalFlavor_StandardNicotine].Price)%></strong></td>
        <td width="20%" align="right"><a href="#" id="add-product-1"><img src="images/button-add-to-cart.png" width="111" height="19" border="0"></a></td>
      </tr>
      <tr id="product-el-2">
        <td width="44%"><strong id="product-id-2" value='<%# (int)KnownProduct.OriginalFlavor_MediumNicotine %>'>Medium Nicotine</strong><br>
          <span class="small">Box of 10 Cartridges</span></td>
        <td width="13%"><span class="bold green">In Stock!</span></td>
        <td width="14%">Qty:
          <input name="quantity" id="product-count-2" type="text" value="1" size="2" maxlength="2" class="quantity" /></td>
        <td width="9%"><strong><%# FormatPrice(ShoppingCart.KnownProducts[KnownProduct.OriginalFlavor_MediumNicotine].Price)%></strong></td>
        <td width="20%" align="right"><a href="#" id="add-product-2"><img src="images/button-add-to-cart.png" width="111" height="19" border="0"></a></td>
      </tr>
      <tr id="product-el-3">
        <td width="44%"><strong id="product-id-3" value='<%# (int)KnownProduct.OriginalFlavor_LowNicotine %>'>Low Nicotine</strong><br>
          <span class="small">Box of 10 Cartridges</span></td>
        <td width="13%"><span class="bold green">In Stock!</span></td>
        <td width="14%">Qty:
          <input name="quantity" id="product-count-3" type="text" value="1" size="2" maxlength="2" class="quantity" /></td>
        <td width="9%"><strong><%# FormatPrice(ShoppingCart.KnownProducts[KnownProduct.OriginalFlavor_LowNicotine].Price)%></strong></td>
        <td width="20%" align="right"><a href="#" id="add-product-3"><img src="images/button-add-to-cart.png" width="111" height="19" border="0"></a></td>
      </tr>
      <tr>
        <td colspan="5">Each Box of 10 Cartridges Equals 20 Packs or 400 Cigarettes. </td>
      </tr>
    </table>
    <img src="images/box-cartridge-menthol.png" width="610" height="200" style="margin-top:40px;">
    <table width="100%" border="0" cellspacing="0" cellpadding="8" style="margin-top:10px;">
      <tr id="product-el-4">
        <td width="44%"><strong id="product-id-4" value='<%# (int)KnownProduct.MentholFlavor_StandardNicotine %>'>Standard Nicotine</strong><br>
          <span class="small">Box of 10 Cartridges</span></td>
        <td width="13%"><span class="bold green">In Stock!</span></td>
        <td width="14%">Qty:
          <input name="quantity" id="product-count-4" type="text" value="1" size="2" maxlength="2" class="quantity" /></td>
        <td width="9%"><strong><%# FormatPrice(ShoppingCart.KnownProducts[KnownProduct.MentholFlavor_StandardNicotine].Price)%></strong></td>
        <td width="20%" align="right"><a href="#" id="add-product-4"><img src="images/button-add-to-cart.png" width="111" height="19" border="0"></a></td>
      </tr>
      <tr id="product-el-5">
        <td width="44%"><strong id="product-id-5" value='<%# (int)KnownProduct.MentholFlavor_LowNicotine %>'>Low Nicotine</strong><br>
          <span class="small">Box of 10 Cartridges</span></td>
        <td width="13%"><span class="bold green">In Stock!</span></td>
        <td width="14%">Qty:
          <input name="quantity" id="product-count-5" type="text" value="1" size="2" maxlength="2" class="quantity" /></td>
        <td width="9%"><strong><%# FormatPrice(ShoppingCart.KnownProducts[KnownProduct.MentholFlavor_LowNicotine].Price)%></strong></td>
        <td width="20%" align="right"><a href="#" id="add-product-5"><img src="images/button-add-to-cart.png" width="111" height="19" border="0"></a></td>
      </tr>
      <tr>
        <td colspan="5"> Each Box of 10 Cartridges Equals 20 Packs or 400 Cigarettes. </td>
      </tr>
    </table>
  </div>
  <div class="right">
    <uc:Accessories ID="cAccessories" runat="server" />
  </div>
  <div style="clear:both;"></div>
</div>
</asp:Content>
