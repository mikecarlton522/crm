<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RecurringPlanCycleEdit.aspx.cs" Inherits="TrimFuel.Web.Admin.AjaxControls.RecurringPlanCycleEdit" %>
<%@ Register Assembly="TrimFuel.Web.UI" Namespace="TrimFuel.Web.UI" TagPrefix="cc1" %>
<cc1:If ID="If1" runat="server" Condition="<%# !IsRemoved %>">
<script type="text/javascript">
    var ddlBillingID = '<%# ddlChargeType.ClientID %>';
    var ddlShippingID = '<%# ddlShipping.ClientID %>';
    var ddlActionID = '<%# ddlAction.ClientID %>';

    function correctForm() {
        EnableControls(true, ".billing-dependent");
        EnableControls(true, ".shipping-dependent");
        EnableControls(true, ".action-dependent");
                    
        if ($("#billing-cycle-form #" + ddlBillingID).val() == "")
            EnableControls(false, ".billing-dependent");
        if ($("#billing-cycle-form #" + ddlShippingID).val() == "false")
            EnableControls(false, ".shipping-dependent");
        if ($("#billing-cycle-form #" + ddlActionID).val() == "false")
            EnableControls(false, ".action-dependent");
    }
    
    function EnableControls(enable, selector)
    {
        if (enable) {
            $("#billing-cycle-form " + selector).removeAttr('disabled');
            $("#billing-cycle-form a." + selector).show();
            $("#billing-cycle-form " + selector + ":not(.prototype)").removeClass('donotvalidate');
        } else {
            $("#billing-cycle-form " + selector).attr('disabled', 'disabled');
            $("#billing-cycle-form a." + selector).hide();
            $("#billing-cycle-form " + selector + ":not(.prototype)").addClass('donotvalidate');
        }
    }

    function CancelValidation(el) {
        $(el).addClass("confirm");
        //$("#billing-cycle-form input:not(.prototype)").addClass('donotvalidate');
        //$("#billing-cycle-form select:not(.prototype)").addClass('donotvalidate');
    }
    
    function addProduct()
    {
        var added = $("#billing-cycle-form #products #product-prototype").clone().attr("id", "").appendTo($("#billing-cycle-form #products")).show();
        setIDs(added);
        added.find(".donotvalidate").removeClass("donotvalidate");
        added.find(".prototype").removeClass("prototype");
    }

    function setIDs(selector) {
        $(selector).find("input[name|=quantity]").attr("id", "quantity" + randomString());
        $(selector).find("select[name|=inventory]").attr("id", "inventory" + randomString());
    }

    function removeProduct(el)
    {
	    $(el).parent().remove();
	    return false;
	}

	function randomString() {
	    var ABC, temp, randomID, a;
	    ABC = "0123456789";
	    temp = ""
	    for (a = 0; a < 6; a++) {
	        randomID = Math.random();
	        randomID = Math.floor(randomID * ABC.length);
	        temp += ABC.charAt(randomID);
	    }
	    return temp;
	}

	$(document).ready(function() {
	    setIDs("#billing-cycle-form");
	    correctForm();
	    $("#billing-cycle-form #" + ddlBillingID).change(function() {
	        correctForm();
	    });
	    $("#billing-cycle-form #" + ddlShippingID).change(function() {
	        correctForm();
	    });
	    $("#billing-cycle-form #" + ddlActionID).change(function() {
	        correctForm();
	    });
	});
