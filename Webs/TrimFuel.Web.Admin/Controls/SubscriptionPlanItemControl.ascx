<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SubscriptionPlanItemControl.ascx.cs" Inherits="TrimFuel.Web.Admin.Controls.SubscriptionPlanItemControl" %>
<%@ Register Assembly="TrimFuel.Web.UI" Namespace="TrimFuel.Web.UI" TagPrefix="cc1" %>
<%@ Register src="~/Controls/SubscriptionAction.ascx" tagname="SubscriptionAction" tagprefix="uc1" %>
<div class="plan-item">
    <uc1:SubscriptionAction ID="SubscriptionAction1" runat="server" />
    <div class="interim">
    <cc1:If ID="If1" runat="server">
        <%# PlanItem.Interim %> days interim
    </cc1:If>
    <cc1:If ID="If2" runat="server">
        <div class="edit"><label>Interim:</label><asp:TextBox runat="server" ID="tbInterim" Text='<%# PlanItem.Interim %>' CssClass="validate[custom[Numeric]]"></asp:TextBox></div>        
    </cc1:If>
    </div>
</div>
