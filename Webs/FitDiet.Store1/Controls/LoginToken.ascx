<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LoginToken.ascx.cs" Inherits="Fitdiet.Store1.Controls.LoginToken" %>
<div class="login" id="login-token">
<asp:PlaceHolder runat="server" ID="phLogin" Visible='<%# Referer == null %>'>
  <a id="A1" href="~/cp-login.aspx" runat="server">Referral Program Login</a>
</asp:PlaceHolder>  
<asp:PlaceHolder runat="server" ID="phLogout" Visible='<%# Referer != null %>'>  
  <a id="A2" href="~/cp-logout.aspx" runat="server">Signout as <%# (Referer != null) ? Referer.FullName : "" %></a>
</asp:PlaceHolder>
</div>