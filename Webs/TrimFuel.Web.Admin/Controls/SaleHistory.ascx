<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SaleHistory.ascx.cs" Inherits="TrimFuel.Web.Admin.Controls.SaleHistory" %>
<%@ Register assembly="TrimFuel.Web.UI" namespace="TrimFuel.Web.UI" tagprefix="cc1" %>
<%@ Import Namespace="TrimFuel.Web.Admin.Logic" %>
<table class="process-offets" style="width:100%;" border="0" cellspacing="1">
    <tr class="header">
        <!--
        <td>Order ID</td>
        -->
        <td>Sale ID</td>
        <td>Date</td>
        <td>Type</td>
        <td>Charged</td>
        <td>Refunds</td>
        <!--
        <td>Price</td>
        -->
        <td>Description</td>
        <td>Submitted to Ship</td>
        <td>Tracking #</td>
        <td>Returned</td>
        <td>Gift Certificate</td>
        <td colspan="4">Action</td>
    </tr>
    <asp:Repeater runat="server" ID="rSplittedOrders">
    <ItemTemplate>
        <asp:Repeater runat="server" ID="rInvoices" DataSource='<%# Eval("InvoiceList") %>'>
        <ItemTemplate>
            <asp:Repeater runat="server" ID="rSales" DataSource='<%# Eval("SaleList") %>'>
                <ItemTemplate>
                <tr>
                <!--
                <cc1:If ID="If1" runat="server" Condition='<%# Container.ItemIndex == 0 && ((RepeaterItem)Container.Parent.Parent).ItemIndex == 0 %>'>
                <td rowspan='<%# DataBinder.Eval(Container.Parent.Parent.Parent.Parent, "DataItem.SaleList.Count") %>'><%# Eval("Order.Order.OrderID") %></td>
                </cc1:If>                                
                -->
                <td><a class="showPopupIcon" onclick='if (showOrderSaleInfo) showOrderSaleInfo(<%# Eval("OrderSale.SaleID") %>);' href="javascript:void(0)"><%# Eval("OrderSale.SaleID") %></a></td>
                <td><%# Eval("OrderSale.CreateDT")%></td>                
                <td><%# OrderHelper.ShowSaleType(Container.DataItem) %></td>
                <!--
                <cc1:If ID="If2" runat="server" Condition='<%# Container.ItemIndex == 0 %>'>                
                <td rowspan='<%# Eval("Invoice.SaleList.Count") %>'><%# OrderHelper.ShowInvoiceAmount(Eval("Invoice")) %></td>
                </cc1:If>
                -->
                <td nowrap><%# TrimFuel.Web.Admin.Logic.OrderHelper.ShowSalePrice(Container.DataItem) %></td>
                <td><%# ShowSaleRefunds(Convert.ToInt64(Eval("OrderSale.SaleID")))%></td>
                <td><%# OrderHelper.ShowSaleDescription(Container.DataItem) %></td>
                <td><%# ShowShipmentLink(ShowSubmittedShipments(Container.DataItem as TrimFuel.Model.Views.OrderSaleView), Convert.ToInt64(Eval("OrderSale.SaleID"))) %></td>
                <td><%# ShowShipmentLink(ShowShippedShipments(Container.DataItem as TrimFuel.Model.Views.OrderSaleView), Convert.ToInt64(Eval("OrderSale.SaleID"))) %>&nbsp;&nbsp;&nbsp;<a href='javascript:void(0)' class='editIcon' onclick='if (showSaleShipments) showSaleShipments(<%# Eval("OrderSale.SaleID") %>)'>Edit</a></td>
                <td><%# ShowShipmentLink(ShowReturnedShipments(Container.DataItem as TrimFuel.Model.Views.OrderSaleView), Convert.ToInt64(Eval("OrderSale.SaleID")))%></td>
                <td><%# ShowGiftCertificates(Convert.ToInt64(Eval("OrderSale.SaleID")))%></td>
			    <td width="60">
                    <cc1:If runat="server" Condition='<%# IsRefundAvailable(Container.DataItem as TrimFuel.Model.Views.OrderSaleView) %>'>
                        <a href="javascript:void(0)" class='refundIcon' onclick="showRefundOrderSale(<%# Convert.ToInt64(Eval("OrderSale.SaleID")) %>, this);">Refund</a>
                    </cc1:If>
                    <cc1:If runat="server" Condition='<%# !IsRefundAvailable(Container.DataItem as TrimFuel.Model.Views.OrderSaleView) %>'>
                        <span class='disabled-link refundIcon'>Refund</span>
                    </cc1:If>
			    </td>
                <td width="100">
                    <cc1:If runat="server" Condition='<%# IsGiftAvailable %>'>
                        <a href="javascript:void(0)" class='editIcon' onclick="showEditSaleGiftCertificate(<%# Convert.ToInt64(Eval("OrderSale.SaleID")) %>, this);">Gift Certificate</a>
                    </cc1:If>
                    <cc1:If runat="server" Condition='<%# !IsGiftAvailable %>'>
                        <span class='disabled-link editIcon'>Gift Certificate</span>
                    </cc1:If>
                </td>
                <td width="60">
                    <cc1:If runat="server" Condition='<%# IsReturnAvailable(Container.DataItem as TrimFuel.Model.Views.OrderSaleView) %>'>
                        <a href="javascript:void(0)" class='returnIcon' onclick="showReturnOrderSale(<%# Convert.ToInt64(Eval("OrderSale.SaleID")) %>, this);">Return</a>
                    </cc1:If>
                    <cc1:If runat="server" Condition='<%# !IsReturnAvailable(Container.DataItem as TrimFuel.Model.Views.OrderSaleView) %>'>
                        <span class='disabled-link returnIcon'>Return</span>
                    </cc1:If>
                </td>
                <td>
                    <cc1:If runat="server" Condition='<%# IsReturnAvailable(Container.DataItem as TrimFuel.Model.Views.OrderSaleView) %>'>
                        <a href="javascript:void(0)" onclick="showOrderSaleReturnProcessing(<%# Convert.ToInt64(Eval("OrderSale.SaleID")) %>, this);">Add a Return Action</a>
                    </cc1:If>
                    <cc1:If runat="server" Condition='<%# !IsReturnAvailable(Container.DataItem as TrimFuel.Model.Views.OrderSaleView) %>'>
                        <span class='disabled-link'>Add a Return Action</span>
                    </cc1:If>
                </td>
                </tr>
                </ItemTemplate>
            </asp:Repeater>            
        </ItemTemplate>
        </asp:Repeater>
    </ItemTemplate>
    </asp:Repeater>
