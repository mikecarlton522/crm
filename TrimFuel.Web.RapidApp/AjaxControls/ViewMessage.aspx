<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewMessage.aspx.cs" Inherits="TrimFuel.Web.RapidApp.AjaxControls.ViewMessage" %>

<form id="form1" runat="server">
<table cellspacing="1" cellpadding="1" id="table" class="rapidapp-alternate" style="font-size: 12px; width:100%;">
    <asp:repeater id="rRows" runat="server" datasource='<%# Rows %>' >
        <ItemTemplate>
            <tr>
                <td style="width:40%;">
                    <strong><%# Eval("Key") %></strong>
                </td>
                <td>
                    <%# Eval("Value") %>
                </td>
            </tr>
        </ItemTemplate>
    </asp:repeater>
</table>
</form>
