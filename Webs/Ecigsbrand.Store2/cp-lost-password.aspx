<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/Front.Master" AutoEventWireup="true" CodeBehind="cp-lost-password.aspx.cs" Inherits="Ecigsbrand.Store2.cp_lost_password" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">E-Cigs: Password Recovery</asp:Content>
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
        $("#content").validationEngine({
            promptPosition: "topRight",
            scroll: false,
            inline: true
        });
    });
    function validateData() {
        return $("#content").validationEngine({
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
    <h1>Password Recovery</h1>
    <asp:MultiView runat="server" ID="mvSteps">
    <asp:View runat="server">
        <h3>Step 1: Enter your email address.</h3>
        <p>Your password will be reset and emailed to you. If you continue to have issues logging in, please contact support@yourorderhelp.com</p>
        <div class="blueborder">
          <table width="100%" border="0" cellspacing="0">
            <tr class="noborder">
              <td width="36%">Email address:</td>
              <td width="64%">
                  <asp:TextBox runat="server" ID="tbUsername"
                      MaxLength="50" CssClass="validate[custom[Email]]">
                  </asp:TextBox>
              </td>
            </tr>
            <tr>
              <td>&nbsp;</td>
              <td><asp:Button runat="server" ID="btnNext1" Text="Next &gt;" 
                OnClientClick="return validateData();" onclick="btnNext1_Click" /></td>
            </tr>
            <asp:PlaceHolder runat="server" ID="phUsernameError">
            <tr>
              <td>&nbsp;</td>
              <td><span class="error"><asp:Literal runat="server" ID="lUsernameError" /></span></td>
            </tr>
            </asp:PlaceHolder>
          </table>
        </div>
    </asp:View>
    <asp:View runat="server">
        <h3>Step 2: Fill in the following information.</h3>
        <p>Your password will be reset and emailed to you.</p>
        <div class="blueborder">
          <table width="100%" border="0" cellspacing="0">
            <tr class="noborder">
              <td width="36%">What is your first name?</td>
              <td width="64%">
                  <asp:TextBox runat="server" ID="tbFirstName"
                      MaxLength="50" CssClass="validate[custom[FirstName]]">
                  </asp:TextBox>
              </td>
            </tr>
            <tr>
              <td>What is your last name?</td>
              <td>
                  <asp:TextBox runat="server" ID="tbLastName"
                      MaxLength="50" CssClass="validate[custom[LastName]]">
                  </asp:TextBox>
              </td>
            </tr>
            <tr>
              <td>What city do you live in?</td>
              <td>
                  <asp:TextBox runat="server" ID="tbCity"
                      MaxLength="50" CssClass="validate[custom[City]]">
                  </asp:TextBox>
              </td>
            </tr>
            <tr>
              <td>What is your zip code?</td>
              <td>
                <asp:TextBox runat="server" ID="tbZip"
                    MaxLength="5" CssClass="validate[custom[Zip]]">
                </asp:TextBox>
              </td>
            </tr>
            <tr>
              <td>&nbsp;</td>
              <td><asp:Button runat="server" ID="btnNext2" Text="Reset Password" 
                OnClientClick="return validateData();" onclick="btnNext2_Click" /></td>
            </tr>
            <asp:PlaceHolder runat="server" ID="phInfoError">
            <tr>
              <td>&nbsp;</td>
              <td><span class="error"><asp:Literal runat="server" ID="lInfoError" /></span></td>
            </tr>
            </asp:PlaceHolder>
          </table>
        </div>
    </asp:View>
    <asp:View runat="server">
        <h3>Step 3: Check your email.</h3>
        <p>Your password has been reset and emailed to you. Remember that your login is your email address. </p>
    </asp:View>
    </asp:MultiView>
    </div>
</div>
<div style="clear:both;">
</div>
</form>
</asp:Content>
