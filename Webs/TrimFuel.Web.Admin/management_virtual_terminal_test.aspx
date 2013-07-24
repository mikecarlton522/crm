<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/Admin.Master" AutoEventWireup="true" CodeBehind="management_virtual_terminal_test.aspx.cs" Inherits="TrimFuel.Web.Admin.management_virtual_terminal_test" %>
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
                <h2>Test Auth/Capture/Refund</h2>
                <table class="editForm" style="float:left;">
                    <tr class="subheader"><td colspan="2">Billing Address</td></tr>
                    <tr><td>First Name</td><td><asp:TextBox ID="tbAuth_FirstName" runat="server"></asp:TextBox></td></tr>
                    <tr><td>Last Name</td><td><asp:TextBox ID="tbAuth_LastName" runat="server"></asp:TextBox></td></tr>
                    <tr><td>Address 1</td><td><asp:TextBox ID="tbAuth_Address1" runat="server"></asp:TextBox></td></tr>
                    <tr><td>Address 2</td><td><asp:TextBox ID="tbAuth_Address2" runat="server"></asp:TextBox></td></tr>
                    <tr><td>City</td><td><asp:TextBox ID="tbAuth_City" runat="server"></asp:TextBox></td></tr>
                    <tr><td>State</td><td><asp:TextBox ID="tbAuth_State" runat="server"></asp:TextBox></td></tr>
                    <tr><td>Country</td><td><asp:TextBox ID="tbAuth_Country" runat="server"></asp:TextBox></td></tr>
                    <tr><td>Zip</td><td><asp:TextBox ID="tbAuth_Zip" runat="server"></asp:TextBox></td></tr>
                    <tr><td>Phone</td><td><asp:TextBox ID="tbAuth_Phone" runat="server"></asp:TextBox></td></tr>
                    <tr><td>Email</td><td><asp:TextBox ID="tbAuth_Email" runat="server"></asp:TextBox></td></tr>
                    <tr><td>IP</td><td><asp:TextBox ID="tbAuth_IP" runat="server"></asp:TextBox></td></tr>
                </table>
                <table class="editForm" style="float:left;">
                    <tr class="subheader"><td colspan="2">Amount</td></tr>
                    <tr><td>Amount</td><td><asp:TextBox ID="tbAuth_Amount" runat="server"></asp:TextBox></td></tr>
                    <tr><td>Reference(fake BillingID)</td><td><asp:TextBox ID="tbAuth_BillingID" runat="server"></asp:TextBox></td></tr>
                    <tr><td>Currency </td><td><cc1:CurrencyDDL ID="tbCurrency" CssClass="validate[required]" runat="server" Width="150"></cc1:CurrencyDDL></td></tr>
                    <tr class="subheader"><td colspan="2">Credit Card Information</td></tr>
                    <tr><td>Credit Card</td><td><asp:TextBox ID="tbAuth_CreditCard" runat="server"></asp:TextBox></td></tr>
                    <tr><td>CVV</td><td><asp:TextBox ID="tbAuth_CVV" runat="server"></asp:TextBox></td></tr>
                    <tr><td>Exp Date</td><td><cc2:DDLMonth ID="ddlAuth_expDate" runat="server" Width="79" Mode="TwoDigits"></cc2:DDLMonth> / <cc2:DDLYear runat="server" ID="ddlAuth_expYear" Width="61"></cc2:DDLYear></td></tr>
                    <tr><td></td><td><asp:Button runat="server" ID="Button1" Text="Process Auth" onclick="btnAuth_Click" /></td></tr>
                    <tr><td></td><td><asp:Button runat="server" ID="Button5" Text="Process Sale" onclick="btnSale_Click" /></td></tr>
                    <tr><td></td><td></td></tr>
                    <tr><td></td><td></td></tr>
                    <tr><td></td><td><asp:Button runat="server" ID="Button2" Text="Process Capture" onclick="btnCapture_Click" /></td></tr>
                    <tr><td></td><td><asp:Button runat="server" ID="Button4" Text="Process Refund" onclick="btnRefund_Click" /></td></tr>
                    <tr><td></td><td><asp:Button runat="server" ID="Button3" Text="Process Void" onclick="btnVoid_Click" /></td></tr>
                </table>
                <div class="space"></div>
                <div class="pre">
                    <span style="max-width:400px;display:block;">
                    <asp:Label runat="server" ID="litAuth_Request"></asp:Label>                    
                    </span>
                </div>
                <div class="pre">
                    <span style="max-width:400px;display:block;">
                    <asp:Literal runat="server" ID="litAuth_Response">Response appears here</asp:Literal>
                    </span>
                </div>
            </div>
        </div>
    </div>
</form>
</asp:Content>
