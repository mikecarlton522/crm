<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/Front.Master" AutoEventWireup="true" Inherits="Ecigsbrand.Store1.cp_login" %>
<%@ Register TagPrefix="gen" Assembly="TrimFuel.Business" Namespace="TrimFuel.Business.Controls" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">E-Cigs: Electronic Cigarette Customer Register</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphScript" runat="server">
<link rel="stylesheet" type="text/css" href="css/control-panel.css" />
<script type="text/javascript" language="javascript" src="js/jquery.validationEngine.validationRules.js"></script>
<script type="text/javascript" language="javascript" src="js/jquery.validationEngine.js"></script>
<style type="text/css">
    #content .left table td { border-top:1px dotted #2C4762; padding:5px; }
    #content .left table .noborder td { border:none; }
    .blueborder { width:500px; }
    .blueborder table tr td { background-color:#f5f8fe; }
</style>
<script type="text/javascript">
   
    $(document).ready(function() {
        $("#registration").validationEngine({
            promptPosition: "topRight",
            scroll: false,
            inline: true
        });
    });
   
    function validateRegistration() {
        return $("#registration").validationEngine({
            returnIsValid: true,
            promptPosition: "topRight",
            scroll: false,
            inline: true
        });
    }
    function generateRefererCode() {
        var ABC, temp, randomID, a;
        ABC = "0123456789";
        temp = ""
        for (a = 0; a < 4; a++) {
            randomID = Math.random();
            randomID = Math.floor(randomID * ABC.length);
            temp += ABC.charAt(randomID);
        }
        var lastName = $("#<%# tbLastName.ClientID %>").val();
        lastName = lastName.replace(/\W/g, "");
        $("#<%# tbRefererCode.ClientID %>").val(lastName + temp);
    }
    function ValidatePasswordConfirm() {
        return ($("#<%# tbPassword.ClientID %>").val() == $("#<%# tbPasswordConfirm.ClientID %>").val());
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

        <asp:PlaceHolder runat="server" ID="phLoginError">
        
        </asp:PlaceHolder>
        
<h1> Customer/Referral Program Register </h1>
    <h3>Create Referral Account</h3>
    <p>Create a referral account if you want to collect E-Cigs bucks, but are not an E-Cigs <br/>
 customer. If you have ever bought anything from us, you already have a username <br/>
(your email address) and a password. Existing customers <a href="cp-login-test.aspx">login here. </a><br/>
<a href="cp-lost-password.aspx">Lost Password? Reset Now</a>
    <div class="blueborder" id="registration">
      <table width="100%" border="0" cellspacing="0">
        <tr class="noborder">
          <td width="36%">Email (this will be your username):</td>
          <td width="64%">
              <asp:TextBox runat="server" ID="tbUsername"
                  MaxLength="50" CssClass="validate[custom[Email]]">
              </asp:TextBox>
          </td>
        </tr>
        <tr>
          <td>Password:</td>
          <td>
              <asp:TextBox runat="server" ID="tbPassword" TextMode="Password"
                  MaxLength="50" CssClass="validate[custom[Password]]">
              </asp:TextBox>
          </td>
        </tr>
        <tr>
          <td>Password again:</td>
          <td>
              <asp:TextBox runat="server" ID="tbPasswordConfirm" TextMode="Password"
                  MaxLength="50" CssClass='validate[funcCall[ValidatePasswordConfirm]]'>
              </asp:TextBox>
          </td>
        </tr>
        <tr>
          <td>First name:</td>
          <td>
              <asp:TextBox runat="server" ID="tbFirstName"
                  MaxLength="50" CssClass="validate[custom[FirstName]]">
              </asp:TextBox>
          </td>
        </tr>
        <tr>
          <td>Last name:</td>
          <td>
              <asp:TextBox runat="server" ID="tbLastName"
                  MaxLength="50" CssClass="validate[custom[LastName]]">
              </asp:TextBox>
          </td>
        </tr>
        <tr>
          <td>Address line 1:</td>
          <td>
              <asp:TextBox runat="server" ID="tbAddress1"
                  MaxLength="50" CssClass="validate[custom[Address]]">
              </asp:TextBox>
          </td>
        </tr>
        <tr>
          <td>Address line 2:</td>
          <td>
              <asp:TextBox runat="server" ID="tbAddress2"
                  MaxLength="50">
              </asp:TextBox>
          </td>
        </tr>
        <tr>
          <td>City:</td>
          <td>
              <asp:TextBox runat="server" ID="tbCity"
                  MaxLength="50" CssClass="validate[custom[City]]">
              </asp:TextBox>
          </td>
        </tr>
        <tr>
          <td>Country:</td>
          <td>
            <gen:DDLCountry runat="server" ID="ddlCountry" SelectedValue='<%# TrimFuel.Business.RegistrationService.DEFAULT_COUNTRY %>'
              style="width:170px;">
            </gen:DDLCountry>
          </td>
        </tr>
        <tr id="stateUS">
          <td>State:</td>
          <td>
              <gen:DDLStateFullName runat="server" ID="ddlState"
                  style="width:170px;">
              </gen:DDLStateFullName>
          </td>
        </tr>
        <tr id="stateEx">
          <td>State/Province:</td>
          <td>
              <asp:TextBox runat="server" ID="tbStateEx"
                  MaxLength="50">
              </asp:TextBox>
          </td>
        </tr>
        <tr id="zipUS">
          <td>Zip code:</td>
          <td>
            <asp:TextBox runat="server" ID="tbZip"
                MaxLength="5" CssClass="validate[custom[Zip]]">
            </asp:TextBox>
          </td>
        </tr>
        <tr id="zipEx">
          <td>Postal code:</td>
          <td>
            <asp:TextBox runat="server" ID="tbZipEx"
                MaxLength="50">
            </asp:TextBox>
          </td>
        </tr>
        <tr>
          <td>Referer code:</td>
          <td>
            <asp:TextBox runat="server" ID="tbRefererCode"
                MaxLength="10" CssClass="validate[custom[RefererCode]]">
            </asp:TextBox>
            <input type="button" id="bGenerateRefererCode" value="Generate" onclick="generateRefererCode();" />
          </td>
        </tr>
        <tr>
          <td>Who referred you?</td>
          <td>
            <asp:TextBox runat="server" ID="tbParentRefererCode"
                MaxLength="10">
            </asp:TextBox>
            (5-10 Digits Long)
          </td>
        </tr>
        <tr>
 <td>&nbsp;</td>
          <td> <asp:Button runat="server" ID="bRegister" Text="Create Account"
                OnClientClick="return validateRegistration();" onclick="bRegister_Click" /></td>
        </tr>
        <asp:PlaceHolder runat="server" ID="phRegistrationError">
        <tr>
          <td>&nbsp;</td>
          <td><span class="error"><asp:Literal runat="server" ID="lRegistrationError" /></span></td>
        </tr>
        </asp:PlaceHolder>
      </table>
    </div>
  </div>
</div>
<div style="clear:both;">
</div>
</form>
</asp:Content>
