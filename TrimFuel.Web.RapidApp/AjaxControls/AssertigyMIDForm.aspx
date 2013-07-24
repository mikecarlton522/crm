<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AssertigyMIDForm.aspx.cs"
    Inherits="TrimFuel.Web.RapidApp.AjaxControls.AssertigyMIDForm" %>

<script type="text/javascript">
    function updateNMI() {
        var url = "ajaxControls/ClientGatewayService.aspx?ClientId=" + $("#tbTPClientID").val() + "&service=" + $("#tbServiceName").val() + "&companyID=" + $("#tbNMICompanyID").val();
        inlineControl(url, "right");
    }
</script>

<form id="asserForm" runat="server">
<asp:textbox style="display: none;" id="tbAssertigyMIDID" runat="server" text='<%# AssertigyMIDID %>' />
<asp:textbox style="display: none;" id="tbNMICompanyID" runat="server" text='<%# NMICompanyID %>' />
<asp:textbox style="display: none;" id="tbTPClientID" runat="server" text='<%# TPClientID %>' />
<asp:textbox style="display: none;" id="tbServiceName" runat="server" text='<%# ServiceName %>' />
<table width="100%">
    <tr valign="top">
        <td>
            <table style="border:none;">
                <tr>
                    <td>
                        <span>MID</span>
                    </td>
                    <td>
                        <asp:textbox id="tbMID" runat="server" text='<%# AssertigyMIDProp.MID %>' />
                    </td>
                </tr>
                <tr>
                    <td>
                        <span>Display name</span>
                    </td>
                    <td>
                        <asp:textbox id="tbDisplayName" runat="server" text='<%# AssertigyMIDProp.DisplayName %>' />
                    </td>
                </tr>
                <tr>
                    <td>
                        <span>Monthly Cap</span>
                    </td>
                    <td>
                        <asp:textbox id="tbMonthlyCap" runat="server" text='<%# AssertigyMIDProp.MonthlyCap %>' />
                    </td>
                </tr>
                <tr>
                    <td>
                        <span>Transaction Fee (Retail)</span>
                    </td>
                    <td>
                        <asp:textbox id="tbTransactionFeeRetail" runat="server" text='<%# AssertigyMIDProp.TransactionFee %>' />
                    </td>
                </tr>
                <tr>
                    <td>
                        <span>Transaction Fee</span>
                    </td>
                    <td>
                        <asp:textbox id="tbTransactionFee" runat="server" text='<%# AssertigyMIDSettingProp.TransactionFee %>' />
                    </td>
                </tr>
                <tr>
                    <td>
                        <span>Chargeback Fee (Retail)</span>
                    </td>
                    <td>
                        <asp:textbox id="tbChargebackFeeRetail" runat="server" text='<%# AssertigyMIDProp.ChargebackFee %>' />
                    </td>
                </tr>
                <tr>
                    <td>
                        <span>Chargeback Fee</span>
                    </td>
                    <td>
                        <asp:textbox id="tbChargebackFee" runat="server" text='<%# AssertigyMIDSettingProp.ChargebackFee %>' />
                    </td>
                </tr>      
                <tr>
                    <td>
                        <span>Chargeback Representation Fee (Retail)</span>
                    </td>
                    <td>
                        <asp:textbox id="tbChargebackRepresentationFeeRetail" runat="server" text='<%# AssertigyMIDSettingProp.ChargebackRepresentationFeeRetail %>' />
                    </td>
                </tr>
                <tr>
                    <td>
                        <span>Chargeback Representation Fee</span>
                    </td>
                    <td>
                        <asp:textbox id="tbChargebackRepresentationFee" runat="server" text='<%# AssertigyMIDSettingProp.ChargebackRepresentationFee %>' />
                    </td>
                </tr>                            
                <tr>
                    <td>
                        <span>Discount Rate (Retail)</span>
                    </td>
                    <td>
                        <asp:textbox id="tbProcessingRate" runat="server" text='<%# AssertigyMIDProp.ProcessingRate %>' />
                    </td>
                </tr>
                <tr>
                    <td>
                        <span>Discount Rate</span>
                    </td>
                    <td>
                        <asp:textbox id="tbDiscountRate" runat="server" text='<%# AssertigyMIDSettingProp.DiscountRate %>' />
                    </td>
                </tr>                            
                <tr>
                    <td>
                        <span>Reserve Account Rate</span>
                    </td>
                    <td>
                        <asp:textbox id="tbReserveAccountRate" runat="server" text='<%# AssertigyMIDProp.ReserveAccountRate %>' />
                    </td>
                </tr>


                <tr>
                    <td>
                        <span>Gateway Fee (Cost)</span>
                    </td>
                    <td>
                        <asp:textbox id="tbGatewayFee" runat="server" text='<%# AssertigyMIDSettingProp.GatewayFee %>' />
                    </td>
                </tr>                            
                <tr>
                    <td>
                        <span>Gateway Fee (Retail)</span>
                    </td>
                    <td>
                        <asp:textbox id="tbGatewayFeeRetail" runat="server" text='<%# AssertigyMIDSettingProp.GatewayFeeRetail %>' />
                    </td>
                </tr>


                <tr>
                    <td>
                        <span>Active</span>
                    </td>
                    <td>
                        <asp:checkbox id="cbVisible" runat="server" checked='<%# AssertigyMIDProp.Visible %>' />
                    </td>
                </tr>
                <tr>
                    <td>
                        <span>MID Category</span>
                    </td>
                    <td>
                        <asp:dropdownlist id="dpdCategories" runat="server" datatextfield="DisplayName" datavaluefield="MIDCategoryID" />
                    </td>
                </tr>
            </table>
        </td>
        <td>
            <table style="border:none;">
                <tr>
                    <td valign="top">
                        <span>Payment Types</span>
                    </td>
                    <td>
                        <asp:repeater id="rPaymentTypes" runat="server" datasource='<%# PaymentTypes %>'>
                <ItemTemplate>
                    <asp:CheckBox Text='<%#Eval("Key", " {0}") %>' checked='<%#Eval("Value") %>' runat="server" />
                    <br />
                </ItemTemplate>
            </asp:repeater>
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <span>Products</span>
                    </td>
                    <td>
                        <asp:repeater id="rProducts" runat="server" datasource='<%# Products %>'>
                <ItemTemplate>
                    <asp:CheckBox Text='<%#Eval("Key", " {0}") %>' checked='<%#Eval("Value") %>' runat="server" />
                    <br />
                </ItemTemplate>
            </asp:repeater>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <div align="right" onclick="setTimeout(function() { updateNMI(); }, 1000);">
                <asp:button text="Save Changes" runat="server" id="assertSave" onclick="SaveChanges_Click" />
            </div>
        </td>
    </tr>
</table>
<asp:PlaceHolder runat="server" id="lSaved">
Saved <%# DateTime.Now %> by <%# AdminMembership.CurrentAdmin.DisplayName %>
</asp:PlaceHolder>
</form>
