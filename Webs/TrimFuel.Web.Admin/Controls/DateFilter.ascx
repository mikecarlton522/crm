<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DateFilter.ascx.cs" Inherits="TrimFuel.Web.Admin.Controls.DateFilter" %>
<%@ Register Assembly="TrimFuel.Web.UI" Namespace="TrimFuel.Web.UI" TagPrefix="cc1" %>
<div>
    <script type="text/javascript" language="javascript">
        var date1Values_<%# this.ClientID %> = [<%# Date1Values %>];
        var date2Values_<%# this.ClientID %> = [<%# Date2Values %>];
        function dateSelect_<%# this.ClientID %>(ds) {
            $('#<%# tbDate1.ClientID %>').val(date1Values_<%# this.ClientID %>[ds]);
            $('#<%# tbDate2.ClientID %>').val(date2Values_<%# this.ClientID %>[ds]);
        }

        function initCalendar_<%# this.ClientID %>() {
            $("#<%# tbDate1.ClientID %>").datepicker({
                showOn: 'button',
                buttonImage: '../images/icons/calendar.png',
                buttonImageOnly: true
            });
            $("#<%# tbDate2.ClientID %>").datepicker({
                showOn: 'button',
                buttonImage: '../images/icons/calendar.png',
                buttonImageOnly: true
            });
        }

        $(document).ready(function(){
            initCalendar_<%# this.ClientID %>();
        });
    </script>
	<div class="module">
		<h2>Date</h2>
        <cc1:If ID="If1" runat="server" Condition='<%# Mode == PredefinedMode.Backward %>'>
    		<a href="JavaScript:dateSelect_<%# this.ClientID %>(0);">Yesterday</a> | <a href="JavaScript:dateSelect_<%# this.ClientID %>(1);">Today</a> | <a href="JavaScript:dateSelect_<%# this.ClientID %>(2);">This Week</a> | <a href="JavaScript:dateSelect_<%# this.ClientID %>(3);">Last Week</a> | <a href="JavaScript:dateSelect_<%# this.ClientID %>(4);">This Month</a> | <a href="JavaScript:dateSelect_<%# this.ClientID %>(5);">Last Month</a>
        </cc1:If>
        <cc1:If ID="If2" runat="server" Condition='<%# Mode == PredefinedMode.Forward %>'>
    		<a href="JavaScript:dateSelect_<%# this.ClientID %>(0);">Tomorrow</a> | <a href="JavaScript:dateSelect_<%# this.ClientID %>(1);">Next 7 Days</a> | <a href="JavaScript:dateSelect_<%# this.ClientID %>(2);">Next 30 Days</a> | <a href="JavaScript:dateSelect_<%# this.ClientID %>(3);">Next 3 Months</a> | <a href="JavaScript:dateSelect_<%# this.ClientID %>(4);">Next 6 Months</a>
        </cc1:If>
	</div>
	<div class="module">
		<h2>Date Range</h2>
		Start
		<asp:TextBox runat="server" ID="tbDate1" MaxLength="10"></asp:TextBox>
		&nbsp; End
		<asp:TextBox runat="server" ID="tbDate2" MaxLength="10"></asp:TextBox>		
	</div>
	<div class="clear">
	</div>
</div>
