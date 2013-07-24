<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SubscriptionActionUpsell.ascx.cs" Inherits="TrimFuel.Web.Admin.Controls.SubscriptionActionUpsell" %>
<%@ Register Assembly="TrimFuel.Web.UI" Namespace="TrimFuel.Web.UI.Specialized" TagPrefix="cc2" %>
<%@ Register Assembly="TrimFuel.Web.UI" Namespace="TrimFuel.Web.UI" TagPrefix="cc1" %>
<div class="action-name">Upsell</div>
<div class="action-data">    
    <cc1:If ID="If1" runat="server">
        <%# TrimFuel.Business.Utils.Utility.FormatCurrency(PlanItemAction.SubscriptionActionAmount, "$") %>
        <br />
        <%# PlanItemAction.SubscriptionActionQuantity %> item(s) of
        <br />
        <%# PlanItemAction.SubscriptionActionProductName %>
    </cc1:If>
    <cc1:If ID="If2" runat="server">
        <div class="edit"><label>Price:</label><asp:TextBox runat="server" ID="tbAmount" Text='<%# PlanItemAction.SubscriptionActionAmount %>' CssClass="validate[custom[Amount]]"></asp:TextBox></div>
        <div class="edit"><label>Product:</label><cc2:ProductCodeDDL ID="ddlProductCode" runat="server" CssClass="validate[required]" SelectedValue='<%# PlanItemAction.SubscriptionActionProductCode %>'></cc2:ProductCodeDDL></div>
        <div class="edit"><label>Quantity:</label><asp:TextBox runat="server" ID="tbQuantity" Text='<%# PlanItemAction.SubscriptionActionQuantity %>' CssClass="validate[custom[Quantity]]"></asp:TextBox></div>
    </cc1:If>
</div>