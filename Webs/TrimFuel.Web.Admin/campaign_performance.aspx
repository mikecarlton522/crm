<%@ Page Language="C#" AutoEventWireup="true" CodeFile="campaign_performance.aspx.cs" Inherits="campaign_performance" %>

<%@ Register Src="~/Controls/DateFilter.ascx" TagName="DateFilter" TagPrefix="uc" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
<head>
    <title>Campaign Performance</title>
    <!--[if IE]><script language="javascript" type="text/javascript" src="../js/excanvas.js"></script><![endif]-->
   

<link src="https://ajax.googleapis.com/ajax/libs/jquery/1.6.1/jquery.min.js" type="text/javascript"/>



<script type="text/javascript" src="../js/highcharts.js"></script>
<script type="text/javascript" src="../js/exporting.js"></script>


<script type="text/javascript" src="../js/testing-stock.js"></script>
<script type="text/javascript" src="../js/usdeur.js"></script>
    <script language="javascript" type="text/javascript" src="../js/jquery.min.js"></script>
    <script language="javascript" type="text/javascript" src="../js/jquery.jqplot.min.js"></script>
    <script language="javascript" type="text/javascript" src="../js/plugins/jqplot.canvasTextRenderer.min.js"></script>
    <script language="javascript" type="text/javascript" src="../js/plugins/jqplot.canvasAxisTickRenderer.min.js"></script>
    <script language="javascript" type="text/javascript" src="../js/plugins/jqplot.categoryAxisRenderer.min.js"></script>
    <script language="javascript" type="text/javascript" src="../js/plugins/jqplot.dateAxisRenderer.js"></script>
    <script language="javascript" type="text/javascript" src="../js/jquery-ui.min.js"></script>
    <script language="javascript" type="text/javascript" src="../js/jquery.tmpl.min.js"></script>
    <link rel="Stylesheet" type="text/css" href="../css/style.css" />
    <link rel="Stylesheet" type="text/css" href="../css/jquery.jqplot.min.css" />
    <link rel="stylesheet" type="text/css" href="../css/smoothness/jquery-ui-1.8.2.custom.css" />
