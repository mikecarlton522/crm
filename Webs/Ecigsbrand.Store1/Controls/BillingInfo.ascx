<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BillingInfo.ascx.cs" Inherits="Ecigsbrand.Store1.Controls.BillingInfo" %>
<%@ Register TagPrefix="gen" Assembly="TrimFuel.Business" Namespace="TrimFuel.Business.Controls" %>
<asp:PlaceHolder runat="server" ID="phScript">
<script type="text/javascript">
    function ValidatePhone() {
        return (/^\d{3}$/.test($("#<%# tbPhone1.ClientID %>").val()) &&
            /^\d{3}$/.test($("#<%# tbPhone2.ClientID %>").val()) &&
            /^\d{4}$/.test($("#<%# tbPhone3.ClientID %>").val()));
    }
    function ValidateCreditCard() {
        return Mod10($("#<%# tbCreditCardNumber.ClientID %>").val());
    }
    function onCountryChanged(country) {
        if (country == "US") {
            $("#customer #billing #stateEx").hide().find("[class*=validate]").addClass("donotvalidate");
            $("#customer #billing #zipEx").hide().find("[class*=validate]").addClass("donotvalidate");
            $("#customer #billing #phoneEx").hide().find("[class*=validate]").addClass("donotvalidate");
            $("#customer #billing #stateUS").show().find("[class*=validate]").removeClass("donotvalidate");
            $("#customer #billing #zipUS").show().find("[class*=validate]").removeClass("donotvalidate");
            $("#customer #billing #phoneUS").show().find("[class*=validate]").removeClass("donotvalidate");
        }
        else {
            $("#customer #billing #stateEx").show().find("[class*=validate]").removeClass("donotvalidate");
            $("#customer #billing #zipEx").show().find("[class*=validate]").removeClass("donotvalidate");
            $("#customer #billing #phoneEx").show().find("[class*=validate]").removeClass("donotvalidate");
            $("#customer #billing #stateUS").hide().find("[class*=validate]").addClass("donotvalidate");
            $("#customer #billing #zipUS").hide().find("[class*=validate]").addClass("donotvalidate");
            $("#customer #billing #phoneUS").hide().find("[class*=validate]").addClass("donotvalidate");
        }
    }
    function onLoginSuccess(billing) {
        if (billing != null) {
            $("#customer #billing #<%# tbFirstName.ClientID %>").val(billing.FirstName);
            $("#customer #billing #<%# tbLastName.ClientID %>").val(billing.LastName);
            $("#customer #billing #<%# tbAddress1.ClientID %>").val(billing.Address1);
            $("#customer #billing #<%# tbAddress2.ClientID %>").val(billing.Address2);
            $("#customer #billing #<%# tbCity.ClientID %>").val(billing.City);
            if (billing.Country == null || billing.Country == "" || billing.Country == "US") {
                $("#customer #billing #<%# ddlState.ClientID %>").val(billing.State);
                $("#customer #billing #<%# tbZip.ClientID %>").val(billing.Zip);
                if (billing.PhoneCnt != null) {
                    $("#customer #billing #<%# tbPhone1.ClientID %>").val(billing.PhoneCnt.Code);
                    $("#customer #billing #<%# tbPhone2.ClientID %>").val(billing.PhoneCnt.Part1);
                    $("#customer #billing #<%# tbPhone3.ClientID %>").val(billing.PhoneCnt.Part2);
                }
                $("#customer #billing #<%# ddlCountry.ClientID %>").val("US");
                onCountryChanged("US");
            }
            else {
                $("#customer #billing #<%# tbStateEx.ClientID %>").val(billing.State);
                $("#customer #billing #<%# tbZipEx.ClientID %>").val(billing.Zip);
                $("#customer #billing #<%# tbPhoneEx.ClientID %>").val(billing.Phone);
                $("#customer #billing #<%# ddlCountry.ClientID %>").val(billing.Country);
                onCountryChanged(billing.Country);
            }
            $("#customer #billing #<%# tbEmail.ClientID %>").val(billing.Email);
        }
        $("#customer #ajax-login-section").remove();
    }
    function onLoginError(errorMsg) {
        showLoginMessage(errorMsg);
    }
    function showLoginMessage(msg) {
        $("#customer #ajax-login-section #ajax-login-error-msg-container").prev().removeClass("bottom");
        $("#customer #ajax-login-section #ajax-login-error-msg").html(msg);
        $("#customer #ajax-login-section #ajax-login-error-msg-container").show();
    }
    function onLoginBtnClick() {
        $("#customer #ajax-login-section #ajax-login-error-msg").show();
        ajaxLogin($("#customer #ajax-username").val(), $("#customer #ajax-password").val(), onLoginSuccess, onLoginError);
    }
    $(document).ready(function() {
        $("#customer #billing #<%# ddlCountry.ClientID %>").change(function() {
            onCountryChanged($("#customer #billing #<%# ddlCountry.ClientID %>").val());
        });
        onCountryChanged($("#customer #billing #<%# ddlCountry.ClientID %>").val());
    });
