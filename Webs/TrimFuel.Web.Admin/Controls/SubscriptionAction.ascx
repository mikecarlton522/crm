<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SubscriptionAction.ascx.cs" Inherits="TrimFuel.Web.Admin.Controls.SubscriptionAction" %>
<%@ Register Assembly="TrimFuel.Web.UI" Namespace="TrimFuel.Web.UI" TagPrefix="cc1" %>
<%@ Register src="SubscriptionActionFreeProduct.ascx" tagname="SubscriptionActionFreeProduct" tagprefix="uc1" %>
<%@ Register src="SubscriptionActionUpsell.ascx" tagname="SubscriptionActionUpsell" tagprefix="uc2" %>
<cc1:If ID="If1" runat="server">
    <uc2:SubscriptionActionUpsell ID="SubscriptionActionUpsell1" runat="server" />
</cc1:If>
<cc1:If ID="If2" runat="server">
    <uc1:SubscriptionActionFreeProduct ID="SubscriptionActionFreeProduct1" runat="server" />        
</cc1:If>
<cc1:If ID="If3" runat="server">
</cc1:If>