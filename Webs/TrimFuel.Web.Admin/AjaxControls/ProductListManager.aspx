<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProductListManager.aspx.cs"
    Inherits="TrimFuel.Web.Admin.AjaxControls.ProductListManager" %>

<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<script type="text/javascript">
    $(document).ready(function () {
        $(".editMode").hide();
    })
    function editClick(el) {
        cancelEditClick();
        var tr = $(el).closest('tr');
        $(tr).find('.viewMode').hide();
        $(tr).find('.editMode').show();
    }
    function cancelEditClick() {
        $('.viewMode').show();
        $('.editMode').hide();
    }
</script>
<form id="formProductList" runat="server">
<asp:hiddenfield id="hdnProductID" value='<%# ProductID %>' runat="server" />
<table cellspacing="1" class="process-offets sortable" style="margin-left: 0;">
    <tr class="header">
        <td>
            <strong>Product</strong>
        </td>
        <td>
            <strong>Virtual SKU</strong>
        </td>
        <td>
            <strong>Retail Price</strong>
        </td>
        <td>
        </td>
        <td>
        </td>
    </tr>
    <asp:repeater id="rProducts" runat="server">
        <ItemTemplate>
            <tr>
                <td>
                    <%#Eval("Key.Name")%>
                </td>
                <td>
                    <%#Eval("Key.ProductCode_")%>
                </td>
                <td>
                    <asp:hiddenfield value='<%#Eval("Key.ProductCode_")%>' runat="server" />
                    <asp:Label class="viewMode" Text='<%# GetRetailPrice(Eval("Key.ProductCode_").ToString())%>' runat="server" />
                    <asp:TextBox class="editMode" width="50" Text='<%# GetRetailPriceWithoutCurrencySymbol(Eval("Key.ProductCode_").ToString())%>' runat="server" />
                </td>
                <td>
                    <input type="checkbox" <%# Convert.ToBoolean(Eval("Value")) ? "checked" : "" %> value='<%# Eval("Key.ProductCodeID") %>' name='prodCodeID' />
                </td>
                <td>
                    <a href="javascript:void(0);" ID="A3" onclick="editClick(this)" class="editIcon viewMode" >Edit</a>
                    <asp:LinkButton runat="server" ID="lbSave" Text="Save" CssClass="submit saveIcon editMode" style="display:none;" OnClick="btnSavePrice_Click" ></asp:LinkButton>
                    <a href="javascript:void(0);" ID="A4" onclick="cancelEditClick(this);" class="cancelIcon editMode" style="display:none;">Cancel</a>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            <tr>
                <td colspan="5">
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
