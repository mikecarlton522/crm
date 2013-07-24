<%@ Page Language="C#" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="css/home.css" rel="stylesheet" type="text/css" />
    <link src="http://ajax.googleapis.com/ajax/libs/jquery/1.6.1/jquery.min.js" type="text/javascript"/>

<script type="text/javascript" src="../js/highcharts.js"></script>
<script type="text/javascript" src="../js/exporting.js"></script>
    <script type="text/javascript" src="../js/jquery.min.js"></script>
    <script language="javascript" type="text/javascript" src="../js/jquery.jqplot.min.js"></script>
    <script language="javascript" type="text/javascript" src="../js/plugins/jqplot.barRenderer.min.js"></script>
    <script language="javascript" type="text/javascript" src="../js/plugins/jqplot.categoryAxisRenderer.min.js"></script>
    <script language="javascript" type="text/javascript" src="../js/plugins/jqplot.pointLabels.min.js"></script>
	<script language="javascript" type="text/javascript" src="../js/plugins/jqplot.ClickableBars.js"></script>
    <link rel="stylesheet" type="text/css" href="../css/jquery.jqplot.min.css" />
    <script type="text/javascript">
        $(document).ready(function() {
            var output = $('#output');
            $.ajax({
                type: 'POST',
                url: 'dotnet/Services/FailReason.aspx/Reasons',
                data: '{"startDT": "<%= Request["startDT"] %>", "endDT": "<%= Request["endDT"] %>", "groupMode": "<%= Request["groupMode"] %>", "affiliate": "<%= Request["affiliate"] %>", "subAffiliate": "<%= Request["subAffiliate"] %>", "mid": "<%= Request["mid"] %>", "statusTID": <%= string.IsNullOrEmpty(Request["statusTID"]) ? "-1" : Request["statusTID"]  %>, "productID": <%= string.IsNullOrEmpty(Request["productID"]) ? "-1" : Request["productID"] %>, "salesType": <%= string.IsNullOrEmpty(Request["salesType"]) ? "0" : Request["salesType"]%>, "excludeReattempts": <%= string.IsNullOrEmpty(Request["excludeReattempts"]) ? "0" : Request["excludeReattempts"]%>, "paymentTypeID": "<%= Request["paymentTypeID"] %>"}',
                contentType: 'application/json; charset=utf-8',
                // string start_date: 'Request["startDT"]',
                //string end_date = 'Request["endDT"]',
                //string aff = 'Request["affiliate"]',
                dataType: 'json',
                success: function(msg) {
                    var json = msg.d;
                    var series = [];
                    var ticks = [];
                    var bids = [];
                    var a = 0;
//                    function GetBIDByReasonName(str) {
//                        var i = 0;
//                        while (str != ticks[i]) {
//                            i++;
//                        }
//                        return bids[i];
//                    }
                    if (json.AvsRejected != 0) {
                        series[a] = [json.AvsRejected];
                        ticks[a] = "AVS Rejected";
                       // bids[a] = json.AvsRejectedBID;
                        a++;
                    }

                    if (json.CallVoiceCenter != 0) {
                        series[a] = [json.CallVoiceCenter];
                        ticks[a] = "Call Voice Center";
                      //  bids[a] = json.CallVoiceCenterBID;
                        a++;
                    }
                    if (json.CurrencyNotAvailable != 0) {
                        series[a] = [json.CurrencyNotAvailable];
                        ticks[a] = "Currency Not Available";
                      //  bids[a] = json.CurrencyNotAvailableBID;
                        a++;
                    }
                    if (json.CvvFailure != 0) {
                        series[a] = [json.CvvFailure];
                        ticks[a] = "Cvv Failure";
                      //  bids[a] = json.CvvFailureBID;
                        a++;
                    }
                    if (json.DeclinedByIssuer != 0) {
                        series[a] = [json.DeclinedByIssuer];
                        ticks[a] = "Declined By Issuer";
                      //  bids[a] = json.DeclinedByIssuerBID;
                        a++;
                    }
                    if (json.DeclinedByRiskManagement != 0) {
                        series[a] = [json.DeclinedByRiskManagement];
                        ticks[a] = "Declined By Risk Management";
                      //  bids[a] = json.DeclinedByRiskManagementBID;
                        a++;
                    }
                    if (json.DuplicateTransaction != 0) {
                        series[a] = [json.DuplicateTransaction];
                        ticks[a] = "Duplicate Transaction";
                      //  bids[a] = json.DuplicateTransactionBID;
                        a++;
                    }
                    if (json.ExpiredCard != 0) {
                        series[a] = [json.ExpiredCard];
                        ticks[a] = "Expired Card";
                       // bids[a] = json.ExpiredCardBID;
                        a++;
                    }
                    if (json.GatewayError != 0) {
                        series[a] = [json.GatewayError];
                        ticks[a] = "Gateway Error";
                       // bids[a] = json.GatewayErrorBID;
                        a++;
                    }
                    if (json.InsufficientFunds != 0) {
                        series[a] = [json.InsufficientFunds];
                        ticks[a] = "Insufficient Funds";
                       // bids[a] = json.InsufficientFundsBID;
                        a++;
                    }
                    if (json.InvalidCardNumber != 0) {
                        series[a] = [json.InvalidCardNumber];
                        ticks[a] = "Invalid Card Number";
                      //  bids[a] = json.InvalidCardNumberBID;
                        a++;
                    }
                    if (json.InvalidExpirationDate != 0) {
                        series[a] = [json.InvalidExpirationDate];
                        ticks[a] = "Invalid Expiration Date";
                     //   bids[a] = json.InvalidExpirationDateBID;
                        a++;
                    }
                    if (json.InvalidRefundAmount != 0) {
                        series[a] = [json.InvalidRefundAmount];
                        ticks[a] = "Invalid Refund Amount";
                     //   bids[a] = json.InvalidRefundBID;
                        a++;
                    }
                    if (json.LostOrStolenCard != 0) {
                        series[a] = [json.LostOrStolenCard];
                        ticks[a] = "Lost Or Stolen Card";
                     //   bids[a] = json.LostOrStolenCardBID;
                        a++;
                    }
                    if (json.TransactionAlreadyVoided != 0) {
                        series[a] = [json.TransactionAlreadyVoided];
                        ticks[a] = "Transaction Already Voided";
                      //  bids[a] = json.TransactionAlreadyVoidedBID;
                        a++;
                    }
                    //alert("Data collected" + "\n\n" + "Starting heighchart configuration");
                    var amount = [0];
                    for (k = 0; k < a; k++) {
                        amount[k] = k;
                    }
                    document.getElementById("loadingIMG").style.visibility = "hidden";

                    //   alert(start_date);
                    var options = {
                        chart: {

                            renderTo: 'chart',
                            defaultSeriesType: 'column'
                        },

                        title: { text: "" },
                        credits: {
                            enabled: false
                        },
                        legend: {
                            layout: 'vertical',
                            backgroundColor: '#EEE9E9',
                            align: 'left',
                            verticalAlign: 'top',
                            x: 10,
                            y: -10,
                            floating: false,
                            shadow: true
                        },
                        plotOptions: {
                            series: {
                                borderWidth: 1,
                                borderColor: 'black',
                                pointPadding: 0,
                                minPointLength: 10,
                                // pointWidth: 90,
                                cursor: 'pointer',
                                events: {
                                click: function() {
                                //alert('<%=Request["startDT"]%>' + "\n\n" + <%=Request["startDT"]%>);
                                       // Session["startDT"] = '<%=Request["startDT"]%>';
                                      //  Session["endDT"] = '<%=Request["endDT"]%>';
                                     //   Session["BIDS"] = GetBIDByReasonName(this.name);
                                    window.open('https://dashboard.trianglecrm.com/dotnet/failed_order_report.aspx?startDT=' + '<%=Request["startDT"]%>' + '&endDT=' + '<%=Request["endDT"]%>' + '&affiliate=' + '<%=Request["affiliate"]%>' + '&reason=' + this.name + '&statusTID=' + '<%=Request["statusTID"] %>' + '&mid=' + '<%=Request["mid"] %>' + '&salesType=' + '<%=Request["salesType"]%>' + '&excludeReattempts=' + '<%=Request["excludeReattempts"]%>', "_blank");
                                    },
                                    legendItemClick: function() {
                                      //  Session["BIDS"] = GetBIDByReasonName(this.name);
                                        window.open('https://dashboard.trianglecrm.com/dotnet/failed_order_report.aspx?startDT=' + '<%=Request["startDT"]%>' + '&endDT=' + '<%=Request["endDT"]%>' + '&affiliate=' + '<%=Request["affiliate"]%>' + '&reason=' + this.name + '&statusTID=' + '<%=Request["statusTID"] %>' + '&mid=' + '<%=Request["mid"] %>' + '&salesType=' + '<%=Request["salesType"]%>' + '&excludeReattempts=' + '<%=Request["excludeReattempts"]%>', "_blank");
                                        return false;
                                    }
                                }
                            },
                            column: {
                                stacking: 'normal',


                                borderWidth: 0,
                                dataLabels: {

                                    enabled: true,

                                    y: this.y,
                                    color: '#000000'


                                }

                            }
                        },
                        tooltip: {
                            backgroundColor: '#FCFFC5',
                            formatter: function() {
                                return this.series.name + ': ' + this.y;
                            }
                        },
                        xAxis: { labels: { enabled: false },
                            categories: amount
                        },
                        yAxis: {
                            allowDecimals: false,
                            title: "",
                            labels: { enabled: true }
                        },
                        series: []
                    };

                    for (k = 0; k < amount.length; k++) {
                        options.series.push({
                            type: "column",
                            name: ticks[k],
                            data: [{
                                name: ticks[k],
                                y: parseInt(series[k]),
                                x: k
}]
                            })
                        }

                        // alert("heighchart configuration completed." + "\n\n" + "Starting building chart with " + a + " types of errors");

                        //                var plot2 = $.jqplot('chart', series, {
                        //                    grid: { background: '#fff', borderWidth: 0, shadow: false, drawGridLines: false, gridLineWidth: 0 },
                        //                    seriesDefaults: {
                        //                        renderer: $.jqplot.BarRenderer,
                        //                        rendererOptions: { fillToZero: true },
                        //                        pointLabels: { show: true, location: 'n', edgeTolerance: -15, formatString: '%d' }
                        //                    },
                        //                    series: { label: ticks[0] },
                        //                    legend: {
                        //                        show: true,
                        //                        placement: 'outsideGrid',
                        //                        labels: ticks,
                        //                    },
                        //					ClickableBars: {
                        //						onClick: function(i, j, data) {
                        //                            window.open("https://dashboard.trianglecrm.com/dotnet/failed_order_report.aspx?bids=" + bids[i], "_blank");
                        //						}
                        //					},
                        //                    axes: {
                        //                        xaxis: {
                        //                            showTicks: false
                        //                        },
                        //                        yaxis: {
                        //                            showTicks: false
                        //                        }
                        //                    }
                        //                });
                        var chart = new Highcharts.Chart(options);
                        //  alert("Finishing building chart");


                    }
                });

            });
         
    </script>
</head>
<body> 
    <div class="module">
        <img src="images/loading.gif" id="loadingIMG" />
        <div id="chart" style="width: 900px; height: 400px;">
        </div> 
     </div>
</body>
</html>