</script>
<form id="form1" runat="server">
<asp:HiddenField runat="server" ID="hdnRecurringPlanCycleID" Value='<%# PlanCycle.Cycle.RecurringPlanCycleID %>'></asp:HiddenField>
<div class="module" style="width:420px;">
<table class="editForm" id="billing-cycle-form" width="100%">
    <tr class="subheader"><td style="width:70px;">Billing</td><td>
        <asp:DropDownList runat="server" ID="ddlChargeType">
            <asp:ListItem Text="None" Value=''></asp:ListItem>
            <asp:ListItem Text="Charge" Value='1'></asp:ListItem>
            <asp:ListItem Text="Authorize" Value='4'></asp:ListItem>
        </asp:DropDownList>
    </td></tr>
    <tr><td style="width:70px;">Amount</td><td>
        <asp:TextBox runat="server" ID="tbAmount" style="width: 50px;" CssClass="narrow billing-dependent validate[custom[Amount]]"></asp:TextBox>
    </td></tr>
    <tr class="subheader"><td style="width:70px;">Shipping</td><td>
        <asp:DropDownList runat="server" ID="ddlShipping" style="float:left;">
            <asp:ListItem Text="Yes" Value='true'></asp:ListItem>
            <asp:ListItem Text="No" Value='false'></asp:ListItem>
        </asp:DropDownList>
        <a href="javascript:addProduct()" class="addNewIcon shipping-dependent" style="float:right;">Add Product</a>
    </td></tr>
    <tr><td style="width:70px;">Products</td><td nowrap>
        <div id="products">
	        <div style="margin:5px;width:100%;display:none;" id="product-prototype">
		        <input type="text" name="quantity" id="quantity-prototype" style="width: 30px;" class="xnarrow shipping-dependent validate[custom[Numeric]] donotvalidate prototype" maxlength="2" value="1"/>&nbsp;
		        <select name="inventory" id="inventory-prototype" style="width:150px;" class="shipping-dependent validate[required] donotvalidate prototype">
			        <option value="">-- Select --</option>
			        <%# InventoryOptionList("") %>
		        </select>
		        <a href="#" onclick="return removeProduct(this);" class="removeIcon shipping-dependent">Remove</a>
	        </div>
	        <asp:Repeater runat="server" ID="rSelectedInventories" DataSource="<%# SelectedInventoryList %>">
	            <ItemTemplate>
		            <div style="margin:5px;width:100%;">
			            <input type="text" name="quantity" style="width: 30px;" class="xnarrow shipping-dependent validate[custom[Numeric]]" maxlength="2" value='<%# Eval("Value") %>'/>&nbsp;
			            <select name="inventory" style="width:150px;" class="shipping-dependent validate[required]">
				            <option value="">-- Select --</option>
				            <%# InventoryOptionList(Convert.ToString(Eval("Key"))) %>
			            </select>
			            <a href="#" onclick="return removeProduct(this);" class="removeIcon shipping-dependent">Remove</a>
		            </div>
	            </ItemTemplate>
	        </asp:Repeater>
	        <div class="clear"></div>
        </div>
    </td></tr>
    <tr class="subheader"><td style="width:70px;">Action</td><td>
        <asp:DropDownList runat="server" ID="ddlAction">
        </asp:DropDownList>
    </td></tr>
    <tr><td style="width:70px;">Next Cycle</td><td>
        <asp:DropDownList runat="server" ID="ddlNextCycle" CssClass="action-dependent">
        </asp:DropDownList>
    </td></tr>
    <tr><td style="width:70px;">Next Cycle Days</td><td>
        <asp:TextBox runat="server" ID="tbInterim" style="width: 50px;" CssClass="narrow action-dependent validate[custom[Numeric]]"></asp:TextBox>
    </td></tr>
    <tr><td colspan="2" align="right">
        <cc1:If ID="If3" runat="server" Condition="<%# PlanCycle.Cycle.RecurringPlanCycleID != null %>">
        <asp:Button runat="server" ID="btnRemove" Text="Remove" onclick="btnRemove_Click" OnClientClick='CancelValidation(this);' />
        </cc1:If>
        <asp:Button runat="server" ID="btnSave" Text="Save" onclick="btnSave_Click" />
    </td></tr>
</table>    
<div id="errorMsg" style="max-width:500px;">
    <asp:PlaceHolder runat="server" ID="phError">    
        <span class='small-alert'>Can't save Billing Cycle.</span>
    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" ID="phSuccess">
        <span>Billing Cycle was successfully saved.</span>
    </asp:PlaceHolder>        
</div>
</div>
</form>
<div class="space"></div>
</cc1:If>
<cc1:If ID="If2" runat="server" Condition="<%# IsRemoved %>">
<div id="errorMsg" style="max-width:500px;">
    <span>Billing Cycle was successfully deleted.</span>
</div>
</cc1:If>