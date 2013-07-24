<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BillingInfo.ascx.cs" Inherits="Kaboom.Store1.Controls.BillingInfo" %>
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
    function onLoginSuccess(billing) {
        if (billing != null) {
            $("#customer #billing #<%# tbFirstName.ClientID %>").val(billing.FirstName);
            $("#customer #billing #<%# tbLastName.ClientID %>").val(billing.LastName);
            $("#customer #billing #<%# tbAddress1.ClientID %>").val(billing.Address1);
            $("#customer #billing #<%# tbAddress2.ClientID %>").val(billing.Address2);
            $("#customer #billing #<%# tbCity.ClientID %>").val(billing.City);
            $("#customer #billing #<%# ddlState.ClientID %>").val(billing.State);
            $("#customer #billing #<%# tbZip.ClientID %>").val(billing.Zip);
            $("#customer #billing #<%# tbEmail.ClientID %>").val(billing.Email);
            if (billing.PhoneCnt != null)
            {
                $("#customer #billing #<%# tbPhone1.ClientID %>").val(billing.PhoneCnt.Code);
                $("#customer #billing #<%# tbPhone2.ClientID %>").val(billing.PhoneCnt.Part1);
                $("#customer #billing #<%# tbPhone3.ClientID %>").val(billing.PhoneCnt.Part2);
            }
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
</script>
</asp:PlaceHolder>
  <div id="customer">
    <div id="credit">
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
            <img src="images/visa-mc.png" width="108" height="30" />
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
      <asp:PlaceHolder runat="server" ID="phAjaxLogin" Visible="<%# Referer == null %>">
          <div class="space"></div>
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
            <b>State:</b>
          </td>
          <td class="last">
            <label>
              <gen:DDLStateFullName runat="server" ID="ddlState"
                  TabIndex="11" style="width:170px;">
              </gen:DDLStateFullName>
            </label>
          </td>
        </tr>
        <tr>
          <td>
            <b>Zip code:</b>
          </td>
          <td class="last">
            <asp:TextBox runat="server" ID="tbZip"
                TabIndex="12"
                MaxLength="5" CssClass="validate[custom[Zip]] x-short">
            </asp:TextBox>
          </td>
        </tr>
        <tr>
          <td>
            <b>Phone number:</b>
          </td>
          <td class="last">
            (
            <asp:TextBox runat="server" ID="tbPhone1"
                TabIndex="13" style="width: 30px;"
                MaxLength="3">
            </asp:TextBox>
            )
            <asp:TextBox runat="server" ID="tbPhone2"
                TabIndex="14" style="width: 30px;"
                MaxLength="3">
            </asp:TextBox>
            -
            <asp:TextBox runat="server" ID="tbPhone3"
                TabIndex="15" style="width: 40px;"
                MaxLength="4" CssClass="validate[funcCall[ValidatePhone]]">
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
                  TabIndex="16"
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
