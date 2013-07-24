<%@ Page Title="" Language="C#" MasterPageFile="~/admin/Controls/Admin.Master" AutoEventWireup="true" CodeBehind="sales_agr_report.aspx.cs" Inherits="Securetrialoffers.admin.sales_agr_report" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphScript" runat="server">
<script type="text/javascript" src="js/sorttable.js"></script>
<script type="text/javascript">
    $(document).ready(function() {
        $("#start-date").datepicker({
            showOn: 'button',
            buttonImage: 'images/img_calendaricon.gif',
            buttonImageOnly: true
        });
        $("#end-date").datepicker({
            showOn: 'button',
            buttonImage: 'images/img_calendaricon.gif',
            buttonImageOnly: true
        });
        $("#lnk-show").click(function() {
            document.location = "sales_agr_report.aspx?startDate=" + $("#start-date").val() + "&endDate=" + $("#end-date").val() + "&tab=" + selectedTab;
        });
        $("#lnk-download-csv").click(function() {
            document.location = "sales_agr_report.aspx?csv=1&startDate=" + $("#start-date").val() + "&endDate=" + $("#end-date").val() + "&tab=" + selectedTab;
        });
        initMultilevelTables();
    });

    var selectedTab = <%# ReportTab %>;
    var tabberOptions = {
      'onClick': function(argsObj) {
        selectedTab = argsObj.index; /* Which tab was clicked (0..n) */
      },
      'onLoad': function(argsObj) {
        argsObj.tabber.tabShow(selectedTab);
      }
    }
    
    function initMultilevelTables()
    {
        $("table.multilevel").each(function(){
            var table = this;
            $(table).find("tr.master").click(function(){            
                multilevelTableToggleDetail(table, this);
            });
        });
    }
    
    function multilevelTableToggleDetail(tableEl, masterEl)
    {
        var masterId = $(masterEl).attr("master-id");
        //alert(masterId);
        //Recursive hide children
        $(tableEl).find("tr.detail[detail-id=" + masterId + "]").filter(".master").filter(":visible").each(function(){
            multilevelTableHideDetail(tableEl, this)
        });
        $(tableEl).find("tr.detail[detail-id=" + masterId + "]").toggle();
    }

    function multilevelTableHideDetail(tableEl, masterEl)
    {
        var masterId = $(masterEl).attr("master-id");
        //alert(masterId);
        //Recursive hide children
        $(tableEl).find("tr.detail[detail-id=" + masterId + "]").filter(".master").filter(":visible").each(function(){
            multilevelTableHideDetail(tableEl, this)
        });
        $(tableEl).find("tr.detail[detail-id=" + masterId + "]").hide();
    }
</script>
<script type="text/javascript" src="js/tabber-minimized.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphStyle" runat="server">
<link rel="stylesheet" href="css/tabber.css" type="text/css" media="screen" />
<style type="text/css">
    .sortable tr:hover 
    {
        background-color: Silver;
    }

    tr.level0:hover, tr.level1:hover, tr.level2:hover 
    {
        background-color: Silver;
    }
    
    tr.level0 td 
    {
    	border-bottom: solid 1px grey;
    }

    tr.level1 td 
    {
    	border-bottom: dotted 1px grey;
    }

    tr.detail
    {
    	display:none;
    }

    tr.level0 td 
    {
    	font-size: 1em;
    }

    tr.level1 td 
    {
    	font-size: 0.9em;
    }

    tr.level2 td 
    {
    	font-size: 0.8em;
    }
