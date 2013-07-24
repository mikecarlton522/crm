<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/Front.Master" AutoEventWireup="true" CodeBehind="cp-home.aspx.cs" Inherits="Ecigsbrand.Store2.account.cp_home" %>
<%@ Register src="../Controls/MenuControlPanel.ascx" tagname="MenuControlPanel" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">E-Cigs: Overview</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphScript" runat="server">
<link rel="stylesheet" type="text/css" href="../css/control-panel.css" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <form id="form1" runat="server">
<div id="content">
  <div class="left">
    <h1>Customer &amp; Referral Control Panel</h1>
    <uc1:MenuControlPanel ID="MenuControlPanel1" runat="server" />
    <h2>Overview</h2>
    <h3>My Referral Details</h3>
    <div class="blueborder">
      <table width="100%" border="0" cellspacing="0">
        <tr class="noborder">
          <td width="30%">Referral Code</td>
          <td width="70%"><span style="font-size:20px; font-weight:bold; color:#2C4762;"><%# Referer.RefererCode %></span></td>
        </tr>
        <tr class="white">
          <td>&nbsp;</td>
          <td><i>- Also known as your Customer ID or Billing ID.<br />
          - Your referral code also works as a coupon code for your referrals giving them 20% off their order.</i></td>
        </tr>
        <tr>
          <td>Referral Link</td>
          <td><span style="font-size:20px; font-weight:bold;"><a href="http://www.ecigsbrand.com/webstore/<%# Referer.RefererCode %>" target="_blank">http://www.ecigsbrand.com/webstore/<%# Referer.RefererCode %></a></span></td>
        </tr>
        <tr class="white">
          <td>&nbsp;</td>
          <td><i>(Give this link to your referrals)</i></td>
        </tr>
      </table>
    </div>
    <h3>My Commission To Date</h3>
    <p>Please note that current period earnings are estimates as returns are removed from your commissions.</p>
    <div class="blueborder">
      <table width="100%" border="0" cellspacing="0">
        <tr class="header">
          <td width="50%">Commission</td>
          <td width="50%">&nbsp;</td>
        </tr>
        <tr class="white">
          <td>Earned Today</td>
          <td class="total"><%# TotalComissionToday.ToString("c") %> E-Cigs Dollars</td>
        </tr>
        <tr>
          <td><strong>Earned This Commission Period
            (<%# DateTime.Today.AddDays(1 - DateTime.Today.Day).ToString("MMM d, yyyy") %> - <%# DateTime.Today.AddDays(1 - DateTime.Today.Day).AddMonths(1).ToString("MMM d, yyyy") %>)</strong></td>
          <td class="total"><%# TotalComissionLastMonth.ToString("c") %> E-Cigs Dollars (Estimated)</td>
        </tr>
        <tr class="white">
          <td>Earned Last 7 Days</td>
          <td class="total"><%# TotalComissionLast7Days.ToString("c") %> E-Cigs Dollars</td>
        </tr>
        <tr>
          <td>Earned Last 30 Days</td>
          <td class="total"><%# TotalComissionLast30Days.ToString("c") %> E-Cigs Dollars</td>
        </tr>
        <tr class="white">
          <td>Earned In My Lifetime</td>
          <td class="total"><%# TotalComission.ToString("c") %> E-Cigs Dollars</td>
        </tr>
      </table>
    </div>
  </div>
</div>
<div style="clear:both;">
</div>
    </form>
</asp:Content>
