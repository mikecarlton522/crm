<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SaleRefund.aspx.cs" Inherits="TrimFuel.Web.Admin.AjaxControls.SaleRefund" %>
<%@ Import Namespace="TrimFuel.Web.Admin.Logic" %>
<%@ Register assembly="TrimFuel.Web.UI" namespace="TrimFuel.Web.UI" tagprefix="cc1" %>
<%@ Register assembly="TrimFuel.Web.Admin" namespace="TrimFuel.Web.Admin.Logic" tagprefix="cc3" %>
<form id="form1" runat="server">
<cc1:if id="ifSaleExists" runat="server">
<table class="process-offets sale-info" width="100%">
    <tr class="subheader"><td colspan="4">Transaction Information</td></tr>
    <cc1:if id="ifChargeDoesntExist" runat="server" Condition='<%# Charge == null %>'>
    <tr>
        <td colspan="4">No charge</td>
    </tr>
    </cc1:if>
    <cc1:if id="ifChargeExists" runat="server" Condition='<%# Charge != null %>'>
    <tr>
        <td class="label">Charge Date</td><td><%# Charge.ChargeHistory.ChargeDate %></td>
        <td class="label">MID Name</td><td><%# Charge.MIDName %></td>
    </tr>
    <tr>
        <td class="label">Charge Amount</td><td><%# OrderHelper.ShowChargeAmount(Charge) %></td>
        <td class="label">MID</td><td><%# Charge.ChargeHistory.ChildMID %></td>
    </tr>
    <tr>
        <td class="label">Transaction Code</td><td><%# Charge.ChargeHistory.TransactionNumber %></td>
        <td class="label"></td><td></td>
    </tr>
    <tr>
        <td class="label">Previous Refunds</td><td colspan="3"><%# OrderHelper.ShowSaleRefunds(RefundList) %></td>
    </tr>
    <tr class="subheader"><td colspan="4">Refund Transaction</td></tr>
    <tr>
        <td class="label">Refund Amount</td><td><asp:TextBox runat="server" CssClass="validate[custom[Amount]]" ID="tbRefundAmount" Text='<%# Sale.SaleView.OrderSale.ChargedAmount %>'></asp:TextBox></td>
        <td class="label"></td><td></td>        
    </tr>
    <tr>
        <td colspan="4" align="right">
            <asp:Button runat="server" ID="btnRefund" Text="Refund Now" OnClick="btnRefund_Click"/>
            <cc3:Error ID="Error1" Type="Error" runat="server"></cc3:Error>
        </td>
    </tr>
    </cc1:if>
</table>
</cc1:if>
<cc1:if id="ifSaleDoesntExist" runat="server">
    Sale was not found
</cc1:if>
</form>
