<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Subscription.ascx.cs" Inherits="TrimFuel.Web.Admin.Controls.SubscriptionControl.Subscription" %>
<%@ Register assembly="TrimFuel.Web.UI" namespace="TrimFuel.Web.UI.Specialized" tagprefix="cc1" %>
<%@ Register src="SubscriptionProduct.ascx" tagname="SubscriptionProduct" tagprefix="uc1" %>
<%@ Register src="SubscriptionProductGroup.ascx" tagname="SubscriptionProductGroup" tagprefix="uc2" %>
<%@ Register src="SubscriptionPlanFrequency.ascx" tagname="SubscriptionPlanFrequency" tagprefix="uc3" %>
<%@ Register src="SubscriptionList.ascx" tagname="SubscriptionList" tagprefix="uc4" %>
<style type="text/css">
    .dropdowntree table td {font-size:9px !important;}
    .dropdowntree dt a {width:300px;}
    .dropdowntree ul li a:hover table td{color:white;}
</style>
<tr><td>Product Group:</td><td>
    <select name="productGroup" style="width:300px;"><uc2:SubscriptionProductGroup ID="SubscriptionProductGroup1" runat="server" /></select>
</td></tr>
<tr><td>Product:</td><td>
    <select name="product" style="width:300px;"><uc1:SubscriptionProduct ID="SubscriptionProduct1" runat="server" /></select>
</td></tr>
<tr><td>Plan Frequency:</td><td>
    <select name="planFrequency"><uc3:SubscriptionPlanFrequency ID="SubscriptionPlanFrequency1" runat="server" /></select></td></tr>
<tr><td>Subscription:</td><td style="width:320px;">
    <div name="subscription" style="width:300px;">    
    <div name="options">
    <uc4:SubscriptionList ID="SubscriptionList1" runat="server" />
    </div>
    <asp:TextBox CssClass="hidden" style="display:none;" runat="server" ID="hdnRecurringPlanID" Value='<%# (RecurringPlan != null ? RecurringPlan.RecurringPlanID : null) %>' />
    </div>
</td></tr>


