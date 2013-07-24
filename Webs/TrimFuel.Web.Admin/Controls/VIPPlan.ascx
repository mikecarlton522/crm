<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VIPPlan.ascx.cs" Inherits="TrimFuel.Web.Admin.Controls.VIPPlan" %>
<%@ Register Assembly="TrimFuel.Web.UI" Namespace="TrimFuel.Web.UI" TagPrefix="cc1" %>
<%@ Register src="SubscriptionAction.ascx" tagname="SubscriptionAction" tagprefix="uc1" %>
<%@ Register src="SubscriptionPlanItemControl.ascx" tagname="SubscriptionPlanItemControl" tagprefix="uc2" %>
<cc1:If ID="If0" runat="server">
<table class="vip-plan" vip-plan-id="<%# (Plan != null) ? Plan.SubscriptionPlanID.ToString() : "" %>"><tr class="header"><td>
    <cc1:If ID="If1" runat="server">
        <%# Plan.SubscriptionPlanName %>
    </cc1:If>
    <cc1:If ID="If2" runat="server">
        <div class="edit"><label>Name:</label><asp:TextBox runat="server" ID="tbName" Text='<%# Plan.SubscriptionPlanName %>'></asp:TextBox></div>
    </cc1:If>
    <asp:LinkButton runat="server" ID="btnEdit" CssClass="editIcon"
            onclick="btnEdit_Click" Visible='<%# ViewMode == TrimFuel.Web.Admin.Controls.ViewModeEnum.View %>'>Edit</asp:LinkButton>
</td></tr><tr class="subheader"><td>
    Initial item
</td></tr><tr><td>
    <uc2:SubscriptionPlanItemControl ID="SubscriptionPlanItemControl1" runat="server" />
    <asp:LinkButton runat="server" ID="btnCancel" CssClass="cancelIcon" OnClientClick='<%# CancelValidationJavascript %>'
            onclick="btnCancel_Click" Visible='<%# ViewMode == TrimFuel.Web.Admin.Controls.ViewModeEnum.Edit %>'>Cancel</asp:LinkButton>
    <asp:LinkButton runat="server" ID="btnSave" CssClass="saveIcon" 
            onclick="btnSave_Click" Visible='<%# ViewMode == TrimFuel.Web.Admin.Controls.ViewModeEnum.Edit %>'>Save</asp:LinkButton>
</td></tr>
<cc1:If ID="If3" runat="server">
<tr class="subheader"><td>
    Recurring items
    <asp:LinkButton runat="server" ID="btnAddRecurring" CssClass="addNewIcon" onclick="btnAddRecurring_Click" 
        Visible='<%# ViewMode == TrimFuel.Web.Admin.Controls.ViewModeEnum.View %>' 
        OnClientClick='<%# CancelValidationJavascript %>'>Add</asp:LinkButton>
</td></tr><tr><td>
    <asp:Repeater runat="server" ID="rpRecurringItems">
        <ItemTemplate>
            <uc2:SubscriptionPlanItemControl ID="SubscriptionPlanItemControl1" runat="server" OnRequireData="RecurringItemControl_RequireData" ViewMode="View" />
            <cc1:If ID="IfView" runat="server">
                <!--
                <asp:LinkButton runat="server" ID="btnRemoveExisted" CssClass="removeIcon confirm"
                        onclick="btnRemoveExisted_Click">Remove</asp:LinkButton>
                -->
                <asp:LinkButton runat="server" ID="btnEditExisted" CssClass="editIcon" OnClientClick='<%# CancelValidationJavascript %>'
                        onclick="btnEditExisted_Click">Edit</asp:LinkButton>
            </cc1:If>
            <cc1:If ID="IfEdit" runat="server">
                <asp:LinkButton runat="server" ID="btnCancelExisted" CssClass="cancelIcon" OnClientClick='<%# CancelValidationJavascript %>'
                        onclick="btnCancelExisted_Click">Cancel</asp:LinkButton>
                <asp:LinkButton runat="server" ID="btnSaveExisted" CssClass="saveIcon" 
                        onclick="btnSaveExisted_Click">Save</asp:LinkButton>
            </cc1:If>
            <div class="space"></div>
        </ItemTemplate>
    </asp:Repeater>
    <cc1:If ID="If5" runat="server">
        <uc2:SubscriptionPlanItemControl ID="SubscriptionPlanItemControl2" runat="server" />
        <asp:LinkButton runat="server" ID="btnCancelNew" CssClass="cancelIcon" OnClientClick='<%# CancelValidationJavascript %>'
                onclick="btnCancelNew_Click">Cancel</asp:LinkButton>
        <asp:LinkButton runat="server" ID="btnSaveNew" CssClass="saveIcon" 
                onclick="btnSaveNew_Click">Save</asp:LinkButton>
    </cc1:If>
</td></tr>
</cc1:If>
<tr><td>
</td></tr><tr><td>
</table>
</cc1:If>