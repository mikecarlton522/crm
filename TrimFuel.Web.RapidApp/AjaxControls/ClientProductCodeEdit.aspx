<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ClientProductCodeEdit.aspx.cs"
    Inherits="TrimFuel.Web.RapidApp.AjaxControls.ClientProductCodeEdit" %>

<script>
    function addNewR_ProductRow() {
        var clone = $("#r_prorotype").clone(true);
        $(clone).find("input[name='quantity_proto']").attr("name", "quantity");
        $(clone).find("select[name='inventoryID_proto']").attr("name", "inventoryID");
        $(clone).appendTo("#r_products tbody").css('display', 'table-row').find().attr("name", "inventoryID");
    }
    function removeR_Row(el) {
        $(el).parent().parent().remove()
    }
</script>

<form id="form1" runat="server">
<asp:hiddenfield runat="server" id="hdnProductCodeID" value="<%# ProductCodeID %>" />
<table width="100%">
    <tr>
        <td>
            Virtual SKU:<asp:textbox style="margin-left: 5px;" runat="server" id="tbProductCode"
                cssclass="validate[required]" text='<%# ProductCode.ProductCode_ %>'></asp:textbox>
        </td>
        <td style="width: 30%;">
            <a class="addNewIcon" href="javascript:addNewR_ProductRow()">Add Product</a>
        </td>
    </tr>
</table>
<table style="width: 100%;" id="r_products">
    <tbody>
        <tr style="display: none;" id="r_prorotype">
            <td style="width: 40px;">
                <input type="text" id='TextBox' name='quantity_proto' style="width: 30px" value='' />
            </td>
            <td>
                <select name="inventoryID_proto">
                    <asp:repeater runat="server" datasource='<%# InventoryList %>'>
                            <ItemTemplate>
                                <option value='<%#Eval("InventoryID") %>'><%#Eval("Product")%></option>
                            </ItemTemplate>
                        </asp:repeater>
                </select>
            </td>
            <td>
                <a href="#" onclick="removeR_Row(this); return false;" class="removeIcon">Remove</a>
            </td>
        </tr>
        <asp:repeater id="rProductCodeInventories" runat="server">
        <ItemTemplate>
            <tr>
                <td style="width:40px;">
                    <input type="text" ID='TextBox' name='quantity' class="validate[required]" style="width:30px;" value='<%# Eval("Quantity") %>'/>
                </td>
                <td>
                    <select name="inventoryID">
                        <asp:repeater runat="server" DataSource='<%# InventoryList %>'>
                            <ItemTemplate>
                                <option value='<%#Eval("InventoryID") %>' <%# (Eval("InventoryID").ToString() == DataBinder.Eval(Container.Parent.Parent, "DataItem.InventoryID").ToString()) ? "selected='selected'" : "" %>><%#Eval("Product")%></option>
                            </ItemTemplate>
                        </asp:repeater>
                    </select>
                </td>
                <td>
                    <a href="#" onclick="removeR_Row(this); return false;" class="removeIcon">Remove</a>
                </td>
            </tr>
        </ItemTemplate>
    </asp:repeater>
    </tbody>
    <tfoot>
        <tr>
            <td colspan="3" align="right">
                <asp:button runat="server" id="btnSave" text="Save" onclick="btnSave_Click" />
            </td>
        </tr>
    </tfoot>
</table>
<asp:placeholder runat="server" id="lSaved">
Saved <%# DateTime.Now %> by <%# AdminMembership.CurrentAdmin.DisplayName %>
</asp:placeholder>
</form>
