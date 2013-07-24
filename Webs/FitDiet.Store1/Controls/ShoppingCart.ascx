<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ShoppingCart.ascx.cs"
    Inherits="Fitdiet.Store1.Controls.ShoppingCart_" %>
<%@ Import Namespace="Fitdiet.Store1.Logic" %>
<%@ Register TagPrefix="uc" TagName="ProductDescription" Src="~/Controls/ProductDescription.ascx" %>
<%@ Register TagPrefix="uc" TagName="ProductImg" Src="~/Controls/ProductImg.ascx" %>
<%@ Register TagPrefix="uc" TagName="CartMenu" Src="~/Controls/CartMenu.ascx" %>
<asp:PlaceHolder runat="server" ID="phScripts">

    <script type="text/javascript">
        function validateCoupon() {
            return !($.validationEngine.loadValidation("#<%# tbCoupon.ClientID %>"));
        }
        function validateRefererCode() {
            return !($.validationEngine.loadValidation("#<%# tbRefererCode.ClientID %>"));
        }
        function doZero(el) {
            var tr = $(el).closest('tr');
            var tb = $(tr).find('input[type=text]');
            tb.val('0');
        }
    </script>

</asp:PlaceHolder>
<table width="900" border="0" align="center" cellpadding="0" cellspacing="0">
    <tr>
        <td>
            &nbsp;
        </td>
    </tr>
    <tr>
        <td valign="top">
            <table width="900" border="0" cellspacing="0" cellpadding="0">
                <tr>
                    <td style="width: 489px" valign="top" class="main_heading">
                        MY SHOPPING CART
                    </td>
                </tr>
                <tr>
                    <td valign="top" style="padding-top: 15px; width: 697px;">
                        <table width="697" border="0" cellspacing="0" cellpadding="0">
                            <tr>
                                <td width="697" valign="top">
                                    <table width="697" border="0" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td width="697" valign="top">
                                                <uc:CartMenu runat="server" OnStepChanged="ChangeStep_Click" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:PlaceHolder runat="server" ID="phError">
                                                    <div id="error" class="validation-error">
                                                        <asp:Literal runat="server" ID="lError" />
                                                    </div>
                                                </asp:PlaceHolder>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center" valign="top" style="padding-top: 12px;">
                                                <table width="697" border="0" cellspacing="0" cellpadding="3">
                                                    <tr class="green_tr">
                                                        <td width="238" align="left" class="copy14green">
                                                            <strong>Product(s) </strong>
                                                        </td>
                                                        <td width="99" align="center" class="copy14green">
                                                            <strong>Price </strong>
                                                        </td>
                                                        <td width="82" align="center" class="copy14green">
                                                            <strong>Quantity</strong>
                                                        </td>
                                                        <td width="95" align="center" class="copy14green">
                                                            <strong>Total</strong>
                                                        </td>
                                                        <td width="68">
                                                            &nbsp;
                                                        </td>
                                                        <td width="32">
                                                            &nbsp;
                                                        </td>
                                                    </tr>
                                                    <asp:Repeater runat="server" ID="rProducts" OnItemCommand="Products_ItemCommand">
                                                        <ItemTemplate>
                                                            <tr class="item">
                                                                <td align="left" class="copy14grey">
                                                                    <a href="#" class="copy12grey">
                                                                        <uc:ProductDescription runat="server" ID="ProductDescription1" Product='<%# DataBinder.Eval(Container.DataItem, "Key") %>' />
                                                                    </a>
                                                                </td>
                                                                <td align="center" class="copy12grey1">
                                                                    <%# FormatPrice((decimal)DataBinder.Eval(Container.DataItem, "Key.Price")) %>
                                                                </td>
                                                                <td align="center">
                                                                    <asp:TextBox runat="server" ID="tbProductCount" MaxLength="2" Width="30" Text='<%# DataBinder.Eval(Container.DataItem, "Value") %>'>
                            </asp:TextBox>
                                                                    <asp:HiddenField runat="server" ID="hdnProductID" Value='<%# (int)ShoppingCart.GetProductNumber((ShoppingCartProduct)DataBinder.Eval(Container.DataItem, "Key")) %>' />
                                                                    <td align="center" class="copy12grey1">
                                                                        <%# FormatPrice(((decimal)DataBinder.Eval(Container.DataItem, "Key.Price")) * ((int)DataBinder.Eval(Container.DataItem, "Value")))%>
                                                                    </td>
                                                                    <td>
                                                                        <asp:ImageButton ID="ImageButton1" src="images/update_btn.gif" alt="" Width="68"
                                                                            Height="25" runat="server" OnClick="bUpdateQuantities_Click" OnClientClick="return validateProducts();" />
                                                                    </td>
                                                                    <td>
                                                                        <asp:ImageButton runat="server" ID="DeleteButton" src="images/close_btn.gif" alt=""
                                                                            Width="28" Height="26" CommandName="Delete" OnClientClick="doZero(this);" />
                                                                    </td>
                                                            </tr>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                    <tr>
                                                        <td>
                                                            &nbsp;
                                                        </td>
                                                        <td>
                                                            &nbsp;
                                                        </td>
                                                        <td class="copy14green">
                                                            <strong>Subtotal</strong>
                                                        </td>
                                                        <td align="center" class="copy12grey1">
                                                            <%# FormatPrice(TotalCost) %>
                                                        </td>
                                                        <td>
                                                            &nbsp;
                                                        </td>
                                                        <td>
                                                            &nbsp;
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            &nbsp;
                                                        </td>
                                                        <td>
                                                            &nbsp;
                                                        </td>
                                                        <td class="copy14green">
                                                            <strong>S&amp;H</strong>
                                                        </td>
                                                        <td align="center" class="copy12grey1">
                                                            Free
                                                        </td>
                                                        <td>
                                                            &nbsp;
                                                        </td>
                                                        <td>
                                                            &nbsp;
                                                        </td>
                                                    </tr>
                                                    <tr style="display: none;">
                                                        <td>
                                                            &nbsp;
                                                        </td>
                                                        <td>
                                                            &nbsp;
                                                        </td>
                                                        <td class="copy14green">
                                                            <strong>Tax</strong>
                                                        </td>
                                                        <td align="center" class="copy12grey1">
                                                            $0.00
                                                        </td>
                                                        <td>
                                                            &nbsp;
                                                        </td>
                                                        <td>
                                                            &nbsp;
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            &nbsp;
                                                        </td>
                                                        <td>
                                                            &nbsp;
                                                        </td>
                                                        <td class="copy14grey">
                                                            <strong>TOTAL</strong>
                                                        </td>
                                                        <td align="center" class="copy14grey">
                                                            <strong>
                                                                <%# FormatPrice(TotalCost) %></strong>
                                                        </td>
                                                        <td>
                                                            &nbsp;
                                                        </td>
                                                        <td>
                                                            &nbsp;
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="green_tr">
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td width="203" align="right" valign="top">
                        <table width="203" border="0" cellspacing="0" cellpadding="0">
                            <tr>
                                <td class="shopping_gr">
                                    <table width="171" border="0" align="center" cellpadding="2" cellspacing="2">
                                        <tr>
                                            <td style="height: 14px;">
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="copy14grey">
                                                <strong>FIT™ DIET GUARANTEE</strong>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="padding-top: 10px;">
                                                <table width="171" border="0" cellspacing="0" cellpadding="0">
                                                    <tr>
                                                        <td class="copy14green">
                                                            <strong>30-Day Return Policy</strong>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="left" class="main_text">
                                                            If you are completely unsatisfied with Fit™ Diet for any reason, simply contact
                                                            us within 30 days for a full refund of your purchase price.
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="padding-top: 10px;">
                                                <table width="171" border="0" cellspacing="0" cellpadding="0">
                                                    <tr>
                                                        <td class="copy13green">
                                                            <strong class="copy14green">Secure online ordering</strong>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="left" class="main_text">
                                                            Fit™ Diet uses advanced encryption technology to ensure your information is private
                                                            and safe. We will NEVER sell, trade or rent your name or email address to any third
                                                            party.
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
</td> </tr> </table>
<table>
    <!--
<asp:Button runat="server" ID="bUpdateQuantities" Text="Update Quantities" OnClick="bUpdateQuantities_Click" OnClientClick="return validateProducts();" CssClass="button" />
<tr class="item update">
      <td colspan="4" align="right"></td>
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
        </asp:PlaceHolder>
      </td>
      <td align="right"></td>
      <td class="last" align="right"></td>
    </tr>
    <asp:PlaceHolder runat="server" ID="phEcigBucks" Visible='<%# Fitdiet.Store1.Logic.ShoppingCart.Current.EcigBucksRedeem > 0M %>'>
        <tr class="total">
          <td colspan="4" align="right">
            E-Cigs Dollars Applied
            (<asp:LinkButton runat="server" ID="lbRemoveEcigBucks" OnClick="lbRemoveEcigBucks_Click">Remove</asp:LinkButton>)
          </td>
          <td class="last" align="right"><%# FormatPrice(0M - Fitdiet.Store1.Logic.ShoppingCart.Current.EcigBucksRedeem) %></td>
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
    </tr>-->
</table>
