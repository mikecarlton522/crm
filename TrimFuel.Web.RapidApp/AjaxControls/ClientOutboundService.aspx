<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ClientOutboundService.aspx.cs"
    Inherits="TrimFuel.Web.RapidApp.AjaxControls.ClientOutboundService" %>

<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>

<script type="text/javascript">
    $(document).ready(function() {
        $(".editMode").hide();
    })
    function editClick(el) {
        cancelEditClick();
        var tr = $(el).closest('tr');
        $(tr).find('.viewMode').hide();
        $(tr).find('.editMode').show();

        var prodID = $(tr).find('#hdnSelectedProductID').val();
        var key = $(tr).find('#lblKey').html();
        var value = $(tr).find('#lblValue').html();
        var tdArray = $(tr).find('td');
        var templateObj = $('#template')
        templateObj.find('#teditTbValue').clone().attr("name", "editTbValue").attr("id", "editTbValue").addClass("validate[required]").appendTo(tdArray[2]).val(value);
        templateObj.find('#teditdpdrProducts').clone().attr("name", "editdpdrProducts").attr("id", "editdpdrProducts").appendTo(tdArray[1]).val(prodID);
        templateObj.find('#teditdpdrPossibleKeys').clone().attr("name", "editdpdrPossibleKeys").attr("id", "editdpdrPossibleKeys").appendTo(tdArray[0]).val(key);

        $(tr).find('#hdnleadTypeID').attr("name", "lblLeadTypeID").attr("id", "lblLeadTypeID");
        $(tr).find('#hdnSelectedProductID').attr("name", "lblSelectedProductID").attr("id", "lblSelectedProductID");
        $(tr).find('#hdnKey').attr("name", "old_lblKey").attr("id", "old_lblKey");
    }
    function cancelEditClick() {
        $('.viewMode').show();
        $('.editMode').hide();
        $('#editTbValue').remove();
        $('#lblLeadTypeID').attr("name", "hdnleadTypeID").attr("id", "hdnleadTypeID");
        $('#lblSelectedProductID').attr("name", "hdnSelectedProductID").attr("id", "hdnSelectedProductID");
        $('#old_lblKey').attr("name", "hdnKey").attr("id", "hdnKey");
        $('#editdpdrProducts').remove();
        $('#editdpdrPossibleKeys').remove();
    }
    function addFlow(leadTypeID) {
        cancelFlow();
        var table = $('#lead_' + leadTypeID);
        $('#template').find('#addConfigRow').clone().insertAfter(table.find('tr')[0]);
        table.find('#hdnLeadTypeID').attr("name", "fLeadTypeID").attr("id", "fLeadTypeID").val(leadTypeID);
        table.find('#dpdrPossibleKeys').attr("name", "fKey").attr("id", "fKey");
        table.find('#tbValue').attr("name", "fValue").attr("id", "fValue");
        table.find('#dpdrProducts').attr("name", "fProductID").attr("id", "fProductID");
        table.find('#fValue').addClass("validate[required]");
        table.find('#addConfigRow').show();
    }
    function cancelFlow() {
        $(document).find('#addConfigRow').each(function(index) {
            if ($(this).css('display') != "none")
                $(this).remove();
        });
    }
</script>

<h1>
    <%# HeaderTitle %></h1>
<form id="form1" runat="server">
<asp:hiddenfield runat="server" id="hdnLeadPartnerID" value="<%# LeadPartnerID %>" />
<asp:hiddenfield runat="server" id="hdnLeadPartnerName" value="<%# LeadPartnerName %>" />
<asp:hiddenfield runat="server" id="hdnTPClientID" value="<%# TPClientID %>" />
<asp:hiddenfield runat="server" id="hdnLeadPartnerSettingID" value="<%# OutboundSettingsProp.LeadPartnerSettingID %>" />
<div id="template" style="display: none;">
    <asp:dropdownlist clientidmode="static" id="teditdpdrProducts" runat="server" datasource='<%# Products %>'
        datatextfield="ProductName" datavaluefield="ProductID" />
    <asp:dropdownlist clientidmode="static" id="teditdpdrPossibleKeys" runat="server"
        datasource='<%# PossibleKeys %>' />
    <asp:textbox id="teditTbValue" clientidmode="static" runat="server" />
    <asp:linkbutton runat="server" clientidmode="static" id="lbAddConfig" text="Save"
        cssclass="submit saveIcon" onclick="btnAddConfig_Click"></asp:linkbutton>
    <table>
        <tr id="addConfigRow" style="display: none;">
            <td>
                <input type="hidden" id="hdnLeadTypeID" name="hdnLeadTypeID" value='' />
                <asp:dropdownlist clientidmode="static" id="dpdrPossibleKeys" runat="server" datasource='<%# PossibleKeys %>' />
            </td>
            <td>
                <asp:dropdownlist clientidmode="static" id="dpdrProducts" runat="server" datasource='<%# Products %>'
                    datatextfield="ProductName" datavaluefield="ProductID" />
            </td>
            <td>
                <asp:textbox clientidmode="static" id="tbValue" runat="server" />
            </td>
            <td>
                <a href="javascript:void(0);" onclick="$('#template').find('#lbAddConfig').click();"
                    class="saveIcon">Save</a> <a href="javascript:void(0);" onclick="cancelFlow();" class="cancelIcon">
                        Cancel</a>
            </td>
        </tr>
    </table>
