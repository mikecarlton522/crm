<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProductCodeManager.aspx.cs" Inherits="TrimFuel.Web.Admin.AjaxControls.ProductCodeManager" %>
<%@ Register assembly="TrimFuel.Web.Admin" namespace="TrimFuel.Web.Admin.Logic" tagprefix="cc1" %>
<script type="text/javascript">
    function editProductCode(productCodeId) {
        popupControl2("popup-multi-product", "Product", 500, 500, "/controls/product_code.asp?productCodeId=" + productCodeId, null, null, function () {
            if (initProductCodeManager)
                initProductCodeManager();
        });
    }

    function newProductCode() {
        editProductCode('');
    }
</script>
<form id="form1" runat="server">
<h2>Virtual SKU List</h2>
<cc1:Error ID="Error1" Type="Error" runat="server" />
<table class='process-offets sortable add-csv-export' border="0" cellpadding="0" cellspacing="1" width="100%">
	<tr class="header">
		<td>Product ID</td>
		<td>Virtual SKU</td>
		<td>Product</td>
        <td></td>
	</tr>
    <asp:Repeater runat="server" ID="rProductCodeList" OnItemCommand="rProductCodeList_ItemCommand">
        <ItemTemplate>
            <tr>
                <td><%# Eval("ProductCodeID") %></td>
                <td><%# Eval("ProductCode_")%></td>
                <td><%# Eval("Name") %></td>
                <td nowrap>
                    <a href="javascript:void(0)" onclick="editProductCode(<%# Eval("ProductCodeID") %>);" class="editIcon">Edit</a>
                    <asp:LinkButton runat="server" ID="lbDeleteProductCode" CommandName="DeleteProductCode" CommandArgument='<%# Eval("ProductCodeID") %>' CssClass="removeIcon confirm">Delete</asp:LinkButton>
                </td>
            </tr>
        </ItemTemplate>
    </asp:Repeater>
    <asp:PlaceHolder runat="server" ID="phNoRecords">
        <tr><td colspan="6">No records</td></tr>
    </asp:PlaceHolder>
</table>
</form>