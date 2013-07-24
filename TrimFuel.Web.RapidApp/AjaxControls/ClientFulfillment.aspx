<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ClientFulfillment.aspx.cs"
    Inherits="TrimFuel.Web.RapidApp.AjaxControls.ClientFulfillment" %>

<script type="text/javascript">
    function editInventory(inventoryID) {
        var clientID = <%# TPClientID %>;        
        var title = (inventoryID == '' ? "New Inventory" : "Edit Inventory");
        popupControl2('inventory-edit-' + inventoryID, title, 300, 300, "ajaxControls/ClientInventoryEdit.aspx?clientId=" + clientID + "&inventoryID=" + inventoryID, function(){                                    
            if (showFulfillmentSettings) {
                showFulfillmentSettings(clientID);
            }
        });
    }
    
    function editProductCode(productCodeID) {
        var clientID = <%# TPClientID %>;        
        var title = (productCodeID == '' ? "New Product" : "Edit Product");
        popupControl2('product-edit-' + productCodeID, title, 400, 400, "ajaxControls/ClientProductCodeEdit.aspx?clientId=" + clientID + "&productCodeID=" + productCodeID, function(){                                    
            if (showFulfillmentSettings) {
                showFulfillmentSettings(clientID);
            }
        });
    }
</script>

<form id="form1" runat="server">
<h1>
    Product Groups Settings</h1>
<table cellspacing="1" class="rapidapp-alternate" style="font-size: 12px; width: 50%;">
    <tr>
        <td>
            <strong>Product</strong>
        </td>
        <td>
            <strong>Shipper</strong>
        </td>
    </tr>
    <asp:repeater id="rShipProd" runat="server">
        <ItemTemplate>
            <tr>
                <td>
                    <asp:HiddenField runat="server" ID="hdnProductID" Value='<%#Eval("ProductID")%>' />
                    <%#Eval("Product")%>
                </td>
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
        <asp:PlaceHolder runat="server" id="lSaved">
           Saved <%# DateTime.Now %> by <%# AdminMembership.CurrentAdmin.DisplayName %>
        </asp:PlaceHolder>
<table style="border: none; padding: 0px; margin: 0px;" id="table1" cellspacing="0" cellpadding="0">
    <tr>
        <td style="vertical-align: top !important;">
            <h1>
                Inventory List</h1>
            <a onclick="editInventory(''); return false;" class="addNewIcon" href="#">Add Inventory</a>
            <div class="space">
            </div>
            <table cellspacing="1" class="rapidapp-alternate" style="font-size: 12px;">
                <tr>
                    <td>
                        <strong>SKU</strong>
                    </td>
                    <td>
                        <strong>Product Name</strong>
                    </td>
                    <td>
                        <strong>Actions</strong>
                    </td>
                </tr>
                <asp:repeater runat="server" id="rInventories">
                <ItemTemplate>
                <tr>
                    <td><%# Eval("SKU") %></td>
                    <td><%# Eval("Product") %></td>
                    <td>
                        <a onclick="editInventory(<%# Eval("InventoryID") %>); return false;" class="editIcon" href="#">Edit</a>
                    </td>
                </tr>
                </ItemTemplate>
            </asp:repeater>
            </table>
        </td>
        <td style="vertical-align: top !important;">
            <h1>
                Product List</h1>
            <a onclick="editProductCode(''); return false;" style="font-size: 12px;" class="addNewIcon"
                href="#">Add Product</a>
            <div class="space">
            </div>
            <table cellspacing="1" class="rapidapp-alternate" style="font-size: 12px;">
                <tr>
                    <td>
                        <strong>Product ID</strong>
                    </td>
                    <td>
                        <strong>Virtual SKU</strong>
                    </td>
                    <td>
                        <strong>Product</strong>
                    </td>
                    <td>
                        <strong>Actions</strong>
                    </td>
                </tr>
                <asp:repeater runat="server" id="rProducts">
                <ItemTemplate>
                <tr>
                    <td><%# Eval("ProductCodeID")%></td>
                    <td><%# Eval("ProductCode_")%></td>
                    <td><%# Eval("Name")%></td>
                    <td>
                        <a onclick="editProductCode(<%# Eval("ProductCodeID") %>); return false;" class="editIcon" href="#">Edit</a>
                    </td>
                </tr>
                </ItemTemplate>
            </asp:repeater>
            </table>
        </td>
    </tr>
</table>
</form>
