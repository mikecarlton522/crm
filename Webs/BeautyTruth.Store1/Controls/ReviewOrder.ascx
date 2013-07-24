<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReviewOrder.ascx.cs"
    Inherits="BeautyTruth.Store1.Controls.ReviewOrder" %>
<%@ Register TagPrefix="uc" TagName="ProductDescription" Src="~/Controls/ProductDescription.ascx" %>
<%@ Import Namespace="BeautyTruth.Store1.Logic" %>
<div class="container_12">
    <div class="grid_12 cart">
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
<div class="container_12">
    <div class="grid_6 cart color1">
        <h1>
            Account Information</h1>
        <table width="100%">
            <tr>
                <th colspan="2">
                    Billing Address
                </th>
            </tr>
            <tr>
                <td width="35%">
                    First Name
                </td>
                <td width="65%">
                    <asp:Label ID="lblFirstName" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    Last Name
                </td>
                <td>
                    <asp:Label ID="lblLastName" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    Address Line One
                </td>
                <td>
                    <asp:Label ID="lblAddress1" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    Address Line Two
                </td>
                <td>
                    <asp:Label ID="lblAddress2" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    Country
                </td>
                <td>
                    <asp:Label ID="lblCountry" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    State/Province
                </td>
                <td>
                    <asp:Label ID="lblState" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    City/Town
                </td>
                <td>
                    <asp:Label ID="lblCity" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    Zip Code
                </td>
                <td>
                    <asp:Label ID="lblZip" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    Phone Number
                </td>
                <td colspan="2">
                    <asp:Label ID="lblPhone1" runat="server" /><asp:PlaceHolder ID="phPhoneEx" runat="server">
                        -<asp:Label ID="lblPhone2" runat="server" />-<asp:Label ID="lblPhone3" runat="server" /></asp:PlaceHolder>
                </td>
            </tr>
            <tr>
                <td>
                    Email Address
                </td>
                <td>
                    <asp:Label ID="lblEmail" runat="server" />
                </td>
            </tr>
        </table>
    </div>
    <div class="grid_6 cart color1">
        <h1>
            &nbsp;</h1>
        <table width="100%">
            <tr>
                <th colspan="2">
                    Shipping Address
                </th>
            </tr>
            <tr>
                <td width="35%">
                    First Name
                </td>
                <td width="65%">
                    <asp:Label ID="lblShippingFirstName" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    Last Name
                </td>
                <td>
                    <asp:Label ID="lblShippingLastName" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    Address Line One
                </td>
                <td>
                    <asp:Label ID="lblShippingAddress1" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    Address Line Two
                </td>
                <td>
                    <asp:Label ID="lblShippingAddress2" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    Country
                </td>
                <td>
                    <asp:Label ID="lblShippingCountry" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    State/Province
                </td>
                <td>
                    <asp:Label ID="lblShippingState" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    City/Town
                </td>
                <td>
                    <asp:Label ID="lblShippingCity" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    Zip Code
                </td>
                <td>
                    <asp:Label ID="lblShippingZip" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    Phone Number
                </td>
                <td colspan="2">
                    <asp:Label ID="lblShippingPhone1" runat="server" /><asp:PlaceHolder ID="phShippingPhoneEx"
                        runat="server">-<asp:Label ID="lblShippingPhone2" runat="server" />-<asp:Label ID="lblShippingPhone3"
                            runat="server" /></asp:PlaceHolder>
                </td>
            </tr>
            <tr>
                <td>
                    Email Address
                </td>
                <td>
                    <asp:Label ID="lblShippingEmail" runat="server" />
                </td>
            </tr>
        </table>
    </div>
</div>
<asp:PlaceHolder runat="server" ID="phCreditCardInformation" Visible="<%# IsGiftCertificateValidate == false %>">
    <div class="container_12">
        <div class="grid_6 cart color1">
            <h1>
                Payment Information</h1>
            <table width="100%">
                <tr>
                    <th colspan="2">
                        Payment Method
                    </th>
                </tr>
                <tr>
                    <td width="35%">
                        Card Number
                    </td>
                    <td width="65%">
                        <asp:Label ID="lblCreditCard" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td>
                        Expiry Date
                    </td>
                    <td>
                        <asp:Label ID="lblExpMonth" runat="server" />
                        /
                        <asp:Label ID="lblExpYear" runat="server" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
</asp:PlaceHolder>
