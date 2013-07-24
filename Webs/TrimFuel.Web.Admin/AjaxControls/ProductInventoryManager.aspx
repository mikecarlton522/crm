<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProductInventoryManager.aspx.cs"
    Inherits="TrimFuel.Web.Admin.AjaxControls.ProductInventoryManager" %>

<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>

<form id="formProductInventory" runat="server">
<asp:hiddenfield id="hdnProductID" value='<%# ProductID %>' runat="server" />
<table cellspacing="1" class="process-offets sortable" style="margin-left: 0;">
    <tr class="header">
        <td>
            <strong>Product</strong>
        </td>
        <td>
            <strong>SKU</strong>
        </td>
        <td>
            <strong>Costs</strong>
        </td>
        <td>
        </td>
    </tr>
    <asp:repeater id="rInventory" runat="server" datasource="<%# InventoryList %>">
        <ItemTemplate>
            <tr>
                <td>
                    <asp:hiddenfield value='<%# Eval("InventoryID") %>' runat="server" />
                    <%#Eval("Product")%>
                </td>
                <td>
                    <%#Eval("SKU")%>
                </td>
                <td>
                    <%#Eval("Costs")%>
                </td>
                <td>
                    <asp:CheckBox runat="server" Checked='<%# ProductInventoryList.Where(u => u.InventoryID == Convert.ToInt32(Eval("InventoryID"))).Count() > 0 %>' />
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            <tr>
                <td colspan="4">
                    <asp:button text="Save Changes" runat="server" id="assertSave" onclick="SaveChanges_Click" />                    
                </td>
            </tr>
        </FooterTemplate>
    </asp:repeater>
</table>
<div class="space">
</div>
<div id="errorMsg">
    <asp:literal runat="server" id="Note"></asp:literal>
</div>
</form>
