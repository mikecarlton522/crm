<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/Admin.Master" AutoEventWireup="true" CodeBehind="salesagent_management.aspx.cs" Inherits="TrimFuel.Web.Admin.salesagent_management" %>
<%@ Register Src="Controls/DateFilter.ascx" TagName="DateFilter" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphScript" runat="server">
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

        function editTPClient(Id) {
            editForm("EditForms/TPClient.aspx?id=" + Id, 400, "salesagent_management.aspx");
        }

        var fulfillmentVisible = false;
        var gatewayVisible = false;
        var callCenterVisible = false;
        
        function toggleSaColumns(mode, el) {
            if ($(el).text() == "+") {
                $(el).text("-");
            }
            else {
                $(el).text("+");
            }

            if (mode == 'fulfillment') {
                $('.accordion-sa tr:gt(0)>td:nth-child(2)').toggle();
                $('.accordion-sa tr:gt(0)>td:nth-child(4)').toggle();
                $('.accordion-sa tr:gt(0)>td:nth-child(6)').toggle();
                $('.accordion-sa tr:gt(0)>td:nth-child(7)').toggle();
                $('.accordion-sa tr:gt(0)>td:nth-child(9)').toggle();

                if (fulfillmentVisible) {
                    $(el).parent().attr('colspan', '4');
                    fulfillmentVisible = false;
                }
                else {
                    $(el).parent().attr('colspan', '9');
                    fulfillmentVisible = true;
                }
            }
            else if (mode == 'gateway') {
                $('.accordion-sa tr:gt(0)>td:nth-child(11)').toggle();
                $('.accordion-sa tr:gt(0)>td:nth-child(12)').toggle();
                $('.accordion-sa tr:gt(0)>td:nth-child(14)').toggle();
                $('.accordion-sa tr:gt(0)>td:nth-child(16)').toggle();
                $('.accordion-sa tr:gt(0)>td:nth-child(18)').toggle();
                $('.accordion-sa tr:gt(0)>td:nth-child(20)').toggle();

                if (gatewayVisible) {
                    $(el).parent().attr('colspan', '5');
                    gatewayVisible = false;
                }
                else {
                    $(el).parent().attr('colspan', '11');
                    gatewayVisible = true;
                }
            }
            else if (mode == 'callCenter') {
                $('.accordion-sa tr:gt(0)>td:nth-child(22)').toggle();
                $('.accordion-sa tr:gt(0)>td:nth-child(23)').toggle();
                $('.accordion-sa tr:gt(0)>td:nth-child(25)').toggle();

                if (callCenterVisible) {
                    $(el).parent().attr('colspan', '2');
                    callCenterVisible = false;
                }
                else {
                    $(el).parent().attr('colspan', '5');
                    callCenterVisible = true;
                }
            }
        }

        $(document).ready(function () {
            $('.accordion-sa tr:gt(0)>td:nth-child(2)').hide();
            $('.accordion-sa tr:gt(0)>td:nth-child(4)').hide();
            $('.accordion-sa tr:gt(0)>td:nth-child(6)').hide();
            $('.accordion-sa tr:gt(0)>td:nth-child(7)').hide();
            $('.accordion-sa tr:gt(0)>td:nth-child(9)').hide();
            $('.accordion-sa tr:gt(0)>td:nth-child(11)').hide();
            $('.accordion-sa tr:gt(0)>td:nth-child(12)').hide();
            $('.accordion-sa tr:gt(0)>td:nth-child(14)').hide();
            $('.accordion-sa tr:gt(0)>td:nth-child(16)').hide();
            $('.accordion-sa tr:gt(0)>td:nth-child(18)').hide();
            $('.accordion-sa tr:gt(0)>td:nth-child(20)').hide();
            $('.accordion-sa tr:gt(0)>td:nth-child(22)').hide();
            $('.accordion-sa tr:gt(0)>td:nth-child(23)').hide();
            $('.accordion-sa tr:gt(0)>td:nth-child(25)').hide();
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphStyle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
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
                <table class="process-offets add-csv-export accordion-sa" border="0" cellspacing="1" width="100%">
                    <tr class="header">
                        <td class=>
                        </td>
                        <td colspan="4" align="center">
                            <a href="#" onclick="toggleSaColumns('fulfillment', this);" style="text-decoration: none;font-size:14px;">+</a>&nbsp;&nbsp;Fulfillment
                        </td>
                        <td colspan="5" align="center">
                            <a href="#" onclick="toggleSaColumns('gateway', this);" style="text-decoration: none;font-size:14px;">+</a>&nbsp;&nbsp;Gateway
                        </td>
                        <td colspan="2" align="center">
                            <a href="#" onclick="toggleSaColumns('callCenter', this);" style="text-decoration: none;font-size:14px;">+</a>&nbsp;&nbsp;Call Center
                        </td>
                        <td colspan="2" align="center">                            
                        </td>
                        <td>                            
                        </td>
                    </tr>
                    <tr class="header">
                        <td>
                            Client
                        </td>
                        <td>
                            Fee/ shipment Avg
                        </td>
                        <td>
                            Total shipment fees
                        </td>
                        <td>
                            Fee/ shipment SKU Avg
                        </td>
                        <td>
                            Total shipment SKU fees
                        </td>
                        <td>
                            Fee/ Setup
                        </td>
                        <td>
                            Fee/ Returns Avg
                        </td>
                        <td>
                            Total returns fees
                        </td>
                        <td>
                            Fee/ Kitting & Assembly Avg
                        </td>
                        <td>
                            Total kitting & assembly fees
                        </td>
                        <td>
                            Number of transactions
                        </td>
                        <td>
                            Fee/ Transaction Avg
                        </td>
                        <td>
                            Total transaction fees
                        </td>
                        <td>
                            Gateway Fee Avg
                        </td>
                        <td>
                            Total gateway fees
                        </td>
                        <td>
                            Fee/ Chargeback Avg
                        </td>
                        <td>
                            Total chargeback fees
                        </td>
                        <td>
                            Representation Fee/Chargeback Avg
                        </td>
                        <td>
                            Total chargeback representation fees
                        </td>
                        <td>
                            Discount Rate
                        </td>
                        <td>
                            Total discounts
                        </td>
                        <td>
                            Fee/Call Center Setup
                        </td>
                        <td>
                            Fee/Call Center Hour
                        </td>
                        <td>
                            Total hour fees
                        </td>
                        <td>
                            Fee/Call Center Monthly
                        </td>
                        <td>
                            Total monthly fees
                        </td>
                        <td>
                            Total revenue
                        </td>
                        <td>
                            Customer total revenue
                        </td>
                        <td nowrap="nowrap">
                            Edit commission
                        </td>
                    </tr>
                    <asp:Repeater runat="server" ID="rTPClients">
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("Client") %></td>
                                <%# Eval("Tr")%>
                                <td nowrap="nowrap">
                                    <a href="javascript:editTPClient(<%# Eval("TPClientID") %>)"
                                        class="editIcon">Edit</a>
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
                        <td nowrap="nowrap">
                            Unmanaged Merchant Accounts (%)
                        </td>
                        <td nowrap="nowrap">
                            Managed Services (%)
                        </td>
                        <td>
                            Admin
                        </td>
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
                                    <%# DataBinder.Eval(Container.DataItem, "CommissionMerchant")%>
                                </td>
                                <td>
                                    <%# DataBinder.Eval(Container.DataItem, "Commission")%>
                                </td>
                                <td>
                                    <%# GetAdminNameByID(DataBinder.Eval(Container.DataItem, "AdminID"))%>
                                </td>
                                <td nowrap="nowrap">
                                    <a href="javascript:editSalesAgent(<%# DataBinder.Eval(Container.DataItem, "SalesAgentID") %>)"
                                        class="editIcon">Edit</a>
                                </td>
                                <td nowrap="nowrap">
                                    <a href="javascript:window.location='tpclient_report.aspx?salesAgentID=<%# DataBinder.Eval(Container.DataItem, "SalesAgentID") %>'"
                                        class="editIcon">View Report</a>
                                </td>
                                <td nowrap="nowrap">
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
