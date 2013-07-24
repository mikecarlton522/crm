<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrderFreeItem.aspx.cs" Inherits="TrimFuel.Web.Admin.AjaxControls.OrderFreeItem" %>
<%@ Register src="../Controls/Address.ascx" tagname="Address" tagprefix="uc1" %>
<%@ Register src="../Controls/DropDownFreeItem.ascx" tagname="DropDownFreeItem" tagprefix="uc2" %>
<%@ Register assembly="TrimFuel.Web.UI" namespace="TrimFuel.Web.UI" tagprefix="cc1" %>
<%@ Register assembly="TrimFuel.Web.Admin" namespace="TrimFuel.Web.Admin.Logic" tagprefix="cc3" %>
<%@ Register Assembly="TrimFuel.Web.UI" Namespace="TrimFuel.Web.UI.Specialized" TagPrefix="cc2" %>
<form id="form1" runat="server">
<cc1:if id="ifCustomerExists" runat="server">
<div class="module">
    <h2>Send Free Shipment</h2>
    <table border="0" cellspacing="0" cellpadding="3" class="editForm">
        <tr><td>Product Group</td><td><cc2:ProductDDL ID="freeProductID" runat="server" class="validate[required]" /></td></tr>
        <tr><td>Product</td><td><uc2:DropDownFreeItem ID="ddlFreeItem" runat="server" CssClass="validate[required]" ProductGroupID="freeProductID" /></td></tr>
        <tr><td>Quantity</td><td><asp:TextBox runat="server" ID="tbQuantity" CssClass="validate[custom[Numeric]] xnarrow" MaxLength="2"></asp:TextBox></td></tr>
        <tr><td></td><td><asp:Button runat="server" ID="btnSendShipment" Text="Send Shipment" OnClick="btnSendShipment_Click" ></asp:Button></td></tr>
        <tr><td colspan="2">
            <cc3:Error ID="Error1" Type="Error" runat="server"></cc3:Error>
        </td></tr>
    </table>
</div>
</cc1:if>
<cc1:if id="ifCustomerDoesntExist" runat="server">
    Customer not found
</cc1:if>
</form>
