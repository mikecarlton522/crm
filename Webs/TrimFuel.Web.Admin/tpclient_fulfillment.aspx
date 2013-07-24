<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/Admin.Master" AutoEventWireup="true" CodeBehind="tpclient_fulfillment.aspx.cs"
    Inherits="TrimFuel.Web.Admin.tpclient_fulfillment" %>

<%@ Register Src="Controls/DateFilter.ascx" TagName="DateFilter" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphScript" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphStyle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <form runat="server" id="frmMain">
    <div id="toggle" class="section">
        <a href="#">
            <h1>
                Quick View</h1>
        </a>
        <uc1:DateFilter ID="dateFilter" runat="server" />
    </div>
    <div id="buttons">
        <asp:Button runat="server" ID="btnGo" Text="Generate Report" OnClick="btnGo_Click" />
    </div>
    <div class="data">
        <table border="0" cellspacing="1">
            <tr class="header">
                <td>Client</td>
                <td>Postage Used</td>
                <td>Postage Allowed</td>
                <td>Shipments Posted #</td>
            </tr>
            <asp:Repeater runat="server" ID="repPostage">
                <ItemTemplate>
                    <tr>
                        <td><%# Eval("ClientName") %></td>
                        <td><%# Convert.ToDecimal(Eval("PostageUsed")).ToString("C2") %></td>
                        <td><%# Convert.ToDecimal(Eval("PostageAllowed")).ToString("C2") %></td>
                        <td><%# Eval("ShipmentCount") %></td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>
    </div>
    </form>
</asp:Content>
