<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/Admin.Master" AutoEventWireup="true" CodeBehind="reports_referers_sales.aspx.cs" Inherits="TrimFuel.Web.Admin.reports_referers_sales" %>
<%@ Register src="Controls/DateFilter.ascx" tagname="DateFilter" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphScript" runat="server">
<script type="text/javascript" language="javascript">
    function showRefererSales(refId, refName) {
        popupControl2("referer-sales-" + refId, refName, 800, 400, "AjaxControls/RefererSales.aspx?refererId=" + refId);
        return false;
    }
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphStyle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
<form runat="server" id="frmMain">
<div id="toggle" class="section">
	<a href="#">
	<h1>Quick View</h1>
	</a>
	<uc1:datefilter ID="DateFilter1" runat="server" />
</div>
<div id="buttons">
    <asp:Button runat="server" ID="btnGo" Text="Generate Report" onclick="btnGo_Click" />
</div>
<div class="data" id="tabs">
    <table class="process-offets sortable add-csv-export" border='0' cellspacing='1' width='100%'>
        <tr class="header">
            <td>ID</td>
            <td>Name</td>
            <td>Company</td>
            <td>Referer Code</td>
            <td>Primary Sales</td>
            <td>Secondary Sales</td>
            <td>Earnings</td>
        </tr>
        <asp:Repeater runat="server" ID="rReferersSales">
            <ItemTemplate>
                <tr>
                    <td><a href='#' onclick='return showRefererSales(<%# Eval("[\"RefererID\"]") %>, "<%# Eval("[\"FullName\"]") %>");'><%# Eval("[\"RefererID\"]") %></a></td>
                    <td><%# Eval("[\"FullName\"]") %></td>
                    <td><%# Eval("[\"Company\"]")%></td>
                    <td><%# Eval("[\"RefererCode\"]")%></td>
                    <td><%# Eval("[\"PrimarySalesCount\"]")%></td>
                    <td><%# Eval("[\"SecondarySalesCount\"]")%></td>
                    <td><%# Convert.ToDecimal(Eval("[\"SalesCommission\"]")).ToString("c")%> E-Cigs Dollars</td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
              <tr class="total">
                <td>Total</td>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
                <td><%# TotalPrimarySales %></td>
                <td><%# TotalSecondarySales %></td>
                <td><%# TotalSalesCommission.ToString("c") %> E-Cigs Dollars</td>
              </tr>
            </FooterTemplate>
        </asp:Repeater>
        <asp:PlaceHolder runat="server" ID="phNoSales" Visible="false">
            <tr class="white">
              <td colspan="7">No records found</td>
            </tr>
        </asp:PlaceHolder>
    </table>
    <div class="space"></div>
</div>
</form>
</asp:Content>
