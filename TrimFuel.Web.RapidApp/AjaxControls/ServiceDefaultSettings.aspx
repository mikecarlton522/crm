<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ServiceDefaultSettings.aspx.cs"
    Inherits="TrimFuel.Web.RapidApp.AjaxControls.ServiceDefaultSettings" %>

<script type="text/javascript">
    $("#form1").ready(function() {
        alternate('table');
        alternate('table1');
        alternate('table2');
    });
</script>

<form runat="server" id="form1">
<asp:hiddenfield runat="server" id="hdnServiceID" value="<%# ServiceID %>" />
<asp:hiddenfield runat="server" id="hdnServiceName" value="<%# ServiceName %>" />
<asp:hiddenfield runat="server" id="hdnServiceType" value="<%# ServiceType %>" />
<h1>
    Configuration</h1>
    <asp:textbox id="tbShipperName" text='' runat="server" style="display: none;" />
<table cellspacing="1" id="table">
 <asp:repeater id="rFees" runat="server" datasource='<%# Fees %>'>
        <ItemTemplate>
            <tr>
                <td style="width:30%;">
                    <asp:HiddenField runat="server" Value='<%#Eval("Key")%>' />
                    <%#Eval("DisplayName")%>
                </td>
                <td>
                    <asp:TextBox runat="server" Text='<%#Eval("Value")%>' />
                </td>
            </tr>
        </ItemTemplate>
    </asp:repeater>
</table>
<div id="bottom">
    <div class="left">
        <asp:PlaceHolder runat="server" id="lSaved">
           Saved <%# DateTime.Now %> by <%# AdminMembership.CurrentAdmin.DisplayName %>
        </asp:PlaceHolder>
    </div>
    <div class="right">
        <asp:Button Text="Save Changes" runat="server" onclick="SaveChanges_Click" />
    </div>
</div>
</form>
