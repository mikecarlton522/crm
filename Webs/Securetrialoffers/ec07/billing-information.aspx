<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="billing-information.aspx.cs" Inherits="Securetrialoffers.ec07.billing_information" %>
<%@ Import Namespace="TrimFuel.Business.Utils" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
<title>E-Cigs Electonic Cigarettes</title>
<style type="text/css">
body { margin:0; padding:0; font-family:arial, helvetica, sans-serif; background-image:url(images/default-billing-bg.png); background-repeat:repeat-x; }
a:link, a:visited { color:#004ABE; }
input { width:150px; background:url(images/topfade.gif) repeat-x top; vertical-align:middle; border:1px solid #999999; padding:4px; }
textarea { width:310px; height:110px; font-size:12px; }
#wrapper { margin:auto; width:950px; background-color:white; background-image:url(images/default-billing.png); background-repeat:no-repeat; }
#footer { margin:auto; width:950px; text-align:center; color:white; font-size:11px; padding:0 0 40px 0; }
#left { float:left; width:550px; }
#left .producttable { float:left; margin:296px 0 0 30px; font-size:12px; border:1px solid #b4b4b4; border-top:0; }
#left .arrow { float:left; margin:10px 0 0 29px; }
#left .terms { float:left; margin:5px 0 0 43px; font-size:12px; }
#left .seals { float:left; margin:15px 0 0 110px; }
#left .price { font-size:18px; font-weight:bold; }
.seperator { border-top:1px dotted #c2c2c2; }
#right { float:right; margin:380px 30px 0 0; text-align:left; border:1px solid #b4b4b4; border-top:none; font-size:12px; width:288px; background-color:#FFF8AD; padding:9px 10px; }
#right table td { padding:3px 5px; }
#right .button { cursor:pointer; margin:10px 0; }
#footer { clear:both; text-align:center; width:700px; margin:auto; padding:30px 0; font-size:12px; color:black; }
#error { margin:0; border:1px dotted red; color:red; font-weight:bold; font-size:14px; padding:10px; text-align:center; background:url(images/topfade-red.gif) repeat-x top; }
</style>
<script type="text/javascript" src="js/jquery.js"></script>
<% if (IsExitPopup) { %>
<script type="text/javascript" src="js/exitpop.js"></script>
<% } %>
<script type="text/javascript" src="js/mod10.js"></script>
<script type="text/javascript" src="js/IHS.validate.js"></script>
<script type="text/javascript" src="js/billing.js"></script>
<script>
    function goToS(el) {
        internalLink = true;
        if ($('#promo1').val().length > 0) {
            $('.price').html('$1.95');
            $(el).hide();
        } else {
            alert('Please enter a promotion code');
        }
    }
</script>
</head>
<body>
<% if (affiliateCode != null) { %>
<%= HtmlHelper.Pixels(affiliateCode, 2, clickID, null)%>
<% } %>
<form name='frmPay' id='frmPay' method='post' action='billing-information.aspx?aff=<%= affiliateCode %>&sub=<%= subAffiliateCode %>&cid=<%= clickID%>'>
  <input type="hidden" name="promo" id="hdnPromo" value="<%= promo %>" />
  <input type="hidden" name="_action" id="_action" />
  <input type="hidden" name="couponCode" id="hdnCouponCode" value="<%= couponCode %>" />
  <input type="hidden" name="id" id="hdnRegId" value="<%= registrationID %>" />
  <input type="hidden" name="bid" id="hdnRillId" value="<%= billingID %>" />
  <div id="wrapper">
    <div id="left">
      <div class="producttable">
        <table width="538" border="0" cellspacing="0" cellpadding="5" id="tblOptions">
          <tr>
            <td width="15%" align="center">
              <img src="images/box.png" width="60" height="73" style="padding:10px 0;" />
            </td>
            <td width="65%">
              <div style="font-weight:bold; font-size:16px; margin-bottom:8px;">
                Exclusive E-Cigarette Starter Kit
              </div>
              <ul>
                <li>E-Cigarette Unit with Advanced Atomizer</li>
                <li>High Capacity Lithium-Ion Battery</li>
                <li>2 Genuine Tobacco Flavored Cartridges</li>
                <li>USB Charger</li>
                <li>Limited Lifetime Warranty</li>
              </ul>
            </td>
            <td width="10%">
              <span class="price" style="color:red;">$0.00</span>
            </td>
          </tr>
          <tr>
            <td align="right" colspan="2" class="seperator">
              Sub Total
            </td>
            <td class="price seperator">
              $0.00
            </td>
          </tr>
          <tr>
            <td align="right" colspan="2" class="seperator">
              Shipping & Handling
            </td>
            <td class="price seperator">
              $9.99
            </td>
          </tr>
          <% if (IsSpecialOffer) { %>
          <tr>
            <td align="right" colspan="2" class="seperator">
              Special Discount Activated
            </td>
            <td class="price seperator" >
              <span style="color:red;">(2.00)</span>
            </td>
          </tr>
          <% } %>
          <tr>
            <td colspan="2" class="price seperator" style="padding-left:10px;">
              <table cellpadding="0" cellspacing="0" border="0" width="100%">
                <tr>
                  <td>
                    <% if (!IsSpecialOffer) { %>
                    <% if (Coupon != null) { %>
                    <%= Coupon.CouponCode %>
                    <% } else { %>
                    <input type="text" name="coupon" id="tbCoupon" maxlength="15" title="Please Enter Valid Coupon Code" style="width:100px;" tabindex="50" />
                    <input type="button" onclick='submitCoupon(this)' value="Apply Coupon" style="width:90px;" />
                    <% } %>
                    <% } %>
                  </td>
                  <td align="right">
                    Total
                  </td>
                </tr>
              </table>
            </td>
            <td class="price seperator">
              <div id="total" style="margin:10px 0;">
                $<%= HtmlHelper.FormatPrice(TotalPrice) %>
              </div>
            </td>
          </tr>
        </table>
      </div>
      <div class="arrow">
        <img src="images/arrow.png">
      </div>
      <div class="terms">
        <table cellpadding="0" cellspacing="0">
          <tr>
            <td>
              Exclusive Offer Terms: By placing an order you will also be enrolled in our refill membership program. Unless you call to cancel,
                in 15 days and every 30 days thereafter, you will be sent 10 refill packs (equivalent to 20 packs of tobacco cigarettes) for
                only $79.99 plus S&H. To modify your order at anytime call 1-866-830-2464.
            </td>
          </tr>
        </table>
      </div>
      <div class="seals">
        <img src="images/seals.png">
      </div>
    </div>
    <div id="right">
      <% if (error_msg != null) { %>
      <table cellpadding="10" cellspacing="0" width="100%">
        <tr>
          <td>
            <div id="error">
              <%= error_msg %>
            </div>
          </td>
        </tr>
      </table>
      <% } %>
      <table width="100%">
        <tr>
          <td width="40%">
            <b>We accept:</b>
          </td>
          <td width="60%">
            <img src="images/visa-mc.png" width="108" height="30" />
          </td>
        </tr>
        <tr>
          <td>
            <b>Payment type:</b>
          </td>
          <td>
            <select name="PaymentTID" id="PaymentTID" title="Please Choose Credit Card Type" tabindex="1" >
              <%= HtmlHelper.DDLPaymentType(Billing.PaymentTypeID) %>
            </select>
          </td>
        </tr>
        <tr>
          <td>
            <b>Card number:</b>
          </td>
          <td>
            <input type="text" value="<%=Billing.CreditCardCnt.DecryptedCreditCard%>" name="CC_Number" id="CC_Number" maxlength="16" title="Please Enter Valid Credit Card Number" tabindex="2" />
          </td>
        </tr>
        <tr>
          <td>
            <b>Expiry date:</b>
          </td>
          <td>
            <select name="Month" id="Month" title="Please Choose Credit Card Expiration Month" style="width:75px;" tabindex="3">
              <%= HtmlHelper.DDLMonth(Billing.ExpMonth) %>
            </select>
            <select name="Year" id="Year" title="Please Choose Credit Card Expiration Year" style="width:60px;" tabindex="4">
              <%= HtmlHelper.DDLYear(Billing.ExpYear) %>
            </select>
          </td>
        </tr>
        <tr>
          <td>
            <b>CVV:</b>
          </td>
          <td>
            <input type="text" value="<%=Billing.CVV%>" name="CVV" maxlength="5" id="CVV" title="Please Enter Enter Valid CVV" style="width:50px;" tabindex="5" />
            &nbsp; <a href="javascript: void(0)" tabindex="5" onclick="window.open('images/cvv.png','buttonwin','height=350,width=520,scrollbars=yes,toolbar=no,location=no,screenX=100,screenY=20,top=20,left=100','fullscreen=no'); 
        return false;">(what's this?)</a>
          </td>
        </tr>
      </table>
      <% if (error_msg != null || IsSpecialOffer || IsRebill) { %>
      <div id="billing-address">
        <table width="100%">
          <tr>
            <td colspan="2" style="font-size:16px; font-weight:bold;">
              Billing Address
            </td>
          <tr>
            <td width="45%">
              <b>First name:</b>
            </td>
            <td width="55%">
              <label>
                <input type="text" value="<%=Billing.FirstName%>" name="First_Name" id="First_Name" class="field" tabindex="6" />
              </label>
            </td>
          </tr>
          <tr>
            <td>
              <b>Last name:</b>
            </td>
            <td>
              <label>
                <input type="text" value="<%=Billing.LastName%>" name="Last_Name" id="Last_Name" class="field" tabindex="7" />
              </label>
            </td>
          </tr>
          <tr>
            <td>
              <b>Address line 1:</b>
            </td>
            <td>
              <label>
                <input type="text" value="<%=Billing.Address1%>" name="Billing_Address_1" id="Billing_Address_1" class="field" tabindex="8" />
              </label>
            </td>
          </tr>
          <tr>
            <td>
              <b>Address line 2:</b>
            </td>
            <td>
              <label>
                <input type="text" value="<%=Billing.Address2%>" name="Billing_Address_2" id="Billing_Address_1" class="field" tabindex="9" />
              </label>
            </td>
          </tr>
          <tr>
            <td>
              <b>City:</b>
            </td>
            <td>
              <label>
                <input type="text" value="<%=Billing.City%>" name="City" id="City" class="field" tabindex="10" />
              </label>
            </td>
          </tr>
          <tr>
            <td>
              <b>State:</b>
            </td>
            <td>
              <label>
                <select name="state" id="state" style="width:150px;" tabindex="11">
                  <option value="">Select a State</option>
                  <%= HtmlHelper.DDLStateFullName(Billing.State)%>
                </select>
              </label>
            </td>
          </tr>
          <tr>
            <td>
              <b>Zip code:</b>
            </td>
            <td>
              <input type="text" id="Zip1" value="<%=Billing.ZipCnt.Part1%>" name="zip" class="field x-short" maxlength="5" tabindex="12" />
            </td>
          </tr>
          <tr>
            <td>
              <b>Phone number:</b>
            </td>
            <td>
              (
              <input type="hidden" name="Home_Tel" id="Home_Tel" value="" />
              <input type="text" value="<%=Billing.PhoneCnt.Code%>" id="Home_Tel1" class="field" style="width: 30px" maxlength="3" tabindex="13" />
              )
              <input type="text" value="<%=Billing.PhoneCnt.Part1%>" id="Home_Tel2" class="field" style="width: 30px" maxlength="3" tabindex="14" />
              -
              <input type="text" value="<%=Billing.PhoneCnt.Part2%>" id="Home_Tel3" class="field" style="width: 40px" maxlength="4" tabindex="15" />
            </td>
          </tr>
          <tr>
            <td>
              <b>Email eddress:</b>
            </td>
            <td>
              <label>
                <input type="text" value="<%=Billing.Email%>" name="Email" id="Email" class="field" tabindex="16" />
              </label>
            </td>
          </tr>
        </table>
      </div>
      <% } %>
      <table width="288" cellpadding="3" cellspacing="0" border="0">
        <tr>
          <td colspan="2" align="center">
            <a href="#order" onclick="IHS.validate()" tabindex="40"><img src="images/button-confirm.png" value="Conrim My Order" alt="Confirm My Order" width="265" height="62" border="0" class="button"></a>
          </td>
        </tr>
      </table>
    </div>
    <div id="footer">
      <a href="javascript: void(0)" onclick="window.open('http://www.securetrialoffers.com/dynamic/terms/terms-and-conditions-vgm-ecig.asp?ph=1-866-830-2464','buttonwin','height=400,width=400,scrollbars=yes,toolbar=no,location=no,screenX=100,screenY=20,top=20,left=100','fullscreen=no'); 
        return false;">Terms, Conditions and Refund Policy</a> | <a href="javascript: void(0)" onclick="window.open('http://www.securetrialoffers.com/dynamic/terms/privacy-policy-vgm.asp?ph=1-866-830-2464','buttonwin','height=400,width=400,scrollbars=yes,toolbar=no,location=no,screenX=100,screenY=20,top=20,left=100','fullscreen=no'); 
        return false;">Privacy Policy</a> | <a href="javascript: void(0)" onclick="window.open('http://www.securetrialoffers.com/dynamic/terms/contact-vgm.asp?ph=1-866-830-2464','buttonwin','height=400,width=400,scrollbars=yes,toolbar=no,location=no,screenX=100,screenY=20,top=20,left=100','fullscreen=no'); 
        return false;">Contact Us</a>
    </div>
  </div>
</form>
</body>
</html>
<% HtmlHelper.Counter(2, campaignID.Value, affiliateCode, subAffiliateCode); %>