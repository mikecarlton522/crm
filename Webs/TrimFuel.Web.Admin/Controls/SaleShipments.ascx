<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SaleShipments.ascx.cs"
    Inherits="TrimFuel.Web.Admin.Controls.SaleShipments" %>
<%@ Register Assembly="TrimFuel.Web.Admin" Namespace="TrimFuel.Web.Admin.Logic" TagPrefix="cc1" %>
<%@ Register Assembly="TrimFuel.Web.UI" Namespace="TrimFuel.Web.UI.Specialized" TagPrefix="cc2" %>
<form runat="server">
    <script type="text/javascript">
        function clickCancel() {
            $('#btnCancelShipment').hide();
        }
    </script>
<asp:HiddenField ID="hdnSaleID" runat="server" Value="<%# SaleID %>" />
<table class="process-offets" width="100%">
    <tr class="header">
        <td>
            Date
        </td>
        <td>
            Status
        </td>
        <td>
            Shipper
        </td>
        <td>
            Description
        </td>
    </tr>
    <asp:Repeater runat="server" ID="rRequestList">
        <ItemTemplate>
            <tr>
                <td>
                    <%# Eval("CreateDT") %>
                </td>
                <td>
                    <%# TrimFuel.Model.Enums.ShipmentStatusEnum.Name[Convert.ToInt32(Eval("ResultShipmentStatus"))] %>
                </td>
                <td>
                    <%# (Eval("Shipper") != null ? Eval("Shipper.Name") : "CRM Note") %>
                </td>
                <td>
                    <%# HttpUtility.HtmlEncode(Convert.ToString(Eval("EventText")))%>
                </td>
            </tr>
        </ItemTemplate>
    </asp:Repeater>
    <asp:PlaceHolder runat="server" Visible='<%# RequestList != null && RequestList.Count == 0 %>'>
        <tr>
            <td colspan="4">
                No records
            </td>
        </tr>
    </asp:PlaceHolder>
</table>
<div class="space"></div>
<div class="module">
    <h2>
        Edit Tracking #</h2>
    <table border="0" cellspacing="0" cellpadding="3" class="editForm">
        <tr>
            <td>
                Tracking #
            </td>
            <td>
                <asp:TextBox runat="server" CssClass="xwide" ID="txtTrackingNumber" Text="<%# TrackingNumber %>" />
            </td>
        </tr>
        <asp:PlaceHolder runat="server" Visible="<%# string.IsNullOrEmpty(TrackingNumber) %>" >
            <tr>
                <td>
                    Shipper
                </td>
                <td>
                    <cc2:ShipperDDL runat="server" ID="ShipperDDL1" />
                </td>
            </tr>
        </asp:PlaceHolder>
        <tr>
            <td>
                <asp:Button class="submit" runat="server" ID="btnSendShipment" Text="Update" OnClick="btnUpdate_Click" />
            </td>
            <td>
                <asp:Button runat="server" ID="btnCancelShipment" Text="Cancel" OnClick="btnCancel_Click" OnClientClick="return clickCancel();" Visible='<%# IsBlockingAllowed == true %>'/>
            </td>
        </tr>
     </table>
    <cc1:Error ID="Error1" Type="Error" runat="server"></cc1:Error>
</div>
</form>
