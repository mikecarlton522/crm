<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Menu.ascx.cs" Inherits="Fitdiet.Store1.Controls.Menu" %>
<script type="text/javascript">
$ = jQuery;

$(document).ready(function() {
	
$("#mainMenu").hover(function() { //Hover over event on list item
	$("#subMenu").show(); //Show the subnav
} , function() { //on hover out...
    $("#subMenu").hide(); //Hide the subnav
});
	
});
</script>
<table width="100%" border="0" align="center" cellpadding="0" cellspacing="0" class="Nav_MainBG">
  <tr>
    <td align="center" valign="middle" style="padding-top:9px;"><table width="956" border="0" align="center" cellpadding="0" cellspacing="0">
      <tr>
        <td height="59" align="center" valign="middle">
        <%--<td>--%>
            <%--<div style="height: 59px; text-align: center; vertical-align: middle;">--%>
                <a href="<%# RealativePathPrefix %>default.aspx" style="margin-top: -15px;">
                <img src="<%# RealativePathPrefix %>images/home_icon.jpg" alt="" width="24" height="21" border="0" /></a>
            <%--</div>--%>
        </td>
        <td width="8" align="left" valign="top" class="Nav_GreenBG">
            <img src="<%# RealativePathPrefix %>images/nav_div1.jpg" width="5" height="58" alt="" />
        </td>
        
       
        <td align="center" valign="middle" class="Nav_GreenBG" style="padding:0px 25px 0px 25px" id="mainMenu">
            <a href="<%# RealativePathPrefix %>fitdiet-how-it-works.aspx" class="Nav_Text">HOW IT WORKS</a>
            <div id="subMenu" style="display: none; position: absolute; margin-left: -30px;">
                <table style="width: 900px;" border="0" align="center" cellpadding="0" cellspacing="0" >
                    <tr>
                        <td>
                            <table width="395" border="0" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td width="17" align="left"><img src="images/nav2_left.jpg" alt="" width="17" height="30" /></td>
                                    <td width="360" valign="top" class="second_nav_bg">
                                        <table width="331" border="0" align="center" cellpadding="0" cellspacing="0">
                                            <tr>
                                                <td align="center"><a href="fitdiet-how-it-works.aspx"" class="secondnav_text">how is works</a></td>
                                                <td align="center" class="secondnav_text">I</td>
                                                <td align="center"><a href="fitdiet-how-to-use.aspx" class="secondnav_text">how to use</a></td>
                                                <td align="center" class="secondnav_text">I</td>
                                                <td align="center"><a href="fitdiet-secret.aspx" class="secondnav_text"><strong>THE FIT™ SECRE</strong>T</a></td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td width="18" align="left"><img src="images/nav2_right.jpg" alt="" width="17" height="30" /></td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </div>
        </td>
        <td width="2" align="center" valign="top" class="Nav_GreenBG">
            <img src="<%# RealativePathPrefix %>images/nav_div.jpg" width="2" height="58" alt="" /></td>
        <td align="center" valign="top" class="Nav_GreenBG" style="padding:0px 25px 0px 25px">
            <a href="<%# RealativePathPrefix %>fitdiet-success-stories.aspx" class="Nav_Text">SUCCESS STORIES</a></td>
        <td width="2" align="center" valign="top" class="Nav_GreenBG">
            <img src="<%# RealativePathPrefix %>images/nav_div.jpg" width="2" height="58" alt="" /></td>
        <td align="center" valign="top" class="Nav_GreenBG" style="padding:0px 25px 0px 25px">
            <a href="<%# RealativePathPrefix %>fitdiet-press.aspx" class="Nav_Text">PRESS</a></td>
        <td width="2" align="center" valign="top" class="Nav_GreenBG">
            <img src="<%# RealativePathPrefix %>images/nav_div.jpg" width="2" height="58" alt="" /></td>
        <td align="center" valign="top" class="Nav_GreenBG" style="padding:0px 25px 0px 25px">
            <a href="<%# RealativePathPrefix %>fitdiet-faq.aspx" class="Nav_Text">FAQ</a></td>
        <td width="2" align="center" valign="top" class="Nav_GreenBG">
            <img src="<%# RealativePathPrefix %>images/nav_div2.jpg" width="2" height="58" alt="" /></td>
        <td  align="center" valign="top" class="Nav_OrangeBG" style="padding:0px 25px 0px 25px">
        <table width="0" border="0" align="center" cellpadding="0" cellspacing="0">
          <tr>
            <td><img src="<%# RealativePathPrefix %>images/shop_icon.png" width="25" height="22" alt="" /></td>
            <td><a href="fitdiet-shop.aspx" class="Nav_Text">SHOP</a></td>
          </tr>
        </table></td>
        <td align="left" valign="top" class="Nav_PinkBG"><img src="<%# RealativePathPrefix %>images/nav_div3.jpg" width="2" height="58" alt="" /></td>
        <td align="center" valign="top" class="Nav_PinkBG" style="padding:0px 25px 0px 25px"><a href="<%# RealativePathPrefix %>fitdiet-shop.aspx" class="Nav_Text">TRY FIT FREE *|</a></td>
        <td width="2" align="left" valign="top"><img src="<%# RealativePathPrefix %>images/nav_div4.jpg" width="2" height="58" alt="" /></td>
        <td width="64" align="center" valign="middle"><a href="fitdiet-contact_us.aspx"><img src="<%# RealativePathPrefix %>images/contact_icon.jpg" alt="" width="25" height="20" border="0" /></a></td>
      </tr>       
    </table></td>
  </tr>
  </table>