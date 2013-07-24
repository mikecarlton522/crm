<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SaleReturn.aspx.cs" Inherits="TrimFuel.Web.Admin.AjaxControls.SaleReturn" %>
<%@ Register assembly="TrimFuel.Web.UI" namespace="TrimFuel.Web.UI" tagprefix="cc1" %>
<%@ Register assembly="TrimFuel.Web.Admin" namespace="TrimFuel.Web.Admin.Logic" tagprefix="cc3" %>
<div class="module" style="width:97%">
<form id="form1" runat="server">
<cc1:if id="ifSaleExists" runat="server">
    <table class="editForm">
        <tr>
            <td>Return reason/state</td>
            <td><asp:DropDownList runat="server" ID="ddlReason">
                <asp:ListItem Value="Good for Resale">Good for Resale</asp:ListItem>
                <asp:ListItem Value="Damaged">Damaged</asp:ListItem>
                <asp:ListItem Value="Refused">Refused</asp:ListItem>
                <asp:ListItem Value="Invalid/Incorrect Address">Invalid/Incorrect Address</asp:ListItem>
                <asp:ListItem Value="Need Replacement">Need Replacement</asp:ListItem>
                <asp:ListItem Value="Fraud">Fraud</asp:ListItem>
            </asp:DropDownList></td>
        </tr>
        <tr>
            <td>Notes</td>
            <td><asp:TextBox runat="server" ID="tbNotes"></asp:TextBox></td>
        </tr>
        <tr>
            <td></td>
            <td><asp:Button runat="server" ID="btnReturn" Text="Process Return" OnClick="btnReturn_Click" /></td>
        </tr>
        <tr>
            <td colspan="2">
                <cc3:Error ID="Error1" Type="Error" runat="server"></cc3:Error>
                <cc3:Error ID="Error2" Type="Notification" runat="server"></cc3:Error>
            </td>
        </tr>
    </table>
</cc1:if>
<cc1:if id="ifSaleDoesntExist" runat="server">
    <h2>Sale was not found</h2>
</cc1:if>
</form>
</div>
