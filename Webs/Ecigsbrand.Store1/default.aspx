<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Ecigsbrand.Store1._default" MasterPageFile="~/Controls/Front.Master" %>
<%@ Register TagPrefix="uc" TagName="Features" Src="~/Controls/Features.ascx" %>
<%@ Register TagPrefix="uc" TagName="Accessories" Src="~/Controls/Accessories.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">E-Cigs: Electronic Cigarette: E-Cigarette Kits, Accessories & Nicotine Refill Cartridges</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
<uc:Features ID="cFeatures" runat="server" />
<div id="content">
  <div class="left">
    <h1>Do You <img src="images/front-heart.png" width="48" height="45" align="absmiddle"> Your E-Cigarettes?</h1>
    <img src="images/front-money-tree.jpg" width="257" height="273" align="right">
    <h2>Refer Your Friends &amp;<br>
      <span style="font-size:43px; color:#336600;">Earn Money!</span></h2>
    <p>Did you know that we offer a great incentive program for our members? You can start earning money by selling our e-cigarettes and accessories to your friends. We take care of billing, customer service, shipping... Well, everything. All you do is
      tell your friends and start earning money.</p>
  <a href="e-cigarette-referral-program.aspx"><img src="images/button-learn-more.png" width="111" height="19" border="0"></a><img src="images/front-zero-worry.png" width="610" height="107" style="padding-top:20px;"></div>
  <div class="right">
    <uc:Accessories ID="cAccessories" runat="server" />
  </div>
  <div style="clear:both;"></div>
</div>
</asp:Content>