<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InventoryEdit.aspx.cs" Inherits="TrimFuel.Web.Admin.AjaxControls.InventoryEdit" %>
<%@ Register assembly="TrimFuel.Web.Admin" namespace="TrimFuel.Web.Admin.Logic" tagprefix="cc1" %>
<form id="form1" runat="server">
<asp:HiddenField runat="server" ID="hdnInventoryID" Value='<%# InventoryID %>'></asp:HiddenField>
    <table class="editForm" cellspacing="0" cellpadding="3" border="0" width="100%">
        <tr><td>SKU</td><td><asp:TextBox runat="server" ID="tbSKU" MaxLength="50" CssClass="validate[required]" Enabled='<%# InventoryID == null %>'></asp:TextBox></td></tr>
        <tr><td>Product Name</td><td><asp:TextBox runat="server" ID="tbProductName" MaxLength="50" CssClass="validate[required]"></asp:TextBox></td></tr>
        <tr><td>Cost</td><td><asp:TextBox runat="server" ID="tbCost" MaxLength="10" CssClass="validate[custom[Amount]]"></asp:TextBox></td></tr>
        <tr><td>In Stock</td><td><asp:TextBox runat="server" ID="tbQty" MaxLength="10" CssClass="validate[custom[Numeric]]"></asp:TextBox></td></tr>
        <tr><td>Price Retail</td><td><asp:TextBox runat="server" ID="tbRetailPrice" MaxLength="10" CssClass="validate[custom[Amount]]"></asp:TextBox></td></tr>
        <tr><td>Does not ship</td><td><asp:CheckBox runat="server" ID="chbDoesNotShip"></asp:CheckBox></td></tr>
        <asp:PlaceHolder runat="server" ID="phShippers">
        <tr><td colspan="2" class="subheader" style="text-align:left;">Shipper SKU mapping</td></tr>
        <asp:Repeater runat="server" ID="rpMapping">
            <ItemTemplate>
                <tr>
                <td><%# Eval("Value1.Name") %></td>
                <td>
                    <asp:HiddenField runat="server" ID="hdnShipperID" Value='<%# Eval("Value1.ShipperID") %>'></asp:HiddenField>
                    SKU: <asp:TextBox runat="server" ID="tbSKU" MaxLength="50" Text='<%# (Eval("Value2") != null ? Eval("Value2.InventorySKU_") : "")  %>'></asp:TextBox>
                </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
        </asp:PlaceHolder>
        <tr><td colspan="2">
            <div style="float:right;">                
                <asp:Button runat="server" ID="btnSave" Text="Save" OnClick="btnSave_Click" />
            </div>
            <cc1:Error ID="Error1" Type="Error" runat="server" />
        </tr>        
    </table>
</form>