</head>
<body style="background: #fff">
    <form runat="server">
    <label>Filter by Affiliate</label><asp:DropDownList ID="affiliates" runat="server">
    </asp:DropDownList>
    &nbsp;&nbsp;&nbsp;&nbsp; <label>Filter by SubAffiliate</label><asp:DropDownList ID="subaffiliates" runat="server" Enabled="false">
        <asp:ListItem Text="All" Value="All"></asp:ListItem>
    </asp:DropDownList>
    &nbsp;&nbsp;&nbsp;&nbsp; <label>Start Date</label><input id="date_tbDate1" name="date_tbDate1" value="<%= startDate %>" />
    <label>End Date</label><input id="date_tbDate2" name="date_tbDate2" value="<%= endDate %>" />
    &nbsp;&nbsp;&nbsp;&nbsp;
    <button id="update">
        Update</button>
    </form>
    <br />
    <div id="tabs">
        <ul>
            <li><a href="#tabs-1">Clicks/Reg/Orders</a></li>
            <li><a href="#tabs-2">Cancels, Refunds, Chargebacks</a></li>
            <li><a href="#tabs-3">CPA/media costs</a></li>
            <li><a href="#tabs-4">Merchant fees/reserve</a></li>
            <li><a href="#tabs-5">Fulfillment</a></li>
        </ul>
        <div id="tabs-1">
            <div id="jq-loader-1" class="jq-loader">
                Loading...
            </div>
            <div id="jq-content-1" class="jq-content">
                <div id="chart-1" style="height: 400px; width: 1100px;">
                </div>
                <br /> 
                <br />
                <div class="data">
                    <span id="jq-message-1" style="display: none">Showing first 100 records</span>
                    <table class="process-offets sortable add-csv-export" border="0" cellspacing="1" width="100%" id="table-1">
                        <tr class="header">
                            <td>BillingID </td>
                            <td>Name</td>
                            <td>Signup Date </td>
                            <td>Affiliate </td>
                            <td>SubID </td>
                            <td>Status </td>
                        </tr>
                    </table>
                </div>
                <div class="clear">
                </div>
            </div>
            <div id="jq-error-1" class="jq-error">
            </div>
        </div>
        <div id="tabs-2">
            <div id="jq-loader-2" class="jq-loader">
                Loading...
            </div>
            <div id="jq-content-2" class="jq-content">
                <div id="chart-2" style="height: 400px; width: 1100px;">
                </div>
                <br />
                <br />
                <div class="data">
                    <span id="jq-message-2" style="display: none">Showing first 100 records</span>
                    <table class="process-offets sortable add-csv-export" border="0" cellspacing="1" width="100%" id="table-2">
                        <tr class="header">
                            <td>BillingID </td>
                            <td>Name</td>
                            <td>Signup Date </td>
                            <td>Affiliate </td>
                            <td>SubID </td>
                            <td>Status </td>
                        </tr>
                    </table>
                </div>
                <div class="clear">
                </div>
            </div>
            <div id="jq-error-2" class="jq-error">
            </div>
        </div>
        <div id="tabs-3">
            <div id="jq-loader-3" class="jq-loader">
                Loading...
            </div>
            <div id="jq-content-3" class="jq-content">
                <div id="chart-3" style="height: 400px; width: 1100px;">
                </div>
                <br />
                <br />
                <div class="data">
                    <span id="jq-message-3" style="display: none">Showing first 100 records</span>
                    <table class="process-offets sortable add-csv-export" border="0" cellspacing="1" width="100%" id="table-3">
                        <tr class="header">
                            <td>BillingID </td>
                            <td>Name</td>
                            <td>Signup Date </td>
                            <td>Affiliate </td>
                            <td>SubID </td>
                            <td>Status </td>
                        </tr>
                    </table>
                </div>
                <div class="clear">
                </div>
            </div>
            <div id="jq-error-3" class="jq-error">
            </div>
        </div>
        <div id="tabs-4">
            <div id="jq-loader-4" class="jq-loader">
                Loading...
            </div>
            <div id="jq-content-4" class="jq-content">
                <div id="chart-4" style="height: 400px; width: 1100px;">
                </div>
                <br />
                <br />
                <div class="data">
                    <span id="jq-message-4" style="display: none">Showing first 100 records</span>
                    <table class="process-offets sortable add-csv-export" border="0" cellspacing="1" width="100%" id="table-4">
                        <tr class="header">
                            <td>BillingID </td>
                            <td>Name</td>
                            <td>Signup Date </td>
                            <td>Affiliate </td>
                            <td>SubID </td>
                            <td>Status </td>
                        </tr>
                    </table>
                </div>
                <div class="clear">
                </div>
            </div>
            <div id="jq-error-4" class="jq-error">
            </div>
        </div>
        <div id="tabs-5">
            <div id="jq-loader-5" class="jq-loader">
                Loading... (this may take up to 40 seconds)
            </div>
            <div id="jq-content-5" class="jq-content">
                <div id="chart-5" style="height: 400px; width: 1100px;">
                </div>
                <br />
                <br />
                <div class="data">
                    <span id="jq-message-5" style="display: none">Showing first 100 records</span>
                    <table class="process-offets sortable add-csv-export" border="0" cellspacing="1" width="100%" id="table-5">
                        <tr class="header">
                            <td>BillingID </td>
                            <td>Name</td>
                            <td>Signup Date </td>
                            <td>Affiliate </td>
                            <td>SubID </td>
                            <td>Status </td>
                        </tr>
                    </table>
                </div>
                <div class="clear">
                </div>
            </div>
            <div id="jq-error-5" class="jq-error">
            </div>
        </div>
    </div>
    
    <script type="text/javascript">
        function sdf_FTS(_number, _decimal, _separator) {
            var decimal = (typeof (_decimal) != 'undefined') ? _decimal : 2;

            var separator = (typeof (_separator) != 'undefined') ? _separator : ',';

            var r = parseFloat(_number)
            if (_decimal != 0) {
                var exp10 = Math.pow(10, decimal);
                r = Math.round(r * exp10) / exp10;
            }
            rr = Number(r).toFixed(decimal).toString().split('.');

            b = rr[0].replace(/(\d{1,3}(?=(\d{3})+(?:\.\d|\b)))/g, "\$1" + separator);
            if (_decimal != 0) {
                r = b + '.' + rr[1];
            }
            else r = b;
            return r;
        }
        function GetDayforSlidingWindow(str) {
          
            switch (str) {
                case 1: dayofweek = "Monday "; break;
                case 2: dayofweek = "Tuesday "; break;
                case 3: dayofweek = "Wednesday "; break;
                case 4: dayofweek = "Thursday "; break;
                case 5: dayofweek = "Friday "; break;
                case 6: dayofweek = "Saturday "; break;
                case 0: dayofweek = "Sunday "; break;
                default: break;
            }
            return dayofweek;
        }
        $('.jq-loader').hide();

        function FetchSeriesForGraph1(affiliate, start, end) {
            $('#jq-loader-1').show();
            $('#jq-content-1').hide();
            $('#jq-error-1').hide();
            $('#jq-message-1').hide();
            $('#chart-1').empty();
            $("#table-1").find("tr:gt(0)").remove();
            $.ajax({
                type: 'POST',
                url: 'dotnet/campaign_performance.aspx/FetchSeriesForGraph1',
                data: '{ "affiliate": "' + affiliate + '", "start": "' + start + '", "end": "' + end + '"}',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function(msg) {
                    var json = jQuery.parseJSON(msg.d);
                    if (json.landings == undefined || json.landings == null || json.landings == '') {
                        $('#jq-loader-1').hide();
                        $('#jq-error-1').show();
                        $('#jq-error-1').text('No data found');
                        return;
                    }

                    $('#jq-loader-1').hide();
                    $('#jq-content-1').show();
                   
                  
                    var readydata_landings = [];
                    var readydata_billings = [];
                    var readydata_conversions = [];
                   
                    // we can't parse those three in one cycle, cause sometimes there are different lengths of those strings.
                    for (i = 0, j = 0; i < json.landings.length; i += 2, j++) {
                        var datetmp = new Date(Date.parse(json.landings[i]));
                        
                        readydata_landings[j] = [Date.UTC(datetmp.getFullYear(), datetmp.getMonth(), datetmp.getDate()), json.landings[i + 1]];
                    }
                    for (i = 0, j = 0; i < json.billings.length; i += 2, j++) {
                        var datetmp = new Date(Date.parse(json.billings[i]));
                        readydata_billings[j] = [Date.UTC(datetmp.getFullYear(), datetmp.getMonth(), datetmp.getDate()), json.billings[i + 1]];
                    }
                    for (i = 0, j = 0; i < json.conversions.length; i += 2, j++) {
                        var datetmp = new Date(Date.parse(json.conversions[i]));
                        readydata_conversions[j] = [Date.UTC(datetmp.getFullYear(), datetmp.getMonth(), datetmp.getDate()), json.conversions[i + 1]];
                    }
                   

                    var chart = new Highcharts.Chart({
                        chart: {
                            renderTo: 'chart-1',
                            defaultSeriesType: 'spline'
                        },
                        credits: {
                            enabled: false
                        },
                        title: {
                            text: ""
                        },

                        subtitle: {
                            text: ''
                        },
                        tooltip: {
                            backgroundColor: '#FCFFC5',
                            crosshairs: true,
                            shared: true
//                            formatter: function() {
//                            var s = '<b>' + this.x.getMonth() + this.x.getDate() + this.x.getFullYear() + '</b>' + 
//                            '<br/>' + '<b>' + GetDayforSlidingWindow(this.x.getDay()) + '</b>';
//            
//            $.each(this.points, function(i, point) {
//                s += '<br/>'+ point.series.name +': '+
//                 sdf_FTS(point.y,0);
//            });
            
//            return s;
//        }
                        },
                        xAxis:
                        {
                           type: 'datetime',
                            dateTimeLabelFormats: {
                                second: '%m-%d-%Y',
                                minute: '%m-%d-%Y',
                                hour: '%m-%d-%Y',
                                day: '%m-%d-%Y',
                                week: '%m-%d-%Y',
                                month: '%m-%d-%Y',
                                year: '%Y'
                            },
                            labels: {
                                step: 2,
                                style: {
                                    fontWeight: "bold",
                                    fontSize: "10px"
                                },
                                //staggerLines: 2,
                                enabled: true
                            }
                        },

                        yAxis: {
                            allowDecimals: false,
                            title: {
                                text: ''
                            }
                        },
                        series: [

                                    {
                                        name: 'Clicks',
                                        data: readydata_landings

                                    }
                                    ,
                                    {
                                        name: 'Registrations',
                                        data: readydata_billings
                                    },
                                    {
                                        name: 'Orders',
                                        data: readydata_conversions

                                    }

                                ]
                    });
                    // alert("4");
                    //                    var plot = $.jqplot('chart-1', [json.landings, json.billings, json.conversions], {
                    //                        grid: { background: '#fff', borderWidth: 0, shadow: false },
                    //                        legend: { show: true, location: 'ne', placement: 'outside' },
                    //                        series: [
                    //			                { label: 'Clicks' },
                    //                            { label: 'Registrations' },
                    //                            { label: 'Orders' }
                    //		                ],
                    //                        axes: {
                    //                            xaxis: {
                    //                                renderer: $.jqplot.DateAxisRenderer,
                    //                                showTickMarks: false
                    //                                //tickOptions: { formatString: '%#m/%#d/%#yy' }
                    //                            },
                    //                            yaxis: {
                    //                                numberTicks: 5,
                    //                                min: 0,
                    //                                tickOptions: { formatString: '%d', markSize: 0 }
                    //                            }
                    //                        }
                    //                    });
                    FetchRowsForTable(affiliate, start, end, 1);
                }
            });
        }

        function FetchSeriesForGraph2(affiliate, start, end) {
            $('#jq-loader-2').show();
            $('#jq-content-2').hide();
            $('#jq-error-2').hide();
            $('#jq-message-2').hide();
            $('#chart-2').empty();
            $("#table-2").find("tr:gt(0)").remove();
            $.ajax({
                type: 'POST',
                url: 'dotnet/campaign_performance.aspx/FetchSeriesForGraph2',
                data: '{ "affiliate": "' + affiliate + '", "start": "' + start + '", "end": "' + end + '"}',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function(msg) {
                    var json = jQuery.parseJSON(msg.d);
                    if (json.cancellations == undefined || json.cancellations == null || json.cancellations == '') {
                        $('#jq-loader-2').hide();
                        $('#jq-error-2').show();
                        $('#jq-error-2').text('No data found');
                        return;
                    }

                    $('#jq-loader-2').hide();
                    $('#jq-content-2').show();

                    var readydata_cancellations = [];
                    var readydata_refunds = [];
                    var readydata_chargebacks = [];

                    // we can't parse those three in one cycle, cause sometimes there are different lengths of those strings.
                    for (i = 0, j = 0; i < json.cancellations.length; i += 2, j++) {
                        var datetmp = new Date(Date.parse(json.cancellations[i]));
                        readydata_cancellations[j] = [Date.UTC(datetmp.getFullYear(), datetmp.getMonth(), datetmp.getDate()), json.cancellations[i + 1]];
                    }
                    for (i = 0, j = 0; i < json.refunds.length; i += 2, j++) {
                        var datetmp = new Date(Date.parse(json.refunds[i]));
                        readydata_refunds[j] = [Date.UTC(datetmp.getFullYear(), datetmp.getMonth(), datetmp.getDate()), json.refunds[i + 1]];
                    }
                    for (i = 0, j = 0; i < json.chargebacks.length; i += 2, j++) {
                        var datetmp = new Date(Date.parse(json.chargebacks[i]));
                        readydata_chargebacks[j] = [Date.UTC(datetmp.getFullYear(), datetmp.getMonth(), datetmp.getDate()), json.chargebacks[i + 1]];
                    }
                  //  alert(readydata_cancellations + "\n\n" + readydata_chargebacks + "\n\n" + readydata_refunds);
                    var chart = new Highcharts.Chart({
                        chart: {
                            renderTo: 'chart-2',
                            defaultSeriesType: 'spline'
                        },
                        credits: {
                            enabled: false
                        },
                        title: {
                            text: ""
                        },

                        subtitle: {
                            text: ''
                        },
                        tooltip: {
                            backgroundColor: '#FCFFC5',
                            crosshairs: true,
                            shared: true
                        },
                        xAxis:
                        {
                            type: 'datetime',
                            dateTimeLabelFormats: {
                                second: '%m-%d-%Y',
                                minute: '%m-%d-%Y',
                                hour: '%m-%d-%Y',
                                day: '%m-%d-%Y',
                                week: '%m-%d-%Y',
                                month: '%m-%d-%Y',
                                year: '%Y'
                            },
                            labels: {
                                step: 2,
                                style: {
                                    fontWeight: "bold",
                                    fontSize: "10px"
                                },
                                //staggerLines: 2,
                                enabled: true
                            }
                        },

                        yAxis: {
                            allowDecimals: false,
                            title: {
                                text: ''
                            }
                        },


                        series: [

                                    {
                                        name: 'Cancellations',
                                        data: readydata_cancellations

                                    }
                                    ,
                                    {
                                        name: 'Refunds',
                                        data: readydata_refunds
                                    },
                                    {
                                        name: 'Chargebacks',
                                        data: readydata_chargebacks

                                    }

                                ]
                    });
                    //                    var plot = $.jqplot('chart-2', [json.cancellations, json.refunds, json.chargebacks], {
                    //                        grid: { background: '#fff', borderWidth: 0, shadow: false },
                    //                        legend: { show: true, location: 'ne', placement: 'outside' },
                    //                        series: [
                    //			                { label: 'Cancellations' },
                    //			                { label: 'Refunds' },
                    //                            { label: 'Chargebacks' }
                    //		                ],
                    //                        axes: {
                    //                            xaxis: {
                    //                                renderer: $.jqplot.DateAxisRenderer,
                    //                                showTickMarks: false
                    //                                //tickOptions: { formatString: '%#m/%#d/%#y' }
                    //                            },
                    //                            yaxis: {
                    //                                numberTicks: 5,
                    //                                min: 0,
                    //                                tickOptions: { formatString: '%d', markSize: 0 }
                    //                            }
                    //                        }
                    //                    });
                    FetchRowsForTable(affiliate, start, end, 2);
                }
            });
        }

        function FetchSeriesForGraph3(affiliate, start, end) {
            $('#jq-loader-3').show();
            $('#jq-content-3').hide();
            $('#jq-error-3').hide();
            $('#jq-message-3').hide();
            $('#chart-3').empty();
            $("#table-3").find("tr:gt(0)").remove();
            $.ajax({
                type: 'POST',
                url: 'dotnet/campaign_performance.aspx/FetchSeriesForGraph3',
                data: '{ "affiliate": "' + affiliate + '", "start": "' + start + '", "end": "' + end + '"}',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function(msg) {
                    var json = jQuery.parseJSON(msg.d);
                    if (json.sales == undefined || json.sales == null || json.sales == '') {
                        $('#jq-loader-3').hide();
                        $('#jq-error-3').show();
                        $('#jq-error-3').text('No data found');
                        return;
                    }

                    $('#jq-loader-3').hide();
                    $('#jq-content-3').show();
                   // alert(json.sales);
                    var readydata_sales = [];

                    for (i = 0, j = 0; i < json.sales.length; i += 2, j++) {
                         var datetmp = new Date(Date.parse(json.sales[i]));
                        //alert(datetmp);
                        //var dateparts = json.sales[i].split("-");
                         readydata_sales[j] = [Date.UTC(datetmp.getFullYear(), datetmp.getMonth(), datetmp.getDate()), json.sales[i + 1]];
                    }

                    //alert(readydata_sales);
                    var chart = new Highcharts.Chart({
                        chart: {
                            renderTo: 'chart-3',
                            defaultSeriesType: 'spline'
                        },
                        credits: {
                            enabled: false
                        },
                        title: {
                            text: ""
                        },

                        subtitle: {
                            text: ''
                        },
                        tooltip: {
                            backgroundColor: '#FCFFC5',
                            crosshairs: true,
                            shared: true
                        },
                        xAxis:
                        {
                            type: 'datetime',
                            dateTimeLabelFormats: {
                                second: '%m-%d-%Y',
                                minute: '%m-%d-%Y',
                                hour: '%m-%d-%Y',
                                day: '%m-%d-%Y',
                                week: '%m-%d-%Y',
                                month: '%m-%d-%Y',
                                year: '%Y'
                            },
                            labels: {
                                step: 2,
                                style: {
                                    fontWeight: "bold",
                                    fontSize: "10px"
                                },
                                //staggerLines: 2,
                                enabled: true
                            }
                        },
                        yAxis: {
                            allowDecimals: true,
                            title: {
                                text: ''
                            },
                            labels: {
                                formatter: function() {
                                    return '$' + this.value;
                                }
                            }

                        },


                        series: [

                                    {
                                        name: 'Sales',
                                        data: readydata_sales

                                    }


                                ]
                    });

                    //                    var plot = $.jqplot('chart-3', [json.sales], {
                    //                        grid: { background: '#fff', borderWidth: 0, shadow: false },
                    //                        series: [
                    //			                { label: 'Sales' },
                    //		                ],
                    //                        legend: { show: true, location: 'ne', placement: 'outside' },
                    //                        axes: {
                    //                            xaxis: {
                    //                                renderer: $.jqplot.DateAxisRenderer,
                    //                                showTickMarks: false
                    //                                //tickOptions: { formatString: '%#m/%#d/%#Y' }
                    //                            },
                    //                            yaxis: {
                    //                                numberTicks: 5,
                    //                                min: 0,
                    //                                tickOptions: { formatString: '$%.2f', markSize: 0 }
                    //                            }
                    //                        }
                    //                    });
                    FetchRowsForTable(affiliate, start, end, 3);
                }
            });
        }

        function FetchSeriesForGraph4(affiliate, start, end) {
            $('#jq-loader-4').show();
            $('#jq-content-4').hide();
            $('#jq-error-4').hide();
            $('#jq-message-4').hide();
            $('#chart-4').empty();
            $("#table-4").find("tr:gt(0)").remove();
            $.ajax({
                type: 'POST',
                url: 'dotnet/campaign_performance.aspx/FetchSeriesForGraph4',
                data: '{ "affiliate": "' + affiliate + '", "start": "' + start + '", "end": "' + end + '"}',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (msg) {
                    var json = jQuery.parseJSON(msg.d);
                    if (json.merchantingFees == undefined || json.merchantingFees == null || json.merchantingFees == '') {
                        $('#jq-loader-4').hide();
                        $('#jq-error-4').show();
                        $('#jq-error-4').text('No data found');
                        return;
                    }

                    $('#jq-loader-4').hide();
                    $('#jq-content-4').show();

                    var readydata_merchantingFees = [];
                    var readydata_reserveAccountFees = [];

                    for (i = 0, j = 0; i < json.merchantingFees.length; i += 2, j++) {
                        var datetmp = new Date(Date.parse(json.merchantingFees[i]));
                        readydata_merchantingFees[j] = [Date.UTC(datetmp.getFullYear(), datetmp.getMonth(), datetmp.getDate()), json.merchantingFees[i + 1]];
                    }
                    for (i = 0, j = 0; i < json.reserveAccountFees.length; i += 2, j++) {
                        var datetmp = new Date(Date.parse(json.reserveAccountFees[i]));
                        readydata_reserveAccountFees[j] = [Date.UTC(datetmp.getFullYear(), datetmp.getMonth(), datetmp.getDate()), json.reserveAccountFees[i + 1]];
                    }
                    
                    var chart = new Highcharts.Chart({
                        chart: {
                            renderTo: 'chart-4',
                            defaultSeriesType: 'spline'
                        },
                        credits: {
                            enabled: false
                        },
                        title: {
                            text: ""
                        },

                        subtitle: {
                            text: ''
                        },
                        tooltip: {
                            backgroundColor: '#FCFFC5',
                            crosshairs: true,
                            shared: true
                        },
                        xAxis:
                        {
                            type: 'datetime',
                            dateTimeLabelFormats: {
                                second: '%m-%d-%Y',
                                minute: '%m-%d-%Y',
                                hour: '%m-%d-%Y',
                                day: '%m-%d-%Y',
                                week: '%m-%d-%Y',
                                month: '%m-%d-%Y',
                                year: '%Y'
                            },
                            labels: {
                                step: 2,
                                style: {
                                    fontWeight: "bold",
                                    fontSize: "10px"
                                },
                                //staggerLines: 2,
                                enabled: true
                            }
                        },
                        yAxis: {
                            allowDecimals: true,
                            title: {
                                text: ''
                            },
                            labels: {
                                formatter: function() {
                                    return '$' + this.value;
                                }
                            }

                        },


                        series: [

                                    {
                                        name: 'Merchanting Fees',
                                        data: readydata_merchantingFees

                                    },
                                    {
                                        name: 'Reserve Account Fees',
                                        data: readydata_reserveAccountFees

                                    }


                                ]
                    });
//                    var plot = $.jqplot('chart-4', [json.merchantingFees, json.reserveAccountFees], {
//                        grid: { background: '#fff', borderWidth: 0, shadow: false },
//                        series: [
//			                { label: 'Merchanting Fees' },
//			                { label: 'Reserve Account Fees' }
//		                ],
//                        legend: { show: true, location: 'ne', placement: 'outside' },
//                        axes: {
//                            xaxis: {
//                                renderer: $.jqplot.DateAxisRenderer,
//                                showTickMarks: false
//                                //tickOptions: { formatString: '%#m/%#d/%#Y' }
//                            },
//                            yaxis: {
//                                numberTicks: 5,
//                                max: 0,
//                                tickOptions: { formatString: '$%.2f', markSize: 0 }
//                            }
//                        }
//                    });
                    FetchRowsForTable(affiliate, start, end, 4);
                }
            });
        }

        function FetchSeriesForGraph5(affiliate, start, end) {
            $('#jq-loader-5').show();
            $('#jq-content-5').hide();
            $('#jq-error-5').hide();
            $('#jq-message-5').hide();
            $('#chart-5').empty();
            $("#table-5").find("tr:gt(0)").remove();


            $.ajax({
                type: 'POST',
                url: 'dotnet/campaign_performance.aspx/FetchSeriesForGraph5',
                data: '{ "affiliate": "' + affiliate + '", "start": "' + start + '", "end": "' + end + '"}',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (msg) {

                    var json = jQuery.parseJSON(msg.d);
                    if ((json.tsn == undefined || json.tsn == null || json.tsn == '') &&
                        (json.abf == undefined || json.abf == null || json.abf == '') &&
                        (json.keymail == undefined || json.keymail == null || json.keymail == '')) {
                        $('#jq-loader-5').hide();
                        $('#jq-error-5').show();
                        $('#jq-error-5').text('No data found');
                        return;
                    }
                    $('#jq-loader-5').hide();
                    $('#jq-content-5').show();

                    var readydata_tsn = [];
                    var readydata_abf = [];
                    var readydata_keymail = [];

                    if (!(json.tsn == undefined || json.tsn == null || json.tsn == '')) {
                        for (i = 0, j = 0; i < json.tsn.length; i += 2, j++) {
                            var datetmp = new Date(Date.parse(json.tsn[i]));
                            readydata_tsn[j] = [Date.UTC(datetmp.getFullYear(), datetmp.getMonth(), datetmp.getDate()), json.tsn[i + 1]];
                        }
                    }
                    if (!(json.abf == undefined || json.abf == null || json.abf == '')) {
                        for (i = 0, j = 0; i < json.abf.length; i += 2, j++) {
                            var datetmp = new Date(Date.parse(json.abf[i]));
                            readydata_abf[j] = [Date.UTC(datetmp.getFullYear(), datetmp.getMonth(), datetmp.getDate()), json.abf[i + 1]];
                        }
                    }
                    if (!(json.keymail == undefined || json.keymail == null || json.keymail == '')) {
                        for (i = 0, j = 0; i < json.keymail.length; i += 2, j++) {
                            var datetmp = new Date(Date.parse(json.keymail[i]));
                            readydata_keymail[j] = [Date.UTC(datetmp.getFullYear(), datetmp.getMonth(), datetmp.getDate()), json.keymail[i + 1]];
                        }
                    }

                    var chart = new Highcharts.Chart({
                        chart: {
                            renderTo: 'chart-5',
                            defaultSeriesType: 'spline'
                        },
                        credits: {
                            enabled: false
                        },
                        title: {
                            text: ""
                        },

                        subtitle: {
                            text: ''
                        },
                        tooltip: {
                            backgroundColor: '#FCFFC5',
                            crosshairs: true,
                            shared: true
                        },
                        xAxis:
                        {
                            type: 'datetime',
                            dateTimeLabelFormats: {
                                second: '%m-%d-%Y',
                                minute: '%m-%d-%Y',
                                hour: '%m-%d-%Y',
                                day: '%m-%d-%Y',
                                week: '%m-%d-%Y',
                                month: '%m-%d-%Y',
                                year: '%Y'
                            },
                            labels: {
                                step: 2,
                                style: {
                                    fontWeight: "bold",
                                    fontSize: "10px"
                                },
                                //staggerLines: 2,
                                enabled: true
                            }
                        },
                        yAxis: {
                            allowDecimals: true,
                            title: {
                                text: ''
                            },
                            labels: {
                                formatter: function () {
                                    return '$' + this.value;
                                }
                            }

                        },

                        series: [

//                                    {
//                                        name: 'TSN/Novaship',
//                                        data: readydata_tsn

//                                    },
                                    {
                                        name: 'ABF/USellWeShip',
                                        data: readydata_abf

                                    }//,
//                                    {
//                                        name: 'Keymail',
//                                        data: readydata_keymail

//                                    }


                                ]
                    });
                    //                    var plot = $.jqplot('chart-5', [json.tsn, json.abf, json.keymail], {
                    //                        grid: { background: '#fff', borderWidth: 0, shadow: false },
                    //                        series: [
                    //			                { label: 'TSN/Novaship' },
                    //			                { label: 'ABF/USellWeShip' },
                    //                            { label: 'Keymail' }
                    //		                ],
                    //                        legend: { show: true, location: 'ne', placement: 'outside' },
                    //                        axes: {
                    //                            xaxis: {
                    //                                renderer: $.jqplot.DateAxisRenderer,
                    //                                showTickMarks: false
                    //                                //tickOptions: { formatString: '%#m/%#d/%#Y' }
                    //                            },
                    //                            yaxis: {
                    //                                numberTicks: 5,
                    //                                max: 0,
                    //                                tickOptions: { formatString: '$%.2f', markSize: 0 }
                    //                            }
                    //                        }
                    //                    });
                    FetchRowsForTable(affiliate, start, end, 5);
                }
            });
        }

        function FetchRowsForTable(affiliate, start, end, mode) {
            var table = $('#table-' + mode);
            $.ajax({
                type: 'POST',
                url: 'dotnet/campaign_performance.aspx/FetchOrdersForTable',
                data: '{ "affiliate": "' + affiliate + '", "start": "' + start + '", "end": "' + end + '", "mode": ' + mode + '}',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (msg) {
                    var json = msg.d;
                    if (json.length > 99) {
                        $('#jq-message-' + mode).show();
                    }
                    $.template("row", $("#row"));
                    $.tmpl("row", json).appendTo(table);
                    $('.data').show();
                }
            });
        }

        $(document).ready(function () {
            var tabs = $('#tabs').tabs({
                select: function (event, ui) {
                    if (ui.index == 0) {
                        FetchSeriesForGraph1($('#affiliates').val(), date1.val(), date2.val());
                    }
                    else if (ui.index == 1) {
                        FetchSeriesForGraph2($('#affiliates').val(), date1.val(), date2.val());
                    }
                    else if (ui.index == 2) {
                        FetchSeriesForGraph3($('#affiliates').val(), date1.val(), date2.val());
                    }
                    else if (ui.index == 3) {
                        FetchSeriesForGraph4($('#affiliates').val(), date1.val(), date2.val());
                    }
                    else if (ui.index == 4) {
                        FetchSeriesForGraph5($('#affiliates').val(), date1.val(), date2.val());
                    }

                    return true;
                }
            });
            $.jqplot.config.enablePlugins = true;

            var date1 = $('#date_tbDate1');
            var date2 = $('#date_tbDate2');

            FetchSeriesForGraph1($('#affiliates').val(), date1.val(), date2.val());

            $('#update').click(function () {
                var selected = tabs.tabs('option', 'selected'); // => 0
                if (selected == 0) {
                    FetchSeriesForGraph1($('#affiliates').val(), date1.val(), date2.val());
                }
                else if (selected == 1) {
                    FetchSeriesForGraph2($('#affiliates').val(), date1.val(), date2.val());
                }
                else if (selected == 2) {
                    FetchSeriesForGraph3($('#affiliates').val(), date1.val(), date2.val());
                }
                else if (selected == 3) {
                    FetchSeriesForGraph4($('#affiliates').val(), date1.val(), date2.val());
                }
                else if (selected == 4) {
                    FetchSeriesForGraph5($('#affiliates').val(), date1.val(), date2.val());
                }
                return false;
            });
        });
   
    </script>
    <script id="row" type="text/x-jQuery-tmpl">
        <tr>
            <td><a href="/billing_edit.asp?id=${BillingID}" target="_blank">${BillingID}</a></td>            
            <td>${Name}</td>            
            <td>${SignupDate}</td>            
            <td>${Affiliate}</td>
            <td>${SubAffiliate}</td>
            <td>${Status}</td>
        </tr>
    </script>
</body>
</html>
