<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BillingDetails.ascx.cs"
    Inherits="FitDiet.Store1.Controls.BillingDetails" %>
<%@ Register TagPrefix="gen" Assembly="TrimFuel.Business" Namespace="TrimFuel.Business.Controls" %>
<%@ Register TagPrefix="uc" TagName="CartMenu" Src="~/Controls/CartMenu.ascx" %>
<asp:PlaceHolder runat="server" ID="phScript">

    <script type="text/javascript">
        function ValidateCreditCard() {
            return Mod10($("#<%# tbCreditCardNumber.ClientID %>").val());
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
                    <td width="489" valign="top" class="main_heading">
                        BILLING DETAILS
                    </td>
                </tr>
                <tr>
                    <td valign="top" style="padding-top: 15px;">
                        <table width="900" border="0" cellspacing="0" cellpadding="0">
                            <tr>
                                <td width="697" valign="top">
                                    <table width="697" border="0" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td valign="middle" class="numbering_bg">
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
                                                <div id="customer">
                                                    <div id="credit">
                                                        <table width="100%" border="0" cellspacing="0" cellpadding="5">
                                                            <tr class="item">
                                                                <td width="40%" class="copy12grey1">
                                                                    <b>We accept:</b>
                                                                </td>
                                                                <td width="60%" class="last">
                                                                    <img src="images/cart-visa-mc.png" width="108" height="30" />
                                                                </td>
                                                            </tr>
                                                            <tr class="item">
                                                                <td class="copy12grey1">
                                                                    <b>Payment type:</b>
                                                                </td>
                                                                <td class="last">
                                                                    <gen:DDLPaymentType runat="server" ID="ddlPaymentType" TabIndex="1" ToolTip="Please Choose Credit Card Type">
                                                                    </gen:DDLPaymentType>
                                                                </td>
                                                            </tr>
                                                            <tr class="item">
                                                                <td class="copy12grey1">
                                                                    <b>Card number:</b>
                                                                </td>
                                                                <td class="last">
                                                                    <asp:TextBox runat="server" ID="tbCreditCardNumber" TabIndex="2" ToolTip="Please Enter Valid Credit Card Number"
                                                                        MaxLength="16" CssClass="validate[funcCall[ValidateCreditCard]]">
            </asp:TextBox>
                                                                </td>
                                                            </tr>
                                                            <tr class="item">
                                                                <td class="copy12grey1">
                                                                    <b>Expiry date:</b>
                                                                </td>
                                                                <td class="last">
                                                                    <gen:DDLMonth runat="server" ID="ddlExpireMonth" TabIndex="3" ToolTip="Please Choose Credit Card Expiration Month"
                                                                        Style="width: 75px;">
                                                                    </gen:DDLMonth>
                                                                    <gen:DDLYear runat="server" ID="ddlExpireYear" TabIndex="4" ToolTip="Please Choose Credit Card Expiration Year"
                                                                        Style="width: 60px;">
                                                                    </gen:DDLYear>
                                                                </td>
                                                            </tr>
                                                            <tr class="item bottom">
                                                                <td class="copy12grey1">
                                                                    <b>CVV:</b>
                                                                </td>
                                                                <td class="last" class="copy12grey1">
                                                                    <asp:TextBox runat="server" ID="tbCreditCardCVV" TabIndex="5" ToolTip="Please Enter Valid CVV"
                                                                        Style="width: 50px;" MaxLength="5" CssClass="validate[custom[CVV]]">
            </asp:TextBox>
                                                                    &nbsp; <a href="javascript: void(0)" style="color:#666;" tabindex="5" onclick="window.open('images/cart-cvv.png','buttonwin','height=350,width=520,scrollbars=yes,toolbar=no,location=no,screenX=100,screenY=20,top=20,left=100','fullscreen=no'); 
        return false;">
                                                                        (what's this?)</a>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                        <asp:PlaceHolder runat="server" ID="phEcigBucks" Visible="<%# Referer != null && Fitdiet.Store1.Logic.ShoppingCart.Current.EcigBucksRedeem == 0M && EcigBucksAvailableToApply > 0M%>">
                                                            <div class="space">
                                                            </div>
                                                            <table width="100%" border="0" cellspacing="0" cellpadding="5">
                                                                <tr class="header">
                                                                    <td colspan="2">
                                                                        E-Cigs Dollars Available for Immediate Use?
                                                                    </td>
                                                                    </th>
                                                                    <tr class="item">
                                                                        <td colspan="2" class="last">
                                                                            You E-Cigs Dollars available to apply to your order. Click Apply below to use your
                                                                            credit towards this purchase.
                                                                        </td>
                                                                    </tr>
                                                                    <tr class="item">
                                                                        <td width="40%" class="copy12grey1">
                                                                            <b>Available:</b>
                                                                        </td>
                                                                        <td width="60%" class="last" style="color: Green;">
                                                                            <%# EcigBucksAvailable.ToString("c") %>
                                                                        </td>
                                                                    </tr>
                                                                    <tr class="item" class="copy12grey1">
                                                                        <td width="40%">
                                                                            <b>Amount to Apply:</b>
                                                                        </td>
                                                                        <td width="60%" class="last">
                                                                            $
                                                                            <asp:TextBox runat="server" ID="tbEcigBucksAmount" MaxLength="50" Text='<%# EcigBucksAvailableToApply.ToString("0.00") %>'>
                </asp:TextBox>
                                                                        </td>
                                                                    </tr>
                                                                    <tr class="item <%= (!phEcigBucksError.Visible ? "bottom" : "") %>">
                                                                        <td>
                                                                            &nbsp;
                                                                        </td>
                                                                        <td class="last">
                                                                            <asp:Button runat="server" ID="btnApplyEcigBucks" Text="Apply" OnClick="btnApplyEcigBucks_Click" />
                                                                        </td>
                                                                    </tr>
                                                                    <asp:PlaceHolder runat="server" ID="phEcigBucksError">
                                                                        <tr class="item bottom">
                                                                            <td colspan="2" align="center" class="last">
                                                                                <span class="localError">Please, specify valid E-Cigs Dollars amount to apply.</span>
                                                                            </td>
                                                                        </tr>
                                                                    </asp:PlaceHolder>
                                                            </table>
                                                        </asp:PlaceHolder>
                                                        <asp:PlaceHolder runat="server" ID="phGiftCertificate" Visible="false">
                                                            <div class="space">
                                                            </div>
                                                            <table width="100%" border="0" cellspacing="0" cellpadding="5" class="sliding-table">
                                                                <tr class="header">
                                                                    <td colspan="2" class="copy12grey1">
                                                                        Have a Gift Certificate?
                                                                    </td>
                                                                    </th>
                                                                    <tr class="item">
                                                                        <td width="40%" class="copy12grey1">
                                                                            <b>Certificate Number:</b>
                                                                        </td>
                                                                        <td width="60%" class="last">
                                                                            <asp:TextBox runat="server" ID="tbGiftCertificateNumber" MaxLength="50">
                </asp:TextBox>
                                                                        </td>
                                                                    </tr>
                                                                    <tr class="item <%= (!phGiftCertificateError.Visible ? "bottom" : "") %>">
                                                                        <td>
                                                                            &nbsp;
                                                                        </td>
                                                                        <td class="last">
                                                                            <asp:Button runat="server" ID="btnApplyGiftCertificate" Text="Apply" OnClick="btnApplyGiftCertificate_Click" />
                                                                        </td>
                                                                    </tr>
                                                                    <asp:PlaceHolder runat="server" ID="phGiftCertificateError">
                                                                        <tr class="item bottom">
                                                                            <td colspan="2" align="center" class="last">
                                                                                <span class="localError">We're sorry, this certificate number is not valid, please verify
                                                                                    number and try again</span>
                                                                            </td>
                                                                        </tr>
                                                                    </asp:PlaceHolder>
                                                            </table>
                                                        </asp:PlaceHolder>
                                                        <asp:PlaceHolder runat="server" ID="phAjaxLogin" Visible="false">
                                                            <div class="space">
                                                            </div>
                                                            <table width="100%" border="0" cellspacing="0" cellpadding="5" id="ajax-login-section">
                                                                <tr class="header">
                                                                    <td colspan="2" class="copy12grey1">
                                                                        Already a member? Log In Now
                                                                    </td>
                                                                </tr>
                                                                <tr class="item">
                                                                    <td width="40%" class="copy12grey1">
                                                                        <b>Email (Login):</b>
                                                                    </td>
                                                                    <td width="60%" class="last">
                                                                        <input type="text" maxlength="50" id="ajax-username" name="username" />
                                                                    </td>
                                                                </tr>
                                                                <tr class="item copy12grey1">
                                                                    <td>
                                                                        <b>Password:</b>
                                                                    </td>
                                                                    <td class="last">
                                                                        <input type="password" maxlength="50" id="ajax-password" name="password" />
                                                                    </td>
                                                                </tr>
                                                                <tr class="item bottom">
                                                                    <td>
                                                                        &nbsp;
                                                                    </td>
                                                                    <td class="last">
                                                                        <input type="button" name="ajax-btn-login" value="Login" onclick="onLoginBtnClick();" />
                                                                    </td>
                                                                </tr>
                                                                <tr class="item bottom" id="ajax-login-error-msg-container" style="display: none;">
                                                                    <td colspan="2" align="center" class="last">
                                                                        <span class="localError" id="ajax-login-error-msg"></span>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </asp:PlaceHolder>
                                                    </div>
                                                    <div style="clear: both;">
                                                    </div>
                                                </div>
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
                </tr>
            </table>
        </td>
    </tr>
</table>
