<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/Admin.Master" AutoEventWireup="true" CodeFile="failed_order_report.aspx.cs" Inherits="order_report" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphScript" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphStyle" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="Server">
    <div class="data">
        <table class="process-offets sortable add-csv-export" border="0" cellspacing="1" width="100%" id="table">
            <tr class="header">
                <td>BID </td>
                <td>Campaign </td>
                <td>Fraud Score </td>
                <td>Status </td>
                <td>Order Date </td>
                <td>First Name </td>
                <td>Last Name </td>
                <td>Address </td>
                <td>City </td>
                <td>State </td>
                <td>Zip </td>
                <td>Phone </td>
                <td>Email </td>
                <td>Affiliate </td>
                <td>SubID </td>
            </tr>
            <asp:Repeater runat="server" ID="rOrders">
                <ItemTemplate>
                    <tr>
                        <td>
                            <a href='../billing_edit.asp?id=<%#  DataBinder.Eval(Container.DataItem, "BID") %>' /><%#  DataBinder.Eval(Container.DataItem, "BID") %></a>
                        </td>
                        <td>
                            <%# DataBinder.Eval(Container.DataItem, "Campaign")%>
                        </td>
                        <td>
                            <%# DataBinder.Eval(Container.DataItem, "FraudScore") %>
                        </td>
                        <td>
                            <%# DataBinder.Eval(Container.DataItem, "Status") %>
                        </td>
                        <td>
                            <%# DataBinder.Eval(Container.DataItem, "OrderDate") %>
                        </td>
                        <td>
                            <%# DataBinder.Eval(Container.DataItem, "FirstName") %>
                        </td>
                        <td>
                            <%# DataBinder.Eval(Container.DataItem, "LastName") %>
                        </td>
                        <td>
                            <%# DataBinder.Eval(Container.DataItem, "Address") %>
                        </td>
                        <td>
                            <%# DataBinder.Eval(Container.DataItem, "City") %>
                        </td>
                        <td>
                            <%# DataBinder.Eval(Container.DataItem, "State") %>
                        </td>
                        <td>
                            <%# DataBinder.Eval(Container.DataItem, "Zip") %>
                        </td>
                        <td>
                            <%# DataBinder.Eval(Container.DataItem, "Phone") %>
                        </td>
                        <td>
                            <%# DataBinder.Eval(Container.DataItem, "Email") %>
                        </td>
                        <td>
                            <%# DataBinder.Eval(Container.DataItem, "Affiliate") %>
                        </td>
                        <td>
                            <%# DataBinder.Eval(Container.DataItem, "SubID") %>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>
    </div>
</asp:Content>
