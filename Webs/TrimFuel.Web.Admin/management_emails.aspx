<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/Admin.Master" AutoEventWireup="true" CodeBehind="management_emails.aspx.cs" Inherits="TrimFuel.Web.Admin.management_emails" EnableViewState="false" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphScript" runat="server">
<script type="text/javascript" src="/ckeditor/ckeditor.js"></script>
<script type="text/javascript">
    function emailSettings(productId, productName) {
        popupControl2("popup-email-settings-" + productId, productName + " Email Settings", 720, 800, "ajaxControls/ProductEmailManager.aspx?productId=" + productId);
    }
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphStyle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
<div id="toggle" class="section">
	<a href="#">
	<h1>Email Management</h1>
	</a>
</div>
<div class="data">
    <form runat="server">
    <table class="process-offets sortable" width="100%">
        <tr class="header">
            <td>ID</td>
            <td>Name</td>
            <td>Tasks</td>
        </tr>
        <asp:Repeater runat="server" ID="rProducts">        
            <ItemTemplate>
            <tr>
                <td><%# Eval("ProductID") %></td>
                <td><%# Eval("ProductName") %></td>
                <td><a href='javascript:emailSettings(<%# Eval("ProductID") %>, "<%# Eval("ProductName") %>")' class="editIcon">Edit Email Settings</a></td>
            </tr>
            </ItemTemplate>
        </asp:Repeater>
    </table>
    </form>
	<div class="space">
	</div>
</div>
</asp:Content>
