<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ShoppingCartView.ascx.cs"
    Inherits="Fitdiet.Store1.Controls.ShoppingCartView" %>
<%@ Import Namespace="Fitdiet.Store1.Logic" %>
<%@ Register TagPrefix="uc" TagName="ProductDescription" Src="~/Controls/ProductDescription.ascx" %>
<%@ Register TagPrefix="uc" TagName="ProductImg" Src="~/Controls/ProductImg.ascx" %>
<table width="900" border="0" align="center" cellpadding="0" cellspacing="0">
    <tr>
        <td valign="top">
            <table width="650" border="0" cellspacing="0" cellpadding="5">
                <tr class="green_tr">
                    <td width="238" align="left" class="copy14green">
                        <strong>Product(s) </strong>
                    </td>
                    <td width="82" align="center" class="copy14green">
                        <strong>Quantity</strong>
                    </td>
                    <td width="95" align="center" class="copy14green">
                        <strong>Total</strong>
                    </td>
                </tr>
                <asp:Repeater runat="server" ID="rProducts">
                    <ItemTemplate>
                        <tr class="item">
                            <td align="left" class="copy14grey">
                                <asp:PlaceHolder runat="server" Visible="false">
                                <div class="cart picture">
                                    <uc:ProductImg runat="server" ID="ProductImg1" Product='<%# DataBinder.Eval(Container.DataItem, "Key") %>' />
                                </div>
                                </asp:PlaceHolder>
                                <uc:ProductDescription runat="server" ID="ProductDescription1" Product='<%# DataBinder.Eval(Container.DataItem, "Key") %>' />
                            </td>
                            <td align="center" class="copy14grey">
                                <%# DataBinder.Eval(Container.DataItem, "Value") %>
                            </td>
                            <td class="last copy14grey" align="center">
                                <%# FormatPrice((decimal)DataBinder.Eval(Container.DataItem, "Key.Price")) %>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
                <tr class="total">
                    <td colspan="1">
                        <asp:PlaceHolder runat="server" ID="phCouponApplied" Visible='<%# Coupon != null %>'>
                            <span class="copy14grey">Coupon Code:
                                <%# (Coupon != null) ? Coupon.CouponCode : string.Empty %>
                                <%# (Coupon != null && Coupon.Discount != null) ? string.Format(", {0:p2} discount applied", Coupon.Discount.Value) : string.Empty %>
                            </span></asp:PlaceHolder>
                    </td>
                    <td align="center" class="copy14green">
                        <b>S&amp;H</b>
                    </td>
                    <td class="last" align="center">
                        free
                    </td>
                </tr>
                <asp:PlaceHolder runat="server" ID="phEcigBucks" Visible='<%# EcigBucksAmountApplied > 0M %>'>
                    <tr class="total">
                        <td colspan="4" align="right">
                            Dollars Applied
                        </td>
                        <td class="last" align="right">
                            <%# FormatPrice(0M - EcigBucksAmountApplied)%>
                        </td>
                    </tr>
                </asp:PlaceHolder>
                <asp:Repeater runat="server" ID="rGiftCertificateList">
                    <ItemTemplate>
                        <tr class="total">
                            <td colspan="4" align="right">
                                Gift Certificate
                                <%# Eval("GiftNumber")%>
                                Applied
                            </td>
                            <td class="last" align="right">
                                <%# FormatPrice(0M - (decimal)Eval("Value"))%>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
                <tr class="total bottom bold copy14grey">
                    <td>
                    </td>
                    <td align="center" class="copy14grey">
                        <b>TOTAL</b>
                    </td>
                    <td class="last copy14grey" align="center">
                        <b><%# FormatPrice(TotalCost) %></b>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
