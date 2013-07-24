<%@ Page Title="" Language="C#" MasterPageFile="~/admin/Controls/Admin.Master" AutoEventWireup="true" CodeBehind="report.aspx.cs" Inherits="Securetrialoffers.admin.report" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphScript" runat="server">
<script language="JavaScript" type="text/javascript">
    $(document).ready(function() {
        dateSelect();
        initCalendars();
    });

    function initCalendars() {
        $(function() {
            $("#date1").datepicker({
                showOn: 'button',
                buttonImage: 'images/img_calendaricon.gif',
                buttonImageOnly: true
            });
        });
        $(function() {
            $("#date2").datepicker({
                showOn: 'button',
                buttonImage: 'images/img_calendaricon.gif',
                buttonImageOnly: true
            });
        });
    }

    function makeToday() {
        formReport.date1.value="<%=TodayDate%>";
        formReport.date2.value="<%=TodayDate%>";
    }

    function makeThisWeek() {
        formReport.date1.value="<%=WeekStartDate%>";
        formReport.date2.value="<%=TodayDate%>";
    }

    function makeThisMonth() {
        formReport.date1.value="<%=MonthStartDate%>";
        formReport.date2.value="<%=TodayDate%>";
    }

    function makeYesterday() {
        formReport.date1.value="<%=YesterdayDate%>";
        formReport.date2.value="<%=YesterdayDate%>";
    }

    function dateSelect() {
        if (formReport.dateselecter.value==0){
        formReport.date1.value="";
        formReport.date2.value="";
        }
        if (formReport.dateselecter.value==1){
        makeToday();
        }
        if (formReport.dateselecter.value==2){
        makeYesterday();
        }
        if (formReport.dateselecter.value==3){
        makeThisWeek();
        }
        if (formReport.dateselecter.value==4){
        makeThisMonth();
    }
}
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<form target='reportmain' name='formReport' method='post' action="<%= Request.Params["type"] %>.aspx">
<center>
<h2><%= Request.Params["caption"] %></h2>
<table>
  <tr class="header">
    <td  class="shortWidth">&nbsp;</td>
    <td style="padding-right:5px;"></td>
    <td></td>
  </tr>
<tr><td>Date Range:</td><td>
<select onchange="JavaScript:dateSelect();" name='dateselecter' >
<option value='0'>Specific (Enter below)</option>
<option value='1'>Today</option>
<option value='2' selected>Yesterday</option>
<option value='3'>This Week</option>
<option value='4'>This Month</option>
</select>
</td>
<td></td></tr>
<tr><td>
Start Date:</td><td><input type="text" size="20" maxLength="20" name="date1" id="date1" value="<%=StartDate%>" />                 
</td><td></td></tr>
<tr><td>
End Date:</td><td><input type="text" size="20" maxLength="20" name="date2" id="date2" value="<%=EndDate%>" />
</td>
<td><input type="submit" value="Generate Report"></td></tr>
</table>
</center>
</form>
<iframe name="reportmain" style="width: 100%; height: 550px; border: 0; overflow: visible">
</iframe>
</asp:Content>
