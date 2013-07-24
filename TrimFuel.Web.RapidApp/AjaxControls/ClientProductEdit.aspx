<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ClientProductEdit.aspx.cs" Inherits="TrimFuel.Web.RapidApp.AjaxControls.ClientProductEdit" %>
<form id="form1" runat="server">
<asp:HiddenField runat="server" ID="hdnProductID" Value="<%# ProductID %>" />
<table width="100%">
    <tr>
        <td>Name</td>
        <td><asp:TextBox runat="server" ID="tbProductName" CssClass="validate[required]" Text='<%# Product.ProductName %>'></asp:TextBox></td>
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
