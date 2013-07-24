<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProductManager.aspx.cs"
    Inherits="TrimFuel.Web.Admin.AjaxControls.ProductManager" %>

<form id="fmProd" runat="server" style="border: 0; text-align: left">
<asp:hiddenfield id="hdnProductID" value='<%# ProductID %>' runat="server" />
<table border="0" cellpadding="0" cellspacing="0" style="margin-left: 0;">
    <tr>
        <td>
            Product Type ID
        </td>
        <td>
            <asp:label id="lblProductID" runat="server" text='<%# ProductProp.ProductID %>' />
        </td>
    </tr>
    <tr>
        <td>
            Product Name
        </td>
        <td>
            <asp:textbox id="tbProductName" runat="server" text='<%# ProductProp.ProductName %>' />
        </td>
    </tr>
    <%-- <tr>
        <td>
            Code
        </td>
        <td>
            <asp:textbox id="tbCode" runat="server" text='<%# ProductProp.Code %>' />
        </td>
    </tr>--%>
    <tr>
        <td colspan="2">
            <asp:checkbox id="cbActive" text=" Active" runat="server" checked='<%# ProductProp.ProductIsActive %>' />
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <div style="float: right;">
                <asp:button id="btnSubmit" text="Save" runat="server" onclick="Save_Click" />
            </div>
        </td>
    </tr>
</table>
<div class="space">
</div>
<div id="errorMsg">
    <asp:literal runat="server" id="Note"></asp:literal>
</div>
</form>
