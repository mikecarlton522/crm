<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NewSubscriptionControl.ascx.cs" Inherits="TrimFuel.Web.Admin.Controls.SubscriptionControl.NewSubscriptionControl" %>
<%@ Register Assembly="TrimFuel.Web.UI" Namespace="TrimFuel.Web.UI.Specialized" TagPrefix="cc1" %>
<%@ Register src="Subscription.ascx" tagname="Subscription" tagprefix="uc1" %>
<table class="subscription-control editForm" id="<%# GenerateID %>">
    <uc1:Subscription ID="Subscription1" runat="server" />
    <tr><td>Trial:</td><td style="width:330px;">
        <cc1:ProductSKUDDL ID="ProductSKUDDL1" runat="server" CssClass=""></cc1:ProductSKUDDL>&nbsp;&nbsp;
        Qty: <asp:TextBox runat="server" ID="tbQty" CssClass="validate[custom[Numeric]]" style="width:20px;" MaxLength="1" Text="1"></asp:TextBox>&nbsp;&nbsp;
        Price: <asp:TextBox runat="server" ID="tbPrice" CssClass="validate[custom[Amount]]" style="width:40px;" MaxLength="6" Text="0.00"></asp:TextBox>&nbsp;&nbsp;
    </td></tr>
    <tr><td>Trial Period:</td><td>
        <asp:TextBox runat="server" ID="tbTrialInterim" Text="15" CssClass="validate[custom[Numeric]]" style="width:40px;"></asp:TextBox>
    </td></tr>
</table>
<script type="text/javascript">
    $(document).ready(function () {
        obtainSubscriptionControl("<%# GenerateID %>");
    });
</script>

