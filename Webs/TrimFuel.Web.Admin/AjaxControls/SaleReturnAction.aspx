<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SaleReturnAction.aspx.cs" Inherits="TrimFuel.Web.Admin.AjaxControls.SaleReturnAction" %>
<%@ Import Namespace="TrimFuel.Web.Admin.Logic" %>
<%@ Register assembly="TrimFuel.Web.UI" namespace="TrimFuel.Web.UI" tagprefix="cc1" %>
<%@ Register assembly="TrimFuel.Web.Admin" namespace="TrimFuel.Web.Admin.Logic" tagprefix="cc3" %>
<%@ Register src="../Controls/DropDownFreeItemOld.ascx" tagname="DropDownFreeItemOld" tagprefix="uc1" %>
<%@ Register src="../Controls/DropDownUpsell.ascx" tagname="DropDownUpsell" tagprefix="uc2" %>
<%@ Register src="../Controls/SubscriptionControl/Subscription.ascx" tagname="Subscription" tagprefix="uc3" %>
<%@ Register assembly="TrimFuel.Web.Admin" namespace="TrimFuel.Web.Admin.Logic" tagprefix="cc3" %>
<div class="module" style="width:96%">
<form id="form1" runat="server">
<cc1:if id="ifSaleExists" runat="server">
    <h2>Return Action</h2>
    <table class="editForm sale-info" width="100%" id="<%# GenerateID %>">
    <tr>
        <td class="label">Enable Custom Return Processing</td>
        <td><asp:CheckBox runat="server" ID="chbEnableAction"></asp:CheckBox></td>
    </tr>
    <tr class="action action-select">
        <td class="label">Upon product return, do the following actions:</td>
        <td><asp:DropDownList runat="server" ID="ddlAction">
                <asp:ListItem Text="-- Select --" Value="" />
                <asp:ListItem Text="Issue Refund" Value="1" />
                <asp:ListItem Text="Cancel Account" Value="2" />
                <asp:ListItem Text="Change Plan" Value="3" />
                <asp:ListItem Text="Ship Free Item" Value="4" />
                <asp:ListItem Text="Bill and Ship Item" Value="5" />
            </asp:DropDownList></td>
    </tr>
    <tr class="action action-1">
        <td class="label">Refund Amount</td>
        <td><asp:TextBox runat="server" ID="tbRefundAmount" CssClass="narrow"></asp:TextBox></td>
    </tr>
    <tr class="action action-4">
        <td class="label">New Shipment (free)</td>
        <td><uc1:DropDownFreeItemOld ID="ddlFreeItem" runat="server" /></td>
    </tr>
    <tr class="action action-5">
        <td class="label">Bill and Ship Item</td>
        <td><uc2:DropDownUpsell ID="ddlUpsellType" runat="server" /></td>
    </tr>
    <tr class="action action-4 action-5">
        <td class="label">Quantity</td>
        <td><asp:DropDownList runat="server" ID="ddlQuantity">
            <asp:ListItem Text="1" Value="1" />
            <asp:ListItem Text="2" Value="2" />
            <asp:ListItem Text="3" Value="3" />
            <asp:ListItem Text="4" Value="4" />
            <asp:ListItem Text="5" Value="5" />
            <asp:ListItem Text="6" Value="6" />
            <asp:ListItem Text="7" Value="7" />
            <asp:ListItem Text="8" Value="8" />
            <asp:ListItem Text="9" Value="9" />
            <asp:ListItem Text="10" Value="10" />
        </asp:DropDownList></td>
    </tr>
    <tbody class="action action-3">
    <uc3:Subscription ID="Subscription1" runat="server" />
    </tbody>
    <tr>
        <td></td>
        <td><asp:Button runat="server" ID="btnSave" Text="Save" OnClick="btnSave_Click" /></td>
    </tr>
    <tr>
        <td colspan="2">
            <cc3:Error ID="Error1" Type="Error" runat="server"></cc3:Error>
            <cc3:Error ID="Error2" Type="Notification" runat="server">Return Action successfully updated</cc3:Error>
        </td>
    </tr>
    </table>
</cc1:if>
<cc1:if id="ifSaleDoesntExist" runat="server">
    <h2>Sale was not found</h2>
</cc1:if>
</form>
</div>
<script type="text/javascript">
    function toggleActions() {
        var tableId = '#<%# GenerateID %>';
        var chbId = '#<%# chbEnableAction.ClientID %>';
        var actionId = '#<%# ddlAction.ClientID %>';
        $(tableId + " .action").hide();
        if ($(tableId + " " + chbId + ":checked").length > 0) {
            $(tableId + " .action-select").show();
            if ($(tableId + " " + actionId).val() != "") {
                $(tableId + " .action-" + $(tableId + " " + actionId).val()).show();
            }
        }
    }
    $(document).ready(function () {
        obtainSubscriptionControl("<%# GenerateID %>");
        toggleActions();
        var tableId = '#<%# GenerateID %>';
        var chbId = '#<%# chbEnableAction.ClientID %>';
        var actionId = '#<%# ddlAction.ClientID %>';
        $(tableId + " " + chbId).change(function () { toggleActions(); });
        $(tableId + " " + actionId).change(function () { toggleActions(); });
    });
</script>
<style type="text/css">
    td.label {width:130px;}
</style>
