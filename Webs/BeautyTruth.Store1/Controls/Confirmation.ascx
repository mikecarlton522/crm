<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Confirmation.ascx.cs"
    Inherits="BeautyTruth.Store1.Controls.Confirmation" %>
<%@ Register TagPrefix="uc" TagName="ProductDescription" Src="~/Controls/ProductDescription.ascx" %>
<%@ Import Namespace="BeautyTruth.Store1.Logic" %>
<div class="container_12">
    <div class="grid_12">
        <div class="cart">
            <table width="100%">
                <tr>
                    <th width="70%">
                        Product
                    </th>
                    <th width="10%">
                        Price
                    </th>
                    <th width="10%">
                        Quantity
                    </th>
                    <th width="10%">
                        Amount
                    </th>
                </tr>
                <asp:Repeater runat="server" ID="rProducts">
                    <ItemTemplate>
                        <tr>
                            <td>
                                <uc:ProductDescription runat="server" ID="ProductDescription" Product='<%# DataBinder.Eval(Container.DataItem, "Key") %>' />
                            </td>
                            <td>
                                <%# FormatPrice((decimal)DataBinder.Eval(Container.DataItem, "Key.Price")) %>
                            </td>
                            <td>
                                <%# DataBinder.Eval(Container.DataItem, "Value") %>
                            </td>
                            <td>
                                <%# FormatPrice(((decimal)DataBinder.Eval(Container.DataItem, "Key.Price")) * ((int)DataBinder.Eval(Container.DataItem, "Value")))%>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
                <tr>
                    <td>
                    </td>
                    <td colspan="2" class="right">
                        Shipping
                    </td>
                    <td>
                        <%# FormatPrice(ShippingValue) %>
                    </td>
                </tr>
                <asp:PlaceHolder runat="server" ID="phCouponApplied" Visible='<%# Coupon != null %>'>
                    <tr class="color2">
                        <td colspan="3" class="right">
                            Coupon Code Redeemed
                            <%# (Coupon != null) ? "(" + Coupon.CouponCode + ")" : string.Empty %>
                        </td>
                        <td>
                            <%# (Coupon != null && Coupon.Discount != null) ? string.Format("({0}%)", Coupon.Discount.Value * 100) : string.Empty %>
                        </td>
                    </tr>
                </asp:PlaceHolder>
                <asp:PlaceHolder runat="server" ID="phGiftApplied" Visible='<%# Gift != null %>'>
                    <tr class="color2">
                        <td colspan="3" class="right">
                            Gift Certificate Code Redeemed
                        </td>
                        <td>
                            <%# (Gift != null && Gift.Value != null) ? string.Format("({0})", FormatPrice(Gift.Value.Value)) : string.Empty%>
                        </td>
                    </tr>
                    <tr class="color2">
                        <td colspan="3" class="right">
                            Gift Certificate Value Remaining After Checkout
                        </td>
                        <td>
                            <%# (Gift != null && Gift.RemainingValue != null) ? string.Format("({0})", FormatPrice(Gift.RemainingValue.Value)) : string.Empty%>
                        </td>
                    </tr>
                </asp:PlaceHolder>
                <tr>
                    <td>
                    </td>
                    <td colspan="2" class="right">
                        Total
                    </td>
                    <td>
                        <%# FormatPrice(TotalCost) %>
                    </td>
                </tr>
            </table>
        </div>
    </div>
</div>
<div class="container_12">
    <div class="grid_12">
        <h1>
            Thank You For Your Order!</h1>
        <div class="cart color1">
            <table width="100%">
                <tr>
                    <th colspan="2">
                        Order Details
                    </th>
                </tr>
                <tr>
                    <td>
                        Order Confirmation Number
                    </td>
                    <td>
                        <asp:Label ID="lblConfirmNumber" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td width="22%">
                        Credit Card Successfully Billed
                    </td>
                    <td width="78%">
                        <asp:Label ID="lblCreditCard" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td>
                        Receipt Sent To
                    </td>
                    <td>
                        <asp:Label ID="lblEmail1" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td>
                        Beauty &amp; Truth Login
                    </td>
                    <td>
                        <asp:Label ID="lblEmail2" runat="server" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
</div>
