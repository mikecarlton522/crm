<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RefererSales.ascx.cs" Inherits="TrimFuel.Web.Admin.Controls.RefererSales" %>
<table class="process-offets" border="0" cellspacing="1" width="100%">
  <tr class="header">
    <td>Commission Period</td>
    <td>Released</td>
    <td>Primary Sales</td>
    <td>Secondary Sales</td>
    <td>Commission</td>
  </tr>
  <asp:Repeater runat="server" ID="rSalesCommissions">
    <ItemTemplate>
      <tr>
        <td><%# (Container.DataItem as System.Collections.Generic.IDictionary<string, object>)["Period"] %></td>
        <td><%# (Container.DataItem as System.Collections.Generic.IDictionary<string, object>)["Released"] %></td>
        <td><%# ((System.Collections.Generic.IList<TrimFuel.Model.Views.SaleView>)(Container.DataItem as System.Collections.Generic.IDictionary<string, object>)["PrimarySales"]).Count %></td>
        <td><%# ((System.Collections.Generic.IList<TrimFuel.Model.Views.SaleView>)(Container.DataItem as System.Collections.Generic.IDictionary<string, object>)["SecondarySales"]).Count %></td>
        <td><%# (Container.DataItem as System.Collections.Generic.IDictionary<string, object>)["Commission"] %></td>
      </tr>
    </ItemTemplate>
    <FooterTemplate>
      <tr class="total">
        <td>Total</td>
        <td>&nbsp;</td>
        <td><%# TotalPrimarySales %></td>
        <td><%# TotalSecondarySales %></td>
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
<div class="space">
</div>
<div class="module">
<h2>Previous Commission Use</h2>
<table>
    <tr class="header">
        <td>Date</td>
        <td>Amount</td>
        <td>Completed</td>
        <td>Use</td>
    </tr>
<asp:Repeater runat="server" ID="rCommissions">
<ItemTemplate>
    <tr>
        <td><%# Convert.ToDateTime(Eval("CreateDT")).ToString("MMMM d, yyyy") %></td>
        <td>
            <%# ConvertToEcigsDollars(Convert.ToDecimal(Eval("Amount"))).ToString("c") %> E-Cigs Dollars
            <%# (Convert.ToInt32(Eval("RefererCommissionTID")) == 2) ? string.Format("({0} USD)", Convert.ToDecimal(Eval("Amount")).ToString("c")) : ""%>
        </td>        
        <asp:PlaceHolder runat="server" ID="ifCompleted" Visible='<%# Convert.ToBoolean(Eval("Completed")) %>'>
        <td class="green">Yes</td>
        </asp:PlaceHolder>
        <asp:PlaceHolder runat="server" ID="ifNotCompleted" Visible='<%# !Convert.ToBoolean(Eval("Completed")) %>'>
        <td class="red">No</td>
        </asp:PlaceHolder>
        <td>
            <%# (Convert.ToInt32(Eval("RefererCommissionTID")) == 1) ? "Use as store credit at ecigsbrand.com." : "" %> 
            <%# (Convert.ToInt32(Eval("RefererCommissionTID")) == 2) ? "Convert to $USD and send cheque." : ""%> 
        </td>
    </tr>
</ItemTemplate>
</asp:Repeater>
<asp:PlaceHolder runat="server" ID="phNoCommissions" Visible="false">
    <tr>
        <td colspan="4">No records found</td>
    </tr>
</asp:PlaceHolder>
</table>
</div>
<div class="space">
</div>
<div class="module">
<h2>Available Commission</h2>
<%# AvailableCommission.ToString("c") %> E-Cigs Dollars
</div>


