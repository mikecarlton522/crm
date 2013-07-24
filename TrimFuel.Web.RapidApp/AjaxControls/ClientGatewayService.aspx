<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ClientGatewayService.aspx.cs"
    Inherits="TrimFuel.Web.RapidApp.AjaxControls.ClientGatewayService" %>

<script type="text/javascript">
    $("#gatewayForm").ready(function () {
        alternate('table');
        alternate('table1');
    });
    function editMID(MIDID) {
        popupControl2("edit-assertigyMIDForm", "Edit Merchant Account", 600, 400, "ajaxControls/AssertigyMIDForm.aspx?nmiCompanyId=<%# ServiceID %>&assertigyMIDID=" + MIDID + "&ClientId=<%# TPClientID %>");
    }
    function addMID() {
        popupControl2("add-assertigyMIDForm", "Add Merchant Account", 600, 400, "ajaxControls/AssertigyMIDForm.aspx?nmiCompanyId=<%# ServiceID %>&ClientId=<%# TPClientID %>");
    }
</script>
<form runat="server" id="gatewayForm">
<h1>
    <%#HeaderTitle%></h1>
<asp:hiddenfield runat="server" id="hdnCompanyID" value="<%# ServiceID %>" />
<asp:hiddenfield runat="server" id="hdnServiceName" value="<%# ServiceName %>" />
<asp:hiddenfield runat="server" id="hdnTPClientID" value="<%# TPClientID %>" />
<table cellspacing="1" id="table">
    <tr>
        <td width="30%">
            Company Name
        </td>
        <td>
            <asp:textbox id="tbCompanyName" text='<%# NMIProp.CompanyName %>' runat="server" />
        </td>
    </tr>
    <tr>
        <td>
            Gateway Username
        </td>
        <td>
            <asp:textbox id="tbGatewayUsername" text='<%# NMIProp.GatewayUsername %>' runat="server" />
        </td>
    </tr>
    <tr>
        <td>
            Gateway Password
        </td>
        <td>
            <asp:textbox id="tbGatewayPassword" text='<%# NMIProp.GatewayPassword %>' runat="server" />
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <asp:checkbox id="cbActive" checked='<%# NMIProp.Active %>' text=" Active" runat="server" />
        </td>
    </tr>
</table>
<div runat="server" id="MID">
    <h1 class="margintop10">
        Merchant Accounts</h1>
    <a href="#" class="addNewIcon" onclick="addMID(); return false;">Add Merchant Account</a>
    <table cellspacing="1" id="table1" style="margin-top: 10px;">
        <tr>
            <td>
                <strong>MID</strong>
            </td>
            <td>
                <strong>Display Name</strong>
            </td>
            <td>
                <strong>Monthly Cap</strong>
            </td>
            <td>
                <strong>Transaction Fee (Retail)</strong>
            </td>
            <td>
                <strong>Transaction Fee</strong>
            </td>
            <td>
                <strong>Chargeback Fee (Retail)</strong>
            </td>
            <td>
                <strong>Chargeback Fee</strong>
            </td>
            <td>
                <strong>Chargeback Representation Fee (Retail)</strong>
            </td>
            <td>
                <strong>Chargeback Representation Fee</strong>
            </td>
            <td>
                <strong>Discount Rate (Retail)</strong>
            </td>
            <td>
                <strong>Discount Rate</strong>
            </td>
            <td>
                <strong>Reserve Account Rate</strong>
            </td>
            <td>
                <strong>Gateway Fee (Cost)</strong>
            </td>
            <td>
                <strong>Gateway Fee (Retail)</strong>
            </td>
            <td>
                <strong>Payment Types</strong>
            </td>
            <td>
                <strong>Active/Inactive</strong>
            </td>
            <td>
                <strong>Actions</strong>
            </td>
        </tr>
        <asp:repeater id="rMID" datasource='<%# AssertigyMIDList %>' runat="server" onitemcommand="rMID_ItemCommand">
        <ItemTemplate>
            <tr>
                <td>
                    <%# Eval("AssertigyMID.MID")%>
                </td>
                <td>
                    <%# Eval("AssertigyMID.DisplayName")%>
                </td>
                <td>
                    <%# String.Format("${0:f2}", Eval("AssertigyMID.MonthlyCap") ?? 0)%>
                </td>
                <td>
                    <%# String.Format("${0:f2}", Eval("AssertigyMID.TransactionFee") ?? 0)%>
                </td>
                <td>
                    <%# String.Format("${0:f2}", Eval("AssertigyMIDSetting.TransactionFee") ?? 0)%>
                </td>
                <td>
                    <%# String.Format("${0:f2}", Eval("AssertigyMID.ChargebackFee") ?? 0)%>
                </td>
                <td>
                    <%# String.Format("${0:f2}", Eval("AssertigyMIDSetting.ChargebackFee") ?? 0)%>
                </td>
                <td>
                    <%# String.Format("${0:f2}", Eval("AssertigyMIDSetting.ChargebackRepresentationFeeRetail") ?? 0)%>
                </td>
                <td>
                    <%# String.Format("${0:f2}", Eval("AssertigyMIDSetting.ChargebackRepresentationFee") ?? 0)%>
                </td>                                
                <td>
                    <%# String.Format("{0:f2}%", Eval("AssertigyMID.ProcessingRate") ?? 0)%>
                </td>
                <td>
                    <%# String.Format("{0:f2}%", Eval("AssertigyMIDSetting.DiscountRate") ?? 0)%>
                </td>                                
                <td>
                    <%# String.Format("{0:f2}%", Eval("AssertigyMID.ReserveAccountRate") ?? 0)%>
                </td>
                <td>
                    <%# String.Format("${0:f2}", Eval("AssertigyMIDSetting.GatewayFee") ?? 0)%>
                </td>
                <td>
                    <%# String.Format("${0:f2}", Eval("AssertigyMIDSetting.GatewayFeeRetail") ?? 0)%>
                </td>
                <td>
                    <asp:Repeater runat="server" datasource='<%# GetPaymentTypes(Convert.ToInt32(Eval("AssertigyMID.AssertigyMIDID")), TPClientID.Value) %>' >
                        <ItemTemplate>
                            <%# Container.DataItem %><br/>
                        </ItemTemplate>
                    </asp:Repeater>
                </td>                
                <td>
                    <%# Convert.ToBoolean(Eval("AssertigyMID.Visible")) ? "Active" : "Inactive" %>
                </td>
                <td>
                    <a href="#" class="editIcon" onclick='editMID(<%# Eval("AssertigyMID.AssertigyMIDID") %>); return false;'>Edit</a>&nbsp;
                    <asp:LinkButton ID="lbDelete" CssClass="confirm removeIcon" title="Are you sure you want to delete?" CommandName="delete" CommandArgument='<%# Eval("AssertigyMID.AssertigyMIDID") %>' Text="Delete" runat="server" /> 
                </td>
            </tr>
        </ItemTemplate>
     </asp:repeater>
    </table>
</div>
<div id="bottom">
    <div class="left">
        <asp:placeholder runat="server" id="lSaved">
           Saved <%# DateTime.Now %> by <%# AdminMembership.CurrentAdmin.DisplayName %>
        </asp:placeholder>
    </div>
    <div class="right" onclick="setTimeout(function(){updateMenu();},1000);">
        <asp:button text="Save Changes" runat="server" onclick="SaveChanges_Click" />
    </div>
</div>
</form>
