<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/Front.Master" AutoEventWireup="true"
    CodeBehind="fitdiet-shop.aspx.cs" Inherits="Fitdiet.Store1.FitdietShop" %>

<%@ Register TagPrefix="uc" TagName="RefillCartridges" Src="~/Controls/RefillCartridges.ascx" %>
<%@ Import Namespace="Fitdiet.Store1.Logic" %>
<asp:Content ContentPlaceHolderID="cphScript" runat="server">

    <script type="text/javascript">
        function ClickHere() {
            if (shoppingCart.getProductCount(1) == 0) {
                shoppingCart.addProduct(1, 1);
            }
            document.location = "cart.aspx";
        }
    </script>

</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    Shop</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <table width="100%" border="0" cellspacing="0" cellpadding="0">
        <tr>
            <td align="center" valign="top" class="sky_bg">
                <table width="900" border="0" align="center" cellpadding="0" cellspacing="0">
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td valign="top" style="padding-top: 15px;">
                            <table width="860" border="0" align="center" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td width="489" valign="top">
                                        <table width="860" border="0" cellspacing="0" cellpadding="0">
                                            <tr>
                                                <td align="center" class="main_heading">
                                                    TODAY IS THE PERFECT DAY TO START YOUR WEIGHT LOSS SUCCESS!
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <table width="754" border="0" align="center" cellpadding="0" cellspacing="0">
                                                        <tr>
                                                            <td width="365">
                                                                <img src="images/100sure.jpg" alt="" width="365" height="301" />
                                                            </td>
                                                            <td width="389" align="center" valign="top" style="padding-top: 25px;">
                                                                <table width="335" border="0" cellspacing="2" cellpadding="2">
                                                                    <tr>
                                                                        <td align="left" class="footer_heading">
                                                                            30-DAY TRIAL KIT
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td align="left" class="copy18grey">
                                                                            HERE IS WHAT YOU WILL GET:
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td align="left" class="copy14grey">
                                                                            • FIT™ CALORIE CRYSTALS (30-DAY SUPPLY),&nbsp;&nbsp;ONE 1.9 OZ. SHAKER<br />
                                                                            <br />
                                                                            • SECRET INGREDIENT INFO, FAQ &amp; USAGE GUIDE
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td class="main_heading">
                                                                            <strong>TRY IT FOR 30-DAYS* </strong>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>
                                                                            <a href="javascript:ClickHere();">
                                                                                <img src="images/clickhere_btn.png" alt="" width="206" height="58" vspace="10" style="border: 0;" /></a>
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
                                <tr>
                                    <td valign="top" style="padding-top: 75px;">
                                        <table width="860" border="0" cellspacing="0" cellpadding="0">
                                            <tr>
                                                <td width="379" align="center" valign="top" id="product-el-1">
                                                    <table width="225" border="0" cellspacing="0" cellpadding="0">
                                                        <tr>
                                                            <td align="center">
                                                                <img src="images/30day.jpg" alt="" width="238" height="192" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="center">
                                                                <table width="115" border="0" cellspacing="2" cellpadding="2">
                                                                    <tr>
                                                                        <td class="main_text">
                                                                            30-DAY TRIAL KIT
                                                                            <input type="hidden" id="product-id-1" value="1" />
                                                                            <input type="hidden" id="product-count-1" value="1" product-limit="1"/>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>
                                                                            <img src="images/view_detail_btn.jpg" alt="" width="85" height="17" />
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td class="main_text">
                                                                            <strong>
                                                                                <%# FormatPrice(ShoppingCart.KnownProducts[KnownProduct.A30DayTrialKit].Price) %></strong>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>
                                                                            <a href="#" id="add-product-1" class="copy12orange"><strong>ADD TO CART</strong></a>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="center">
                                                                &nbsp;
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                                <td width="447" align="center" valign="top" style="border-left: 2px #e4e4e4 solid;
                                                    border-right: 2px #e4e4e4 solid" id="product-el-2">
                                                    <table width="225" border="0" cellspacing="0" cellpadding="0">
                                                        <tr>
                                                            <td align="center">
                                                                <img src="images/60day.jpg" alt="" width="278" height="192" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="center">
                                                                <table width="130" border="0" cellspacing="2" cellpadding="2">
                                                                    <tr>
                                                                        <td class="main_text">
                                                                            60-DAY STARTER KIT
                                                                            <input type="hidden" id="product-id-2" value="2" />
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>
                                                                            <img src="images/view_detail_btn.jpg" alt="" width="85" height="17" />
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td class="main_text">
                                                                            <strong id="product-price-1">
                                                                                <%# FormatPrice(ShoppingCart.KnownProducts[KnownProduct.B60DayStarterKit].Price) %></strong>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>
                                                                            <a href="#" id="add-product-2" class="copy12orange"><strong>ADD TO CART</strong></a>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="center">
                                                                &nbsp;
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                                <td width="34" align="center" valign="top">
                                                    <table width="225" border="0" cellspacing="0" cellpadding="0" id="product-el-3">
                                                        <tr>
                                                            <td align="center">
                                                                <img src="images/fir_herbal.jpg" alt="" width="164" height="192" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="center" valign="top">
                                                                <table width="160" border="0" cellspacing="2" cellpadding="2">
                                                                    <tr>
                                                                        <td class="main_text">
                                                                            FIT™ HERBAL ENERGY<br />
                                                                            FAT BURNER CAPSULES
                                                                            <input type="hidden" id="product-id-3" value="3" />
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>
                                                                            <img src="images/view_detail_btn.jpg" alt="" width="85" height="17" />
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td class="main_text">
                                                                            <strong id="Strong1">
                                                                                <%# FormatPrice(ShoppingCart.KnownProducts[KnownProduct.FitHerbalEnergyFatBurnerCapsules].Price) %></strong>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>
                                                                            <a href="#" id="add-product-3" class="copy12orange"><strong>ADD TO CART</strong></a>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="center">
                                                                &nbsp;
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
</asp:Content>
