<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/Admin.Master" AutoEventWireup="true" CodeBehind="salesagent_management_test.aspx.cs" Inherits="TrimFuel.Web.Admin.salesagent_management_test" %>
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
                <table class="process-offets sortable add-csv-export" border="0" cellspacing="1" width="100%">
                    <tr class="header">
                        <td>
                        </td>
                        <td colspan="9" align="center">
                            Fulfillment
                        </td>
                        <td colspan="9" align="center">
                            Gateway
                        </td>
                        <td colspan="5" align="center">
                            Call Center
                        </td>
                        <td colspan="1" align="center">                            
                        </td>
                        <td>
                        </td>
                    </tr>
                    <tr class="header">
                        <td>
                            Client
                        </td>
                        <td>
                            Fee/ shipment
                        </td>
                        <td>
                            Total shipment fees
                        </td>
                        <td>
                            Fee/ shipment SKU
                        </td>
                        <td>
                            Total shipment SKU fees
                        </td>
                        <td>
                            Fee/ Setup
                        </td>
                        <td>
                            Fee/ Returns
                        </td>
                        <td>
                            Total returns fees
                        </td>
                        <td>
                            Fee/ Kitting & Assembly
                        </td>
                        <td>
                            Total kitting & assembly fees
                        </td>
                        <td>
                            Number of transactions
                        </td>
                        <td>
                            Fee/ Transaction
                        </td>
                        <td>
                            Total transaction fees
                        </td>
                        <td>
                            Fee/ Chargeback
                        </td>
                        <td>
                            Total chargeback fees
                        </td>
                        <td>
                            Representation Fee/Chargeback
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
                    </tr>
                    <asp:Repeater runat="server" ID="rTPClients">
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("Client") %></td>
                                <%# Eval("Tr")%>
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
