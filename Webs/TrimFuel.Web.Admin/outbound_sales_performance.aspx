<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="outbound_sales_performance.aspx.cs"
    Inherits="TrimFuel.Web.Admin.outbound_sales_performance" MasterPageFile="~/Controls/Admin.Master" %>

<%@ Register Assembly="TrimFuel.Web.UI" Namespace="TrimFuel.Web.UI.Specialized" TagPrefix="cc1" %>
<%@ Register Src="Controls/DateFilter.ascx" TagName="DateFilter" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphScript" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphStyle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <form runat="server" id="frmMain">
    <div id="toggle" class="section">
        <a href="#">
            <h1>
                Quick View</h1>
        </a>
        <uc1:DateFilter ID="DateFilter" runat="server" />
        <a href="#">
            <h1>
                Refine by Affiliate</h1>
        </a>
        <div>
            <div class="module">
                <h2>
                    Affiliate</h2>
                <cc1:AffiliateDDL runat="server" ID="AffiliateDDL" Style="width: 150px;" />
            </div>
        </div>
    </div>
    <div id="buttons">
        <asp:Button runat="server" ID="btnGo" Text="Generate Report" OnClick="btnGo_Click" />
    </div>
    <div id="load">
        <img src="../images/loading2.gif" style="display: none;" alt="Please wait" />
    </div>
    <div class="data">
        <asp:Repeater ID="rLeadPartners" runat="server" DataSource='<%# LeadPartnerList %>'>
            <HeaderTemplate>
                <table class="process-offets sortable add-csv-export" cellspacing="1" cellpadding="0"
                    border="0" style="width: 100%;">
                    <thead>
                        <tr class="header">
                            <th>
                                Call Center
                            </th>
                            <th>
                                Number Of Leads
                            </th>
                            <th>
                                Sales
                            </th>
                            <th>
                                Cost of Sales
                            </th>
                            <th>
                                Refunds
                            </th>
                            <th>
                                Chargebacks
                            </th>
                            <th>
                                Gross Revenue
                            </th>
                            <th>
                                Net Revenue
                            </th>
                            <th>
                                Gross Conversion
                            </th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td>
                        <%# Eval("DisplayName")%>
                    </td>
                    <td>
                        <%# GetOutboundSalesView(Eval("LeadPartnerID").ToString()).NumberOfLeads%>
                    </td>
                    <td>
                        <%# GetOutboundSalesView(Eval("LeadPartnerID").ToString()).NumberOfSales%>
                    </td>
                    <td>
                        <%# ShowAmount(GetOutboundSalesView(Eval("LeadPartnerID").ToString()).CostOfSales)%>
                    </td>
                    <td>
                        <%# ShowAmount(GetOutboundSalesView(Eval("LeadPartnerID").ToString()).Refunds)%>
                    </td>
                    <td>
                        <%# GetOutboundSalesView(Eval("LeadPartnerID").ToString()).NumberOfChargebacks%>
                    </td>
                    <td>
                        <%# ShowAmount(GetOutboundSalesView(Eval("LeadPartnerID").ToString()).GrossRevenue)%>
                    </td>
                    <td>
                        <%# ShowAmount(GetOutboundSalesView(Eval("LeadPartnerID").ToString()).NetRevenue)%>
                    </td>
                    <td>
                        <%# ShowConversion(GetOutboundSalesView(Eval("LeadPartnerID").ToString()).Conversion)%>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </tbody> </table>
            </FooterTemplate>
        </asp:Repeater>
    </div>
    </form>
</asp:Content>
