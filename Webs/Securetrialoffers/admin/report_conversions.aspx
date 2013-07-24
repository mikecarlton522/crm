<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="report_conversions.aspx.cs" Inherits="Securetrialoffers.admin.report_conversions" %>
<html>
<head>
<title><%# ReportTitle %></title>
<link rel="stylesheet" href="include\style.css" type="text/css"/>
</head>
<body>
<p/>
<p class='datab' align='center'><%# ReportTitle %></p>
<table width='100%'><tr><td>
<p/>
<p>&nbsp;<b>Conversion Statistics</b></p>
<p/>
<table border="0" cellspacing="1" width="90%">
    <tr class="header">
        <td>Affiliate</td>
        <td>Sub Affiliate</td>
        <td>Date</td>
        <td>HITS TO LANDING</td>
        <td>HITS TO BILLING</td>
        <td>HITS TO UPSELL</td>
    </tr>
    <asp:Repeater runat="server" ID="rConversions">
        <ItemTemplate>
            <tr>
                <td><%# DataBinder.Eval(Container.DataItem, "Value1.Value.Affiliate") %></td>
                <td><%# DataBinder.Eval(Container.DataItem, "Value1.Value.SubAffiliate")%></td>
                <td><%# DataBinder.Eval(Container.DataItem, "Value1.Value.CreateDT")%></td>
                <td><%# DataBinder.Eval(Container.DataItem, "Value2.Value") %></td>
                <td><%# DataBinder.Eval(Container.DataItem, "Value3.Value") %></td>
                <td><%# DataBinder.Eval(Container.DataItem, "Value4.Value") %></td>
            </tr>
        </ItemTemplate>
    </asp:Repeater>
</table>
</td></tr></table>
</body>
</html>
