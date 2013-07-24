﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/Front.Master" AutoEventWireup="true" CodeBehind="cp-referrals.aspx.cs" Inherits="Fitdiet.Store1.account.cp_referrals" %>
<%@ Register src="../Controls/MenuControlPanel.ascx" tagname="MenuControlPanel" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">E-Cigs: My Referrals</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphScript" runat="server">
<link rel="stylesheet" type="text/css" href="../css/control-panel.css" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
<div id="content">
  <div class="left">
    <h1>Customer &amp; Referral Control Panel</h1>
    <uc1:MenuControlPanel ID="MenuControlPanel1" runat="server" />
    <h2>My Referrals</h2>
    <h3>Direct Referrals</h3>
    <p>These are referrals that used your referral code. Please note these are estimates as returns are removed from your commissions.</p>
    <div class="blueborder">
      <table width="100%" border="0" cellspacing="0">
        <tr class="header">
          <td>Name</td>
          <td>Orders In Past 30 Days</td>
          <td>Lifetime Orders</td>
          <td>My Commission (40%)</td>
        </tr>
        <asp:Repeater runat="server" ID="rPrimarySales">
            <ItemTemplate>
                <tr class="white">
                  <td width="25%"><%# (Container.DataItem as System.Collections.Generic.IDictionary<string, object>)["Name"] %></td>
                  <td width="25%"><%# Convert.ToDecimal((Container.DataItem as System.Collections.Generic.IDictionary<string, object>)["Last30Days"]).ToString("c") %></td>
                  <td width="25%"><%# Convert.ToDecimal((Container.DataItem as System.Collections.Generic.IDictionary<string, object>)["Lifetime"]).ToString("c") %></td>
                  <td width="25%"><%# Convert.ToDecimal((Container.DataItem as System.Collections.Generic.IDictionary<string, object>)["Commission"]).ToString("c")%> E-Cigs Dollars</td>
                </tr>
            </ItemTemplate>
            <AlternatingItemTemplate>
                <tr>
                  <td width="25%"><%# (Container.DataItem as System.Collections.Generic.IDictionary<string, object>)["Name"] %></td>
                  <td width="25%"><%# Convert.ToDecimal((Container.DataItem as System.Collections.Generic.IDictionary<string, object>)["Last30Days"]).ToString("c") %></td>
                  <td width="25%"><%# Convert.ToDecimal((Container.DataItem as System.Collections.Generic.IDictionary<string, object>)["Lifetime"]).ToString("c") %></td>
                  <td width="25%"><%# Convert.ToDecimal((Container.DataItem as System.Collections.Generic.IDictionary<string, object>)["Commission"]).ToString("c")%> E-Cigs Dollars</td>
                </tr>
            </AlternatingItemTemplate>
            <FooterTemplate>
                <tr class="total">
                  <td>Total</td>
                  <td width="25%"><%# Convert.ToDecimal(PrimaryTotal["Last30Days"]).ToString("c")%></td>
                  <td width="25%"><%# Convert.ToDecimal(PrimaryTotal["Lifetime"]).ToString("c")%></td>
                  <td width="25%"><%# Convert.ToDecimal(PrimaryTotal["Commission"]).ToString("c")%> E-Cigs Dollars</td>
                </tr>
            </FooterTemplate>
        </asp:Repeater>
        <asp:PlaceHolder runat="server" ID="phNoPrimarySales" Visible="false">
            <tr class="white">
              <td colspan="4">No orders found</td>
            </tr>
        </asp:PlaceHolder>
      </table>
    </div>
    <h3>Indirect Referrals</h3>
    <p>These are referrals generated by your referrals. Please note these are estimates as returns are removed from your commissions.</p>
    <div class="blueborder">
      <table width="100%" border="0" cellspacing="0">
        <tr class="header">
          <td>Name</td>
          <td>Orders In Past 30 Days</td>
          <td>Lifetime Orders</td>
          <td>My Commission (8%)</td>
        </tr>
        <asp:Repeater runat="server" ID="rSecondarySales">
            <ItemTemplate>
                <tr class="white">
                  <td width="25%"><%# (Container.DataItem as System.Collections.Generic.IDictionary<string, object>)["Name"] %></td>
                  <td width="25%"><%# Convert.ToDecimal((Container.DataItem as System.Collections.Generic.IDictionary<string, object>)["Last30Days"]).ToString("c") %></td>
                  <td width="25%"><%# Convert.ToDecimal((Container.DataItem as System.Collections.Generic.IDictionary<string, object>)["Lifetime"]).ToString("c") %></td>
                  <td width="25%"><%# Convert.ToDecimal((Container.DataItem as System.Collections.Generic.IDictionary<string, object>)["Commission"]).ToString("c")%> E-Cigs Dollars</td>
                </tr>
            </ItemTemplate>
            <AlternatingItemTemplate>
                <tr>
                  <td width="25%"><%# (Container.DataItem as System.Collections.Generic.IDictionary<string, object>)["Name"] %></td>
                  <td width="25%"><%# Convert.ToDecimal((Container.DataItem as System.Collections.Generic.IDictionary<string, object>)["Last30Days"]).ToString("c") %></td>
                  <td width="25%"><%# Convert.ToDecimal((Container.DataItem as System.Collections.Generic.IDictionary<string, object>)["Lifetime"]).ToString("c") %></td>
                  <td width="25%"><%# Convert.ToDecimal((Container.DataItem as System.Collections.Generic.IDictionary<string, object>)["Commission"]).ToString("c")%> E-Cigs Dollars</td>
                </tr>
            </AlternatingItemTemplate>
            <FooterTemplate>
                <tr class="total">
                  <td>Total</td>
                  <td width="25%"><%# Convert.ToDecimal(SecondaryTotal["Last30Days"]).ToString("c")%></td>
                  <td width="25%"><%# Convert.ToDecimal(SecondaryTotal["Lifetime"]).ToString("c")%></td>
                  <td width="25%"><%# Convert.ToDecimal(SecondaryTotal["Commission"]).ToString("c")%> E-Cigs Dollars</td>
                </tr>
            </FooterTemplate>
        </asp:Repeater>
        <asp:PlaceHolder runat="server" ID="phNoSecondarySales" Visible="false">
            <tr class="white">
              <td colspan="4">No orders found</td>
            </tr>
        </asp:PlaceHolder>
      </table>
    </div>
    
  </div>
</div>
<div style="clear:both;">
</div>
</asp:Content>
