<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/Front.Master" AutoEventWireup="true" CodeBehind="e-cigarette-referral-program.aspx.cs" Inherits="Ecigsbrand.Store2.e_cigarette_referral_program" %>
<%@ Register TagPrefix="uc" TagName="Accessories" Src="~/Controls/Accessories.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">E-Cigs: Electronic Cigarette Referral & Reseller Program</asp:Content>
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
<div id="content">
  <div class="left"><img src="images/referral-secret.jpg" width="150" height="150" align="right" style="border:1px solid black; margin:30px 0 0 15px;">
    <h1>E-Cigs Referral Program</h1>
    <h3>Earn Money The Simple Way</h3>
    <p>Our referral system is simple. Just give your customer number out to your friends, family or collegues and tell them to use it as their coupon code. They save 20% on their orders on this site and we cut you a check for 20% of their order and send
      it straight to your mailbox. *Please read the following FAQ for program details.</p>
    <div id="accordion">
      <h3><a href="#">What do I tell my referrals to do?</a></h3>
      <p>Just tell them to visit this site, <a href="http://www.ecigsbrand.com/">www.ecigsbrand.com</a>, and when they're ready to make their order have them use your customer number in the coupon code box. They receive 20% off their order and you receive
        20% of their order in a check sent straight to you plus potential bonuses. Everybody wins. Please <a href="mailto:support@yourorderhelp.com">email us</a> for details and to get started.</p>
      <h3><a href="#">Where do I find my customer number to give out?</a></h3>
      <p>Your customer number is found on both your order confirmation and shipping confirmation email sent to you when you placed your first order.<br>
        <!--<br>
        <a href="javascript: void(0)" onclick="window.open('images/referral-customer-number.png','buttonwin','height=490,width=750,scrollbars=yes,toolbar=no,location=no,screenX=100,screenY=20,top=20,left=100','fullscreen=no'); 
        return false;">[Click here for image of customer number location]</a><br>-->
        <br>
        You can also call us at 1-866-830-2464 and one of our customer service representatives will be happy to help you find your customer number.</p>
      <h3><a href="#">When do I get my check?</a></h3>
      <p>Checks are mailed on a monthly basis.You need to have earned a minimum of $125 before your check will arrive. This process is automatic.</p>
      <h3><a href="#">Reseller: I'd love to sell your products in hand, can I buy in volume?</a></h3>
      <p>Yes, most definitely! If you want to buy our product in higher volumes we have great discounts for resellers. Minimum orders are required, but you can get started for only a few hundred dollars. Please <a href="mailto:support@yourorderhelp.com">email us</a> for details and to get started. More details below.</p>
    </div>
    <img src="images/referral-cash.jpg" width="150" height="150" align="right" style="border:1px solid black; margin:30px 0 0 15px;">
    <h1>E-Cigs Reseller Program</h1>
    <h3>Great Opportunity To Earn Serious Money</h3>
    <p>Want to earn some extra cash on the side? Do you own a convenience store and want to carry our products? Do you want host E-Cigs parties?</p>
    <p>There's many reasons to become an E-Cig Reseller including the great variety of products that your friends or customers will love and will want
      to keep coming back for. As well as offering some real discounts, we also handle all customer issues, so all you have to do is sell and we take care of the rest. Now, how easy is that?</p>
    <div id="accordion_reseller">
      <h3><a href="#">How do I get started?</a></h3>
      <p>If you want to start buying our product in higher volumes we offer great discounts for resellers. Minimum orders are required, but you can get started for only a few hundred dollars. Please <a href="mailto:support@yourorderhelp.com">email us</a> for details and to get started. for inquiries.</p>
      <h3><a href="#">What happens if I have product/customer issue?</a></h3>
      <p>That's the beauty of working with E-Cigs, you don't need to worry about customer issues or product warranties. Just give them our customer service number, 1-866-830-2464, and your Reseller ID/Customer Number for reference and we'll handle any returns or customer issues.</p>
    </div>
  </div>
  <div class="right">
    <uc:Accessories ID="cAccessories" runat="server" />
  </div>
  <div style="clear:both;"></div>
</div>
</asp:Content>
