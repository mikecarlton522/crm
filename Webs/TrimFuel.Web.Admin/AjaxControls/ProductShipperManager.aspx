<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProductShipperManager.aspx.cs"
    Inherits="TrimFuel.Web.Admin.AjaxControls.ProductShipperManager" %>

<form id="formProductShipperList" runat="server">
<asp:HiddenField ID="hdnProductID" Value='<%# ProductID %>' runat="server" />
<table cellspacing="1" class="rapidapp-alternate" style="margin-left:0;">
    <tr>
        <td>
            <strong>Shipper</strong>
        </td>
    </tr>
    <asp:repeater id="rShipProd" runat="server">
        <ItemTemplate>
            <tr>
                <td>
                    <asp:DropDownList id="dpdShippers" runat="server" datasource='<%# Shippers %>' datatextfield="Name" datavaluefield="ShipperID" selectedvalue='<%# ActiveShippersID.Contains(Convert.ToInt32(Eval("ShipperID"))) ? Eval("ShipperID") : "0"%>' />
                </td>
            </tr>
        </ItemTemplate>
    </asp:repeater>
    <tr>
        <td colspan="2" align="right">
            <asp:Button runat="server" ID="btnSave" Text="Save" onclick="btnSave_Click" />
        </td>
    </tr>
</table>
<div class="space">
</div>
<div id="errorMsg">
    <asp:literal runat="server" id="Note"></asp:literal>
</div>
</form>
