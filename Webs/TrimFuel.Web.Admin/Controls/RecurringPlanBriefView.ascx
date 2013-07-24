<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RecurringPlanBriefView.ascx.cs" Inherits="TrimFuel.Web.Admin.Controls.RecurrinPlanBriefView" %>
<%@ Register Assembly="TrimFuel.Web.UI" Namespace="TrimFuel.Web.UI" TagPrefix="cc1" %>
<table class="editForm" border="0" cellpadding="0" cellspacing="2"><tr valign="top">    
<asp:Repeater runat="server" ID="rCycles">
    <HeaderTemplate>
        <td style="width:20px;padding:0px;" nowrap>#<%# RecurringPlan.RecurringPlanID %></td>
        <td style="padding:0px;border-left:1px solid grey;">&nbsp;&nbsp;</td>
    </HeaderTemplate>
    <ItemTemplate>        
        <td style="width:70px;padding:0px;" nowrap>
        <asp:Repeater runat="server" ID="rShipments" DataSource='<%# Eval("ShipmentList") %>'>
            <ItemTemplate>
                <%# Eval("Quantity") %>x <%# Eval("ProductSKU") %>
            </ItemTemplate>
            <SeparatorTemplate><br /></SeparatorTemplate>            
        </asp:Repeater>
        <asp:PlaceHolder runat="server" ID="phNoShipments" Visible='<%# Convert.ToInt32(Eval("ShipmentList.Count")) == 0 %>'>
            No Shipments            
        </asp:PlaceHolder>
        </td>
        <td style="width:50px;padding:0px;" nowrap>            
            <cc1:If ID="If1" runat="server" Condition='<%# Eval("Constraint") != null %>'>
                <%# ShowAmount((decimal?)Eval("Constraint.Amount"), RecurringPlan.ProductID)%>
            </cc1:If>
            <cc1:If ID="If2" runat="server" Condition='<%# Eval("Constraint") != null && Convert.ToInt32(Eval("Constraint.ChargeTypeID")) == TrimFuel.Model.Enums.ChargeTypeEnum.AuthOnly %>'>
                <br />auth
            </cc1:If>
            <cc1:If ID="If3" runat="server" Condition='<%# Eval("Constraint") == null %>'>
                <%# ShowAmount(0, RecurringPlan.ProductID)%>
            </cc1:If>           

            <div style='vertical-align:bottom;white-space:nowrap;'>
        <%# Eval("Cycle.Interim")%> days
        </div>
        </td>
    </ItemTemplate>
    <SeparatorTemplate><td style="padding:0px;border-left:1px solid grey;">&nbsp;&nbsp;</td></SeparatorTemplate>
</asp:Repeater>
</tr></table>