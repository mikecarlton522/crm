<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrderRecurringPlans.aspx.cs" Inherits="TrimFuel.Web.Admin.AjaxControls.OrderRecurringPlans" %>
<asp:Repeater runat="server" ID="rPlans">
    <ItemTemplate><%# Eval("OrderRecurringPlanID") %></ItemTemplate>
    <SeparatorTemplate>,</SeparatorTemplate>
</asp:Repeater>