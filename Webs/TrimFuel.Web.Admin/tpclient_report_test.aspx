<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/Admin.Master" AutoEventWireup="true" CodeBehind="tpclient_report_test.aspx.cs" Inherits="TrimFuel.Web.Admin.tpclient_report_test" %>
<%@ Register Src="Controls/DateFilter.ascx" TagName="DateFilter" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphScript" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            initMultilevelTables();
        });

        function initMultilevelTables() {
            $("table.multilevel").each(function () {
                var table = this;
                $(table).find("tr.master").click(function () {
                    multilevelTableToggleDetail(table, this);
                });
            });
        }

        function multilevelTableToggleDetail(tableEl, masterEl) {
            var masterId = $(masterEl).attr("master-id");
            //alert(masterId);
            //Recursive hide children
            $(tableEl).find("tr.detail[detail-id=" + masterId + "]").filter(".master").filter(":visible").each(function () {
                multilevelTableHideDetail(tableEl, this)
            });
            $(tableEl).find("tr.detail[detail-id=" + masterId + "]").toggle();
        }

        function multilevelTableHideDetail(tableEl, masterEl) {
            var masterId = $(masterEl).attr("master-id");
            //alert(masterId);
            //Recursive hide children
            $(tableEl).find("tr.detail[detail-id=" + masterId + "]").filter(".master").filter(":visible").each(function () {
                multilevelTableHideDetail(tableEl, this)
            });
            $(tableEl).find("tr.detail[detail-id=" + masterId + "]").hide();
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphStyle" runat="server">
    <style type="text/css">
        tr.level0 td
        {
            padding: 5px;
            background-color: #e4e4e4;
            font-weight: bold;
        }
        tr.level1 td
        {
            background-color: #ecf0f7;
        }
        tr.detail
        {
            display: none;
        }
        tr.level0 td
        {
        }
        tr.level1 td
        {
        }
        tr.level2 td
        {
        }
    </style>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <form runat="server" id="frmMain">
    <div id="toggle" class="section">
        <a href="#">
            <h1>
                Quick View</h1>
        </a>
        <uc1:DateFilter ID="DateFilter1" runat="server" />
    </div>
    <div id="buttons">
        <asp:Button runat="server" ID="btnGo" Text="Generate Report" OnClick="btnGo_Click" />
    </div>
    <div class="data">
        <table class="multilevel sortable" border="0" cellspacing="1" width="100%">
            <tr class="header">
                <td>
                </td>
                <td colspan="9" align="center">
                    Fulfillment
                </td>
                <td colspan="8" align="center">
                    Gateway
                </td>
                <td colspan="5" align="center">
                    Call Center
                </td>
                <td>
                </td>
            </tr>
            <tr class="header">
                <td>
                    Client/ Product
                </td>
                <td>
                    Fee/ shipment
                </td>
                <td>
                    Total shipment fees
                </td>
                <td>
                    Fee/ shipment SKU
                </td>
                <td>
                    Fee/ Setup
                </td>
                <td>
                    Total shipment SKU fees
                </td>
                <td>
                    Fee/ Returns
                </td>
                <td>
                    Total returns fees
                </td>
                <td>
                    Fee/ Kitting & Assembly
                </td>
                <td>
                    Total kitting & assembly fees
                </td>
                <td>
                    Fee/ Transaction
                </td>
                <td>
                    Total transaction fees
                </td>
                <td>
                    Fee/ Chargeback
                </td>
                <td>
                    Total chargeback fees
                </td>
                <td>
                    Representation Fee/Chargeback
                </td>
                <td>
                    Total chargeback representation fees
                </td>
                <td>
                    Discount Rate
                </td>
                <td>
                    Total discounts
                </td>
                <td>
                    Fee/Call Center Setup
                </td>
                <td>
                    Fee/Call Center Hour
                </td>
                <td>
                    Total hour fees
                </td>
                <td>
                    Fee/Call Center Monthly
                </td>
                <td>
                    Total monthly fees
                </td>
                <td>
                    Customer total revenue
                </td>
            </tr>
            <asp:Literal ID="ltOutput" runat="server" />
            <tr class="total">
                <td>
                    Total
                </td>
                <td>
                </td>
                <td>
                    <%# Convert.ToDecimal(Total["TotalShipmentFees"]).ToString("c")%>
                </td>
                <td>
                </td>
                <td>
                    <%# Convert.ToDecimal(Total["TotalShipmentSKUFees"]).ToString("c")%>
                </td>
                <td>
                    <%# Convert.ToDecimal(Total["TotalSetupFee"]).ToString("c")%>
                </td>
                <td>
                </td>
                <td>
                    <%# Convert.ToDecimal(Total["TotalReturnsFees"]).ToString("c")%>
                </td>
                <td>
                </td>
                <td>
                    <%# Convert.ToDecimal(Total["TotalKittingFees"]).ToString("c")%>
                </td>
                <td>
                </td>
                <td>
                    <%# Convert.ToDecimal(Total["TotalTransactionFees"]).ToString("c")%>
                </td>
                <td>
                </td>
                <td>
                    <%# Convert.ToDecimal(Total["TotalChargebackFees"]).ToString("c")%>
                </td>
                <td>
                </td>
                <td>
                    <%# Convert.ToDecimal(Total["TotalChargebackRepresentationFees"]).ToString("c")%>
                </td>
                <td>
                </td>
                <td>
                    <%# Convert.ToDecimal(Total["TotalDiscounts"]).ToString("c")%>
                </td>
                <td>
                    <%# Convert.ToDecimal(Total["TotalCallCenterSetupFee"]).ToString("c")%>
                </td>
                <td>
                </td>
                <td>
                    <%# Convert.ToDecimal(Total["TotalCallCenterHourFees"]).ToString("c")%>
                </td>
                <td>
                </td>
                <td>
                    <%# Convert.ToDecimal(Total["TotalCallCenterMonthlyFees"]).ToString("c")%>
                </td>
                <td>
                    <%# Convert.ToDecimal(Total["TotalRevenue"]).ToString("c")%>
                </td>
            </tr>
        </table>
    </div>
    </form>
</asp:Content>
