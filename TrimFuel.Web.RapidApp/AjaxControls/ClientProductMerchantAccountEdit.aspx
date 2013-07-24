<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ClientProductMerchantAccountEdit.aspx.cs"
    Inherits="TrimFuel.Web.RapidApp.AjaxControls.ClientProductMerchantAccountEdit" %>

<form id="Form" runat="server">
<asp:textbox style="display: none;" id="tbProductID" runat="server" text='<%# ProductID %>' />
<asp:textbox style="display: none;" id="tbTPClientID" runat="server" text='<%# TPClientID %>' />
<table>
    <tr>
        <td>
            <span>Currency</span>
        </td>
        <td>
            <asp:DropDownList ID="dpCurrency" runat="server" datasource='<%# CurrencyList %>' datatextfield="CurrencyName" datavaluefield="CurrencyID" selectedvalue='<%# CurrCurrency %>' />
        </td>
    </tr>
    <tr>
        <td>
            <span>Merchant Accounts</span>
        </td>
        <td>
            <asp:repeater id="rProducts" runat="server" datasource='<%# MerchantAccounts %>'>
                <ItemTemplate>
                    <asp:CheckBox Text='<%#Eval("Key.DisplayName", " {0}") %>' checked='<%#Eval("Value") %>' runat="server" />
                    <br />
                </ItemTemplate>
            </asp:repeater>
        </td>
    </tr>
    <tr>
        <td colspan="2" align="right">
            <asp:button text="Save" runat="server" id="assertSave" onclick="btnSave_Click" />
        </td>
    </tr>
</table>
<asp:PlaceHolder runat="server" id="lSaved">
Saved <%# DateTime.Now %> by <%# AdminMembership.CurrentAdmin.DisplayName %>
</asp:PlaceHolder>
</form>
