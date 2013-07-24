<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Kaboom.Store1._default" MasterPageFile="~/Controls/Front.Master" %>
<%@ Import Namespace="Kaboom.Store1.Logic" %>
<%@ Register TagPrefix="uc" TagName="ProductDescription" Src="~/Controls/ProductDescription.ascx" %>
<%@ Register TagPrefix="uc" TagName="ProductImg" Src="~/Controls/ProductImg.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">Kaboom For Men: Male Enhancement Product</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
  <form runat="server">
  <asp:PlaceHolder runat="server" ID="phError">
  <div class="error">
    <asp:Literal runat="server" ID="lError" />
  </div>
  </asp:PlaceHolder>
  <img src="images/form-step1.jpg" width="950" height="45" />
  <table width="950" cellpadding="10" cellspacing="0" class="product">
    <tr class="header">
      <td width="152" align="center">Product</td>
      <td width="480">Description</td>
      <td width="60">Availability</td>
      <td width="49">Shipping</td>
      <td width="38">Price</td>
      <td width="49">Quantity</td>
    </tr>
    <asp:PlaceHolder runat="server" ID="phHideTrial" Visible="false">
    <tr class="bottom">
      <td align="center"><uc:ProductImg runat="server" ID="ProductImg1" Product='<%# ShoppingCart.KnownProducts[KnownProduct.KaboomCombo_1x2_30_Trial] %>' /></td>
      <td class="description"><uc:ProductDescription runat="server" Product='<%# ShoppingCart.KnownProducts[KnownProduct.KaboomCombo_1x2_30_Trial] %>' Type="Promotion" /></td>
      <td class="bold green">In Stock</td>
      <td class="bold blue">Free</td>
      <td><%# FormatPrice(ShoppingCart.KnownProducts[KnownProduct.KaboomCombo_1x2_30_Trial].Price)%></td>
      <td><asp:DropDownList runat="server" ID="DropDownList1" /></</td>
    </tr>
    <tr class="bottom">
      <td colspan="6"><div style="margin:5px; padding:10px; background-color:#FFC; border:1px dotted gray;">
          <span class="bold red">Trial Offer Terms: </span>By placing an order you will be enrolled in our Kaboom refill membership program. This program will ship a full-size, 60 capsule,
          bottle of Kaboom Daily along with 12 Kaboom Action Strips for $79.99 + S/H on the 12th day and every 30 days thereafter. You can cancel or modify your membership anytime by calling 1-800-969-5044.
        </div></td>
    </tr>
    </asp:PlaceHolder>
    <tr class="bottom">
      <td align="center"><uc:ProductImg runat="server" ID="ProductImg2" Product='<%# ShoppingCart.KnownProducts[KnownProduct.KaboomCombo_1x12_60_Upsell] %>' /></td>
      <td class="description"><uc:ProductDescription runat="server" Product='<%# ShoppingCart.KnownProducts[KnownProduct.KaboomCombo_1x12_60_Upsell] %>' Type="Promotion" /></td>
      <td class="bold green">In Stock</td>
      <td class="bold blue">Free</td>
      <td><%# FormatPrice(ShoppingCart.KnownProducts[KnownProduct.KaboomCombo_1x12_60_Upsell].Price)%></td>
      <td><asp:DropDownList runat="server" ID="DropDownList2" /></</td>
    </tr>
    <tr class="bottom">
      <td align="center"><uc:ProductImg runat="server" ID="ProductImg3" Product='<%# ShoppingCart.KnownProducts[KnownProduct.KaboomStrips_1x12_Upsell] %>' /></td>
      <td class="description"><uc:ProductDescription runat="server" Product='<%# ShoppingCart.KnownProducts[KnownProduct.KaboomStrips_1x12_Upsell] %>' Type="Promotion" /></td>
      <td class="bold green">In Stock</td>
      <td class="bold blue">Free</td>
      <td><%# FormatPrice(ShoppingCart.KnownProducts[KnownProduct.KaboomStrips_1x12_Upsell].Price)%></td>
      <td><asp:DropDownList runat="server" ID="DropDownList3" /></</td>
    </tr>
    <tr class="bottom">
      <td align="center"><uc:ProductImg runat="server" ID="ProductImg4" Product='<%# ShoppingCart.KnownProducts[KnownProduct.KaboomDaily_1x60_Upsell] %>' /></td>
      <td class="description"><uc:ProductDescription runat="server" Product='<%# ShoppingCart.KnownProducts[KnownProduct.KaboomDaily_1x60_Upsell] %>' Type="Promotion" /></td>
      <td class="bold green">In Stock</td>
      <td class="bold blue">Free</td>
      <td><%# FormatPrice(ShoppingCart.KnownProducts[KnownProduct.KaboomDaily_1x60_Upsell].Price)%></td>
      <td><asp:DropDownList runat="server" ID="DropDownList4" /></</td>
    </tr>
    <tr>
      <td align="center"><uc:ProductImg runat="server" ID="ProductImg5" Product='<%# ShoppingCart.KnownProducts[KnownProduct.KaboomDaily_1x30_Upsell] %>' /></td>
      <td class="description"><uc:ProductDescription runat="server" Product='<%# ShoppingCart.KnownProducts[KnownProduct.KaboomDaily_1x30_Upsell] %>' Type="Promotion" /></td>
      <td class="bold green">In Stock</td>
      <td class="bold blue">Free</td>
      <td><%# FormatPrice(ShoppingCart.KnownProducts[KnownProduct.KaboomDaily_1x30_Upsell].Price)%></td>
      <td><asp:DropDownList runat="server" ID="DropDownList5" /></</td>
    </tr>
  </table>
  <div id="cart">
    <div id="checkout">
      <table width="100%" border="0" cellpadding="0" cellspacing="0" class="noborder" >
        <tr>
          <td><span style="font-weight:bold; font-size:20px; color:#2c4762;">Order By Phone: 1-800-969-5044</span></td>
          <td align="right"><span class="buttons checkout">
            <asp:LinkButton runat="server" ID="lbCheckout" Text="Continue Order" OnClick="lbCheckout_Click"
                OnClientClick="return validateProducts();" TabIndex="17"></asp:LinkButton>            
          </span></td>
        </tr>
      </table>
    </div>
  </div>
  </form>
</asp:Content>