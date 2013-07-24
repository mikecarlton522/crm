<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Ecigsbrand.ec07.Default" %>
<%@ Import Namespace="TrimFuel.Business.Utils" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
<title>E-Cigs Electronic Cigarettes</title>
<style type="text/css">
body { margin:0; padding:0; font-family:arial, sans-serif; background-color:#17192E; background-image:url(<%= HtmlHelper.IMAGE_URL %>ecigsbrand.com/ec07/bg.jpg); background-repeat:no-repeat; background-position:top center; }
a:link, a:visited { color:white; }
input { width:160px; background:url(<%= HtmlHelper.IMAGE_URL %>ecigsbrand.com/ec02/topfade.gif) repeat-x top; vertical-align:middle; border:1px solid #999999; padding:4px; margin:0px; }
select { width:160px; }
#wrapper { position:relative; margin:auto; width:930px; }
#form { position:absolute; top:380px; left:605px; font-size:12px; width:300px; color:white; }
#form .description { float:left; width:100px; padding:12px 10px 0 0; font-weight:bold; color:white; text-align:right; }
#form .field { float:left; width:170px; padding:6px 0; }
#form .button { float:left; width:100%; text-align:center; margin:15px 0 0 0; }
#footer { margin:auto; width:950px; text-align:center; font-size:12px; padding:0 0 40px 0; color:white; }
</style>
<link type="text/css" rel="stylesheet" href="css/popup.css">
<script type="text/javascript" src="js/jquery.js"></script>
<script type="text/javascript" src="js/IHS.validate.js"></script>
<% if (exitPop != "0") { %>
<script type="text/javascript" src="js/exitpop.js"></script>
<% } %>
<script type="text/javascript" src="js/landing.js"></script>
</head>
<body>
<a name="top"></a>
<div id="wrapper">
  <div style="position:absolute; left:25px; top:630px;">
  <object>
    <param name="movie" value="http://www.youtube.com/v/GVhA1GqC1lE&hl=en&fs=1"></param>
    <param name="allowFullScreen" value="true"></param>
    <embed src="http://www.youtube.com/v/GVhA1GqC1lE&hl=en&fs=1&autoplay=1" type="application/x-shockwave-flash" allowscriptaccess="always" allowfullscreen="true" width="280" height="210"></embed>
  </object>
</div>
  <img src="<%= HtmlHelper.IMAGE_URL %>ecigsbrand.com/ec07/default-02a.jpg" width="930" height="200"><br>
  <img src="<%= HtmlHelper.IMAGE_URL %>ecigsbrand.com/ec07/default-05a.jpg" width="930" height="200"><br>
  <img src="<%= HtmlHelper.IMAGE_URL %>ecigsbrand.com/ec07/default-08a.jpg" width="930" height="200"><br>
  <img src="<%= HtmlHelper.IMAGE_URL %>ecigsbrand.com/ec07/default-11a.jpg" width="930" height="200"><br>
  <img src="<%= HtmlHelper.IMAGE_URL %>ecigsbrand.com/ec07/default-14a.jpg" width="930" height="200"><br>
  <img src="<%= HtmlHelper.IMAGE_URL %>ecigsbrand.com/ec07/default-17a.jpg" width="930" height="200"><br>
  <img src="<%= HtmlHelper.IMAGE_URL %>ecigsbrand.com/ec07/default-20a.jpg" width="930" height="200"><br>
  <img src="<%= HtmlHelper.IMAGE_URL %>ecigsbrand.com/ec07/default-23a.jpg" width="930" height="200"><br>
  <img src="<%= HtmlHelper.IMAGE_URL %>ecigsbrand.com/ec07/default-26a.jpg" width="930" height="200"><br>
  <img src="<%= HtmlHelper.IMAGE_URL %>ecigsbrand.com/ec07/default-29a.jpg" width="930" height="200"><br>
  <img src="<%= HtmlHelper.IMAGE_URL %>ecigsbrand.com/ec07/default-32a.jpg" width="930" height="200"><br>
  <img src="<%= HtmlHelper.IMAGE_URL %>ecigsbrand.com/ec07/default-35a.jpg" width="930" height="200"><br>
  <img src="<%= HtmlHelper.IMAGE_URL %>ecigsbrand.com/ec07/default-38a.jpg" width="930" height="200"><br>
  <img src="<%= HtmlHelper.IMAGE_URL %>ecigsbrand.com/ec07/default-41a.jpg" width="930" height="200">
  <form method="post" action="../../Securetrialoffers/ec07/billing-information.aspx?aff=<%=affiliateCode%>&sub=<%=subAffiliateCode%>&cid=<%=clickID%>&exit=<%=exitPop%>" name="frmReg" id="frmReg" class="form">
    <input type="hidden" name="promo" id="hdnPromo" />
    <input type="hidden" name="shw" id="hdnSHW" value="<%= hdnSHW %>" />
    <div id="form">
      <div class="description">
        First Name
      </div>
      <div class="field">
        <input name="firstname" id="firstname" maxlength="30" title="Please Enter First Name" tabindex="1" type="text">
      </div>
      <div class="description">
        Last Name
      </div>
      <div class="field">
        <input name="lastname" id="lastname" maxlength="30" title="Please Enter Last Name" tabindex="2" type="text">
      </div>
      <div class="description">
        Address Line 1
      </div>
      <div class="field">
        <input name="address1" id="address1" maxlength="30" title="Please Enter Address Line 1" tabindex="3" type="text">
      </div>
      <div class="description">
        Address Line 2
      </div>
      <div class="field">
        <input name="address2" id="address2" maxlength="30" title="Please Enter Address Line 2" tabindex="4" type="text">
      </div>
      <div class="description">
        City
      </div>
      <div class="field">
        <input name="city" id="city" maxlength="30" title="Please Enter City" tabindex="5" type="text">
      </div>
      <div class="description">
        State
      </div>
      <div class="field">
        <select name="state" id="state" title="Please Select State" tabindex="6">
        <option value="">Select a State</option>
          <%= HtmlHelper.DDLStateFullName("")%>
        </select>
      </div>
      <div class="description">
        Zip Code
      </div>
      <div class="field">
        <input name="zip" id="zip1" maxlength="5" title="Please Enter Zip Code" tabindex="7" type="text">        
      </div>
      <div class="description">
        Phone Number
      </div>
      <div class="field">
        (
        <input name="phone" id="phone1" style="width:33px;" maxlength="3" title="Please Enter Phone Number" tabindex="9" type="text">
        )
        <input name="phone" id="phone2" style="width:33px;" maxlength="3" title="Please Enter Phone Number" tabindex="10" type="text">
        -
        <input name="phone" id="phone3" style="width:45px;" maxlength="4" title="Please Enter Phone Number" tabindex="11" type="text">
      </div>
      <div class="description">
        Email Address
      </div>
      <div class="field">
        <input name="email" id="email" maxlength="30" title="Please Enter Email Address" tabindex="12" type="text">
      </div>
      <div class="button">
        <a href="#order" onclick="IHS.validate()" tabindex="13"><img src="<%= HtmlHelper.IMAGE_URL %>ecigsbrand.com/ec07/button-rush.png" style="cursor:pointer;" value="Rush My Order" alt="Rush My Order" width="265" height="62" border="0"></a>
      </div>
    </div>
  </form>
  <div id="footer">
        <a href="javascript: void(0)" onclick="window.open('http://www.securetrialoffers.com/dynamic/terms/terms-and-conditions-vgm-ecig.asp?ph=1-866-830-2464','buttonwin','height=400,width=400,scrollbars=yes,toolbar=no,location=no,screenX=100,screenY=20,top=20,left=100','fullscreen=no'); 
        return false;">Terms, Conditions and Refund Policy</a> | <a href="javascript: void(0)" onclick="window.open('http://www.securetrialoffers.com/dynamic/terms/privacy-policy-vgm.asp?ph=1-866-830-2464','buttonwin','height=400,width=400,scrollbars=yes,toolbar=no,location=no,screenX=100,screenY=20,top=20,left=100','fullscreen=no'); 
        return false;">Privacy Policy</a> | <a href="javascript: void(0)" onclick="window.open('http://www.securetrialoffers.com/dynamic/terms/contact-vgm.asp?ph=1-866-830-2464','buttonwin','height=400,width=400,scrollbars=yes,toolbar=no,location=no,screenX=100,screenY=20,top=20,left=100','fullscreen=no'); 
        return false;">Contact Us</a>
  </div>
</div>
<% if (affiliateCode.Length > 0) { %>
<%= HtmlHelper.Pixels(affiliateCode, 1, clickID, null)%>
<% } %>
</body>
</html>
<% HtmlHelper.Counter(1, 229, affiliateCode, subAffiliateCode); %>