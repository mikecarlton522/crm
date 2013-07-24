<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/Front.Master" AutoEventWireup="true" Inherits="Ecigsbrand.Store1.cp_login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">E-Cigs: Electronic Cigarette Customer Login</asp:Content>
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
        $("#login").validationEngine({
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

  
    
</script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
<form runat="server" ID="form1">
<div id="content">
  <div class="left">
    <h1>Customer/Referral Program Login</h1>
    <h3>Existing Customer Login</h3>
    <p><i><b>Sign</b> in to view your order details, see referral commissions, change address…</i> <br/>
Your username is the email address you signed up with.<br/>
Your password is in the order confirmation email we sent you.<br/>
</p>
    <div class="blueborder" id="login">
      <table width="100%" border="0" cellspacing="0">
        <tr class="noborder">
          <td width="36%">Username (email):</td>
          <td width="64%">
              <asp:TextBox runat="server" ID="tbLoginUsername"
                  MaxLength="50" CssClass="validate[custom[Email]]">
              </asp:TextBox>
          </td>
        </tr>
        <tr>
          <td>Password:</td>
          <td>
              <asp:TextBox runat="server" ID="tbLoginPassword" TextMode="Password"
                  MaxLength="50" CssClass="validate[custom[Password]]">
              </asp:TextBox>
          </td>
        </tr>
        <tr>
          <td>&nbsp;</td>
          <td><asp:Button runat="server" ID="bLogin" Text="Login"
                OnClientClick="return validateLogin();" onclick="bLogin_Click"></asp:Button></td>
        </tr>
        <asp:PlaceHolder runat="server" ID="phLoginError">
        <tr>
          <td>&nbsp;</td>
          <td><span class="error"><asp:Literal runat="server" ID="lLoginError" /></span></td>
        </tr>
        </asp:PlaceHolder>
        <tr>
          <td>&nbsp;</td>
          <td><p style="padding:0; margin:0; font-size:11px;"><a href="cp-lost-password.aspx">Lost Password? Reset Now</a></p></td>
        </tr>
      </table>
    </div>
    <p>
If you have never bought anything from us, but wish to join the referral program, <br/>
<a href="cp-create-acc.aspx"> click here.</a>
</p>

        <asp:PlaceHolder runat="server" ID="phRegistrationError">
       
        </asp:PlaceHolder>
  

  </div>
</div>
<div style="clear:both;">
</div>
</form>
</asp:Content>
