<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/Front.Master" AutoEventWireup="true"
    CodeBehind="cart.aspx.cs" Inherits="Fitdiet.Store1.cart" EnableViewState="true" %>

<%@ Register TagPrefix="uc" TagName="ShoppingCart" Src="~/Controls/ShoppingCart.ascx" %>
<%@ Register TagPrefix="uc" TagName="ShippingDetails" Src="~/Controls/ShippingDetails.ascx" %>
<%@ Register TagPrefix="uc" TagName="BillingDetails" Src="~/Controls/BillingDetails.ascx" %>
<%@ Register TagPrefix="uc" TagName="PlaceMyOrder" Src="~/Controls/PlaceMyOrder.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    Online Store Cart</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphScript" runat="server">

    <script type="text/javascript" language="javascript" src="js/jquery.validationEngine.validationRules.js"></script>

    <script type="text/javascript" language="javascript" src="js/jquery.validationEngine.js"></script>

    <script type="text/javascript" language="javascript" src="js/mod10.js"></script>

    <script type="text/javascript" language="javascript" src="js/ajax.login.js"></script>

    <script type="text/javascript">
    $(document).ready(function() {
        $("#products").validationEngine({
            promptPosition: "topRight",
            inline: true
        });
    });
    $(document).ready(function() {
        $("#customer").validationEngine({
            promptPosition: "topRight",
            inline: true
        });
    });
    function validateProducts() {
        return $("#products").validationEngine({
                returnIsValid: true,
                promptPosition: "topRight",
                inline: true
            });
    }
        function deleteProducts() {

        return validateProducts();
    }
    function validateBilling() {
        return $("#customer").validationEngine({
            returnIsValid: true,
            promptPosition: "topRight",
            inline: true
        });
    }
    function completeOrder() {
        if (validateProducts() && validateBilling())
        {
            //$("#<%# lbCompleteOrder.ClientID %>").removeAttr('onclick').click(function() { return false; });
            return true;
        }
        return false;
    }
    </script>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <form runat="server" id="form1">
    <div id="cart">
        <asp:PlaceHolder runat="server" ID="phSecureBanner" Visible='false'>
            <div style="text-align: center; font-size: 20px; font-weight: bold; margin-bottom: 20px;">
                Secure Checkout&nbsp;&nbsp;<img src="images/godaddy.png" width="115" height="60"
                    align="absmiddle">&nbsp;&nbsp;128-Bit Encryption
            </div>
        </asp:PlaceHolder>
<%--        <asp:PlaceHolder runat="server" ID="phError">
            <div id="error" class="validation-error">
                <asp:Literal runat="server" ID="lError" />
            </div>
        </asp:PlaceHolder>--%>
        <uc:ShoppingCart runat="server" Visible='<%# CurrentState == CartState.Cart %>' ID="ShoppingCart1"
            OnProductsChanged="ShoppingCart1_ProductsChanged" OnCouponChanged="ShoppingCart1_CouponChanged"
            OnRefererCodeChanged="ShoppingCart1_RefererCodeChanged" OnGiftCertificateRemoved="BillingInfo1_GiftCertificateRemoved"
            OnEcigBucksRemoved="ShoppingCart1_EcigBucksRemoved" OnGiftCertificatePopulated="ShoppingCart1_GiftCertificatePopulated" OnStepChanged="ChangeStep_Click"/>
        <uc:ShippingDetails runat="server" ID="ShippingDetails" Visible='<%# CurrentState == CartState.ShippingDetails %>' OnStepChanged="ChangeStep_Click" />
        <uc:BillingDetails runat="server" ID="BillingDetails" Visible='<%# CurrentState == CartState.BillingDetails %>'
            OnGiftCertificateAdded="ShoppingCart1_GiftCertificateAdded" OnEcigBucksApplied="BillingInfo1_EcigBucksApplied" OnStepChanged="ChangeStep_Click" />
        <uc:PlaceMyOrder runat="server" ID="PlaceMyOrder" Visible='<%# CurrentState == CartState.PlaceMyOrder %>'
            Billing="<%#Billing %>" OnStepChanged="ChangeStep_Click" />
        <div id="checkout">
            <table width="100%" border="0" cellpadding="0" cellspacing="0" class="noborder" style="padding-top: 15px;">
                <tr>
                    <asp:PlaceHolder runat="server" ID="phButtonsCart" Visible='<%# CurrentState != CartState.PlaceMyOrder %>'>
                        <table width="300" border="0" align="center" cellpadding="3" cellspacing="3">
                            <tr>
                                <td>
                                    <a href="<%# KeepShoppingUrl %>">
                                        <img src="images/cont_shopping.gif" alt="" width="175" height="35" border="0" /></a>
                                </td>
                                <td>
                                    <asp:ImageButton runat="server" ID="imbCheckout" OnClick="lbCheckout_Click" OnClientClick="<%# ValidateMethod %>"
                                        TabIndex="17" />
                                </td>
                            </tr>
                        </table>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder runat="server" ID="phButtonsComplete" Visible='<%# CurrentState == CartState.PlaceMyOrder %>'>
                        <td style="width: 57%">
                            <span class="buttons back"><a href="cart.aspx">&laquo; Change Order</a></span>
                        </td>
                        <td>
                            <asp:LinkButton runat="server" ID="lbCompleteOrder" Text="" OnClick="lbCompleteOrder_Click"
                                OnClientClick="return completeOrder();" TabIndex="17"><img src='images/complete_out_btn.gif' alt="" Width="108" Height="35" border="0" /></asp:LinkButton>
                        </td>
                    </asp:PlaceHolder>
                </tr>
            </table>
        </div>
    </div>
    </form>
</asp:Content>
