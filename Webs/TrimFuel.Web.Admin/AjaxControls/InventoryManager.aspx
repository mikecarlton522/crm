<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InventoryManager.aspx.cs" Inherits="TrimFuel.Web.Admin.AjaxControls.InventoryManager" %>
<%@ Register assembly="TrimFuel.Web.Admin" namespace="TrimFuel.Web.Admin.Logic" tagprefix="cc1" %>
<script type="text/javascript">
    function editInventory(inventoryID) {
        popupControl2("popup-inventory-edit", (inventoryID == '' ? "Create Inventory" : "Edit Inventory"),
            400, 400, "/dotNet/ajaxControls/InventoryEdit.aspx?inventoryID=" + inventoryID, null, null,
            function(){
                if (initInventoryManager)
                    initInventoryManager();
            });
    }

    function addInventory() {
        editInventory('');
    }
</script>
<form id="form1" runat="server">
<h2>Inventory List</h2>
<cc1:Error ID="Error1" Type="Error" runat="server" />
<table class='process-offets sortable add-csv-export' border="0" cellpadding="0" cellspacing="1" width="100%">
	<tr class="header">
		<td>SKU</td>
		<td>Product Name</td>
		<td>Cost</td>
        <td>In Stock</td>
        <td>Retail Price</td>
        <td></td>
	</tr>
    <asp:Repeater runat="server" ID="rInventoryList" OnItemCommand="rInventoryList_ItemCommand">
        <ItemTemplate>
            <tr>
                <td nowrap><%# Eval("SKU") %></td>
                <td><%# Eval("Product") %></td>
                <td><%# ShowPrice(Eval("Costs")) %></td>
                <td><%# Eval("InStock") %></td>
                <td><%# ShowPrice(Eval("RetailPrice"))%></td>
                <td nowrap>
                    <a href="javascript:void(0)" onclick="editInventory(<%# Eval("InventoryID") %>);" class="editIcon">Edit</a>
                    <asp:LinkButton runat="server" ID="lbDeleteInventory" CommandName="DeleteInventory" CommandArgument='<%# Eval("InventoryID") %>' CssClass="removeIcon confirm">Delete</asp:LinkButton>
                </td>
            </tr>
        </ItemTemplate>
    </asp:Repeater>
    <asp:PlaceHolder runat="server" ID="phNoRecords">
        <tr><td colspan="6">No records</td></tr>
    </asp:PlaceHolder>
</table>
</form>