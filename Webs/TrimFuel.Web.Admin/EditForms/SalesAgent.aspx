<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SalesAgent.aspx.cs" Inherits="TrimFuel.Web.Admin.EditForms.SalesAgent_" %>

<input type="hidden" name="action" value="process" />
<div class="module" style="width: 95%">
    <% if (SalesAgent.SalesAgentID != null)
       { %>
    <h2>
        Edit Sales Agent</h2>
    <% }
       else
       { %>
    <h2>
        Create Sales Agent</h2>
    <% } %>
    <table class="editForm" border="0" cellspacing="1" cellpadding="0">
        <tr>
            <td>
                Sales Agent ID
            </td>
            <td colspan="2">
                <% if (SalesAgent.SalesAgentID != null)
                   { %>
                <%= SalesAgent.SalesAgentID %>
                <input type="hidden" name="id" id="id" value='<%= SalesAgent.SalesAgentID %>' />
                <% }
                   else
                   { %>
                N/A
                <% } %>
            </td>
        </tr>
        <tr>
            <td>
                Name
            </td>
            <td>
                <input type="text" name="name" id="name" class="validate[required]" maxlength="50"
                    value="<%= SalesAgent.Name %>" />
            </td>
        </tr>
        <%-- <tr>
            <td>
                Fee/transaction (fixed amount)
            </td>
            <td>
                <input type="text" name="transactionFeeFixed" id="transactionFeeFixed" class="narrow validate[custom[Amount]]" maxlength="10" value="<%= SalesAgent.TransactionFeeFixed.ToString()  %>" />
            </td>
        </tr>
        <tr>
            <td>
                Fee/transaction (percentage)
            </td>
            <td>
                <input type="text" name="transactionFeePercentage" id="transactionFeePercentrage" class="narrow validate[custom[Amount]]" maxlength="2" value="<%= SalesAgent.TransactionFeePercentage.ToString()  %>" />
            </td>
        </tr>
        <tr>
            <td>
                Fee/shipment (base)
            </td>
            <td>
                <input type="text" name="shipmentFee" id="shipmentFee" class="narrow validate[custom[Amount]]" maxlength="10" value="<%= SalesAgent.ShipmentFee.ToString() %>" />
            </td>
        </tr>
        <tr>
            <td>
                ChargebackFee
            </td>
            <td>
                <input type="text" name="chargebackFee" id="chargebackFee" class="narrow validate[custom[Amount]]" maxlength="10" value="<%= SalesAgent.ChargebackFee.ToString() %>" />
            </td>
        </tr>
        <tr>
            <td>
                Callcenter Fee/Minute
            </td>
            <td>
                <input type="text" name="callCenterFeePerMinute" id="callCenterFeePerMinute" class="narrow validate[custom[Amount]]" maxlength="10" value="<%= SalesAgent.CallCenterFeePerMinute.ToString() %>" />
            </td>
        </tr>
        <tr>
            <td>
                Callcenter Fee/Call
            </td>
            <td>
                <input type="text" name="callCenterFeePerCall" id="callCenterFeePerCall" class="narrow validate[custom[Amount]]" maxlength="10" value="<%= SalesAgent.CallCenterFeePerCall.ToString()  %>" />
            </td>
        </tr>
        <tr>
            <td>
                Monthly CRM Fee
            </td>
            <td>
                <input type="text" name="monthlyCrmFee" id="monthlyCrmFee" class="narrow validate[custom[Amount]]" maxlength="10" value="<%= SalesAgent.MonthlyCRMFee.ToString() %>" />
            </td>
        </tr>        --%>
        <tr class="header">
            <td>
                Commission
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td>
                Unmanaged Merchant Accounts
            </td>
            <td>
                <input type="text" name="commissionMerchant" id="commissionMerchant" class="narrow validate[custom[Amount]]"
                    maxlength="10" value="<%= SalesAgent.CommissionMerchant.ToString() %>" />%
            </td>
        </tr>
        <tr>
            <td>
                Managed Services
            </td>
            <td>
                <input type="text" name="commission" id="commission" class="narrow validate[custom[Amount]]"
                    maxlength="10" value="<%= SalesAgent.Commission.ToString() %>" />%
            </td>
        </tr>
        <tr>
            <td>
                Admin
            </td>
            <td>
                <select name="adminID" id="adminID">
                    <option>-- Select --</option>
                    <asp:repeater id="rAdmins" runat="server">
                        <ItemTemplate>
                            <option value="<%# Eval("AdminID") %>"<%# Convert.ToInt32(Eval("AdminID")) == SalesAgent.AdminID ? " selected" : "" %>><%# Eval("DisplayName") %></option>
                        </ItemTemplate>
                    </asp:repeater>
                </select>
            </td>
        </tr>
    </table>
</div>