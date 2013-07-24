<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Controls/Admin.Master" CodeBehind="management_recurring_plan.aspx.cs" Inherits="TrimFuel.Web.Admin.management_recurring_plan" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphScript" runat="server">
<script language="javascript" type="text/javascript" src="/js/subscriptionControl.js"></script>
<script language="javascript" type="text/javascript">
    $(document).ready(function () {
        $("#tabs-managers").tabs({ cookie: true });
	    showSubscriptionManager();
	    showRecurringPlanManager();
	});

	function showSubscriptionManager() {
	    inlineControl("/controls/subscription_manager.asp", "subscription-manager-container");
	}
	
	function showRecurringPlanManager()
	{
	    inlineControl("/dotNet/ajaxControls/RecurringPlanManager.aspx", "recurring-plan-manager-container");
	}
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphStyle" runat="server">
<style type="text/css">
	#subscription-list-container {
		overflow: visible !important;
	}
	#recurring-plan-list-container {
		overflow: visible !important;
	}
	#recurring-plan-list-container table.recurring-plan tr td {
		font-size:9px !important;
		background-color:transparent !important;
	}
</style>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
<div id="tabs-managers" class="data">
    <ul>
        <li><a href="#manager-tab-1">Subscription Manager</a></li>
        <li><a href="#manager-tab-2">Subscription Manager (Advanced)</a></li>
    </ul>
    <div id="manager-tab-1">
        <div id="subscription-manager-container" class="data">
	        <img src="/images/loading2.gif"/>
        </div>
        <div class="space"></div>
    </div>
    <div id="manager-tab-2">
        <div id="recurring-plan-manager-container" class="data">
	        <img src="/images/loading2.gif"/>
        </div>
        <div class="space"></div>
    </div>
</div>
</asp:Content>