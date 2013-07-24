<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="recommended.aspx.cs" Inherits="Securetrialoffers.ec07.recommended" %>
<%@ Import Namespace="TrimFuel.Business.Utils" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<script>
    function take(b) {
        if (b) {
            document.getElementById('form1').submit();            
        } else {
        window.location = "tflf.asp?bid=<%=billingID%>&cid=<%=clickID%>&aff=<%=affiliateCode%>&sub=<%=subAffiliateCode%>";
        }
    }
</script>

<meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
<title>E-Cigs Electronic Cigarettes</title>
<style type="text/css">
body { margin:8px 0 0 0; padding:0; font-family:arial, helvetica, sans-serif; }
#wrapper { margin:0 auto 20px auto; width:910px; font-size:12px; }
#thanks { text-align:right; margin-top:15px; }
#button { text-align:right; margin-top:10px; }
a:link, a:visited { color:#004ABE; }
table tr.bottom td { border-bottom:1px dotted gray; }
table tr.header { background-color:#A10000; color:white; font-weight:bold; }
table tr.header td { padding:10px 3px; }
.bold { font-weight:bold; }
.red { color:red; }
.blue { color:#004ABE; }
.green { color:green; }
.strike { text-decoration:line-through; }
.center { text-align:center; }
</style>
</head>
<body>
<% if (affiliateCode != null) { %>
<%= HtmlHelper.Pixels(affiliateCode, 4, clickID, null)%>
<% } %>
<form id="form1" action="recommended.aspx?bid=<%=billingID%>&cid=<%=clickID%>&aff=<%=affiliateCode%>&subaff=<%=subAffiliateCode%>" method="post">
<input type="hidden" name="_action" id="_action" value="upsell" />
<div id="wrapper">
  <img src="images/default-upgrade-table.png" />
  <table width="890" border="0" cellspacing="0" cellpadding="3" style="margin:auto; border:1px solid #b4b4b4; border-top:none; padding-top:10px;">
    <tr class="header">
      <td width="10%" class="center">
        Product
      </td>
      <td width="55%">
        Description
      </td>
      <td width="10%" class="center">
        Availability
      </td>
      <td width="10%" class="center">
        Shipping
      </td>
      <td width="10%">
        Price
      </td>
      <td width="5%" class="center">
        Buy
      </td>
    </tr>
    <tr class="bottom">
      <td class="center">
        <img src="images/product-car-charger.png" width="60" height="60" />
      </td>
      <td>
        <b>Universal Car Charger for E-Cigarette.</b> Adapts to Starter Kit USB charger.
      </td>
      <td class="bold green center">
        In Stock
      </td>
      <td class="bold blue center">
        FREE
      </td>
      <td class="bold">
        <span class="strike">$19.99</span> <span class="red">$9.99</span>
      </td>
      <td class="center">
        <input type="checkbox" name="upsell" value="10" />
      </td>
    </tr>
    <tr class="bottom">
      <td class="center">
        <img src="images/product-wall-charger.png" width="60" height="60" />
      </td>
      <td>
        <b>Universal Wall Charger for E-Cigarette.</b> Adapts to Starter Kit USB charger.</strong>
      </td>
      <td class="bold green center">
        In Stock
      </td>
      <td class="bold blue center">
        FREE
      </td>
      <td class="bold">
        <span class="strike">$25.99</span> <span class="red">$12.99</span>
      </td>
      <td class="center">
        <input type="checkbox" name="upsell" value="11" />
      </td>
    </tr>
    <tr class="bottom">
      <td class="center">
        <img src="images/product-menthol-cartridges.png" width="60" height="60" />
      </td>
      <td>
        <b>10 Menthol Nicotine Cartridges for E-Cigarette.</b> Equivalent to 20 Packs</strong>.
      </td>
      <td class="bold green center">
        In Stock
      </td>
      <td class="bold blue center">
        FREE
      </td>
      <td class="bold">
        <span class="strike">$79.99</span> <span class="red">$59.99</span>
      </td>
      <td class="center">
        <input type="checkbox" name="upsell" value="12" />
      </td>
    </tr>
    <tr class="bottom">
      <td class="center">
        <img src="images/product-celebrity-smile.png" width="60" height="60" />
      </td>
      <td>
        <b>Celebrity Smile Professional Teeth Whitening Kit.</b> Same as National drugstore brands.</strong>.
      </td>
      <td class="bold green center">
        In Stock
      </td>
      <td class="bold blue center">
        FREE
      </td>
      <td class="bold">
        <span class="strike">$39.99</span> <span class="red">$9.99</span>
      </td>
      <td class="center">
        <input type="checkbox" name="upsell" value="9" />
      </td>
    </tr>
    <tr>
      <td class="center">
        <img src="images/product-nature-renew.png" width="60" height="60" />
      </td>
      <td>
        <b>Nature Renew Total Colon &amp; Body Cleanse.</b> 60 Capsule Bottle.
      </td>
      <td class="bold green center">
        In Stock
      </td>
      <td class="bold blue center">
        FREE
      </td>
      <td class="bold">
        <span class="strike">$39.99</span> <span class="red">$9.99</span>
      </td>
      <td class="center">
        <input type="checkbox" name="upsell" value="7" />
      </td>
    </tr>
  </table>
  <div id="button">
    Please note that items are <strong>billed seperately</strong>.&nbsp;&nbsp;<img src="images/button-add-products.png" align="absmiddle" width="265" height="62" style="cursor:pointer;" onclick="take(true)" />
  </div>
  <div id="thanks">
    <a href="#" onclick="take(false)">No thanks, don't add any products at incredible savings.</a>
  </div>
</div>
</div>
</form>
</body>
</html>
<% HtmlHelper.Counter(3, campaignID.Value, affiliateCode, subAffiliateCode); %>