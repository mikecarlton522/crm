<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/Front.Master" AutoEventWireup="true" CodeBehind="e-cigarette-referral-program.aspx.cs" Inherits="Ecigsbrand.Store1.e_cigarette_referral_program" %>
<%@ Register TagPrefix="uc" TagName="Accessories" Src="~/Controls/Accessories.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">E-Cigs: Electronic Cigarette Friends Network Referral & Reseller Program</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphScript" runat="server">
<script>
  $(document).ready(function() {
    $("#accordion").accordion({ autoHeight: false, collapsible: true});
  });
  </script>
  <script>
  $(document).ready(function() {
    $("#accordion_reseller").accordion({ autoHeight: false, collapsible: true});
  });
</script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
<style type="text/css">
	#login-link { border:1px dotted gray; padding:10px; text-align:center; font-weight:bold; font-size:32px; }
	#login-link a:link, #login-link a:visited { color:#BC0800; }



</style>
<div id="content">
  <div class="left"><img src="images/referral-secret.jpg" width="150" height="150" align="right" style="border:1px solid black; margin:30px 0 0 15px;">
    <h1>E-Cigs Friends Network</h1>
    <h3>Earn Money The Simple Way</h3>
    <p>If our customers tell us one thing, it's that they can't believe how much of an impact our ecigarettes have had on their smoking habits. They are always amazed that it took them this long to discover them.  Well, there are millions more smokers out there just like you who could be benefiting from the ecigarette.  With our new Referral Program you can introduce these customers to a great product and make fantastic commissions!<br/>
    
    </p>
    <div id="accordion">
      <h3><a href="#">How does it work?</a></h3>
      <ul style="padding:15px 15px 15px 25px;">
			  <li>Sign in to your account with your username and password. Look to the top right of this page for the login area.</li>
			  <li>You will see your Referral ID</li>
			  <li>Give your Referral ID to your friends to use when they shop at our web store. By using your Referral ID, your friends will get 20% off, and you will earn Ecigs Bucks up to 40% of the value of any referred customers' purchase. You can exchange Ecigs Bucks in our webstore to buy yourself more product or you can exchange them for real dollars which we will send to you monthly.</li>
			  <li>When your referred customers themselves refer more friends, you will also earn 8% commission for those second-level referred sales. There's no limit to how many customers you can refer.</li>
			  <li>Your account on the web site will allow you to track and monitor all the sales you generate. We look forward to teaming up with you to help. As always we value your feedback and input, so do send us a note at any time.</li>
			</ul>
			<h3><a href="#">What If I don't have a username or password?</a></h3>
				<ul style="padding:15px 15px 15px 25px;">
			  <li>If you've ever bought anything from us before you have already been assigned a referral username and password. You can find your Referral ID, also known as your Customer ID, in your email receipt for any purchase. Or call us at 1-866-830-2464.</li>
			  <li>If you've never bought anything from us before, you can sign up <a href="http://ecigsbrand.com/webstore/cp-login.aspx">here</a>.
			</ul>
			<h3><a href="#">What happens if I have product/customer issue?</a></h3>
      <p>That's the beauty of working with E-Cigs, you don't need to worry about customer issues or product warranties. Just give them our customer service number, 1-866-830-2464, and your Reseller ID/Customer Number for reference and we'll handle any returns or customer issues.</p>
      
    </div>
    
    <div id="login-link"><a href="cp-login.aspx">Login Now</a></div>
    
    <!--<img src="images/referral-cash.jpg" width="150" height="150" align="right" style="border:1px solid black; margin:30px 0 0 15px;">
    <h1>E-Cigs Reseller Program</h1>
    <h3>Great Opportunity To Earn Serious Money</h3>
    <p>Want to earn some extra cash on the side? Do you own a convenience store and want to carry our products? Do you want host E-Cigs parties?</p>
    <p>There's many reasons to become an E-Cig Reseller including the great variety of products that your friends or customers will love and will want
      to keep coming back for. As well as offering some real discounts, we also handle all customer issues, so all you have to do is sell and we take care of the rest. Now, how easy is that?</p>
    <div id="accordion_reseller">
      <h3><a href="#">How do I get started?</a></h3>
      <p>If you want to start buying our product in higher volumes we offer great discounts for resellers. Minimum orders are required, but you can get started for only a few hundred dollars. Please <a href="mailto:support@yourorderhelp.com">email us</a> for details and to get started. for inquiries.</p>
    </div>
    -->
  </div>
  <div class="right">
    <!--<uc:Accessories ID="cAccessories" runat="server" />-->
    <img src="images/commission-structure.png" align="middle" />
    <p style="margin-top:0;">
				If you signed up just one person per week onto a monthly supply of Ecigs ($80 average purchase), and those people signed up just one person per month, after one year, your monthly Ecigs Bucks commission check would be:
				<br />
				<br />
				- 52 Direct Referrals x $80/mo x 40% = $1664<br />
				- 312 Indirect Referrals x $80/mo x 8% = $1997<br />
				- Total Monthly Commission: $3667 E-Cigs Bucks or convert these into real dollars.<br /><br />
				
			</p>
  </div>
  <div style="clear:both;"></div>
</div>
</asp:Content>
