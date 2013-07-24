<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SubscriptionControl.ascx.cs" Inherits="TrimFuel.Web.Admin.Controls.SubscriptionControl.SubscriptionControl" %>
<%@ Register Assembly="TrimFuel.Web.UI" Namespace="TrimFuel.Web.UI.Specialized" TagPrefix="cc1" %>
<%@ Register src="Subscription.ascx" tagname="Subscription" tagprefix="uc1" %>
<div class="module">
<h2>Subscription</h2>
<table class="subscription-control editForm" id="<%# GenerateID %>">
    <uc1:Subscription ID="Subscription1" runat="server" />
    <asp:PlaceHolder runat="server" ID="phDiscount">
    <tr><td>Discount:</td><td><input type="text" id="discountValue" runat="server" disabled style="width:50px; text-align:right;" /><br/>Amount for charge = <asp:Label runat="server" ID="lblChargeAmount" /></td></tr>
    </asp:PlaceHolder>
    <tr><td>Status:</td><td><cc1:RecurringPlanStatusDDL ID="ddlRecurringStatus" runat="server"></cc1:RecurringPlanStatusDDL></td></tr>
    <tr><td>Next Bill Date:</td><td><asp:TextBox runat="server" ID="tbNextBillDate" CssClass="date"></asp:TextBox></td></tr>
    <tr><td></td><td><asp:Button runat="server" ID="btnUpdate" Text="Update" onclick="btnUpdate_Click" /></td></tr>
</table>
<div id="bs-plan-container-<%# GenerateID %>">
</div>
<asp:PlaceHolder runat="server" ID="phError">
<div id="errorMsg"><asp:Literal runat="server" ID="lError"></asp:Literal></div>
</asp:PlaceHolder>
</div>
<script type="text/javascript">
    $(document).ready(function () {
        obtainSubscriptionControl("<%# GenerateID %>");
        inlineEditForm("bs-plan-<%# GenerateID %>", "editForms/loyalty_plan_order.asp?orPlanId=<%# OrderRecurringPlanID %>&status=<%# RecurringStatus %>", "bs-plan-container-<%# GenerateID %>");	
    });
</script>
