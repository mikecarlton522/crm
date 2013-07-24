<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/Front.Master" AutoEventWireup="true" CodeBehind="order-confirmation.aspx.cs" Inherits="Ecigsbrand.Store2.order_confirmation" %>
<%@ Register TagPrefix="uc" TagName="Accessories" Src="~/Controls/Accessories.ascx" %>
<%@ Register TagPrefix="uc" TagName="ShoppingCartView" Src="~/Controls/ShoppingCartView.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">E-Cigs: Electronic Cigarette: E-Cigarette Kits, Accessories & Nicotine Refill Cartridges</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
<div id="cart">
<asp:PlaceHolder runat="server" ID="phPageIsOutOfDate" Visible='<%# Order == null %>'>
  <div style="text-align:center; font-size:20px; font-weight:bold; margin:20px;">
    Page is out of date!
  </div>
</asp:PlaceHolder>
<asp:PlaceHolder runat="server" ID="PlaceHolder1" Visible='<%# Order != null %>'>
  <div style="text-align:center; font-size:20px; font-weight:bold; margin:20px;">
    Thank You For Your Order!
  </div>
  <uc:ShoppingCartView runat="server" ID="ShoppingCartView1" />
  <table width="100%" cellpadding="5" cellspacing="0" style="margin:10px auto 20px auto; font-size:12px; font-weight:normal; text-align:left;">
    <tr class="header">
      <td width="40%" class="header"> Order Details</td>
      <td width="60%" class="header"> Description</td>
    </tr>
    <asp:PlaceHolder runat="server" ID="phAssertigyMid" Visible='<%# (AssertgyMid != null) %>'>
    <tr>
      <td>This charge will appear as</td>
      <td class="last"><%# (AssertgyMid != null) ? AssertgyMid.DisplayName : string.Empty %></td>
    </tr>
    </asp:PlaceHolder>
    <tr>
      <td>Confirmation number(s)</td>
      <td class="last"><span style="color:red;"><%# CustomerReferenceNumbers %></span></td>
    </tr>
    <tr>
      <td>E-Cigarette Quick Start Guide</td>
      <td class="last"><a href="http://www.ecigsbrand.com/getting-started-faq.pdf">Download Now! (3 MB)</a></td>
    </tr>
  </table>
  <table width="100%" cellpadding="5" cellspacing="0" style="text-align:left; font-weight:normal; margin:0 auto 20px auto; font-size:12px;">
    <tr>
      <td width="40%" class="header"> Shipping Information</td>
      <td width="60%" class="header last"></td>
    </tr>
    <tr>
      <td>First name</td>
      <td class="last"><%# Registration.FirstName%></td>
    </tr>
    <tr>
      <td>Last name</td>
      <td class="last"><%# Registration.LastName%></td>
    </tr>
    <tr>
      <td>Address line one</td>
      <td class="last"><%# Registration.Address1%></td>
    </tr>
    <tr>
      <td>Address line two</td>
      <td class="last"><%# Registration.Address2%></td>
    </tr>
    <tr>
      <td>City</td>
      <td class="last"><%# Registration.City%></td>
    </tr>
    <tr>
      <td>Country</td>
      <td class="last"><%# RegistrationInfo.Country%></td>
    </tr>
    <tr>
      <td>State</td>
      <td class="last"><%# Registration.State%></td>
    </tr>
    <tr>
      <td>Zip code</td>
      <td class="last"><%# Registration.Zip%></td>
    </tr>
    <tr>
      <td>Phone number</td>
      <td class="last"><%# (RegistrationInfo.Country == TrimFuel.Business.RegistrationService.DEFAULT_COUNTRY) ? Registration.PhoneCnt.ToString() : Registration.Phone%></td>
    </tr>
    <tr>
      <td>Email address</td>
      <td class="last"><%# Registration.Email%></td>
    </tr>
  </table>
</asp:PlaceHolder>
</div>
</asp:Content>
