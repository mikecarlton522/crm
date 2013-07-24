<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/Front.Master" AutoEventWireup="true" CodeBehind="order-confirmation.aspx.cs" Inherits="Fitdiet.Store1.order_confirmation" %>
<%@ Register TagPrefix="uc" TagName="Accessories" Src="~/Controls/Accessories.ascx" %>
<%@ Register TagPrefix="uc" TagName="ShoppingCartView" Src="~/Controls/ShoppingCartView.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">Order Confirmation</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
<div id="cart">
<asp:PlaceHolder runat="server" ID="phPageIsOutOfDate" Visible='<%# Order == null %>'>
  <div style="text-align:center; height:50px;" class="main_heading">
    Page is out of date!<br/>
  </div>
</asp:PlaceHolder>
<asp:PlaceHolder runat="server" ID="PlaceHolder1" Visible='<%# Order != null %>'>
  <div style="text-align:center; height:50px;" class="main_heading">
    Thank You For Your Order!
  </div>
  <uc:ShoppingCartView runat="server" ID="ShoppingCartView1" />
  <table width="900px" cellpadding="5" cellspacing="0" style="margin:10px auto 20px auto; text-align:left;" class="copy14grey">
    <tr>
      <td width="40%" class="main_heading"> Description</td>
      <td width="60%" class="header last"></td>
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
  </table>
  <table width="900px" cellpadding="5" cellspacing="0" style="text-align:left; margin:0 auto 20px auto;" class="copy14grey">
    <tr>
      <td width="40%" class="main_heading"> Shipping Information</td>
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
