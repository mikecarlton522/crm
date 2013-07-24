<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SyncCategories.aspx.cs" Inherits="TrimFuel.Web.Admin.AjaxControls.SyncCategories" EnableViewState="true" %>
<%@ Register Assembly="TrimFuel.Web.UI" Namespace="TrimFuel.Web.UI" TagPrefix="gen" %>
<form id="formSyncCategories" runat="server">
<table class="process-offets sortable" width="100%">
<tr>
    <th>
        Product
    </th>
    <th>
        Current Category
    </th>
    <th>
        Magento Category
    </th>
</tr>
    <asp:Repeater ID="rCategories" runat="server" OnItemDataBound="rCategories_ItemDataBound">
        <ItemTemplate>
            <tr>
                <td>
                        <asp:CheckBox runat="server" ID="checkBox" Text='<%#Eval("product")%>' ></asp:CheckBox>
                </td>
                <td>
                    <%#Eval("oldCategory")%>
                </td>
                <td>
                    <%#Eval("newCategory")%>
                </td>
            </tr>
        </ItemTemplate>
    </asp:Repeater>
</table>
<asp:button ID="ConfirmButton" runat="server" text="Confirm" onclick="ConfirmButton_Click" />
<asp:label ID="LabelNoItems" runat="server"></asp:label>
</form>
