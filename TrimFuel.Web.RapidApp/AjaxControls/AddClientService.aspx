<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddClientService.aspx.cs"
    Inherits="TrimFuel.Web.RapidApp.AjaxControls.AddClientService" %>

<script type="text/javascript">
    function add() {
        $("#right").html("<img src='../images/loading2.gif' />");
        var type = $("#tbServiceType").val();
        var serviceID = $("#dpdServices").val();
        var service = $("#dpdServices option:selected").html();
        var clientId = $("#tbTPClientID").val()
        if (type.toLowerCase() == "fulfillment") {
            inlineControl('ajaxControls/ClientFulfillmentService.aspx?ClientId=' + clientId + '&serviceID=' + serviceID + '&service=' + service + "&new=new", 'right', null, function() {
                alternateTables('right');
            });
        }
        if (type.toLowerCase() == "outbound") {
            inlineControl('ajaxControls/ClientOutboundService.aspx?ClientId=' + clientId + '&serviceID=' + serviceID + '&service=' + service + "&new=new", 'right', null, function() {
                alternateTables('right');
            });
        }
        if (type.toLowerCase() == "gateway") {
            inlineControl('ajaxControls/ClientGatewayService.aspx?ClientId=' + clientId + '&service=' + service + "&new=new", 'right', null, function() {
                alternateTables('right');
            });
        }
        $(".ui-button-text").click();
    }
</script>

<form id="form1" runat="server">
<asp:textbox style="display: none;" id="tbTPClientID" runat="server" text='' />
<asp:textbox style="display: none;" id="tbServiceType" runat="server" text='' />
<table>
    <tr>
        <td>
            <span>Services</span>
        </td>
        <td>
            <asp:dropdownlist id="dpdServices" runat="server" datatextfield="ServiceName" datavaluefield="ID" />
        </td>
    </tr>
    <tr>
        <td>
            <a href="#" onclick="add(); return false;" class="addNewIcon">Add</a>
        </td>
    </tr>
</table>
</form>
