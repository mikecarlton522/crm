<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SMTP.aspx.cs" Inherits="TrimFuel.Web.Admin.EditForms.SMTP" %>

<div id="smtp-form">
    <form runat="server">
    <div class="module" style="width: 95%">
        <h2>
            Edit SMTP</h2>
        <table class="editForm" border="0" cellspacing="1" cellpadding="0">
            <tr>
                <td>
                    UserName
                </td>
                <td>
                    <input type="text" name="userName" id="userName" class="validate[required]" maxlength="100"
                        value="<%= Setting.UserName %>" />
                </td>
            </tr>
            <tr>
                <td>
                    Password
                </td>
                <td>
                    <input type="text" name="password" id="password" class="validate[required]" maxlength="100"
                        value="<%= Setting.Password %>" />
                </td>
            </tr>
            <tr>
                <td>
                    Server
                </td>
                <td>
                    <input type="text" name="server" id="server" class="validate[required]" maxlength="100"
                        value="<%= Setting.Server  %>" />
                </td>
            </tr>
            <tr>
                <td>
                    Port
                </td>
                <td>
                    <input type="text" name="port" id="port" class="validate[required]" maxlength="8"
                        value="<%= Setting.Port  %>" />
                </td>
            </tr>
            <tr>
                <td>
                    Enable SSL
                </td>
                <td>
                    <input type="checkbox" <%= Setting.EnableSSL == true ? "checked='checked'" : "" %>
                        name="ssl" id="ssl" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:button runat="server" onclick="Save" text="Save" />
                    <asp:button runat="server" onclick="Reset_Click" onclientclick='$("#smtp-form input").removeClass("validate[required]");'
                        text="Reset Settings to Default" />
                </td>
            </tr>
        </table>
    </form>
    <div class="space">
    </div>
    <div id="errorMsg">
        <asp:literal runat="server" id="Note"></asp:literal>
    </div>
</div>
