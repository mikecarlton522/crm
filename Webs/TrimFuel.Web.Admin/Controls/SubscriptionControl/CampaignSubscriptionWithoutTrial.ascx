<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CampaignSubscriptionWithoutTrial.ascx.cs" Inherits="TrimFuel.Web.Admin.Controls.SubscriptionControl.CampaignSubscriptionWithoutTrial" %>
<%@ Register Assembly="TrimFuel.Web.UI" Namespace="TrimFuel.Web.UI.Specialized" TagPrefix="cc1" %>
<%@ Register src="Subscription.ascx" tagname="Subscription" tagprefix="uc1" %>
<div class="module" style="border:0 !important;">
<h2>Subscription</h2>
<table class="subscription-control editForm" id="<%# GenerateID %>">
    <uc1:Subscription ID="Subscription1" runat="server" />
</table>
</div>
<div class="clear"></div>
<script type="text/javascript">
    $(document).ready(function () {
        obtainSubscriptionControl("<%# GenerateID %>");
    });
</script>