</div>
<asp:placeholder runat="server" visible='<%#(PossibleKeys.Count == 0) || (IsNew) ? false : true %>'>
<h1 class="margintop10">
    Configuration</h1>
<asp:repeater id="rLeadTypes" runat="server" datasource='<%#LeadTypes %>'>
        <ItemTemplate>
            <h4 class="margintop10">
                <%#Eval("DisplayName")%></h4>
            <div style="height: 5px;"></div>
            <a href="javascript:void(0)" onclick='addFlow(<%#Eval("LeadTypeID")%>);' style="font-size: 12px;"
                class="addNewIcon">Add</a>
            <div style="height: 5px;"></div>
            <table cellspacing="1" id="lead_<%#Eval("LeadTypeID")%>" class="rapidapp-alternate">
                <tr class="heder">
                    <td width="30%">
                        <strong>Parameter Name</strong>
                    </td>
                    <td>
                        <strong>Product</strong>
                    </td>
                    <td>
                        <strong>Value</strong>
                    </td>
                    <td>
                        <strong>Actions</strong>
                    </td>
                </tr>
                <asp:repeater runat="server" datasource='<%#ConfigValues.Where(u => u.LeadTypeID == Convert.ToInt32(Eval("LeadTypeID"))) %>' onitemcommand="rConfigValues_ItemCommand" >
                    <ItemTemplate>
                        <tr>
                            <td width="30%">
                                <input type="hidden" id="hdnKey" name="hdnKey" value='<%# Eval("Key") %>' />
                                <input type="hidden" id="hdnleadTypeID" name="hdnleadTypeID" value='<%# Eval("LeadTypeID")%>' />
                                <span id="lblKey" class="viewMode"><%# Eval("Key") %></span>
                            </td>
                            <td width="30%">
                                <input type="hidden" id="hdnSelectedProductID" name="hdnSelectedProductID" value='<%# Eval("ProductID")%>' />
                                <span class="viewMode"><%# Products.Where(u => u.ProductID == Convert.ToInt32(Eval("ProductID"))).SingleOrDefault().ProductName %></span>
                            </td>
                            <td>
                                <span id="lblValue" class="viewMode"><%# Eval("Value")%></span>
                            </td>
                            <td>
                                <a href="javascript:void(0);" ID="lbEdit" onclick="editClick(this)" class="editIcon viewMode" >Edit</a>
                                <asp:LinkButton runat="server" ID="lbRemove" Text='Remove' CssClass="confirm removeIcon viewMode" CommandName='remove' CommandArgument='<%#Eval("LeadTypeID") + "," + Eval("ProductID") + "," + Eval("Key") %>' ></asp:LinkButton>
                                <asp:LinkButton runat="server" ID="lbSave" Text="Save" CssClass="submit saveIcon editMode" onclick="btnEditConfig_Click" style="display:none;" ></asp:LinkButton>
                                <a href="javascript:void(0);" ID="lbCancel" onclick="cancelEditClick(this);" class="cancelIcon editMode" style="display:none;">Cancel</a>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:repeater>
            </table>
        </ItemTemplate>
    </asp:repeater>
</asp:placeholder>
<h1 class="margintop10">
    Fees</h1>
<table cellspacing="1" class="rapidapp-alternate" style="width: 60%;">
    <tr>
        <td width="50%">
            <strong>Fee Type</strong>
        </td>
        <td width="25%">
            <strong>Retail</strong>
        </td>
        <td width="25%">
            <strong>Cost</strong>
        </td>
    </tr>
    <tr>
        <td>
            Setup
        </td>
        <td>
            <asp:textbox id="tbSetupFeeRetail" cssclass="currency" text='<%# OutboundSettingsProp.SetupFeeRetail %>'
                runat="server" />
            <br />
            One Time
        </td>
        <td>
            <asp:textbox id="tbSetupFee" cssclass="currency" text='<%# OutboundSettingsProp.SetupFee %>'
                runat="server" />
            <br />
            One Time
        </td>
    </tr>
    <tr>
        <td>
            Per Hour
        </td>
        <td>
            <asp:textbox id="tbPerPourFeeRetail" cssclass="currency" text='<%# OutboundSettingsProp.PerPourFeeRetail %>'
                runat="server" />
        </td>
        <td>
            <asp:textbox id="tbPerPourFee" cssclass="currency" text='<%# OutboundSettingsProp.PerPourFee %>'
                runat="server" />
        </td>
    </tr>
    <tr>
        <td>
            Monthly
        </td>
        <td>
            <asp:textbox id="tbMonthlyFeeRetail" cssclass="currency" text='<%# OutboundSettingsProp.MonthlyFeeRetail %>'
                runat="server" />
        </td>
        <td>
            <asp:textbox id="tbMonthlyFee" cssclass="currency" text='<%# OutboundSettingsProp.MonthlyFee %>'
                runat="server" />
        </td>
    </tr>
</table>
<div id="bottom">
    <div class="left">
        <asp:placeholder runat="server" id="lSaved">
           Saved <%# DateTime.Now %> by <%# AdminMembership.CurrentAdmin.DisplayName %>
        </asp:placeholder>
    </div>
    <div class="right" onclick="setTimeout(function(){updateMenu();},1000);">
        <asp:button type="submit" name="button7" id="button7" runat="server" text="Save Changes"
            onclick="SaveChanges_Click" />
    </div>
</div>
</form>
