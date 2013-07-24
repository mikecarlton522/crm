<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/Front.Master" AutoEventWireup="true" CodeBehind="cp-commission.aspx.cs" Inherits="Fitdiet.Store1.account.cp_commission" %>
<%@ Register src="../Controls/MenuControlPanel.ascx" tagname="MenuControlPanel" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">E-Cigs: My Commission</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphScript" runat="server">
<link rel="stylesheet" type="text/css" href="../css/control-panel.css" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
<form runat="server" ID="form1">
<div id="content">
  <div class="left">
    <h1>Customer &amp; Referral Control Panel</h1>
    <uc1:MenuControlPanel ID="MenuControlPanel1" runat="server" />
    <h2>My Commission Manager</h2>
    <h3>Available Commission</h3>
    <p>The following amount is your earnings through the E-Cigs Brand Referral Program and available to you immediately. You can use E-Cigs Dollars in our store, or you can convert it into real United States Dollars ($USD) and have a cheque sent to you. A minimum of $200 E-Cigs Dollars is required to convert to $USD.</p>
    <div style="border:1px solid #C3D9FF; width:100%;">
      <table width="100%" border="0" cellspacing="0">
        <tr class="header">
          <td>Available Earnings</td>
          <td>Actions</td>
        </tr>
        <tr class="white">
          <td width="34%" class="total"><%# AvailableCommission.ToString("c") %> E-Cigs Dollars</td>
          <td width="66%">
            <asp:Button runat="server" ID="bUseInStore" Text="Use In Store" 
                  onclick="bUseInStore_Click" />
          </td>
        </tr>
        <tr class="white">
          <td width="34%" class="total"><%# ConvertToUSD(AvailableCommission).ToString("c") %> US Dollars</td>
          <td width="66%">
            <asp:Button runat="server" ID="bConvertToCash" 
                  Text="Convert to US Dollars &amp; Send Me A Cheque" 
                  onclick="bConvertToCash_Click" />
            <br/>Minimum of <%# TrimFuel.Business.RefererService.REFERER_COMMISSION_MIN_FOR_CASH.ToString("c") %> US Dollars Required
          </td>
        </tr>
      </table>
    </div>
    <div class="left">
      <h3>Previous Commission Use</h3>
      <p>How you used your previous earnings.</p>
      <div class="blueborder">
        <table width="100%" border="0" cellspacing="0">
          <tr class="header">
            <td width="34%">Date</td>
            <td width="33%">Details</td>
            <td width="33%">Amount</td>
          </tr>
          <asp:Repeater runat="server" ID="rCommissions">
            <ItemTemplate>
              <tr class="white">
                <td><%# Convert.ToDateTime(Eval("CreateDT")).ToString("MMMM d, yyyy") %></td>
                <td>
                    <%# (Convert.ToInt32(Eval("RefererCommissionTID")) == 1) ? "Used as store credit at ecigsbrand.com." : "" %> 
                    <%# (Convert.ToInt32(Eval("RefererCommissionTID")) == 2) ? "Converted to $USD and cheque sent." : ""%> 
                </td>
                <td>
                    <%# ConvertToEcigsDollars(Convert.ToDecimal(Eval("Amount"))).ToString("c") %> E-Cigs Dollars
                    <%# (Convert.ToInt32(Eval("RefererCommissionTID")) == 2) ? string.Format("({0} USD)", Convert.ToDecimal(Eval("Amount")).ToString("c")) : ""%>
                </td>
              </tr>
            </ItemTemplate>
            <AlternatingItemTemplate>
              <tr>
                <td><%# Convert.ToDateTime(Eval("CreateDT")).ToString("MMMM d, yyyy") %></td>
                <td>
                    <%# (Convert.ToInt32(Eval("RefererCommissionTID")) == 1) ? "Used as store credit at ecigsbrand.com." : "" %> 
                    <%# (Convert.ToInt32(Eval("RefererCommissionTID")) == 2) ? "Converted to $USD and cheque sent." : ""%> 
                </td>
                <td>
                    <%# ConvertToEcigsDollars(Convert.ToDecimal(Eval("Amount"))).ToString("c") %> E-Cigs Dollars
                    <%# (Convert.ToInt32(Eval("RefererCommissionTID")) == 2) ? string.Format("({0} USD)", Convert.ToDecimal(Eval("Amount")).ToString("c")) : ""%>
                </td>
              </tr>
            </AlternatingItemTemplate>
          </asp:Repeater>
          <asp:PlaceHolder runat="server" ID="phNoCommissions" Visible="false">
            <tr class="white">
              <td colspan="3">No records found</td>
            </tr>
          </asp:PlaceHolder>
        </table>
      </div>
      <h3>My Generated Commission </h3>
      <p>Cmmissions are calculated and released 45 days from the commission period to allow for product returns. Please note that current period earnings are estimates as returns are removed from your commissions.</p>
      <div class="blueborder">
        <table width="100%" border="0" cellspacing="0">
          <tr class="header">
            <td>Commission Period</td>
            <td>Released</td>
            <td>My Commission</td>
          </tr>
          <asp:Repeater runat="server" ID="rSalesCommissions">
            <ItemTemplate>
              <tr class="white">
                <td><%# (Container.DataItem as System.Collections.Generic.IDictionary<string, object>)["Period"] %></td>
                <td><%# (Container.DataItem as System.Collections.Generic.IDictionary<string, object>)["Released"] %></td>
                <td><%# (Container.DataItem as System.Collections.Generic.IDictionary<string, object>)["Commission"] %></td>
              </tr>
            </ItemTemplate>
            <AlternatingItemTemplate>
              <tr>
                <td><%# (Container.DataItem as System.Collections.Generic.IDictionary<string, object>)["Period"] %></td>
                <td><%# (Container.DataItem as System.Collections.Generic.IDictionary<string, object>)["Released"] %></td>
                <td><%# (Container.DataItem as System.Collections.Generic.IDictionary<string, object>)["Commission"] %></td>
              </tr>
            </AlternatingItemTemplate>
            <FooterTemplate>
              <tr class="total">
                <td>Total</td>
                <td>&nbsp;</td>
                <td><%# TotalSalesCommission.ToString("c") %> E-Cigs Dollars</td>
              </tr>
            </FooterTemplate>
          </asp:Repeater>
          <asp:PlaceHolder runat="server" ID="phNoSalesCommissions" Visible="false">
            <tr class="white">
              <td colspan="3">No records found</td>
            </tr>
          </asp:PlaceHolder>
        </table>
      </div>
    </div>
  </div>
</div>
<div style="clear:both;">
</div>
</form>
</asp:Content>
