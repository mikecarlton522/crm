<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="queue_report.aspx.cs" Inherits="TrimFuel.Web.Admin.unsent_unpayed_shippments"
    MasterPageFile="~/Controls/Admin.Master" %>

<%@ Register Src="Controls/DateFilter.ascx" TagName="DateFilter" TagPrefix="uc" %>
<%@ Register Assembly="TrimFuel.Web.UI" Namespace="TrimFuel.Web.UI.Specialized" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphScript" runat="server">
    <script language="JavaScript" type="text/javascript">
        var displayedRows;
        $(document).ready(function () {
            $("#tabs").tabs({ cookie: true });
            displayedRows = new Array();
            $("#unsent-shippments-container").html("<div class='data-loading'>Loading...<br/><img src='/images/loading.gif'></div>");
            $("#queued-transactions-container").html("<div class='data-loading'>Loading...<br/><img src='/images/loading.gif'></div>");
            inlineControl("ajaxControls/UnsentShipments.aspx", "unsent-shippments-container");
            inlineControl("ajaxControls/QueuedTransactions.aspx", "queued-transactions-container");
        });

        function selectShippments() {
            if ($('#selectAll').attr('checked') == true) {
                $("tr", "#unsent-shippments-container").each(function () {
                    if ($(this).css("display") == "table-row")
                        $(this).find("INPUT[name='shippmentToSend'][type='checkbox']").attr('checked', $('#selectAll').attr('checked'));
                });
            }
            else {
                $("INPUT[name='shippmentToSend'][type='checkbox']").attr('checked', $('#selectAll').attr('checked'));
            }
        }

        function selectBills() {
            debugger;
            if ($('#selectAllBills').attr('checked') == true) {
                $("tr", "#queued-transactions-container").each(function () {
                    if ($(this).css("display") == "table-row") {
                        $(this).find("INPUT[name='billToSend'][type='checkbox']").attr('checked', $('#selectAllBills').attr('checked'));
                        $(this).find("INPUT[name='rebillToSend'][type='checkbox']").attr('checked', $('#selectAllBills').attr('checked'));
                    }
                });
            }
            else {
                $("INPUT[name='billToSend'][type='checkbox']").attr('checked', $('#selectAllBills').attr('checked'));
                $("INPUT[name='rebillToSend'][type='checkbox']").attr('checked', $('#selectAllBills').attr('checked'));
            }
        }

        function ChangeShipperAction(el) {
            if ($(el).val() == "1") {
                $("#ShipperDDL").attr("disabled", "disabled");
                $('#sameShipper').show();
                $('#diffShipper').hide();
                $('#asSent').hide();
                $('#asSentWithNote').hide();
                $("#ShipperDDL").val("");
            }
            if ($(el).val() == "2") {
                $("#ShipperDDL").attr("disabled", "");
                $('#sameShipper').hide();
                $('#diffShipper').show();
                $('#asSent').hide();
                $('#asSentWithNote').hide();
            }
            if ($(el).val() == "3") {
                $("#ShipperDDL").attr("disabled", "disabled");
                $('#sameShipper').hide();
                $('#diffShipper').hide();
                $('#asSent').show();
                $('#asSentWithNote').hide();
                $("#ShipperDDL").val("");
            }
            if ($(el).val() == "4") {
                $("#ShipperDDL").attr("disabled", "disabled");
                $('#sameShipper').hide();
                $('#diffShipper').hide();
                $('#asSent').hide();
                $('#asSentWithNote').show();
                $("#ShipperDDL").val("");
            }
            if ($(el).val() == "0") {
                $("#ShipperDDL").attr("disabled", "disabled");
                $('#sameShipper').hide();
                $('#diffShipper').hide();
                $("#ShipperDDL").val("");
                $('#asSent').hide();
                $('#asSentWithNote').hide();
            }
        }

        function ChangeMIDAction(el) {
            if ($(el).val() == "1") {
                $("#AssertigyMidDDL").attr("disabled", "disabled");
                $('#sameMID').show();
                $('#diffMID').hide();
                $("#AssertigyMidDDL").val("");
            }
            if ($(el).val() == "2") {
                $("#AssertigyMidDDL").attr("disabled", "");
                $('#sameMID').hide();
                $('#diffMID').show();
            }
            if ($(el).val() == "0") {
                $("#AssertigyMidDDL").attr("disabled", "disabled");
                $('#sameMID').hide();
                $('#diffMID').hide();
                $("#AssertigyMidDDL").val("");
            }
            if ($(el).val() == "3") {
                $("#AssertigyMidDDL").attr("disabled", "disabled");
                $('#sameMID').hide();
                $('#diffMID').hide();
                $('#ignore').show();
                $("#AssertigyMidDDL").val("");
            }
        }

        function initMultilevelTablesTransactions() {
            $("#queued-transactions-container table.multilevel").each(function () {
                var table = this;
                $(table).find("tr.master").unbind("click");
                $(table).find("tr.master").each(function () {
                    var masterId = $(this).attr("master-id");

                    if (displayedRows[masterId] == 1 && $("#img-" + masterId + "-Unwrap").css("display") == "none")
                        multilevelTableToggleDetailTransactions(table, this);
                });
                $(table).find("tr.master").click(function () {
                    multilevelTableToggleDetailTransactions(table, this);
                });
            });
        }

        function multilevelTableToggleDetailTransactions(tableEl, masterEl) {
            var masterId = $(masterEl).attr("master-id");

            if ($("#img-" + masterId + "-Wrap").css("display") == "none") {
                $("#img-" + masterId + "-Wrap").show();
                $("#img-" + masterId + "-Unwrap").hide();
                displayedRows[masterId] = 0;
                
                $(tableEl).find("tr").each(function () {
                    if ($(this).attr("detail-id") == masterId)
                        $(this).remove();
                });
            }
            else {
                $("#img-" + masterId + "-Wrap").hide();
                $("#img-" + masterId + "-Unwrap").show();
                displayedRows[masterId] = 1;

                $(masterEl).after("<tr><td colspan='9'><img src='/images/loading2.gif'></td></tr>");
                $.get('/dotNet/services/QueuedTransactionsGroup.aspx?midID=' + masterId.replace("mid-", ""), function (data) {
                    $(masterEl).next().remove();
                    $(masterEl).after(data);
                    processTableOffsets(tableEl);
                    sorttable.makeSortable(tableEl);
                });
            }
            $('.' + masterId).toggle();
        }

        function initMultilevelTablesShipments() {
            $("#unsent-shippments-container table.multilevel").each(function () {
                var table = this;
                $(table).find("tr.master").unbind("click");
                $(table).find("tr.master").each(function () {
                    var masterId = $(this).attr("master-id");

                    if (displayedRows[masterId] == 1 && $("#img-" + masterId + "-Unwrap").css("display") == "none")
                        multilevelTableToggleDetailShipments(table, this);
                });
                $(table).find("tr.master").click(function () {
                    multilevelTableToggleDetailShipments(table, this);
                });
            });
        }

        function multilevelTableToggleDetailShipments(tableEl, masterEl) {
            var masterId = $(masterEl).attr("master-id");
            if ($("#img-" + masterId + "-Wrap").css("display") == "none") {
                $("#img-" + masterId + "-Wrap").show();
                $("#img-" + masterId + "-Unwrap").hide();
                displayedRows[masterId] = 0;
                $(tableEl).find("tr").each(function () {
                    if ($(this).attr("detail-id") == masterId)
                        $(this).remove();
                });
            }
            else {

                $("#img-" + masterId + "-Wrap").hide();
                $("#img-" + masterId + "-Unwrap").show();
                displayedRows[masterId] = 1;

                $(masterEl).after("<tr><td colspan='8'><img src='/images/loading2.gif'></td></tr>");
                $.get('/dotNet/services/UnsentShipmentsGroup.aspx?shipperID=' + masterId.replace("shipper-", ""), function (data) {
                    $(masterEl).next().remove();
                    $(masterEl).after(data);
                    processTableOffsets(tableEl);
                    sorttable.makeSortable(tableEl);
                });
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphStyle" runat="server">
    <style type="text/css">
        .multilevel tr.total td
        {
            color: white;
            font-size: 1.05em;
            background-color: #555 !important;
        }
        
        .multilevel tr.master:hover td, .multilevel tr.detail:hover td
        {
            background-color: #999 !important;
        }
        
        .multilevel tr.master
        {
            cursor: pointer;
        }
        
        tr.level0 td
        {
            padding: 5px;
            background-color: #e4e4e4;
            font-weight: bold;
        }
        
        tr.level1 td
        {
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
        
        a.reason
        {
            float: right;
        }
    </style>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <form runat="server" id="frmMain">
    <div id="tabs" class="data">
        <ul>
            <li><a href="#tabs-1">Unsent Shipments</a></li>
            <li><a href="#tabs-2">Queued Transactions</a></li>
        </ul>
        <div id="tabs-1">
            <%--<div id="buttons" style="text-align: center !important;">
                <input type="button" value="Generate Report" onclick="GenerateReport1(); return false;" />
            </div>--%>
            <div id="unsent-shippments-container">
            </div>
        </div>
        <div id="tabs-2">
            <%--         <div id="buttons" style="text-align: center !important;">
                <input type="button" value="Generate Report" onclick="GenerateReport2(); return false;" />
            </div>--%>
            <div id="queued-transactions-container">
            </div>
        </div>
    </form>
</asp:Content>
