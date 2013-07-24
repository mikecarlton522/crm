<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/Admin.Master" AutoEventWireup="true"
    CodeFile="salesagent_management.aspx.cs" Inherits="salesagent_management" %>

<%@ Register Src="Controls/DateFilter.ascx" TagName="DateFilter" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphScript" runat="Server">

    <script type="text/javascript" language="javascript">
        function editClientList(Id) {
            editForm("EditForms/ClientList.aspx?id=" + Id, 400, "salesagent_management.aspx");
        }

        function editSalesAgent(Id) {
            editForm("EditForms/SalesAgent.aspx?id=" + Id, 400, "salesagent_management.aspx");
        }

        function newSalesAgent() {
            editForm("EditForms/SalesAgent.aspx", 400, "salesagent_management.aspx");
        }    
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphStyle" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="Server">
    <div id="toggle" class="section">
        <a href="#">
            <h1>
                Overview Report</h1>
        </a>
        <div>
            <form id="Form1" runat="server">
            <uc1:DateFilter ID="DateFilter1" runat="server" />
            <div id="buttons">
                <asp:Button runat="server" ID="btnGo" Text="Generate Report" OnClick="btnGo_Click" />
            </div>
            </form>
            <div class="data">
                <table class="process-offets sortable add-csv-export" border="0" cellspacing="1"
                    width="100%">
                    <tr class="header">
                        <td>
                            Client
                        </td>
                        <td>
                            Number of transactions
                        </td>
                        <td>
                            Fee/transaction
                        </td>
                        <td>
                            Total transaction fees
                        </td>
                        <td>
                            Fee/shipment
                        </td>
                        <td>
                            Total shipment fees
                        </td>
                        <td>
                            CRM fee/month
                        </td>
                        <td>
                            Total CRM fees
                        </td>
                        <td>
                            Number of chargebacks
                        </td>
                        <td>
                            Chargeback fee
                        </td>
                        <td>
                            Total chargeback fees
                        </td>
                        <td>
                            Callcenter fee/call
                        </td>
                        <td>
                            Callcenter fee/minute
                        </td>
                        <td>
                            Total callcenter fees
                        </td>
                        <td>
                            Total revenue
                        </td>
                        <td>
                            Customer total revenue
                        </td>
                    </tr>
                    <asp:Repeater runat="server" ID="rTPClients">
                        <ItemTemplate>
                            <tr>
                                <td>
                                    <%# DataBinder.Eval(Container.DataItem, "Client") %>
                                </td>
                                <td>
                                    <%# DataBinder.Eval(Container.DataItem, "NumberOfTransactions") %>
                                </td>
                                <td>
                                    <%# ShowPrice(DataBinder.Eval(Container.DataItem, "TransactionFee")) %>
                                </td>
                                <td>
                                    <%# ShowPrice(DataBinder.Eval(Container.DataItem, "TotalTransactionFees")) %>
                                </td>
                                <td>
                                    <%# ShowPrice(DataBinder.Eval(Container.DataItem, "ShipmentFee")) %>
                                </td>
                                <td>
                                    <%# ShowPrice(DataBinder.Eval(Container.DataItem, "TotalShipmentFees")) %>
                                </td>
                                <td>
                                    <%# ShowPrice(DataBinder.Eval(Container.DataItem, "CRMFeePerMonth")) %>
                                </td>
                                <td>
                                    <%# ShowPrice(DataBinder.Eval(Container.DataItem, "TotalCRMFees")) %>
                                </td>
                                <td>
                                    <%# DataBinder.Eval(Container.DataItem, "NumberOfChargebacks") %>
                                </td>
                                <td>
                                    <%# ShowPrice(DataBinder.Eval(Container.DataItem, "ChargebackFee")) %>
                                </td>
                                <td>
                                    <%# ShowPrice(DataBinder.Eval(Container.DataItem, "TotalChargebackFee")) %>
                                </td>
                                <td>
                                    <%# ShowPrice(DataBinder.Eval(Container.DataItem, "CallcenterFeePerCall")) %>
                                </td>
                                <td>
                                    <%# ShowPrice(DataBinder.Eval(Container.DataItem, "CallcenterFeePerMinute")) %>
                                </td>
                                <td>
                                    <%# ShowPrice(DataBinder.Eval(Container.DataItem, "TotalCallcenterFees")) %>
                                </td>
                                <td>
                                    <%# ShowPrice(DataBinder.Eval(Container.DataItem, "TotalRevenue")) %>
                                </td>
                                <td>
                                    <%# ShowPrice(DataBinder.Eval(Container.DataItem, "CustomerTotalRevenue")) %>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </table>
            </div>
        </div>
        <br />
        <div class="clear">
        </div>
        <br />
        <a href="#">
            <h1>
                Sales Agents</h1>
        </a>
        <div class="hidden">
            <div class="module">
                <h2>
                    Create Sales Agent</h2>
                <input type="button" value="Create" onclick="newSalesAgent()" />
            </div>
            <div class="clear">
            </div>
            <div class="data">
                <table class="process-offets sortable add-csv-export" border="0" cellspacing="1"
                    width="50%" style="clear:both;">
                    <tr class="header">
                        <td>
                            ID
                        </td>
                        <td>
                            Name
                        </td>
                        <td>
                            Commission (%)
                        </td>
                        <td>
                            Admin
                        </td>
                        <%--  <td>
                            Fee/transaction
                        </td>
                        <td>
                            Fee/transaction (%)
                        </td>
                        <td>
                            Fee/shipment
                        </td>
                        <td>
                            Fee/extra SKU
                        </td>
                        <td>
                            Chargeback fee
                        </td>
                        <td>
                            CallCenter fee/min
                        </td>
                        <td>
                            CallCenter fee/call
                        </td>
                        <td>
                            CRM fee/month
                        </td>--%>
                        <td colspan="3">
                        </td>
                    </tr>
                    <asp:Repeater runat="server" ID="rSalesAgents">
                        <ItemTemplate>
                            <tr>
                                <td>
                                    <%# Eval("SalesAgentID") %>
                                </td>
                                <td>
                                    <%# DataBinder.Eval(Container.DataItem, "Name") %>
                                </td>
                                <td>
                                    <%# DataBinder.Eval(Container.DataItem, "Commission")%>
                                </td>
                                <td>
                                    <%# GetAdminNameByID(DataBinder.Eval(Container.DataItem, "AdminID"))%>
                                </td>
                                <%--  <td>
                                    <%# DataBinder.Eval(Container.DataItem, "TransactionFeeFixed") %>
                                </td>
                                <td>
                                    <%# DataBinder.Eval(Container.DataItem, "TransactionFeePercentage") %>
                                </td>
                                <td>
                                    <%# DataBinder.Eval(Container.DataItem, "ShipmentFee") %>
                                </td>
                                <td>
                                    <%# DataBinder.Eval(Container.DataItem, "ExtraSKUShipmentFee") %>
                                </td>
                                <td>
                                    <%# DataBinder.Eval(Container.DataItem, "ChargebackFee") %>
                                </td>
                                <td>
                                    <%# DataBinder.Eval(Container.DataItem, "CallCenterFeePerMinute") %>
                                </td>
                                <td>
                                    <%# DataBinder.Eval(Container.DataItem, "CallCenterFeePerCall") %>
                                </td>
                                <td>
                                    <%# DataBinder.Eval(Container.DataItem, "MonthlyCRMFee") %>
                                </td>--%>
                                <td>
                                    <a href="javascript:editSalesAgent(<%# DataBinder.Eval(Container.DataItem, "SalesAgentID") %>)"
                                        class="editIcon">Edit</a>
                                </td>
                                <td>
                                    <a href="javascript:window.location='tpclient_report.aspx?salesAgentID=<%# DataBinder.Eval(Container.DataItem, "SalesAgentID") %>'"
                                        class="editIcon">View Report</a>
                                </td>
                                <td>
                                    <a href="javascript:editClientList(<%# DataBinder.Eval(Container.DataItem, "SalesAgentID") %>)"
                                        class="editIcon">Edit Client List</a>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </table>
            </div>
            <div class="clear">
            </div>
        </div>
        <br />
    </div>
</asp:Content>
