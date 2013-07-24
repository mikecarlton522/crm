<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SaleInfo.aspx.cs" Inherits="TrimFuel.Web.Admin.AjaxControls.SaleInfo" %>

<%@ Register Assembly="TrimFuel.Web.UI" Namespace="TrimFuel.Web.UI" TagPrefix="cc1" %>
<%@ Import Namespace="TrimFuel.Web.Admin.Logic" %>
<%@ Register Assembly="TrimFuel.Web.UI" Namespace="TrimFuel.Web.UI.Specialized" TagPrefix="cc2" %>
<%@ Register Assembly="TrimFuel.Web.Admin" Namespace="TrimFuel.Web.Admin.Logic" TagPrefix="cc3" %>
<form id="form1" runat="server">
<cc1:If ID="ifSaleExists" runat="server">
    <table class="process-offets sale-info" width="100%">
        <tr class="subheader">
            <td colspan="4">
                Purchase Information
            </td>
        </tr>
        <tr>
            <td class="label">
                Sale ID
            </td>
            <td>
                <%# Sale.SaleView.OrderSale.SaleID %>
            </td>
            <td class="label">
                Campaign
            </td>
            <td>
                <%# (Sale.OrderCampaign != null ? Sale.OrderCampaign.DisplayName : "Unknown") %>
            </td>
        </tr>
        <tr>
            <td class="label">
                Sale Date
            </td>
            <td>
                <%# Sale.SaleView.OrderSale.CreateDT %>
            </td>
            <td class="label">
                URL
            </td>
            <td>
                <%# Sale.SaleView.Order.Order.URL %>
            </td>
        </tr>
        <tr>
            <td class="label">
                Sale Type
            </td>
            <td>
                <%# OrderHelper.ShowSaleType(Sale.SaleView) %>
            </td>
            <td class="label">
                IP
            </td>
            <td>
                <%# Sale.SaleView.Order.Order.IP %>
                <%# (!string.IsNullOrEmpty(Sale.SaleView.Order.Order.OrderAuthor) ? " - " + Sale.SaleView.Order.Order.OrderAuthor : "")%>
            </td>
        </tr>
        <tr>
            <td class="label">
                Amount
            </td>
            <td>
                <%# OrderHelper.ShowSalePrice(Sale.SaleView) %>
            </td>
            <td class="label">
                Affiliate
            </td>
            <td>
                <%# Sale.SaleView.Order.Order.Affiliate %>
            </td>
        </tr>
        <tr>
            <td class="label">
                Product
            </td>
            <td>
                <%# Sale.OrderProduct.ProductName %>
            </td>
            <td class="label">
                SubAffiliate
            </td>
            <td>
                <%# Sale.SaleView.Order.Order.SubAffiliate %>
            </td>
        </tr>
        <tr>
            <td class="label">
                SKU
            </td>
            <td>
                <%# OrderHelper.ShowSaleSKUs(Sale.SaleView) %>
            </td>
            <td class="label">
                Fraud Score
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td class="label">
                Coupon
            </td>
            <td>
                <%# Sale.SaleView.Order.Order.CouponCode %>
            </td>
            <td class="label">
                <td>
                </td>
            </td>
        </tr>
        <tr class="subheader">
            <td colspan="4">
                Processing Information
            </td>
        </tr>
        <cc1:if id="ifChargeDoesntExist" runat="server" Condition='<%# Charge == null %>'>
            <tr>
                <td colspan="4">
                    No charge
                </td>
            </tr>
        </cc1:if>
        <cc1:if id="ifChargeExists" runat="server" Condition='<%# Charge != null %>'>
            <tr>
                <td class="label">
                    Charge Date
                </td>
                <td>
                    <%# Charge.ChargeHistory.ChargeDate %>
                </td>
                <td class="label">
                    Charge Amount
                </td>
                <td>
                    <%# OrderHelper.ShowChargeAmount(Charge) %>
                </td>
            </tr>
            <tr>
                <td class="label">
                    MID Name
                </td>
                <td>
                    <%# Charge.MIDName %>
                </td>
                <td class="label">
                    Transaction Code
                </td>
                <td>
                    <%# Charge.ChargeHistory.TransactionNumber %>
                </td>
            </tr>
            <tr>
                <td class="label">
                    MID
                </td>
                <td>
                    <%# Charge.ChargeHistory.ChildMID %>
                </td>
                <td class="label">
                    Auth Code
                </td>
                <td>
                    <%# Charge.ChargeHistory.AuthorizationCode %>
                </td>
            </tr>
            <tr>
                <td class="label">
                    Response
                </td>
                <td colspan="3" class="wrapword">
                    <%# HttpUtility.HtmlEncode(Charge.ChargeHistory.Response) %>
                </td>
            </tr>
            <tr>
                <td class="label">
                    Refunds
                </td>
                <td colspan="3">
                    <%# OrderHelper.ShowSaleRefunds(Sale.SaleRefundList) %>
                </td>
            </tr>
        </cc1:if>
        <tr class="subheader">
            <td colspan="4">
                Chargeback Information
            </td>
        </tr>
        <tr>
            <td class="label">
                Chargeback Status
            </td>
            <td>
                <cc2:ChargebackStatusDDL Style="width: 150px;" ID="ChargebackStatusDDL1" runat="server"
                    SelectedValue='<%# Sale.Chargeback != null && Sale.Chargeback.ChargebackStatusTID != null ? Sale.Chargeback.ChargebackStatusTID.ToString() : "" %>'>
                </cc2:ChargebackStatusDDL>
            </td>
            <td class="chargeback-dependent label">
                Reason Code
            </td>
            <td class="chargeback-dependent">
                <cc2:ChargebackReasonCodeDDL PaymentTypeID='<%#PaymentTypeID %>' Style="width: 150px;"
                    ID="ChargebackReasonCodeDDL1" runat="server" SelectedValue='<%# Sale.Chargeback != null && Sale.Chargeback.ChargebackReasonCodeID != null ? Sale.Chargeback.ChargebackReasonCodeID.ToString() : "" %>'>
                </cc2:ChargebackReasonCodeDDL>
            </td>
        </tr>
        <tr class="chargeback-dependent">
            <td class="label">
                Case #
            </td>
            <td>
                <asp:textbox runat="server" id="tbChargebackCase" text='<%# Sale.Chargeback != null ? Sale.Chargeback.CaseNumber : "" %>'></asp:textbox>
            </td>
            <td class="label">
                Post Date
            </td>
            <td>
                <asp:textbox runat="server" id="tbPostDate" cssclass="date" text='<%# Sale.Chargeback != null && Sale.Chargeback.PostDT != null ? Sale.Chargeback.PostDT.Value.ToString("d") : "" %>'></asp:textbox>
            </td>
        </tr>
        <tr class="chargeback-dependent">
            <td class="label">
                ARN #
            </td>
            <td>
                <asp:textbox runat="server" id="tbChargebackARN" text='<%# Sale.Chargeback != null ? Sale.Chargeback.ARN : "" %>'></asp:textbox>
            </td>
            <td class="label">
                Dispute Send Date
            </td>
            <td>
                <asp:textbox runat="server" id="tbDisputeDate" cssclass="date" text='<%# Sale.Chargeback != null && Sale.Chargeback.DisputeSentDT != null ? Sale.Chargeback.DisputeSentDT.Value.ToString("d") : "" %>'></asp:textbox>
            </td>
        </tr>
        <tr>
            <td colspan="4" align="right">
                <cc3:Error ID="Error1" Type="Error" runat="server">Unknown error occurred while updating data</cc3:Error>
                <cc3:Error ID="Error2" Type="Notification" runat="server">Chargeback data was successfully updated</cc3:Error>
                <asp:button runat="server" id="btnUpdateChargeback" text="Update chargeback info"
                    onclick="btnUpdateChargeback_Click" />
            </td>
        </tr>
    </table>
</cc1:If>
<cc1:If ID="ifSaleDoesntExist" runat="server">
    Sale was not found
</cc1:If>
</form>
<style type="text/css">
    tr.invisible, td.invisible
    {
        display: none;
    }
    .sale-info td
    {
        color: #555;
    }
    .sale-info td.label
    {
        color: black;
    }
    .sale-info tr.subheader td
    {
        color: black;
    }
</style>
<script type="text/javascript">
    function chargebackStatusChange() {
        var isChargeback = $("#<%# ChargebackStatusDDL1.ClientID %>").val() != "";
        if (!isChargeback)
            $(".chargeback-dependent").addClass("invisible");
        else
            $(".chargeback-dependent").removeClass("invisible");
    }
    $(document).ready(function () {
        $("#<%# ChargebackStatusDDL1.ClientID %>").change(function () { chargebackStatusChange() }).change();
    });
</script>
