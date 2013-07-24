<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProductLeadManager.aspx.cs" Inherits="TrimFuel.Web.Admin.AjaxControls.ProductLeadManager" EnableViewState="false" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="TrimFuel.Model.Views" %>
<%@ Import Namespace="TrimFuel.Model.Enums" %>
<script type="text/javascript">
    $(document).ready(function() {
        $("#lead-settings-tabs").tabs({
            selected: $("#<%# hdnSelectedTab.ClientID %>").val(),
            select: function(event, ui) {
                $("#<%# hdnSelectedTab.ClientID %>").val(ui.index);
            }
        });
    });
    function addRule(typeID) {
        $("#lead-settings-tabs #lead-rules-" + typeID + " #rule-prototype-" + typeID).clone().attr("id", "").appendTo($("#lead-settings-tabs #lead-rules-" + typeID)).show();
    }
    function removeFlow(el) {
        $(el).parent().remove();
        return false;
    }
    function addConfig() {
        $("#lead-settings-tabs #config-values #config-prototype").clone().attr("id", "").appendTo($("#config-values")).show();
    }
    function removeConfig(el) {
        if (confirm('Are you sure?')) {
            $(el).parent().remove();
        }
        return false;
    }
</script>
<form runat="server" ID="form1">
<asp:HiddenField runat="server" ID="hdnSelectedTab"></asp:HiddenField>
<div id="lead-settings-tabs">
    <ul>
        <li><a href="#lead-settings-tabs-0">All</a></li>
		<li><a href="#lead-settings-tabs-1">Abandons</a></li>
		<li><a href="#lead-settings-tabs-2">Order Confirmations</a></li>
		<li><a href="#lead-settings-tabs-3">Cancellations/Declines</a></li>
        <li><a href="#lead-settings-tabs-4">Configuration</a></li>
	</ul>
    <div id="lead-settings-tabs-0">
        <a href="javascript:addRule(0)" class="addNewIcon">Add Rule</a>
        <div class="space"></div>
        <div class="toolBox" id="lead-rules-0">
	        <div style="margin:5px;width:100%;display:none;" id="rule_prototype-0">
			    
			    <select name="leadPartnerID-0" style="width:270px;">
			        <asp:Literal Text='<%# ShowLeadPartnerOptions(null) %>'></asp:Literal>
			    </select>
			    <a href="#" onclick="return removeFlow(this);" class="removeIcon" style="float:right;">Remove</a>
	        </div>
	        <asp:Repeater runat="server" DataSource='<%# LeadRoutingRules.Where(i => i.LeadTypeID == LeadTypeEnum.All) %>'>
	            <ItemTemplate>
	                <div style="margin:5px;width:100%;">
			           
			            <select name="leadPartnerID-0" style="width:270px;">
			                <asp:Literal Text='<%# ShowLeadPartnerOptions(Eval("LeadPartnerID")) %>'></asp:Literal>			                
			            </select>
			            <a href="#" onclick="return removeFlow(this);" class="removeIcon" style="float:right;">Remove</a>
	                </div>
	            </ItemTemplate>
	        </asp:Repeater>
	    </div>
	    <div class="space"></div>
	    <div id="errorMsg" style="max-width:500px;"><asp:Literal runat="server" ID="lSaved0"></asp:Literal></div>
	    <asp:Button runat="server" ID="bSaveAll" OnClick="bSaveAll_Click" Text="Save"  style="float:right" />
    </div>
	<div id="lead-settings-tabs-1">
	    <a href="javascript:addRule(1)" class="addNewIcon">Add Rule</a>
	    <div class="space"></div>
        <div class="toolBox" id="lead-rules-1">
	        <div style="margin:5px;width:100%;display:none;" id="rule-prototype-1">
			    <input type="text" name="percentage-1" class="xnarrow" maxlength="3" value="0"/>%&nbsp;&nbsp;
			    <select name="leadPartnerID-1" style="width:270px;">
			        <asp:Literal Text='<%# ShowLeadPartnerOptions(null) %>'></asp:Literal>
			    </select>
			    <a href="#" onclick="return removeFlow(this);" class="removeIcon" style="float:right;">Remove</a>
	        </div>
	        <asp:Repeater runat="server" DataSource='<%# LeadRoutingRules.Where(i => i.LeadTypeID == LeadTypeEnum.Abandons) %>'>
	            <ItemTemplate>
	                <div style="margin:5px;width:100%;">
			            <input type="text" name="percentage-1" class="xnarrow" maxlength="3" value="<%# Eval("Percentage") %>"/>%&nbsp;&nbsp;
			            <select name="leadPartnerID-1" style="width:270px;">
			                <asp:Literal Text='<%# ShowLeadPartnerOptions(Eval("LeadPartnerID")) %>'></asp:Literal>			                
			            </select>
			            <a href="#" onclick="return removeFlow(this);" class="removeIcon" style="float:right;">Remove</a>
	                </div>
	            </ItemTemplate>
	        </asp:Repeater>
	    </div>
	    <div class="space"></div>
	    <div id="errorMsg" style="max-width:500px;"><asp:Literal runat="server" ID="lSaved1"></asp:Literal></div>
	    <asp:Button runat="server" ID="bSaveAbandons" OnClick="bSaveAbandons_Click" Text="Save"  style="float:right" />
	</div>
	<div id="lead-settings-tabs-2">
	    <a href="javascript:addRule(2)" class="addNewIcon">Add Rule</a>
	    <div class="space"></div>
        <div class="toolBox" id="lead-rules-2">
	        <div style="margin:5px;width:100%;display:none;" id="rule-prototype-2">
			    <input type="text" name="percentage-2" class="xnarrow" maxlength="3" value="0"/>%&nbsp;&nbsp;
			    <select name="leadPartnerID-2" style="width:270px;">
			        <asp:Literal Text='<%# ShowLeadPartnerOptions(null) %>'></asp:Literal>
			    </select>
			    <a href="#" onclick="return removeFlow(this);" class="removeIcon" style="float:right;">Remove</a>
	        </div>
	        <asp:Repeater runat="server" DataSource='<%# LeadRoutingRules.Where(i => i.LeadTypeID == LeadTypeEnum.OrderConfirmations) %>'>
	            <ItemTemplate>
	                <div style="margin:5px;width:100%;">
			            <input type="text" name="percentage-2" class="xnarrow" maxlength="3" value="<%# Eval("Percentage") %>"/>%&nbsp;&nbsp;
			            <select name="leadPartnerID-2" style="width:270px;">
			                <asp:Literal Text='<%# ShowLeadPartnerOptions(Eval("LeadPartnerID")) %>'></asp:Literal>			                
			            </select>
			            <a href="#" onclick="return removeFlow(this);" class="removeIcon" style="float:right;">Remove</a>
	                </div>
	            </ItemTemplate>
	        </asp:Repeater>
	    </div>
	    <div class="space"></div>
	    <div id="errorMsg" style="max-width:500px;"><asp:Literal runat="server" ID="lSaved2"></asp:Literal></div>
	    <asp:Button runat="server" ID="bSaveConfirms" OnClick="bSaveConfirms_Click" Text="Save"  style="float:right" />
	</div>
	<div id="lead-settings-tabs-3">
	    <a href="javascript:addRule(3)" class="addNewIcon">Add Rule</a>
	    <div class="space"></div>
        <div class="toolBox" id="lead-rules-3">
	        <div style="margin:5px;width:100%;display:none;" id="rule-prototype-3">
			    <input type="text" name="percentage-3" class="xnarrow" maxlength="3" value="0"/>%&nbsp;&nbsp;
			    <select name="leadPartnerID-3" style="width:270px;">
			        <asp:Literal Text='<%# ShowLeadPartnerOptions(null) %>'></asp:Literal>
			    </select>
			    <a href="#" onclick="return removeFlow(this);" class="removeIcon" style="float:right;">Remove</a>
	        </div>
	        <asp:Repeater runat="server" DataSource='<%# LeadRoutingRules.Where(i => i.LeadTypeID == LeadTypeEnum.CancellationsDeclines) %>'>
	            <ItemTemplate>
	                <div style="margin:5px;width:100%;">
			            <input type="text" name="percentage-3" class="xnarrow" maxlength="3" value="<%# Eval("Percentage") %>"/>%&nbsp;&nbsp;
			            <select name="leadPartnerID-3" style="width:270px;">
			                <asp:Literal Text='<%# ShowLeadPartnerOptions(Eval("LeadPartnerID")) %>'></asp:Literal>			                
			            </select>
			            <a href="#" onclick="return removeFlow(this);" class="removeIcon" style="float:right;">Remove</a>
	                </div>
	            </ItemTemplate>
	        </asp:Repeater>
	    </div>
	    <div class="space"></div>
	    <div id="errorMsg" style="max-width:500px;"><asp:Literal runat="server" ID="lSaved3"></asp:Literal></div>
	    <asp:Button runat="server" ID="bSaveDeclines" OnClick="bSaveDeclines_Click" Text="Save" style="float:right" />
	</div>
    <div id="lead-settings-tabs-4">
        <a href="javascript:addConfig()" class="addNewIcon">Add Configuration</a>
        <div class="space"></div>
        <div class="toolBox" id="config-values">
	        <div style="margin:5px;width:100%;display:none;" id="config-prototype">
			    <select name="cfgLeadPartnerID" style="width:170px;">
			        <asp:Literal Text='<%# ShowLeadPartnerOptions(null) %>'></asp:Literal>
			    </select>
                &nbsp;&nbsp;&nbsp;
                <select name="cfgLeadTypeID" style="width:170px;">
                    <asp:Literal Text='<%# ShowLeadTypeOptions(null) %>'></asp:Literal>
                </select>
                &nbsp;&nbsp;&nbsp;
                Key: <input type="text" name="cfgLeadConfigKey" maxlength="50" />&nbsp;&nbsp;&nbsp;
                Value: <input type="text" name="cfgLeadConfigValue" maxlength="250" />
			    <a href="#" onclick="return removeConfig(this);" class="removeIcon" style="float:right;">Remove</a>
	        </div>
	        <asp:Repeater runat="server" DataSource='<%# LeadConfigValues %>'>
	            <ItemTemplate>
	                <div style="margin:5px;width:100%;">
			            <select name="cfgLeadPartnerID" style="width:170px;">
			                <asp:Literal Text='<%# ShowLeadPartnerOptions(Eval("LeadPartnerID")) %>'></asp:Literal>
			            </select>
                        &nbsp;&nbsp;&nbsp;
                        <select name="cfgLeadTypeID" style="width:170px;">
                            <asp:Literal Text='<%# ShowLeadTypeOptions(Eval("LeadTypeID")) %>'></asp:Literal>
                        </select>
                        &nbsp;&nbsp;&nbsp;
                        Key: <input type="text" name="cfgLeadConfigKey" maxlength="50" value='<%# Eval("Key") %>' />&nbsp;&nbsp;&nbsp;
                        Value: <input type="text" name="cfgLeadConfigValue" maxlength="250" value='<%# Eval("Value") %>' />
			            <a href="#" onclick="return removeConfig(this);" class="removeIcon" style="float:right;">Remove</a>
	                </div>
	            </ItemTemplate>
	        </asp:Repeater>
	    </div>
	    <div class="space"></div>
	    <div id="errorMsg" style="max-width:500px;"><asp:Literal runat="server" ID="lSaved4"></asp:Literal></div>
	    <asp:Button runat="server" ID="bSaveConfig" OnClick="bSaveConfig_Click" Text="Save"  style="float:right" />
    </div>
</div>
</form>