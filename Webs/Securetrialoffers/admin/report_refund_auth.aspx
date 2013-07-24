<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="report_refund_auth.aspx.cs" Inherits="Securetrialoffers.admin.report_refund_auth" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="TrimFuel.Model.Views" %>
<html>
<head>
<title><%= ReportTitle %></title>
<script type="text/javascript" src="js/jquery-1.4.2.min.js"></script>
<script language="JavaScript" type="text/javascript">
    function doRefund(billingID) {
        var params = "{billingID:'" + billingID + "'}";
        $.ajax({
            cache: false,
            async: false,
            type: "POST",
            //TODO
            //url: "../../trimfuel/RegistrationService.asmx/DoRefund",
            url: "../../WebServices/PaymentProcessing/RegistrationService.asmx/DoRefund",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: params,
            success: function(msg) {
                if (msg.d.State == 1 && msg.d.ReturnValue) {
                    alert("Refund successful");
                    $("#tr_" + billingID).hide();
                }
                else if (msg.d.State == 1) {
                    alert("Credit successful");
                }
                else {
                    alert("Refund and Credit attempts unsuccessful");
                }
            },
            error: function(xhr, textStatus, errorThrown) {
                //alert("Can't connect web service");
                alert(xhr.responseText);
            }
        });
    }

    function removeRefundRequest(billingID) {
        var params = "{billingID:'" + billingID + "'}";
        $.ajax({
            cache: false,
            async: false,
            type: "POST",
            //url: "../../trimfuel/RegistrationService.asmx/RemoveRefundRequest",
            url: "../../WebServices/PaymentProcessing/RegistrationService.asmx/RemoveRefundRequest",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: params,
            success: function(msg) {
                if (msg.d) {
                    doRefundSuccess(billingID);
                }
                else {
                    doRefundFaild(billingID);
                }
            },
            error: function(xhr, textStatus, errorThrown) {
                alert(xhr.responseText);
            }
        });
    }

    function billingEdit(billingID) {
        window.open("../billing_edit.asp?mode=short&id=" + billingID); 
    }
</script>
<link rel="stylesheet" href="include\style.css" type="text/css"/>
</head>
<body>
<p/>
<p class='datab' align='center'><%= ReportTitle %></p>
<table width='100%'><tr><td>
<p/>
<p>&nbsp;<b>Returns List by Date</b></p>
<p/>
<% IList<ReturnsReportView> reportData = GetReportData();
if (reportData == null) { %>
<p>&nbsp;Error occured while getting report data, see log file</p>
<% } else { %>
<table border="0" cellspacing="1" width="90%">
<tr><td colspan=8 class='datab'>By Date</td></tr>
<tr class="header">
    <td  >Date Returned</td>
    <td  nowrap>RMA Provided</td>
    <td  >Order ID</td>
    <td  >Name</td>
    <td  >Reason</td>
    <td  >Tools</td>
</tr>
<% foreach (ReturnsReportView item in reportData) { %>
<tr id='tr_<%= item.BillingID %>'>
    <td><%= item.RefundCreateDT.Value.ToShortDateString() %></td>
    <td align='left'><%= item.CallRMA %></td>
    <td align='left'><%= item.BillingID %></td>
    <td align='left'><%= string.Format("{0} {1}", item.FirstName, item.LastName) %></td>
    <td align='left'><%= item.RefundReason %></td>
    <td>
        <a href='#' onclick='billingEdit("<%= item.BillingID %>");'>Details</a>&nbsp;
        <a href='#' onclick='doRefund("<%= item.BillingID %>");'>Process</a>&nbsp;
        <a href='#' onclick='removeRefundRequest("<%= item.BillingID %>");'>Remove</a>
    </td>
</tr>
<% }    
} %>
</table>
</td></tr></table>
</body>
</html>
