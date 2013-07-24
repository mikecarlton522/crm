<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PlaceMyOrder.ascx.cs"
    Inherits="FitDiet.Store1.Controls.PlaceMyOrder" %>
<%@ Import Namespace="Fitdiet.Store1.Logic" %>
<%@ Register TagPrefix="uc" TagName="ProductDescription" Src="~/Controls/ProductDescription.ascx" %>
<%@ Register TagPrefix="uc" TagName="ProductImg" Src="~/Controls/ProductImg.ascx" %>
<%@ Register TagPrefix="uc" TagName="CartMenu" Src="~/Controls/CartMenu.ascx" %>
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
                    <td width="489" valign="top" class="main_heading">
                        PLACE MY ORDER
                    </td>
                </tr>
                <tr>
                    <td valign="top" style="padding-top: 15px;">
                        <table width="900" border="0" cellspacing="0" cellpadding="0">
                            <tr>
                                <td width="697" valign="top">
                                    <table width="697" border="0" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td colspan="2" valign="middle" class="numbering_bg">
                                                <uc:CartMenu runat="server" OnStepChanged="ChangeStep_Click" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <asp:PlaceHolder runat="server" ID="phError">
                                                    <div id="error" class="validation-error">
                                                        <asp:Literal runat="server" ID="lError" />
                                                    </div>
                                                </asp:PlaceHolder>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2" align="center" valign="top" style="padding-top: 12px;">
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
                                                    <asp:Repeater runat="server" ID="rProducts">
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
                                                                <td align="center" class="copy12grey1">
                                                                    <asp:Label runat="server" ID="lblProductCount" Text='<%# DataBinder.Eval(Container.DataItem, "Value") %>' />
                                                                    <asp:HiddenField runat="server" ID="hdnProductID" Value='<%# (int)ShoppingCart.GetProductNumber((ShoppingCartProduct)DataBinder.Eval(Container.DataItem, "Key")) %>' />
                                                                </td>
                                                                <td align="center" class="copy12grey1">
                                                                    <%# FormatPrice(((decimal)DataBinder.Eval(Container.DataItem, "Key.Price")) * ((int)DataBinder.Eval(Container.DataItem, "Value")))%>
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
                                                            free
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
                                            <td valign="top" style="padding-top: 22px;" align="left">
                                                <span class="main_heading">SHIPPING DETAILS</span>
                                                <table border="0" cellspacing="0" cellpadding="5" class="copy12grey1" style="padding-left:20px;">
                                                    <%-- <tr class="green_tr">
                                                            <td width="150" align="left" class="copy14green">
                                                                <strong>Parameter </strong>
                                                            </td>
                                                            <td width="99" align="center" class="copy14green">
                                                                <strong>Value </strong>
                                                            </td>
                                                        </tr>--%>
                                                    <tr>
                                                        <td class="copy12grey1">
                                                            First name:
                                                        </td>
                                                        <td class="last">
                                                            <label>
                                                                <asp:Label runat="server" ID="lblFirstName" />
                                                            </label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="copy12grey1">
                                                            Last name:
                                                        </td>
                                                        <td class="last">
                                                            <label>
                                                                <asp:Label runat="server" ID="lblLastName" />
                                                            </label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="copy12grey1">
                                                            Address line 1:
                                                        </td>
                                                        <td class="last">
                                                            <label>
                                                                <asp:Label runat="server" ID="lblAddress1" />
                                                            </label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="copy12grey1">
                                                            Address line 2:
                                                        </td>
                                                        <td class="last">
                                                            <label>
                                                                <asp:Label runat="server" ID="lblAddress2" />
                                                            </label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="copy12grey1">
                                                            City:
                                                        </td>
                                                        <td class="last">
                                                            <label>
                                                                <asp:Label runat="server" ID="lblCity" />
                                                            </label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="copy12grey1">
                                                            Country:
                                                        </td>
                                                        <td class="last">
                                                            <label>
                                                                <asp:Label runat="server" ID="lblCountry" />
                                                            </label>
                                                        </td>
                                                    </tr>
                                                    <asp:PlaceHolder runat="server" ID="phAddressUS">
                                                    <tr id="stateUS">
                                                        <td class="copy12grey1">
                                                            State:
                                                        </td>
                                                        <td class="last">
                                                            <label>
                                                                <asp:Label runat="server" ID="lblState" />
                                                            </label>
                                                        </td>
                                                    </tr>
                                                    <tr id="zipUS">
                                                        <td class="copy12grey1">
                                                            Zip code:
                                                        </td>
                                                        <td class="last">
                                                            <asp:Label runat="server" ID="lblZip" />
                                                        </td>
                                                    </tr>
                                                    <tr id="phoneUS">
                                                        <td class="copy12grey1">
                                                            Phone number:
                                                        </td>
                                                        <td class="last">
                                                            (
                                                            <asp:Label runat="server" ID="lblPhone1" />
                                                            )
                                                            <asp:Label runat="server" ID="lblPhone2" />
                                                            -
                                                            <asp:Label runat="server" ID="lblPhone3" />
                                                        </td>
                                                    </tr>
                                                    </asp:PlaceHolder>
                                                    <asp:PlaceHolder runat="server" ID="phAddressEx">
                                                    <tr id="stateEx">
                                                        <td class="copy12grey1">
                                                            State/Province:
                                                        </td>
                                                        <td class="last">
                                                            <label>
                                                                <asp:Label runat="server" ID="lblStateEx" />
                                                            </label>
                                                        </td>
                                                    </tr>
                                                    <tr id="zipEx">
                                                        <td class="copy12grey1">
                                                            Postal code:
                                                        </td>
                                                        <td class="last">
                                                            <asp:Label runat="server" ID="lblZipEx" />
                                                        </td>
                                                    </tr>
                                                    <tr id="phoneEx">
                                                        <td class="copy12grey1">
                                                            Phone number:
                                                        </td>
                                                        <td class="last">
                                                            <asp:Label runat="server" ID="lblPhoneEx" />
                                                        </td>
                                                    </tr>
                                                    </asp:PlaceHolder>
                                                    <tr class="item bottom">
                                                        <td class="copy12grey1">
                                                            Email eddress:
                                                        </td>
                                                        <td class="last">
                                                            <label>
                                                                <asp:Label runat="server" ID="lblEmail" />
                                                            </label>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                            <td valign="top" style="padding-top: 22px; width:50%;" align="left">
                                                <span class="main_heading">BILLING DETAILS</span>
                                                <table border="0" cellspacing="0" cellpadding="5" class="copy12grey1" style="padding-left:20px;">
                                                    <%--  <tr class="green_tr">
                                                            <td width="150" align="left" class="copy14green">
                                                                <strong>Parameter </strong>
                                                            </td>
                                                            <td width="99" align="center" class="copy14green">
                                                                <strong>Value </strong>
                                                            </td>
                                                        </tr>--%>
                                                    <tr class="item">
                                                        <td class="copy12grey1">
                                                            Payment type:
                                                        </td>
                                                        <td class="last">
                                                            <asp:Label runat="server" ID="lblPaymentType" />
                                                        </td>
                                                    </tr>
                                                    <tr class="item">
                                                        <td class="copy12grey1">
                                                            Card number:
                                                        </td>
                                                        <td class="last">
                                                            <asp:Label runat="server" ID="lblCreditCardNumber" />
                                                        </td>
                                                    </tr>
                                                    <tr class="item">
                                                        <td class="copy12grey1">
                                                            Expiry date:
                                                        </td>
                                                        <td class="last">
                                                            <asp:Label runat="server" ID="lblExpireDate" />
                                                        </td>
                                                    </tr>
                                                    <tr class="item bottom">
                                                        <td class="copy12grey1">
                                                            CVV:
                                                        </td>
                                                        <td class="last" class="copy12grey1">
                                                            <asp:Label runat="server" ID="lblCreditCardCVV" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2" class="green_tr">
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
<table>
