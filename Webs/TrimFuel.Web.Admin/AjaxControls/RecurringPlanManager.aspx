<%@ Page EnableViewState="false" Language="C#" AutoEventWireup="true" CodeBehind="RecurringPlanManager.aspx.cs" Inherits="TrimFuel.Web.Admin.AjaxControls.RecurringPlanManager" %>
<%@ Register assembly="TrimFuel.Web.UI" namespace="TrimFuel.Web.UI.Specialized" tagprefix="cc1" %>
<%@ Register src="../Controls/SubscriptionControl/SubscriptionFilter.ascx" tagname="SubscriptionFilter" tagprefix="uc1" %>
<form id="form1" runat="server">
<uc1:SubscriptionFilter ID="SubscriptionFilter1" runat="server" />
</form>
<div id="recurring-plan-list-container" style="max-height:230px;overflow:auto;width:100%;"></div>
<div class="toolBox"><a href="javascript:editRecurringPlan()" class="addNewIcon">Add new subscription</a></div>
<div id="recurring-plan-edit-container"></div>
<script type="text/javascript">
    var recurringPlanControlId = '<%# SubscriptionFilter1.GenerateID %>';
    $(document).ready(function () {
        loadRecurringPlanList();
        $('#' + recurringPlanControlId + ' select[name=planFrequency]').change(function () {
            loadRecurringPlanList();
            hideRecurringPlan(null);
        });
    });
	
	function loadRecurringPlanList()
	{
	    var selectedProductID = $('#' + recurringPlanControlId + ' select[name=productGroup]').val();
	    var selectedGroupProductSKU = $('#' + recurringPlanControlId + ' select[name=product]').val();
	    var selectedPlanFrequencyID = $('#' + recurringPlanControlId + ' select[name=planFrequency]').val();
	    var selectedRecurringPlanID = $("#recurring-plan-edit-container #hdnRecurringPlanID").val();
		//if (!selectedRecurringPlanID)
		//{
		//	selectedRecurringPlanID = selectedPricingRecurringPlanID;
		//}
	    var url = "/dotNet/ajaxControls/RecurringPlanList.aspx?productID=" + selectedProductID + "&groupProductSKU=" + selectedGroupProductSKU + "&planFrequency=" + selectedPlanFrequencyID + "&selectedRecurringPlanID=" + selectedRecurringPlanID;
		inlineControl(url, "recurring-plan-list-container");
	}

	function deleteRecurringPlan(recurringPlanID) {
	    if (recurringPlanID) {
	        if (confirm("Are you sure you want to delete Plan #" + recurringPlanID + "?")) {
	            inlineControl("/dotNet/ajaxControls/RecurringPlanDelete.aspx?recurringPlanID=" + recurringPlanID, "recurring-plan-edit-container",
                    null, function () {
                        loadRecurringPlanList();
                    });
	        }
	    }
	}

	function hideRecurringPlan() {
	    $("#recurring-plan-edit-container").html("");
	    $("#recurring-plan-list-container tr.selected").removeClass("selected");
	}

	function editRecurringPlan(recurringPlanID, noLoadingImage) {
	    if (noLoadingImage == 'no') {
	    }
	    else {
	        $("#recurring-plan-edit-container").html("<img src=\"/images/loading2.gif\"/>");
	    }
	    var proposedProductID = $('#' + recurringPlanControlId + ' select[name=productGroup]').val();
		
		var url = "/dotNet/ajaxControls/RecurringPlan.aspx?recurringPlanID=";
		if (recurringPlanID) url += recurringPlanID;
		
		url += "&proposedProductID=" + proposedProductID;
		
		inlineControl(url, "recurring-plan-edit-container", function(){
			loadRecurringPlanList();
		});
		
		$("#recurring-plan-list-container tr.selected").removeClass("selected");
		if (recurringPlanID)
		{
			$("#recurring-plan-list-container #recurring-plan_" + recurringPlanID).addClass("selected");
		}
    }
    
    function editPlanCycle(recurringPlanID, recurringPlanCycleID) {
        popupControl2("plan-cycle-edit", "Edit Cycle", 470, 470, "/dotNet/ajaxControls/RecurringPlanCycleEdit.aspx?recurringPlanID=" + recurringPlanID + "&recurringPlanCycleID=" + recurringPlanCycleID, function() {
            loadRecurringPlanList();
            editRecurringPlan(recurringPlanID, 'no');
        });
    }	
</script>
