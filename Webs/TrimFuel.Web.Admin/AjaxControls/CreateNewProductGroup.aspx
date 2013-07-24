<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CreateNewProductGroup.aspx.cs" Inherits="TrimFuel.Web.Admin.AjaxControls.CreateNewProductGroup" %>

<form id="fmProd" runat="server" style="border: 0; text-align: left">
<table border="0" cellpadding="0" cellspacing="0" style="margin-left: 0;">
    <tr>
        <td>
            Product Group
        </td>
        <td>
            <asp:DropDownList id="ddlProductGroup" DataSource='<%# Products %>' DataTextField="ProductName" DataValueField="ProductID" runat="server"></asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td>
            Product Name
        </td>
        <td>
            <asp:textbox id="tbProductName" runat="server"/>        
         </td>
    </tr>
    <tr>
        <td colspan="2">
            <div style="float: right;">
                <asp:button id="btnSubmit" text="Create" runat="server" onclick="Create_Click" />
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