</script>
</asp:PlaceHolder>
  <div id="customer">
    <div id="credit">
      <asp:PlaceHolder runat="server" ID="phCCInformation" Visible="<%# IsGiftCertificateValidate == false && TotalCost != 0M %>">
        <table width="100%" border="0" cellspacing="0" cellpadding="5">
        <tr class="header">
          <td colspan="2">
            Credit Card Information
          </td>
        </tr>
        <tr class="item">
          <td width="40%">
            <b>We accept:</b>
          </td>
          <td width="60%" class="last">
            <img src="images/cart-visa-mc.png" width="108" height="30" />
          </td>
        </tr>
        <tr class="item">
          <td>
            <b>Payment type:</b>
          </td>
          <td class="last">
            <gen:DDLPaymentType runat="server" ID="ddlPaymentType" 
                TabIndex="1" ToolTip="Please Choose Credit Card Type">
            </gen:DDLPaymentType>
          </td>
        </tr>
        <tr class="item">
          <td>
            <b>Card number:</b>
          </td>
          <td class="last">
            <asp:TextBox runat="server" ID="tbCreditCardNumber"
                TabIndex="2" ToolTip="Please Enter Valid Credit Card Number"
                MaxLength="16" CssClass="validate[funcCall[ValidateCreditCard]]">
            </asp:TextBox>
          </td>
        </tr>
        <tr class="item">
          <td>
            <b>Expiry date:</b>
          </td>
          <td class="last">
            <gen:DDLMonth runat="server" ID="ddlExpireMonth"
                TabIndex="3" ToolTip="Please Choose Credit Card Expiration Month" style="width:75px;">
            </gen:DDLMonth>
            <gen:DDLYear runat="server" ID="ddlExpireYear"
                TabIndex="4" ToolTip="Please Choose Credit Card Expiration Year" style="width:60px;">
            </gen:DDLYear>
          </td>
        </tr>
        <tr class="item bottom">
          <td>
            <b>CVV:</b>
          </td>
          <td class="last">
            <asp:TextBox runat="server" ID="tbCreditCardCVV"
                TabIndex="5" ToolTip="Please Enter Valid CVV" style="width:50px;"
                MaxLength="5" CssClass="validate[custom[CVV]]">
            </asp:TextBox>
            &nbsp; <a href="javascript: void(0)" tabindex="5" onclick="window.open('images/cart-cvv.png','buttonwin','height=350,width=520,scrollbars=yes,toolbar=no,location=no,screenX=100,screenY=20,top=20,left=100','fullscreen=no'); 
        return false;">(what's this?)</a>
          </td>
        </tr>
      </table>
        <div class="space"></div>
      </asp:PlaceHolder>
      <asp:PlaceHolder runat="server" ID="phEcigBucks" Visible="<%# Referer != null && Ecigsbrand.Store1.Logic.ShoppingCart.Current.EcigBucksRedeem == 0M && EcigBucksAvailableToApply > 0M%>">
          <table width="100%" border="0" cellspacing="0" cellpadding="5">
            <tr class="header">
              <td colspan="2">
                E-Cigs Dollars Available for Immediate Use?
              </td>
            </th>
            <tr class="item">
              <td colspan="2" class="last">
                You E-Cigs Dollars available to apply to your order. Click Apply below to use your credit towards this purchase.
              </td>
            </tr>
            <tr class="item">
              <td width="40%">
                <b>Available:</b>
              </td>
              <td width="60%" class="last" style="color:Green;">
                <%# EcigBucksAvailable.ToString("c") %>
              </td>
            </tr>
            <tr class="item">
              <td width="40%">
                <b>Amount to Apply:</b>
              </td>
              <td width="60%" class="last">
                $
                <asp:TextBox runat="server" ID="tbEcigBucksAmount"
                    MaxLength="50" Text='<%# EcigBucksAvailableToApply.ToString("0.00") %>'>
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
          <div class="space"></div>
      </asp:PlaceHolder>      
      <asp:PlaceHolder runat="server" ID="phGiftCertificate" Visible="<%# Ecigsbrand.Store1.Logic.ShoppingCart.Current.GiftCertificateList == null || Ecigsbrand.Store1.Logic.ShoppingCart.Current.GiftCertificateList.Count == 0%>">
          <table width="100%" border="0" cellspacing="0" cellpadding="5" class="sliding-table">
            <tr class="header">
              <td colspan="2">
                Have a Gift Certificate?
              </td>
            </th>
            <tr class="item">
              <td width="40%">
                <b>Certificate Number:</b>
              </td>
              <td width="60%" class="last">
                <asp:TextBox runat="server" ID="tbGiftCertificateNumber"
                    MaxLength="50">
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
                    <span class="localError">We're sorry, this certificate number is not valid, please verify number and try again</span>
                  </td>
                </tr>
            </asp:PlaceHolder>
          </table>
          <div class="space"></div>
      </asp:PlaceHolder>
      <asp:PlaceHolder runat="server" ID="phAjaxLogin" Visible="<%# Referer == null %>">
          <table width="100%" border="0" cellspacing="0" cellpadding="5" id="ajax-login-section">
            <tr class="header">
              <td colspan="2">
                Already a member?  Log In Now
              </td>
            </tr>
            <tr class="item">
              <td width="40%">
                <b>Email (Login):</b>
              </td>
              <td width="60%" class="last">
                <input type="text" maxlength="50" id="ajax-username" name="username" />
              </td>
            </tr>
            <tr class="item">
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
            <tr class="item bottom" id="ajax-login-error-msg-container" style="display:none;">
              <td colspan="2" align="center" class="last">
                <span class="localError" id="ajax-login-error-msg"></span>
              </td>
            </tr>
          </table>
          <div class="space"></div>
      </asp:PlaceHolder>
    </div>
    <div id="billing">
      <table width="100%" border="0" cellspacing="0" cellpadding="5">
        <tr class="header">
          <td colspan="2">
            Billing Address
          </td>
        </tr>
        <tr>
          <td width="40%">
            <b>First name:</b>
          </td>
          <td width="60%" class="last">
            <label>
              <asp:TextBox runat="server" ID="tbFirstName"
                  TabIndex="6"
                  MaxLength="50" CssClass="validate[custom[FirstName]]">
              </asp:TextBox>
            </label>
          </td>
        </tr>
        <tr>
          <td>
            <b>Last name:</b>
          </td>
          <td class="last">
            <label>
              <asp:TextBox runat="server" ID="tbLastName"
                  TabIndex="7"
                  MaxLength="50" CssClass="validate[custom[LastName]]">
              </asp:TextBox>
            </label>
          </td>
        </tr>
        <tr>
          <td>
            <b>Address line 1:</b>
          </td>
          <td class="last">
            <label>
              <asp:TextBox runat="server" ID="tbAddress1"
                  TabIndex="8"
                  MaxLength="50" CssClass="validate[custom[Address]]">
              </asp:TextBox>
            </label>
          </td>
        </tr>
        <tr>
          <td>
            <b>Address line 2:</b>
          </td>
          <td class="last">
            <label>
              <asp:TextBox runat="server" ID="tbAddress2"
                  TabIndex="9"
                  MaxLength="50">
              </asp:TextBox>
            </label>
          </td>
        </tr>
        <tr>
          <td>
            <b>City:</b>
          </td>
          <td class="last">
            <label>
              <asp:TextBox runat="server" ID="tbCity"
                  TabIndex="10"
                  MaxLength="50" CssClass="validate[custom[City]]">
              </asp:TextBox>
            </label>
          </td>
        </tr>
        <tr>
          <td>
            <b>Country:</b>
          </td>
          <td class="last">
            <label>
              <gen:DDLCountry runat="server" ID="ddlCountry"
                  TabIndex="11" style="width:170px;">
              </gen:DDLCountry>
            </label>
          </td>
        </tr>
        <tr id="stateUS">
          <td>
            <b>State:</b>
          </td>
          <td class="last">
            <label>
              <gen:DDLStateFullName runat="server" ID="ddlState"
                  TabIndex="12" style="width:170px;">
              </gen:DDLStateFullName>
            </label>
          </td>
        </tr>
        <tr id="stateEx">
          <td>
            <b>State/Province:</b>
          </td>
          <td class="last">
            <label>
              <asp:TextBox runat="server" ID="tbStateEx"
                  TabIndex="12"
                  MaxLength="50">
              </asp:TextBox>
            </label>
          </td>
        </tr>
        <tr id="zipUS">
          <td>
            <b>Zip code:</b>
          </td>
          <td class="last">
            <asp:TextBox runat="server" ID="tbZip"
                TabIndex="13"
                MaxLength="5" CssClass="validate[custom[Zip]] x-short">
            </asp:TextBox>
          </td>
        </tr>
        <tr id="zipEx">
          <td>
            <b>Postal code:</b>
          </td>
          <td class="last">
            <asp:TextBox runat="server" ID="tbZipEx"
                TabIndex="13"
                MaxLength="50">
            </asp:TextBox>
          </td>
        </tr>
        <tr id="phoneUS">
          <td>
            <b>Phone number:</b>
          </td>
          <td class="last">
            (
            <asp:TextBox runat="server" ID="tbPhone1"
                TabIndex="14" style="width: 30px;"
                MaxLength="3">
            </asp:TextBox>
            )
            <asp:TextBox runat="server" ID="tbPhone2"
                TabIndex="15" style="width: 30px;"
                MaxLength="3">
            </asp:TextBox>
            -
            <asp:TextBox runat="server" ID="tbPhone3"
                TabIndex="16" style="width: 40px;"
                MaxLength="4" CssClass="validate[funcCall[ValidatePhone]]">
            </asp:TextBox>
          </td>
        </tr>
        <tr id="phoneEx">
          <td>
            <b>Phone number:</b>
          </td>
          <td class="last">
            <asp:TextBox runat="server" ID="tbPhoneEx"
                TabIndex="14"
                MaxLength="50">
            </asp:TextBox>
          </td>
        </tr>
        <tr class="item bottom">
          <td>
            <b>Email eddress:</b>
          </td>
          <td class="last">
            <label>
              <asp:TextBox runat="server" ID="tbEmail"
                  TabIndex="17"
                  MaxLength="50" CssClass="validate[custom[Email]]">
              </asp:TextBox>
            </label>
          </td>
        </tr>
      </table>
    </div>
    <div style="clear:both;">
    </div>
  </div>
