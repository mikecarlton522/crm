<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/Front.Master" AutoEventWireup="true" CodeBehind="cp-account.aspx.cs" Inherits="Ecigsbrand.Store1.account.cp_account" %>
<%@ Register TagPrefix="gen" Assembly="TrimFuel.Business" Namespace="TrimFuel.Business.Controls" %>
<%@ Register src="../Controls/MenuControlPanel.ascx" tagname="MenuControlPanel" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">E-Cigs: My Account Information</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphScript" runat="server">
<link rel="stylesheet" type="text/css" href="../css/control-panel.css" />
<script type="text/javascript" language="javascript" src="../js/jquery.validationEngine.validationRules.js"></script>
<script type="text/javascript" language="javascript" src="../js/jquery.validationEngine.js"></script>
<style type="text/css">
    #content .left table td { border-top:1px dotted #2C4762; padding:5px; }
    #content .left table .noborder td { border:none; }
    .blueborder { width:500px; }
    .blueborder table tr td { background-color:#f5f8fe; }
</style>
<script type="text/javascript">
    $(document).ready(function() {
        $("#login").validationEngine({
            promptPosition: "topRight",
            scroll: false,
            inline: true
        });
    });
    $(document).ready(function() {
        $("#password").validationEngine({
            promptPosition: "topRight",
            scroll: false,
            inline: true
        });
    });
    $(document).ready(function() {
        $("#registration").validationEngine({
            promptPosition: "topRight",
            scroll: false,
            inline: true
        });
    });
    function validateLogin() {
        return $("#login").validationEngine({
            returnIsValid: true,
            promptPosition: "topRight",
            scroll: false,
            inline: true
        });
    }
    function validatePassword() {
        return $("#password").validationEngine({
            returnIsValid: true,
            promptPosition: "topRight",
            scroll: false,
            inline: true
        });
    }
    function validateRegistration() {
        return $("#registration").validationEngine({
            returnIsValid: true,
            promptPosition: "topRight",
            scroll: false,
            inline: true
        });
    }
    function ValidatePasswordConfirm() {
        return ($("#<%# tbPassword.ClientID %>").val() == $("#<%# tbPasswordConfirm.ClientID %>").val());
    }
    function ValidateUsernameConfirm() {
        return ($("#<%# tbUsername.ClientID %>").val() == $("#<%# tbUsernameConfirm.ClientID %>").val());
    }
    function onCountryChanged(country) {
        if (country == "US") {
            $("#stateEx").hide().find("[class*=validate]").addClass("donotvalidate");
            $("#zipEx").hide().find("[class*=validate]").addClass("donotvalidate");
            $("#stateUS").show().find("[class*=validate]").removeClass("donotvalidate");
            $("#zipUS").show().find("[class*=validate]").removeClass("donotvalidate");
        }
        else {
            $("#stateEx").show().find("[class*=validate]").removeClass("donotvalidate");
            $("#zipEx").show().find("[class*=validate]").removeClass("donotvalidate");
            $("#stateUS").hide().find("[class*=validate]").addClass("donotvalidate");
            $("#zipUS").hide().find("[class*=validate]").addClass("donotvalidate");
        }
    }
    $(document).ready(function() {
        $("#<%# ddlCountry.ClientID %>").change(function() {
            onCountryChanged($("#<%# ddlCountry.ClientID %>").val());
        });
        onCountryChanged($("#<%# ddlCountry.ClientID %>").val());
    });
</script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
<form runat="server" ID="form1">
<div id="content">
  <div class="left">
    <h1>Customer &amp; Referral Control Panel</h1>
    <uc1:MenuControlPanel ID="MenuControlPanel1" runat="server" />
    <h2>My Account Information</h2>
    <h3>Change My Email </h3>
    <div class="blueborder" id="login">
      <table width="100%" border="0" cellspacing="0">
        <tr class="noborder">
          <td width="32%">Current password:</td>
          <td width="68%">
              <asp:TextBox runat="server" ID="tbLoginPassword" TextMode="Password"
                  MaxLength="50" CssClass="validate[custom[Password]]">
              </asp:TextBox>
          </td>
        </tr>
        <tr>
          <td>New email (login):</td>
          <td>
              <asp:TextBox runat="server" ID="tbUsername"
                  MaxLength="50" CssClass="validate[custom[Email]]">
              </asp:TextBox>
          </td>
        </tr>
        <tr>
          <td>New email (login) again:</td>
          <td>
              <asp:TextBox runat="server" ID="tbUsernameConfirm"
                  MaxLength="50" CssClass="validate[funcCall[ValidateUsernameConfirm]]">
              </asp:TextBox>
          </td>
        </tr>
        <tr>
          <td>&nbsp;</td>
          <td><asp:Button runat="server" ID="bChangeLogin" Text="Submit"
                OnClientClick="return validateLogin();" onclick="bChangeLogin_Click"></asp:Button></td>
        </tr>
        <asp:PlaceHolder runat="server" ID="phLoginError">
        <tr>
          <td>&nbsp;</td>
          <td><span class='<asp:Literal runat="server" ID="lLoginErrorClass" Text="error" />'><asp:Literal runat="server" ID="lLoginError" /></span></td>
        </tr>
        </asp:PlaceHolder>
      </table>
    </div>
    <h3>Change My Password</h3>
    <div class="blueborder" id="password">
      <table width="100%" border="0" cellspacing="0">
        <tr class="noborder">
          <td width="32%">Current password:</td>
          <td width="68%">
              <asp:TextBox runat="server" ID="tbPasswordPassword" TextMode="Password"
                  MaxLength="50" CssClass="validate[custom[Password]]">
              </asp:TextBox>
          </td>
        </tr>
        <tr>
          <td>New password:</td>
          <td>
              <asp:TextBox runat="server" ID="tbPassword" TextMode="Password"
                  MaxLength="50" CssClass="validate[custom[Password]]">
              </asp:TextBox>
          </td>
        </tr>
        <tr>
          <td>New password again:</td>
          <td>
              <asp:TextBox runat="server" ID="tbPasswordConfirm" TextMode="Password"
                  MaxLength="50" CssClass='validate[funcCall[ValidatePasswordConfirm]]'>
              </asp:TextBox>
          </td>
        </tr>
        <tr>
          <td>&nbsp;</td>
          <td><asp:Button runat="server" ID="bChangePassword" Text="Submit"
                OnClientClick="return validatePassword();" onclick="bChangePassword_Click"></asp:Button></td>
        </tr>
        <asp:PlaceHolder runat="server" ID="phPasswordError">
        <tr>
          <td>&nbsp;</td>
          <td><span class='<asp:Literal runat="server" ID="lPasswordErrorClass" Text="error" />'><asp:Literal runat="server" ID="lPasswordError" /></span></td>
        </tr>
        </asp:PlaceHolder>
      </table>
    </div>
    <h3>Edit My Shipping Address </h3>
    <div class="blueborder" id="registration">
      <table width="100%" border="0" cellspacing="0">
        <tr class="noborder">
          <td width="32%">Password:</td>
          <td width="68%">
              <asp:TextBox runat="server" ID="tbRegistrationPassword" TextMode="Password"
                  MaxLength="50" CssClass='validate[custom[Password]]'>
              </asp:TextBox>
          </td>
        </tr>
        <tr>
          <td>First name:</td>
          <td>
              <asp:TextBox runat="server" ID="tbFirstName" Text='<%# Referer.FirstName %>'
                  MaxLength="50" CssClass="validate[custom[FirstName]]">
              </asp:TextBox>
          </td>
        </tr>
        <tr>
          <td>Last name:</td>
          <td>
              <asp:TextBox runat="server" ID="tbLastName" Text='<%# Referer.LastName %>'
                  MaxLength="50" CssClass="validate[custom[LastName]]">
              </asp:TextBox>
          </td>
        </tr>
        <tr>
          <td>Address line 1:</td>
          <td>
              <asp:TextBox runat="server" ID="tbAddress1" Text='<%# Referer.Address1 %>'
                  MaxLength="50" CssClass="validate[custom[Address]]">
              </asp:TextBox>
          </td>
        </tr>
        <tr>
          <td>Address line 2:</td>
          <td>
              <asp:TextBox runat="server" ID="tbAddress2" Text='<%# Referer.Address2 %>'
                  MaxLength="50">
              </asp:TextBox>
          </td>
        </tr>
        <tr>
          <td>City:</td>
          <td>
              <asp:TextBox runat="server" ID="tbCity" Text='<%# Referer.City %>'
                  MaxLength="50" CssClass="validate[custom[City]]">
              </asp:TextBox>
          </td>
        </tr>
        <tr>
          <td>Country:</td>
          <td>
            <gen:DDLCountry runat="server" ID="ddlCountry" SelectedValue='<%# string.IsNullOrEmpty(Referer.Country) ? "US" : Referer.Country %>'
              style="width:170px;">
            </gen:DDLCountry>
          </td>
        </tr>
        <tr id="stateUS">
          <td>State:</td>
          <td>
              <gen:DDLStateFullName runat="server" ID="ddlState" SelectedValue='<%# (Referer.Country == TrimFuel.Business.RegistrationService.DEFAULT_COUNTRY) ? Referer.State : ddlState.Items[0].Value %>'
                  style="width:170px;">
              </gen:DDLStateFullName>
          </td>
        </tr>
        <tr id="stateEx">
          <td>State/Province:</td>
          <td>
              <asp:TextBox runat="server" ID="tbStateEx" Text='<%# Referer.State %>'
                  MaxLength="50">
              </asp:TextBox>
          </td>
        </tr>
        <tr id="zipUS">
          <td>Zip code:</td>
          <td>
            <asp:TextBox runat="server" ID="tbZip" Text='<%# Referer.Zip %>'
                MaxLength="5" CssClass="validate[custom[Zip]]">
            </asp:TextBox>
          </td>
        </tr>
        <tr id="zipEx">
          <td>Postal code:</td>
          <td>
            <asp:TextBox runat="server" ID="tbZipEx" Text='<%# Referer.Zip %>'
                MaxLength="50">
            </asp:TextBox>
          </td>
        </tr>
        <tr>
          <td>&nbsp;</td>
          <td><asp:Button runat="server" ID="bRegister" Text="Submit"
                OnClientClick="return validateRegistration();" onclick="bRegister_Click" /></td>
        </tr>
        <asp:PlaceHolder runat="server" ID="phRegistrationError">
        <tr>
          <td>&nbsp;</td>
          <td><span class='<asp:Literal runat="server" ID="lRegistrationErrorClass" Text="error" />'><asp:Literal runat="server" ID="lRegistrationError" /></span></td>
        </tr>
        </asp:PlaceHolder>
      </table>
    </div>
    <asp:PlaceHolder runat="server" ID="inv" Visible="false">
    <h3>Credit Cards on File</h3>
    <div class="blueborder">
      <table width="100%" border="0" cellspacing="0">
        <tr class="noborder">
          <td width="32%">Card</td>
          <td width="68%">Visa...3487</td>
        </tr>
      </table>
    </div>
    <h3>Add a Backup Credit Card</h3>
    <div class="blueborder">
      <table width="100%" border="0" cellspacing="0">
        <tr class="noborder">
          <td>We accept:</td>
          <td><img src="http://www.securetrialoffers.com/ec15/images/visa-mc.png" /></td>
        </tr>
        <tr>
          <td width="32%">Payment type:</td>
          <td width="68%"><select name="state2" id="state2" title="Please Select Country" tabindex="6">
              <option value="Visa" selected="selected">Visa</option>
              <option value="MasterCard">MasterCard</option>
            </select></td>
        </tr>
        <tr>
          <td>Card number:</td>
          <td><input name="input8" type="text" /></td>
        </tr>
        <tr>
          <td>Expiry date:</td>
          <td><select name="Month" id="Month" title="Please Choose Credit Card Expiration Month" style="width:75px;" tabindex="3">
              <option value="1">Jan (1)</option>
              <option value="2">Feb (2)</option>
              <option value="3">Mar (3)</option>
              <option value="4">Apr (4)</option>
              <option value="5">May (5)</option>
              <option value="6">Jun (6)</option>
              <option value="7">Jul (7)</option>
              <option value="8">Aug (8)</option>
              <option value="9">Sep (9)</option>
              <option value="10">Oct (10)</option>
              <option value="11">Nov (11)</option>
              <option value="12">Dec (12)</option>
            </select>
            <select name="Year" id="Year" title="Please Choose Credit Card Expiration Year" style="width:60px;" tabindex="4">
              <option value="2010">2010</option>
              <option value="2011">2011</option>
              <option value="2012">2012</option>
              <option value="2013">2013</option>
              <option value="2014">2014</option>
              <option value="2015">2015</option>
              <option value="2016">2016</option>
              <option value="2017">2017</option>
              <option value="2018">2018</option>
              <option value="2019">2019</option>
              <option value="2020">2020</option>
              <option value="2021">2021</option>
              <option value="2022">2022</option>
              <option value="2023">2023</option>
              <option value="2024">2024</option>
              <option value="2025">2025</option>
            </select></td>
        </tr>
        <tr>
          <td>CVV:</td>
          <td><input name="input9" type="text" style="width:40px;" />
            <a href="javascript: void(0)" tabindex="5" style="font-size:12px;" onclick="window.open('http://www.securetrialoffers.com/ec15/images/cvv.png','buttonwin','height=350,width=520,scrollbars=yes,toolbar=no,location=no,screenX=100,screenY=20,top=20,left=100','fullscreen=no'); return false;">(what's this?)</a></td>
        </tr>
        <tr>
          <td>&nbsp;</td>
          <td><input type="submit" name="button4" id="button4" value="Submit" /></td>
        </tr>
        <!--
        <tr>
          <td>&nbsp;</td>
          <td><span class="error">#Test# Error, passwords do not match</span></td>
        </tr>
        -->
      </table>
    </div>
    </asp:PlaceHolder>
  </div>
</div>
<div style="clear:both;">
</div>
</form>
</asp:Content>
