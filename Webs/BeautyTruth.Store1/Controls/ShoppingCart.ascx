<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ShoppingCart.ascx.cs"
    Inherits="BeautyTruth.Store1.Controls.ShoppingCart_" %>
<%@ Import Namespace="BeautyTruth.Store1.Logic" %>
<%@ Register TagPrefix="uc" TagName="AlsoBought" Src="~/Controls/AlsoBought.ascx" %>
<%@ Register TagPrefix="uc" TagName="ProductDescription" Src="~/Controls/ProductDescription.ascx" %>
<asp:PlaceHolder runat="server" ID="phScripts">

    <script type="text/javascript">
        function validateCoupon() {
            return !($.validationEngine.loadValidation("#<%# tbCoupon.ClientID %>"));
        }
    </script>

</asp:PlaceHolder>
<div class="container_12">
    <div class="grid_12 cart">
        <h1>
            Shopping Cart</h1>
        <table width="100%">
            <tr>
                <th width="7%" class="center">
                    Remove
                </th>
                <th width="60%">
                    Product
                </th>
                <th width="10%">
                    Price
                </th>
                <th width="10%">
                    Quantity
                </th>
                <th width="13%">
                    Amount
                </th>
            </tr>
            <asp:Repeater runat="server" ID="rProducts">
                <ItemTemplate>
                    <tr>
                        <td class="center">
                            <asp:CheckBox runat="server" ID="cbRemoved"/>
                        </td>
                        <td>
                            <a href='product.aspx?productCode=<%# DataBinder.Eval(Container.DataItem, "Key.ProductSKU") %>'
                                class="copy12grey">
                                <uc:ProductDescription runat="server" ID="ProductDescription" Product='<%# DataBinder.Eval(Container.DataItem, "Key") %>' />
                            </a>
                        </td>
                        <td>
                            <%# FormatPrice((decimal)DataBinder.Eval(Container.DataItem, "Key.Price")) %>
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="tbProductCount" MaxLength="2" Width="30" Text='<%# DataBinder.Eval(Container.DataItem, "Value") %>' />
                            <asp:HiddenField runat="server" ID="hdnProductID" Value='<%# (Convert.ToInt32(DataBinder.Eval(Container.DataItem, "Key.ProductType")) == 1 ? "subscription_" : "product_") + DataBinder.Eval(Container.DataItem, "Key.ProductID") %>' />
                        </td>
                        <td>
                            <%# FormatPrice(((decimal)DataBinder.Eval(Container.DataItem, "Key.Price")) * ((int)DataBinder.Eval(Container.DataItem, "Value")))%>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
            <tr>
                <td colspan="3">
                    Coupon Code or Gift Certificate Code&nbsp;&nbsp;
                    <asp:TextBox runat="server" ID="tbCoupon" MaxLength="15" Text="" Style="width: 125px;" />
                    <div class="button inline">
                        <asp:LinkButton runat="server" ID="bApplyCoupon" Text="Apply" OnClick="bApplyCoupon_Click"
                            OnClientClick="return validateCoupon();" ValidationGroup="couponGroup" />
                    </div>
                    <div style="width: 200px; margin-left: 185px;">
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ValidationGroup="couponGroup"
                            ControlToValidate="tbCoupon" Display="Dynamic" ErrorMessage="<div class='error'>Please enter valid coupon code</div>"
                            EnableClientScript="true" runat="server" />
                    </div>
                    <asp:PlaceHolder runat="server" ID="phCouponError">
                        <div class="error" style="width: 400px;">
                            We're sorry, this coupon code is not valid, please verify code and try again</div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder runat="server" ID="phCouponError2">
                        <div class="error" style="width: 400px;">
                            This appears to be a gift card. Would you like to use it?</div>
                    </asp:PlaceHolder>
                </td>
                <td>
                    <span style="float: right">Shipping</span>
                </td>
                <td>
                    <%# FormatPrice(ShippingValue)%>
                </td>
            </tr>
            <asp:PlaceHolder runat="server" ID="phCouponApplied" Visible='<%# Coupon != null %>'>
                <tr class="color2">
                    <td colspan="4" class="right">
                        Coupon Code Redeemed
                        <%# (Coupon != null) ? "(" + Coupon.CouponCode + ")" : string.Empty %>
                    </td>
                    <td>
                        <%# (Coupon != null && Coupon.Discount != null) ? string.Format("({0}%)", Coupon.Discount.Value * 100) : string.Empty %>
                        <asp:LinkButton runat="server" OnClick="bRemoveCoupon_Click" Text="Remove" />
                    </td>
                </tr>
            </asp:PlaceHolder>
            <asp:PlaceHolder runat="server" ID="phGiftApplied" Visible='<%# Gift != null %>'>
                <tr class="color2">
                    <td colspan="4" class="right">
                        Gift Certificate Code Redeemed
                    </td>
                    <td>
                        <%# (Gift != null && Gift.Value != null) ? string.Format("({0})", FormatPrice(Gift.Value.Value)) : string.Empty%>
                        <asp:LinkButton runat="server" OnClick="bRemoveGift_Click" Text="Remove" />
                    </td>
                </tr>
                <tr class="color2">
                    <td colspan="4" class="right">
                        Gift Certificate Value Remaining After Checkout
                    </td>
                    <td>
                        <%# (Gift != null && Gift.RemainingValue != null) ? string.Format("({0})", FormatPrice(Gift.RemainingValue.Value)) : string.Empty%>
                    </td>
                </tr>
            </asp:PlaceHolder>
            <tr>
                <td colspan="4" class="right">
                    Upgrade Shipping
                    <asp:DropDownList ID="ddlShipping" runat="server" OnSelectedIndexChanged="ddlShipping_Changed" AutoPostBack="true">
                        <asp:ListItem Text="Standard 3-5 Days ($0.00)" Value="0" />
                        <asp:ListItem Text="Expedited 2-3 Days (+$7.00)" Value="7" />
                    </asp:DropDownList>
                </td>
                <td>
                    <label>
                        <%# FormatPrice(ShippingValue)%></label>
                </td>
            </tr>
            <tr>
                <td colspan="4" class="right">
                    Total
                </td>
                <td>
                    <%# FormatPrice(TotalCost) %>
                </td>
            </tr>
        </table>
    </div>
</div>
<div id="ShoppingStartButtons">
</div>
<div class="container_12">
    <uc:AlsoBought ID="AlsoBought" GridClass="grid_12" CountToShow="4" ProductCode="<%# ProductCode %>"
        runat="server" />
</div>
