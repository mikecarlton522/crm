<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RecurringPlanCycle.ascx.cs" Inherits="TrimFuel.Web.Admin.Controls.RecurringPlanCycle_" %>
<%@ Register Assembly="TrimFuel.Web.UI" Namespace="TrimFuel.Web.UI" TagPrefix="cc1" %>
<cc1:If ID="If1" runat="server">
    <div class="plan-cycle-description">
        <div class="plan-cycle-billing">
            <b>Billing:</b> <%# ShowBilling() %>
            <cc1:If ID="If3" runat="server" Condition="true">
            <br />&nbsp;&nbsp;<%# ShowAmount(PlanCycle.Constraint.Amount)%>  
            </cc1:If>
        </div>
        <div class="plan-cycle-shipments">
            <b>Shipping:</b> <%# ShowShipping() %>
            <asp:Repeater runat="server" ID="rShipments" DataSource='<%# PlanCycle.ShipmentList%>'>
                <ItemTemplate>
                <br />&nbsp;&nbsp;<%# Eval("Quantity") %>x <%# Eval("ProductSKU") %>
                </ItemTemplate>                
            </asp:Repeater>
        </div>
    </div>
    <div class="plan-cycle-interim <%# (LastOne ? PlanCycle.Cycle.Recurring.Value ? "recurring" : "last" : "follow") %>">
    <div>
    <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible="<%# PlanCycle.Cycle.Recurring.Value || !LastOne %>">
        <%# PlanCycle.Cycle.Interim %> days
    </asp:PlaceHolder>        
    <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible="<%# !PlanCycle.Cycle.Recurring.Value && LastOne %>">
        End Plan
    </asp:PlaceHolder>        
    </div>
    </div>
</cc1:If>
<cc1:If ID="If2" runat="server">
</cc1:If>
