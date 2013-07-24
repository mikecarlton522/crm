<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/Admin.Master" AutoEventWireup="true" CodeBehind="management_virtual_terminal.aspx.cs" Inherits="TrimFuel.Web.Admin.management_virtual_terminal" %>
<%@ Import Namespace="TrimFuel.Business" %>
<%@ Import Namespace="TrimFuel.Model.Enums" %>
<%@ Register assembly="TrimFuel.Web.UI" namespace="TrimFuel.Web.UI.Specialized" tagprefix="cc1" %>
<%@ Register assembly="TrimFuel.Business" namespace="TrimFuel.Business.Controls" tagprefix="cc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphScript" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphStyle" runat="server">
<style type="text/css">
    table.editForm tr.subheader td { padding:5px; background-color:#e4e4e4; font-weight:bold; }
    .pre {
        background-color: #E5E5CC;
        border: 1px solid #F0F0E0;
        font-family: Courier New;
        font-size: x-small;
        margin-top: -5px;
        padding: 5px;
    }
</style>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
<form id="frmMain" runat="server">
    <div id="toggle" class="section">
	    <a href="#"><h1>Virtual Terminal</h1></a>
	    <div>
            MID: <cc1:AssertigyMidDDL runat="server" ID="ddlMID" CssClass="validate[required]"></cc1:AssertigyMidDDL>
            <div class="space"></div>
            <div class="module">
                <h2>Test Charge</h2>
                <table class="editForm" style="float:left;">
                    <tr class="subheader"><td colspan="2">Billing Address</td></tr>
                    <tr><td>First Name</td><td><asp:TextBox ID="tbFirstName" runat="server"></asp:TextBox></td></tr>
                    <tr><td>Last Name</td><td><asp:TextBox ID="tbLastName" runat="server"></asp:TextBox></td></tr>
                    <tr><td>Address 1</td><td><asp:TextBox ID="tbAddress1" runat="server"></asp:TextBox></td></tr>
                    <tr><td>Address 2</td><td><asp:TextBox ID="tbAddress2" runat="server"></asp:TextBox></td></tr>
                    <tr><td>City</td><td><asp:TextBox ID="tbCity" runat="server"></asp:TextBox></td></tr>
                    <tr><td>State</td><td><asp:TextBox ID="tbState" runat="server"></asp:TextBox></td></tr>
                    <tr><td>Country</td><td><asp:TextBox ID="tbCountry" runat="server"></asp:TextBox></td></tr>
                    <tr><td>Zip</td><td><asp:TextBox ID="tbZip" runat="server"></asp:TextBox></td></tr>
                    <tr><td>Phone</td><td><asp:TextBox ID="tbPhone" runat="server"></asp:TextBox></td></tr>
                    <tr><td>Email</td><td><asp:TextBox ID="tbEmail" runat="server"></asp:TextBox></td></tr>
                    <tr><td>IP</td><td><asp:TextBox ID="tbIP" runat="server"></asp:TextBox></td></tr>
                </table>
                <table class="editForm" style="float:left;">
                    <tr class="subheader"><td colspan="2">Amount</td></tr>
                    <tr><td>Amount</td><td><asp:TextBox ID="tbAmount" runat="server" ></asp:TextBox></td></tr>
                    <tr><td>Currency </td><td><cc1:CurrencyDDL ID="ddlCurrency" CssClass="validate[required]" runat="server" Width="150"></cc1:CurrencyDDL></td></tr>
                    <% if (Config.Current.APPLICATION_ID == ApplicationEnum.TwoBoxCRM){ %>
                    <tr><td>Installment </td><td><asp:DropDownList ID="ddlInstallment" Width="150px" runat="server" /></td></tr>
                    <%}%>
                    <tr class="subheader"><td colspan="2">Credit Card Information</td></tr>
                    <tr><td>Credit Card</td><td><asp:TextBox ID="tbCreditCard" runat="server"></asp:TextBox></td></tr>
                    <tr><td>CVV</td><td><asp:TextBox ID="tbCVV" runat="server"></asp:TextBox></td></tr>
                    <tr><td>Exp Date</td><td><cc2:DDLMonth ID="ddlExpMonth" runat="server" Width="79" Mode="TwoDigits"></cc2:DDLMonth> / <cc2:DDLYear runat="server" ID="ddlExpYear" Width="61"></cc2:DDLYear></td></tr>
                    <tr><td></td><td><asp:Button runat="server" ID="btnCharge" Text="Process Charge" onclick="btnCharge_Click" /></td></tr>
                </table>
                <div class="space"></div>
                <div class="pre">
                    <span style="max-width:400px;display:block;">
                    <asp:Literal runat="server" ID="lChargeResponse">Response appears here</asp:Literal>
                    </span>
                </div>
            </div>
        </div>
    </div>
</form>
</asp:Content>
