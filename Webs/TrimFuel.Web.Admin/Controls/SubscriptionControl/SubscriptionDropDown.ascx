<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SubscriptionDropDown.ascx.cs" Inherits="TrimFuel.Web.Admin.Controls.SubscriptionControl.SubscriptionDropDown" %>
<%@ Register Assembly="TrimFuel.Web.UI" Namespace="TrimFuel.Web.UI" TagPrefix="cc1" %>
<%@ Register src="../RecurringPlanBriefView.ascx" tagname="RecurringPlanBriefView" tagprefix="uc1" %>
<dl class="dropdowntree">
    <dt>
        <a href="#">
            <cc1:If ID="If1" runat="server" Condition='<%# SelectedRecurringPlanID != null %>'>
            <uc1:RecurringPlanBriefView ID="RecurringPlanBriefView1" runat="server" RecurringPlan='<%# SelectedRecurringPlan %>' />
            </cc1:If>
            <cc1:If ID="If2" runat="server" Condition='<%# SelectedRecurringPlanID == null %>'>
            -- Select --
            </cc1:If>
            <span class="value"><%# SelectedRecurringPlanID %></span>
        </a>
    </dt>
    <dd><ul>
    <li><a href="#">-- Select --<span class="value"></span></a></li>
    <asp:Repeater runat="server" ID="rPlans">
        <ItemTemplate>
            <li><a href="#"><uc1:RecurringPlanBriefView ID="RecurringPlanBriefView1" runat="server" RecurringPlan='<%# Container.DataItem %>' /><div class="clear"></div><span class="value"><%# Eval("RecurringPlanID") %></span></a></li>
        </ItemTemplate>
    </asp:Repeater>
    </ul></dd>
</dl>