</style>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<h2>Chargeback Performance Report</h2>
<h2><%# StartDate.ToShortDateString() %> - <%# EndDate.ToShortDateString() %></h2>
<hr />
Report Date:&nbsp;
<input type="text" id="start-date" value='<%# StartDate.ToShortDateString() %>' />&nbsp;-&nbsp;<input type="text" id="end-date" value='<%# EndDate.ToShortDateString() %>' />&nbsp;&nbsp;
<a href="#" id="lnk-show">Show Report</a>&nbsp;&nbsp;&nbsp;
<a href="#" id="lnk-download-csv">Download CSV</a>
<hr />
<table><tr><td style="text-align: left;">
<div class="tabber" id="tab1">
    <div class="tabbertab">
        <h2>By Company</h2>
        <table width="960px;" class="sortable">
            <tr class="header">
                <td>DBA</td>
                <td>FMA Number</td>
                <td>CHID Value</td>
                <td>TYP</td>
                <td>MTD SETTCNT</td>
                <td>MTD CBCNT</td>
                <td>MTD CBPCT</td>
                <td>PROJ SETTCNT</td>
                <td>PROJ CBCNT</td>
                <td>PROJ CBPCT</td>
            </tr>
            <asp:Repeater runat="server" ID="rSales">
                <ItemTemplate>
                    <tr>
                        <td><%# DataBinder.Eval(Container.DataItem, "BaseReportView.AssertigyDisplayName") %></td>
                        <td><%# DataBinder.Eval(Container.DataItem, "BaseReportView.AssertigyMID")%></td>
                        <td><%# DataBinder.Eval(Container.DataItem, "BaseReportView.NMICompanyName")%></td>
                        <td><%# PaymentTypeName((int?)DataBinder.Eval(Container.DataItem, "BaseReportView.PaymentTypeID"))%></td>
                        <td><%# DataBinder.Eval(Container.DataItem, "BaseReportView.SaleCount")%></td>
                        <td><%# DataBinder.Eval(Container.DataItem, "SaleChargebackCount")%></td>
                        <td><%# (((decimal)DataBinder.Eval(Container.DataItem, "SaleChargebackPercentage"))).ToString("0.00%")%></td>
                        <td><%# DataBinder.Eval(Container.DataItem, "ProjectedSaleCount")%></td>
                        <td><%# DataBinder.Eval(Container.DataItem, "ProjectedSaleChargebackCount")%></td>
                        <td><%# (((decimal)DataBinder.Eval(Container.DataItem, "ProjectedChargebackPercentage"))).ToString("0.00%")%></td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>
    </div>
    <div class="tabbertab">
        <h2>By SubAffiliate</h2>
        <table width="960px;" class="multilevel">
            <tr class="header">
                <td>Affiliate/SubAffiliate/TYP</td>
                <td>MTD SETTCNT</td>
                <td>MTD CBCNT</td>
                <td>MTD CBPCT</td>
                <td>PROJ SETTCNT</td>
                <td>PROJ CBCNT</td>
                <td>PROJ CBPCT</td>
            </tr>
            <asp:Repeater runat="server" ID="rSalesByAff">
                <ItemTemplate>
                    <tr master-id='a_<%# DataBinder.Eval(Container.DataItem, "Key.BaseReportView.Affiliate")%>' class="master level0">
                        <td><%# (!string.IsNullOrEmpty((string)DataBinder.Eval(Container.DataItem, "Key.BaseReportView.Affiliate"))) ? DataBinder.Eval(Container.DataItem, "Key.BaseReportView.Affiliate") : "-"%></td>
                        <td><%# DataBinder.Eval(Container.DataItem, "Key.BaseReportView.SaleCount")%></td>
                        <td><%# DataBinder.Eval(Container.DataItem, "Key.SaleChargebackCount")%></td>
                        <td><%# (((decimal)DataBinder.Eval(Container.DataItem, "Key.SaleChargebackPercentage"))).ToString("0.00%")%></td>
                        <td><%# DataBinder.Eval(Container.DataItem, "Key.ProjectedSaleCount")%></td>
                        <td><%# DataBinder.Eval(Container.DataItem, "Key.ProjectedSaleChargebackCount")%></td>
                        <td><%# (((decimal)DataBinder.Eval(Container.DataItem, "Key.ProjectedChargebackPercentage"))).ToString("0.00%")%></td>
                    </tr>
                    <asp:Repeater runat="server" ID="rSalesBySub" DataSource='<%# Container.DataItem %>'>
                        <ItemTemplate>
                            <tr master-id='a_<%# DataBinder.Eval(Container.DataItem, "Key.BaseReportView.Affiliate")%>_s<%# DataBinder.Eval(Container.DataItem, "Key.BaseReportView.SubAffiliate")%>' detail-id='a_<%# DataBinder.Eval(Container.DataItem, "Key.BaseReportView.Affiliate")%>' class="master detail level1">
                                <td style="padding-left: 40px;"><%# (!string.IsNullOrEmpty((string)DataBinder.Eval(Container.DataItem, "Key.BaseReportView.SubAffiliate"))) ? DataBinder.Eval(Container.DataItem, "Key.BaseReportView.SubAffiliate") : "-"%></td>
                                <td><%# DataBinder.Eval(Container.DataItem, "Key.BaseReportView.SaleCount")%></td>
                                <td><%# DataBinder.Eval(Container.DataItem, "Key.SaleChargebackCount")%></td>
                                <td><%# (((decimal)DataBinder.Eval(Container.DataItem, "Key.SaleChargebackPercentage"))).ToString("0.00%")%></td>
                                <td><%# DataBinder.Eval(Container.DataItem, "Key.ProjectedSaleCount")%></td>
                                <td><%# DataBinder.Eval(Container.DataItem, "Key.ProjectedSaleChargebackCount")%></td>
                                <td><%# (((decimal)DataBinder.Eval(Container.DataItem, "Key.ProjectedChargebackPercentage"))).ToString("0.00%")%></td>
                            </tr>
                            <asp:Repeater runat="server" ID="rSalesByCC" DataSource='<%# Container.DataItem %>'>
                                <ItemTemplate>
                                    <tr detail-id='a_<%# DataBinder.Eval(Container.DataItem, "Key.BaseReportView.Affiliate")%>_s<%# DataBinder.Eval(Container.DataItem, "Key.BaseReportView.SubAffiliate")%>' class="detail level2">
                                        <td style="padding-left: 80px;"><%# PaymentTypeName((int?)DataBinder.Eval(Container.DataItem, "Key.BaseReportView.PaymentTypeID"))%></td>
                                        <td><%# DataBinder.Eval(Container.DataItem, "Key.BaseReportView.SaleCount")%></td>
                                        <td><%# DataBinder.Eval(Container.DataItem, "Key.SaleChargebackCount")%></td>
                                        <td><%# (((decimal)DataBinder.Eval(Container.DataItem, "Key.SaleChargebackPercentage"))).ToString("0.00%")%></td>
                                        <td><%# DataBinder.Eval(Container.DataItem, "Key.ProjectedSaleCount")%></td>
                                        <td><%# DataBinder.Eval(Container.DataItem, "Key.ProjectedSaleChargebackCount")%></td>
                                        <td><%# (((decimal)DataBinder.Eval(Container.DataItem, "Key.ProjectedChargebackPercentage"))).ToString("0.00%")%></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </ItemTemplate>
                    </asp:Repeater>
                </ItemTemplate>
            </asp:Repeater>
        </table>
    </div>
    <div class="tabbertab">
        <h2>By Charge Type</h2>
        <table width="960px;" class="sortable">
            <tr class="header">
                <td>Charge Type</td>
                <td>DBA</td>
                <td>FMA Number</td>
                <td>TYP</td>
                <td>MTD SETTCNT</td>
                <td>MTD CBCNT</td>
                <td>MTD CBPCT</td>
                <td>PROJ SETTCNT</td>
                <td>PROJ CBCNT</td>
                <td>PROJ CBPCT</td>
            </tr>
            <asp:Repeater runat="server" ID="rSalesByType">
                <ItemTemplate>
                    <tr>
                        <td><%# SaleTypeName((int?)DataBinder.Eval(Container.DataItem, "BaseReportView.SaleTypeID"))%></td>
                        <td><%# DataBinder.Eval(Container.DataItem, "BaseReportView.AssertigyDisplayName") %></td>
                        <td><%# DataBinder.Eval(Container.DataItem, "BaseReportView.AssertigyMID")%></td>
                        <td><%# PaymentTypeName((int?)DataBinder.Eval(Container.DataItem, "BaseReportView.PaymentTypeID"))%></td>
                        <td><%# DataBinder.Eval(Container.DataItem, "BaseReportView.SaleCount")%></td>
                        <td><%# DataBinder.Eval(Container.DataItem, "SaleChargebackCount")%></td>
                        <td><%# (((decimal)DataBinder.Eval(Container.DataItem, "SaleChargebackPercentage"))).ToString("0.00%")%></td>
                        <td><%# DataBinder.Eval(Container.DataItem, "ProjectedSaleCount")%></td>
                        <td><%# DataBinder.Eval(Container.DataItem, "ProjectedSaleChargebackCount")%></td>
                        <td><%# (((decimal)DataBinder.Eval(Container.DataItem, "ProjectedChargebackPercentage"))).ToString("0.00%")%></td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>
    </div>
</div>
</td></tr></table>
</asp:Content>
