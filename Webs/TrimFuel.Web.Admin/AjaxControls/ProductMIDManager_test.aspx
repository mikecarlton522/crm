<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProductMIDManager_test.aspx.cs"
    Inherits="TrimFuel.Web.Admin.AjaxControls.ProductMIDManager_test" %>

<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<script type="text/javascript">
    $(document).ready(function () {
        $(".editMode").hide();
    })
    function editClick(el) {
        var tr = $(el).closest('tr');
        $(tr).find('.editMode').show();
        $(tr).find('.viewMode').hide();
    }
    function cancelEditClick(el) {
        var tr = $(el).closest('tr');
        $(tr).find('.editMode').hide();
        $(tr).find('.viewMode').show();
    }
</script>
<form id="formProductMIDList" runat="server">
<asp:hiddenfield id="hdnProductID" value='<%# ProductID %>' runat="server" />
<div class="module">
    <table border="0" style="border: 0;">
        <tr>
            <td>
                <strong>Currency</strong>
            </td>
            <td>
                <asp:dropdownlist id="dpCurrency" runat="server" datasource='<%# CurrencyList %>'
                    datatextfield="CurrencyName" datavaluefield="CurrencyID" selectedvalue='<%# CurrCurrency %>' />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <div style="float: right;">
                    <asp:button onclick="currency_Save" runat="server" text="Save" />
                </div>
            </td>
        </tr>
    </table>
</div>
<div class="space">
</div>
<div id="errorMsg">
    <asp:literal runat="server" id="CurrencyNote"></asp:literal>
</div>
<div class="clear">
</div>
<a href="javascript:void(0)" onclick="$('#addMIDRow').show();" style="font-size: 12px;"
    class="addNewIcon">Add</a>
