<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ClientProductGroups.aspx.cs" Inherits="TrimFuel.Web.RapidApp.AjaxControls.ClientProductGroups" %>
<script type="text/javascript">
    function editProductGroup(productID) {
        var clientID = <%# TPClientID %>;        
        var title = (productID == '' ? "New Product Group" : "Edit Product Group");
        popupControl2('product-group-edit-' + productID, title, 300, 200, "ajaxControls/ClientProductEdit.aspx?clientId=" + clientID + "&productID=" + productID, function(){                        
            if (ShowProductGroups) {
                ShowProductGroups();
            }
        }, function(){
            alternateTables('product-group-edit-' + productID);
        });
    }
</script>
<form id="form1" runat="server">
<a onclick="editProductGroup(''); return false;" class="addNewIcon" href="#">Add Product Group</a>
<div style="height:5px;"></div>
<table cellspacing="1" class="rapidapp-alternate">
    <tr>
        <td width="30%"><strong>Name</strong></td>
        <td><strong>Actions</strong></td>
    </tr>
<asp:Repeater runat="server" ID="rProducts" OnItemCommand="rProducts_ItemCommand">
    <ItemTemplate>
    <tr>
        <td><%# Eval("ProductName") %></td>
        <td>
            <a onclick="editProductGroup(<%# Eval("ProductID") %>); return false;" class="editIcon" href="#">Edit</a>
            &nbsp;&nbsp;
            <asp:LinkButton runat="server" ID="lbDelete" Text="Delete" CssClass="confirm removeIcon" CommandName="delete" CommandArgument='<%# Eval("ProductID") %>' ></asp:LinkButton>
        </td>
    </tr>
    </ItemTemplate>
</asp:Repeater>
</form>
