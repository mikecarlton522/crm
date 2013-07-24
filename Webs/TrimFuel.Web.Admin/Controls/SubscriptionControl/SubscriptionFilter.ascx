<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SubscriptionFilter.ascx.cs" Inherits="TrimFuel.Web.Admin.Controls.SubscriptionControl.SubscriptionFilter" %>
<%@ Register src="SubscriptionProduct.ascx" tagname="SubscriptionProduct" tagprefix="uc1" %>
<%@ Register src="SubscriptionProductGroup.ascx" tagname="SubscriptionProductGroup" tagprefix="uc2" %>
<%@ Register src="SubscriptionPlanFrequency.ascx" tagname="SubscriptionPlanFrequency" tagprefix="uc3" %>
<table style="background-color: #999; color: #fff;" width="100%" id="<%# GenerateID %>">
<tr><td>Product Group:</td><td>
    <select name="productGroup" style="width:300px;"><uc2:SubscriptionProductGroup ID="SubscriptionProductGroup1" runat="server" /></select>
</td>
<td>Product:</td><td>
    <select name="product" style="width:300px;"><uc1:SubscriptionProduct ID="SubscriptionProduct1" runat="server" /></select>
</td>
<td>Plan Frequency:</td><td>
    <select name="planFrequency"><uc3:SubscriptionPlanFrequency ID="SubscriptionPlanFrequency1" runat="server" /></select></td></tr>
</table>
<script type="text/javascript">
    $(document).ready(function () {
        obtainSubscriptionControl("<%# GenerateID %>");
    });
</script>