</table>
<script type="text/javascript">
    function showSaleShipments(saleId) {
        popupControl2("popup-sale-shipper-response-" + saleId, "Sale #" + saleId, 420, 300, "dotNet/ajaxControls/SaleShipments.aspx?saleId=" + saleId, function () {
            initSalesHistory();
        });
        return false;
    }
    function showOrderSaleInfo(saleId) {
        popupControl2("popup-sale-info-" + saleId, "Sale #" + saleId + " info", 670, 550, "dotNet/ajaxControls/SaleInfo.aspx?saleId=" + saleId);
        return false;
    }
    function showReturnOrderSale(saleId) {
        popupControl2("popup-sale-return-info-" + saleId, "Return Sale #" + saleId + " info", 420, 250, "dotNet/ajaxControls/SaleReturn.aspx?saleId=" + saleId, function () {
            initBillingSubscriptions();
            initBillingNotes();
            initBillingHistory();
            initSalesHistory();
        });
        return false;
    }
    function showRefundOrderSale(saleId) {
        popupControl2("popup-sale-refund-" + saleId, "Refund Sale #" + saleId, 500, 300, "dotNet/ajaxControls/SaleRefund.aspx?saleId=" + saleId, function () {
            initBillingNotes();
            initBillingHistory();
            initSalesHistory();
        });
        return false;
    }
    function showOrderSaleReturnProcessing(saleId) {
        popupControl2("popup-return-action-info-" + saleId, "Return Action for Sale #" + saleId + "", 670, 350, "dotNet/ajaxControls/SaleReturnAction.aspx?saleId=" + saleId);
        return false;
    }
</script>