<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MenuControlPanel.ascx.cs" Inherits="Ecigsbrand.Store1.Controls.MenuControlPanel" %>
<table class="nav" cellpadding="0" cellspacing="5">
  <tr>
    <td<%# IsSelected("cp-home")%>><a href="cp-home.aspx">Home</a></td>
    <td<%# IsSelected("cp-account")%>><a href="cp-account.aspx">My Account Information</a></td>
    <td<%# IsSelected("cp-orders")%>><a href="cp-orders.aspx">My Orders</a></td>
    <td<%# IsSelected("cp-referrals")%>><a href="cp-referrals.aspx">My Referrals</a></td>
    <td<%# IsSelected("cp-commission")%>><a href="cp-commission.aspx">My Commission Manager</a></td>
  </tr>
</table>
