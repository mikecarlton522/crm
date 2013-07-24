<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ClientInventoryEdit.aspx.cs" Inherits="TrimFuel.Web.RapidApp.AjaxControls.ClientInventoryEdit" %>
<form id="form1" runat="server">
<asp:HiddenField runat="server" ID="hdnInventoryID" Value="<%# InventoryID %>" />
<table width="100%">
    <tr>
        <td>SKU</td>
        <td><asp:TextBox runat="server" ID="tbSKU" CssClass="validate[required]" Text='<%# Inventory.SKU %>'></asp:TextBox></td>
    </tr>
    <tr>
        <td>Name</td>
        <td><asp:TextBox runat="server" ID="tbProduct" CssClass="validate[required]" Text='<%# Inventory.Product %>'></asp:TextBox></td>
    </tr>
    <tr>
        <td colspan="2" align="right">
            <asp:Button runat="server" ID="btnSave" Text="Save" onclick="btnSave_Click" />
        </td>
    </tr>    
</table>
<asp:PlaceHolder runat="server" id="lSaved">
Saved <%# DateTime.Now %> by <%# AdminMembership.CurrentAdmin.DisplayName %>
</asp:PlaceHolder>
</form>
