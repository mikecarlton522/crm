<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/Admin.Master" AutoEventWireup="true"
    CodeBehind="reports_chargeback_performance.aspx.cs" Inherits="TrimFuel.Web.Admin.reports_chargeback_performance" %>

<%@ Register Src="Controls/DateFilter.ascx" TagName="DateFilter" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphScript" runat="server">

    <script type="text/javascript">
        $(document).ready(function() {
            $("#tabs").tabs({
                cookie: {
                    expires: 1
                }
            });
            initMultilevelTables();
        });

        function saveTabNumber() {
            $("#<%# hdnSelectedTab.ClientID %>").val($("#tabs").tabs('option', 'selected'));
        }

        function initMultilevelTables() {
            $("table.multilevel").each(function() {
                var table = this;
                $(table).find("tr.master").click(function() {
                    multilevelTableToggleDetail(table, this);
                });
            });
        }

        function multilevelTableToggleDetail(tableEl, masterEl) {
            var masterId = $(masterEl).attr("master-id");
            //alert(masterId);
            //Recursive hide children
            $(tableEl).find("tr.detail[detail-id=" + masterId + "]").filter(".master").filter(":visible").each(function() {
                multilevelTableHideDetail(tableEl, this)
            });
            $(tableEl).find("tr.detail[detail-id=" + masterId + "]").toggle();
        }

        function multilevelTableHideDetail(tableEl, masterEl) {
            var masterId = $(masterEl).attr("master-id");
            //alert(masterId);
            //Recursive hide children
            $(tableEl).find("tr.detail[detail-id=" + masterId + "]").filter(".master").filter(":visible").each(function() {
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
    <asp:HiddenField runat="server" ID="hdnSelectedTab" Value="0" />
    <div id="toggle" class="section">
        <a href="#">
            <h1>Quick View</h1>
        </a>
        <uc1:DateFilter ID="DateFilter1" runat="server" />
	    <a href="#">
	    <h1>Options</h1>
	    </a>
	    <div>
	        <div class="module">
	            <h2>Options</h2>    
	            <asp:DropDownList runat="server" ID="ddlType" OnDataBound="ddlType_DataBound" />
	           
	        </div>  
	         <div class="module">
	                <h2>Product Group</h2>
	                <asp:DropDownList runat="server" ID="PrdGroup" OnDataBound="PrdGroup_OnDataBound" />
	          </div>
	    </div>
    </div>
    <div id="buttons">
        <%--<asp:Button runat="server" ID="btnCSV" Text="Download CSV" OnClick="btnCSV_Click"
            OnClientClick="saveTabNumber();" />&nbsp;&nbsp;--%>
        <asp:Button runat="server" ID="btnGo" Text="Generate Report" OnClick="btnGo_Click"
            OnClientClick="saveTabNumber()" />
    </div>
    <div id="TheReport" runat="server">
    <div class="data" id="tabs">
        <ul>
            <li><a href="#tabs-1">By Company</a></li>
            <li><a href="#tabs-2">By SubAffiliate</a></li>
            <li><a href="#tabs-3">By Charge Type</a></li>
            <li><a href="#tabs-4">By Reason</a></li>
        </ul>
        <div id="tabs-1">
            <table class="process-offets multilevel sortable add-csv-export" border='0' cellspacing='1' width='100%'>
                <tr class="header">
                    <td>
                        DBA
                    </td>
                    <td>
                        FMA Number
                    </td>
                    <td>
                        CHID Value
                    </td>
                    <td>
                        TYP
                    </td>
                    <td>
                       Settlement Count
                    </td>
                    <td>
                        Chargeback Count
                    </td>
                    <td>
                       Chargeback %
                    </td>
                    <%--<td>
                        PROJ SETTCNT
                    </td>
                    <td>
                        PROJ CBCNT
                    </td>
                    <td>
                        PROJ CBPCT
                    </td>--%>
                </tr>
                <asp:Repeater runat="server" ID="rSales">
                    <ItemTemplate>
                        <tr>
                            <td>
                                <%# DataBinder.Eval(Container.DataItem, "BaseReportView.AssertigyDisplayName") %>
                            </td>
                            <td>
                                <%# DataBinder.Eval(Container.DataItem, "BaseReportView.AssertigyMID")%>
                            </td>
                            <td>
                                <%# DataBinder.Eval(Container.DataItem, "BaseReportView.NMICompanyName")%>
                            </td>
                            <td>
                                <%# PaymentTypeName((int?)DataBinder.Eval(Container.DataItem, "BaseReportView.PaymentTypeID"))%>
                            </td>
                            <td>
                                <%# DataBinder.Eval(Container.DataItem, "BaseReportView.SaleCount")%>
                            </td>
                            <td>
                                <%# DataBinder.Eval(Container.DataItem, "SaleChargebackCount")%>
                            </td>
                            <td>
                                <%# (((decimal)DataBinder.Eval(Container.DataItem, "SaleChargebackPercentage"))).ToString("0.00%")%>
                            </td>
                           <%-- <td>
                                <%# DataBinder.Eval(Container.DataItem, "ProjectedSaleCount")%>
                            </td>
                            <td>
                                <%# DataBinder.Eval(Container.DataItem, "ProjectedSaleChargebackCount")%>
                            </td>
                            <td>
                                <%# (((decimal)DataBinder.Eval(Container.DataItem, "ProjectedChargebackPercentage"))).ToString("0.00%")%>
                            </td>--%>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
                <tr class="total">
                    <td>
                        Total
                    </td>
                    <td>
                    </td>
                    <td>
                    </td>
                    <td>
                    </td>
                    <td>
                        <%# TotalSales["CmpSaleCount"]%>
                    </td>
                    <td>
                        <%# TotalSales["CmpSaleChargebackCount"]%>
                    </td>
                    <td>
                    </td>
                   <%-- <td>
                        <%# TotalSales["CmpProjCount"]%>
                    </td>
                    <td>
                        <%# TotalSales["CmpProjChargebackCount"]%>
                    </td>--%>
                    <td>
                    </td>
                </tr>
            </table>
            <div class="clear">
            </div>
        </div>
        <div id="tabs-2">
            <table class="process-offets multilevel sortable add-csv-export" border='0' cellspacing='1' width='100%'>
                <tr class="header">
                    <td>
                        Affiliate
                    </td>
                    <td>
                        TYP
                    </td>
                    <td>
                      Settlement Count
                    </td>
                    <td>
                        Chargeback Count
                    </td>
                    <td>
                        Chargeback %
                    </td>
                   <%-- <td>
                        PROJ SETTCNT
                    </td>
                    <td>
                        PROJ CBCNT
                    </td>
                    <td>
                        PROJ CBPCT
                    </td>--%>
                </tr>
                <asp:Repeater runat="server" ID="rSalesByAff">
                    <ItemTemplate>
                        <tr master-id='a_<%# DataBinder.Eval(Container.DataItem, "Key.BaseReportView.Affiliate")%>'
                            class="master level0">
                            <td>
                                <%# (!string.IsNullOrEmpty((string)DataBinder.Eval(Container.DataItem, "Key.BaseReportView.Affiliate"))) ? DataBinder.Eval(Container.DataItem, "Key.BaseReportView.Affiliate") : "-"%>
                            </td>
                            <td>
                            </td>
                            <td>
                                <%# DataBinder.Eval(Container.DataItem, "Key.BaseReportView.SaleCount")%>
                            </td>
                            <td>
                                <%# DataBinder.Eval(Container.DataItem, "Key.SaleChargebackCount")%>
                            </td>
                            <td>
                                <%# (((decimal)DataBinder.Eval(Container.DataItem, "Key.SaleChargebackPercentage"))).ToString("0.00%")%>
                            </td>
                          <%--  <td>
                                <%# DataBinder.Eval(Container.DataItem, "Key.ProjectedSaleCount")%>
                            </td>
                            <td>
                                <%# DataBinder.Eval(Container.DataItem, "Key.ProjectedSaleChargebackCount")%>
                            </td>
                            <td>
                                <%# (((decimal)DataBinder.Eval(Container.DataItem, "Key.ProjectedChargebackPercentage"))).ToString("0.00%")%>
                            </td>--%>
                        </tr>
                        <asp:Repeater runat="server" ID="rSalesBySub" DataSource='<%# Container.DataItem %>'>
                            <ItemTemplate>
                                <tr master-id='a_<%# DataBinder.Eval(Container.DataItem, "Key.BaseReportView.Affiliate")%>_s<%# DataBinder.Eval(Container.DataItem, "Key.BaseReportView.SubAffiliate")%>'
                                    detail-id='a_<%# DataBinder.Eval(Container.DataItem, "Key.BaseReportView.Affiliate")%>'
                                    class="master detail level1">
                                    <td>
                                        <%# (!string.IsNullOrEmpty((string)DataBinder.Eval(Container.DataItem, "Key.BaseReportView.SubAffiliate"))) ? DataBinder.Eval(Container.DataItem, "Key.BaseReportView.SubAffiliate") : "-"%>
                                    </td>
                                    <td>
                                    </td>
                                    <td>
                                        <%# DataBinder.Eval(Container.DataItem, "Key.BaseReportView.SaleCount")%>
                                    </td>
                                    <td>
                                        <%# DataBinder.Eval(Container.DataItem, "Key.SaleChargebackCount")%>
                                    </td>
                                    <td>
                                        <%# (((decimal)DataBinder.Eval(Container.DataItem, "Key.SaleChargebackPercentage"))).ToString("0.00%")%>
                                    </td>
                                 <%--   <td>
                                        <%# DataBinder.Eval(Container.DataItem, "Key.ProjectedSaleCount")%>
                                    </td>
                                    <td>
                                        <%# DataBinder.Eval(Container.DataItem, "Key.ProjectedSaleChargebackCount")%>
                                    </td>
                                    <td>
                                        <%# (((decimal)DataBinder.Eval(Container.DataItem, "Key.ProjectedChargebackPercentage"))).ToString("0.00%")%>
                                    </td>--%>
                                </tr>
                                <asp:Repeater runat="server" ID="rSalesByCC" DataSource='<%# Container.DataItem %>'>
                                    <ItemTemplate>
                                        <tr detail-id='a_<%# DataBinder.Eval(Container.DataItem, "Key.BaseReportView.Affiliate")%>_s<%# DataBinder.Eval(Container.DataItem, "Key.BaseReportView.SubAffiliate")%>'
                                            class="detail level2">
                                            <td>
                                            </td>
                                            <td>
                                                <%# PaymentTypeName((int?)DataBinder.Eval(Container.DataItem, "Key.BaseReportView.PaymentTypeID"))%>
                                            </td>
                                            <td>
                                                <%# DataBinder.Eval(Container.DataItem, "Key.BaseReportView.SaleCount")%>
                                            </td>
                                            <td>
                                                <%# DataBinder.Eval(Container.DataItem, "Key.SaleChargebackCount")%>
                                            </td>
                                            <td>
                                                <%# (((decimal)DataBinder.Eval(Container.DataItem, "Key.SaleChargebackPercentage"))).ToString("0.00%")%>
                                            </td>
                                          <%--  <td>
                                                <%# DataBinder.Eval(Container.DataItem, "Key.ProjectedSaleCount")%>
                                            </td>
                                            <td>
                                                <%# DataBinder.Eval(Container.DataItem, "Key.ProjectedSaleChargebackCount")%>
                                            </td>
                                            <td>
                                                <%# (((decimal)DataBinder.Eval(Container.DataItem, "Key.ProjectedChargebackPercentage"))).ToString("0.00%")%>
                                            </td>--%>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </ItemTemplate>
                        </asp:Repeater>
                    </ItemTemplate>
                </asp:Repeater>
                <tr class="total">
                    <td>
                        Total
                    </td>
                    <td>
                    </td>
                    <td>
                        <%# TotalSales["SaleCount"]%>
                    </td>
                    <td>
                        <%# TotalSales["SaleChargebackCount"]%>
                    </td>
                    <td>
                    </td>
          <%--          <td>
                        <%# TotalSales["ProjectedCount"]%>
                    </td>
                    <td>
                        <%# TotalSales["ProjectedChargebackCount"]%>
                    </td>--%>
                    <td>
                    </td>
                </tr>
            </table>
            <div class="clear">
            </div>
        </div>
        <div id="tabs-3">
            <table class="process-offets multilevel sortable add-csv-export" border='0' cellspacing='1' width='100%'>
                <tr class="header">
                    <td>
                        Charge Type
                    </td>
                    <td>
                        DBA
                    </td>
                    <td>
                        FMA Number
                    </td>
                    <td>
                        TYP
                    </td>
                    <td>
                      Settlement Count
                    </td>
                    <td>
                        Chargeback Count
                    </td>
                    <td>
                       Chargeback %
                    </td>
                   <%-- <td>
                        PROJ SETTCNT
                    </td>
                    <td>
                        PROJ CBCNT
                    </td>
                    <td>
                        PROJ CBPCT
                    </td>--%>
                </tr>
                <asp:Repeater runat="server" ID="rSalesByType">
                    <ItemTemplate>
                        <tr>
                            <td>
                                <%# SaleTypeName((int?)DataBinder.Eval(Container.DataItem, "BaseReportView.SaleTypeID"))%>
                            </td>
                            <td>
                                <%# DataBinder.Eval(Container.DataItem, "BaseReportView.AssertigyDisplayName") %>
                            </td>
                            <td>
                                <%# DataBinder.Eval(Container.DataItem, "BaseReportView.AssertigyMID")%>
                            </td>
                            <td>
                                <%# PaymentTypeName((int?)DataBinder.Eval(Container.DataItem, "BaseReportView.PaymentTypeID"))%>
                            </td>
                            <td>
                                <%# DataBinder.Eval(Container.DataItem, "BaseReportView.SaleCount")%>
                            </td>
                            <td>
                                <%# DataBinder.Eval(Container.DataItem, "SaleChargebackCount")%>
                            </td>
                            <td>
                                <%# (((decimal)DataBinder.Eval(Container.DataItem, "SaleChargebackPercentage"))).ToString("0.00%")%>
                            </td>
                          <%--  <td>
                                <%# DataBinder.Eval(Container.DataItem, "ProjectedSaleCount")%>
                            </td>
                            <td>
                                <%# DataBinder.Eval(Container.DataItem, "ProjectedSaleChargebackCount")%>
                            </td>
                            <td>
                                <%# (((decimal)DataBinder.Eval(Container.DataItem, "ProjectedChargebackPercentage"))).ToString("0.00%")%>
                            </td>--%>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
                <tr class="total">
                    <td>
                        Total
                    </td>
                    <td>
                    </td>
                    <td>
                    </td>
                    <td>
                    </td>
                    <td>
                        <%# TotalSales["CmpSaleCount"]%>
                    </td>
                    <td>
                        <%# TotalSales["CmpSaleChargebackCount"]%>
                    </td>
                    <td>
                    </td>
                    <%--<td>
                        <%# TotalSales["CmpProjCount"]%>
                    </td>
                    <td>
                        <%# TotalSales["CmpProjChargebackCount"]%>
                    </td>--%>
                    <td>
                    </td>
                </tr>
            </table>
            <div class="clear">
            </div>
        </div>
        <%--Add tab by BinName--%>
        <div id="tabs-4">
            <div style="width: 30%;">
                <h2>
                    Summary</h2>
                <table class="process-offets multilevel sortable add-csv-export" border='0' cellspacing='1' width='100%'>
                    <tr class="header">
                        <td>
                            Chargeback Status
                        </td>
                        <td>
                            Count
                        </td>
                        <td>
                            Percentage
                        </td>
                    </tr>
                    <asp:Repeater runat="server" ID="rReasonSummary">
                        <ItemTemplate>
                            <tr>
                                <td>
                                    <%# Eval("Key") %>
                                </td>
                                <td>
                                    <%# Eval("Value") %>
                                </td>
                                <td>
                                    <%# (Convert.ToDouble(Eval("Value")) / TotalChargebackCountByReason).ToString("0.00%")%>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                    <tr class="total">
                        <td>
                            Total
                        </td>
                        <td>
                            <%# TotalChargebackCountByReason%>
                        </td>
                        <td>
                        </td>
                    </tr>
                </table>
            </div>
            <div class="clear" style="height: 10px;">
            </div>
            <table class="process-offets multilevel sortable add-csv-export" border='0' cellspacing='1' width='100%'>
                <tr class="header">
                    <td>
                        Chargeback Reason
                    </td>
                    <td>
                        Won
                    </td>
                    <td>
                        Lost
                    </td>
                    <td>
                        Total
                    </td>
                    <td>
                        Won
                    </td>
                    <td>
                        Lost
                    </td>
                </tr>
                <asp:Repeater runat="server" ID="rSalesReasons">
                    <ItemTemplate>
                        <tr>
                            <td>
                                <%# Eval("ReasonCode") %>
                            </td>
                            <td>
                                <%# Eval("WonCount").ToString() %>
                            </td>
                            <td>
                                <%# Eval("LostCount").ToString() %>
                            </td>
                            <td>
                                <%# Eval("TotalWonLostCount").ToString()%>
                            </td>
                            <td>
                                <%# (Convert.ToDouble(Eval("WonCount")) / Convert.ToInt32(Eval("TotalWonLostCount"))).ToString("0.00%")%>
                            </td>
                            <td>
                                <%# (Convert.ToDouble(Eval("LostCount")) / Convert.ToInt32(Eval("TotalWonLostCount"))).ToString("0.00%")%>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
                <tr class="total">
                    <td>
                        Total
                    </td>
                    <td>
                        <asp:Label ID="lblTotalWonCount" runat="server" />
                    </td>
                    <td>
                        <asp:Label ID="lblTotalLostCount" runat="server" />
                    </td>
                    <td>
                        <asp:Label ID="lblTotalWonLostCount" runat="server" />
                    </td>
                    <td>
                    </td>
                    <td>
                    </td>
                </tr>
            </table>
            <div class="clear">
            </div>
        </div>
    </div>
    </div>
    </form>
    
</asp:Content>
