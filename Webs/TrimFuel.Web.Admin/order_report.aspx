<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/Admin.Master" AutoEventWireup="true" CodeFile="order_report.aspx.cs" Inherits="order_report" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphScript" runat="Server">
    <script type="text/javascript" src="../js/jquery.tmpl.min.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            var load = $('#load');
            var query = $('#query');
            var table = $('#table');
            var limit = $('#limit');
            var delay = undefined;
            query.keyup(function () {
                if (query.val().length > 3) {
                    if (delay != undefined)
                        clearTimeout(delay);
                    delay = setTimeout(function () {
                        load.show();
                        table.find("tr:gt(0)").remove()
                        $.ajax({
                            type: 'POST',
                            url: 'order_report.aspx/Search',
                            data: '{"search": "' + query.val() + '", "limit": ' + !limit.is(':checked') + ' }',
                            contentType: 'application/json; charset=utf-8',
                            dataType: 'json',
                            success: function (msg) {
                                var json = msg.d;
                                $.template("row", $("#row"));
                                $.tmpl("row", json).appendTo(table);
                                load.hide();
                            }
                        });
                    }, 700);
                }
            });
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphStyle" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="Server">
    <script id="row" type="text/x-jQuery-tmpl">
        <tr>
            <td>${BID}</td>
            <td>${Campaign}</td>
            <td>${FraudScore}</td>
            <td>${Status}</td>
            <td>${OrderDate}</td>
            <td>${FirstName}</td>
            <td>${LastName}</td>
            <td>${Address}</td>
            <td>${City}</td>
            <td>${State}</td>
            <td>${Zip}</td>
            <td>${Phone}</td>
            <td>${Email}</td>
            <td>${Affiliate}</td>
            <td>${SubID}</td>
        </tr>
    </script>
    <input type="text" id="query" name="query" maxlength="100" size="80" />&nbsp;&nbsp;
    <input type="checkbox" id="limit" name="limit" />Show all results
    <br />
    <br />
    <img src="../images/loading2.gif" id="load" style="display: none;" alt="Please wait" />
    <br />
    <br />
    <div class="data">
        <table class="process-offets sortable add-csv-export" border="0" cellspacing="1" width="100%" id="table">
            <tr class="header">
                <td>BID </td>
                <td>Campaign </td>
                <td>Fraud Score </td>
                <td>Status </td>
                <td>Order Date </td>
                <td>First Name </td>
                <td>Last Name </td>
                <td>Address </td>
                <td>City </td>
                <td>State </td>
                <td>Zip </td>
                <td>Phone </td>
                <td>Email </td>
                <td>Affiliate </td>
                <td>SubID </td>
            </tr>
        </table>
    </div>
</asp:Content>
