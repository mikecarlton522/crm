<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SubscriptionData.aspx.cs" Inherits="TrimFuel.Web.Admin.AjaxControls.SubscriptionData" %>
<%@ Register src="../Controls/SubscriptionControl/SubscriptionProduct.ascx" tagname="SubscriptionProduct" tagprefix="uc1" %>
<%@ Register src="../Controls/SubscriptionControl/SubscriptionPlanFrequency.ascx" tagname="SubscriptionPlanFrequency" tagprefix="uc2" %>
<%@ Register src="../Controls/SubscriptionControl/SubscriptionList.ascx" tagname="SubscriptionList" tagprefix="uc3" %>
<%@ Register assembly="TrimFuel.Web.UI" namespace="TrimFuel.Web.UI" tagprefix="cc1" %>
<cc1:If ID="If1" runat="server" Condition='<%# Data == "product" %>'><uc1:SubscriptionProduct ID="SubscriptionProduct1" runat="server" ProductID='<%# ProductID %>' /></cc1:If>
<cc1:If ID="If2" runat="server" Condition='<%# Data == "planFrequency" %>'><uc2:SubscriptionPlanFrequency ID="SubscriptionPlanFrequency1" runat="server" ProductID='<%# ProductID %>' GroupProductSKU='<%# GroupProductSKU %>' /></cc1:If>
<cc1:If ID="If3" runat="server" Condition='<%# Data == "subscription" %>'><uc3:SubscriptionList ID="SubscriptionList1" runat="server" ProductID='<%# ProductID %>' GroupProductSKU='<%# GroupProductSKU %>' PlanFrequency=<%# PlanFrequency %> /></cc1:If>