<div style="height: 10px;">
</div>
<table cellspacing="1" class="process-offets sortable" style="margin-left: 0;">
    <tr class="header">
        <td>
            <strong>MID</strong>
        </td>
        <td>
            <strong>Load Balanced</strong>
        </td>
        <td>
            <strong>Queue Rebills</strong>
        </td>
        <td>
            <strong>Rollover MID</strong>
        </td>
        <td>
            <strong>MID status</strong>
        </td>
        <td>
            <strong>Actions</strong>
        </td>
    </tr>
    <asp:repeater id="rMIDProd" runat="server" onitemcommand="rMIDProd_ItemCommand">
        <ItemTemplate>
            <tr>
                <td>
                    <asp:Label id="lblMIDName" runat="server" Text='<%# MIDs.Where(u => u.AssertigyMIDID == Convert.ToInt32(Eval("AssertigyMIDID"))).SingleOrDefault().DisplayName%>' />
                </td>
                <td>
                    <asp:CheckBox class="viewMode" runat="server" enabled="false" checked='<%# Eval("UseForTrial") %>' />
                    <asp:CheckBox clientidmode="static" id="editcbUseForTrial" class="editMode" runat="server" checked='<%# Eval("UseForTrial") %>'/>
                </td>
                <td>
                    <asp:CheckBox class="viewMode" runat="server" enabled="false" checked='<%# Eval("QueueRebills") %>' />
                    <asp:CheckBox clientidmode="static" id="editcbQueueRebills" class="editMode" runat="server" checked='<%# Eval("QueueRebills") %>' />
                </td>
                <td>
                    <asp:Label class="viewMode" id="lblrMIDName" runat="server" Text='<%# MIDs.Where(u => u.AssertigyMIDID == Convert.ToInt32(Eval("RolloverAssertigyMIDID"))).Count() == 0 ? "-- Not Set --" : MIDs.Where(u => u.AssertigyMIDID == Convert.ToInt32(Eval("RolloverAssertigyMIDID"))).SingleOrDefault().DisplayName%>' />
                    <asp:dropdownlist clientidmode="static" class="editMode" id="editdpdrMIDs" runat="server" datasource='<%# MIDs %>' datatextfield="DisplayName"
                        datavaluefield="AssertigyMIDID" selectedvalue='<%# ActiveMIDs.Contains(Convert.ToInt32(Eval("RolloverAssertigyMIDID"))) ? Eval("RolloverAssertigyMIDID") : "0" %>' />
                </td>
                <td>                    
                    <div class="meter-wrap">
                        <div class="meter-value" style='background-color: <%# Math.Round((decimal)(ChargedHistoryEx.Where(ch=>ch.MerchantAccountID == Convert.ToInt32(Eval("AssertigyMIDID"))).Sum(ch => ch.Amount) / MIDs.SingleOrDefault(m=> m.AssertigyMIDID == Convert.ToInt32(Eval("AssertigyMIDID"))).MonthlyCap * 100), 1) <= 50 ?  "green" : (Math.Round((decimal)(ChargedHistoryEx.Where(ch=>ch.MerchantAccountID == Convert.ToInt32(Eval("AssertigyMIDID"))).Sum(ch => ch.Amount) / MIDs.SingleOrDefault(m=> m.AssertigyMIDID == Convert.ToInt32(Eval("AssertigyMIDID"))).MonthlyCap * 100), 1) <= 80 ? "orange" :"red") %>; width: <%# Math.Round((decimal)(ChargedHistoryEx.Where(ch=>ch.MerchantAccountID == Convert.ToInt32(Eval("AssertigyMIDID"))).Sum(ch => ch.Amount) / MIDs.SingleOrDefault(m=> m.AssertigyMIDID == Convert.ToInt32(Eval("AssertigyMIDID"))).MonthlyCap * 100), 1) %>%;'>
                            <div class="meter-text">
                                <asp:Label id="lblMIDStatus" runat="server" Text='<%# Math.Round((decimal)(ChargedHistoryEx.Where(ch=>ch.MerchantAccountID == Convert.ToInt32(Eval("AssertigyMIDID"))).Sum(ch => ch.Amount) / MIDs.SingleOrDefault(m=> m.AssertigyMIDID == Convert.ToInt32(Eval("AssertigyMIDID"))).MonthlyCap * 100), 1) +"%" %>%' />
                            </div>
                        </div>
                    </div>
                </td>
                <td>
                    <a href="javascript:void(0);" ID="lbEdit" onclick="editClick(this)" class="editIcon viewMode" >Edit</a>
                    <asp:LinkButton runat="server" ID="lbDelete" Text="Delete" CssClass="confirm removeIcon viewMode" CommandName="delete" CommandArgument='<%# Eval("MerchantAccountProductID") %>'></asp:LinkButton>
                    <asp:LinkButton runat="server" ID="lbSave" Text="Save" CssClass="saveIcon editMode" CommandName="save" CommandArgument='<%# Eval("MerchantAccountProductID") %>' style="display:none;"></asp:LinkButton>
                    <a href="javascript:void(0);" ID="lbCancel" onclick="cancelEditClick(this);" class="cancelIcon editMode" style="display:none;">Cancel</a>
                </td>
            </tr>
        </ItemTemplate>
    </asp:repeater>
    <tr id="addMIDRow" style="display: none;">
        <td>
            <asp:dropdownlist id="dpdMIDs" runat="server" datasource='<%# MIDsToAdd %>' datatextfield="DisplayName"
                datavaluefield="AssertigyMIDID" />
        </td>
        <td>
            <asp:checkbox id="cbUseForTrial" runat="server" checked="true" cssclass="confirm" />
        </td>
        <td>
            <asp:checkbox id="cbQueueRebills" runat="server" checked="false" cssclass="confirm" />
        </td>
        <td>
            <asp:dropdownlist id="dpdrMIDs" runat="server" datasource='<%# MIDs %>' datatextfield="DisplayName"
                datavaluefield="AssertigyMIDID" />
        </td>
        <td>
        </td>
        <td>
            <asp:linkbutton runat="server" id="lbAddMID" text="Save" cssclass="saveIcon" commandname="add"
                onclick="btnAddMID_Click"></asp:linkbutton>
            <a href="javascript:void(0);" onclick="$('#addMIDRow').hide();" class="cancelIcon">Cancel</a>
        </td>
    </tr>
</table>
<div class="space">
</div>
<div id="errorMsg">
    <asp:literal runat="server" id="Note"></asp:literal>
</div>
</form>
