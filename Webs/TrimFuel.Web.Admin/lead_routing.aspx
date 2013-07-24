<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/Admin.Master" AutoEventWireup="true" CodeBehind="lead_routing.aspx.cs" Inherits="TrimFuel.Web.Admin.lead_routing" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="TrimFuel.Model.Views" %>
<%@ Import Namespace="TrimFuel.Model.Enums" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphScript" runat="server">
<script type="text/javascript">
    function leadSettings(productId, productName) {
        popupControl2("popup-lead-settings-" + productId, productName + " Lead Routing", 700, 700, "ajaxControls/ProductLeadManager.aspx?productId=" + productId, null, null, function() { location.reload(); });
    }
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphStyle" runat="server">
<style type="text/css">
    #products-leads tr:hover td
    {
        background-color: #ccc !important;
    }
</style>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
<div id="toggle" class="section">
	<a href="#">
	<h1>Lead Routing</h1>
	</a>
</div>
<div class="data">
    <form id="Form1" runat="server">
    <table class="process-offets sortable add-csv-export" border="0" cellspacing="1" width="100%" id="products-leads">
        <tr class="header">
            <td>Product</td>
            <td>Abandons</td>
            <td>Order Confirmations</td>
            <td>Cancellations/Declines</td>
            <td></td>
        </tr>
        <asp:Repeater runat="server" ID="rProducts">
            <ItemTemplate>
            <tr>
                <td><%# Eval("Key.Value") %></td>
                <td>
                    <asp:Repeater runat="server" ID="rAbandons" DataSource='<%# ((IEnumerable<LeadRoutingView>)Container.DataItem).Where(i => i.LeadRouting.LeadTypeID == LeadTypeEnum.Abandons) %>'>
                        <ItemTemplate>
                            <%# Eval("LeadRouting.Percentage") %>% - <%# Eval("LeadPartnerName") %>
                        </ItemTemplate>
                        <SeparatorTemplate>
                            <br/>
                        </SeparatorTemplate>
                    </asp:Repeater>
                </td>
                <td>
                    <asp:Repeater runat="server" ID="rConfirms" DataSource='<%# ((IEnumerable<LeadRoutingView>)Container.DataItem).Where(i => i.LeadRouting.LeadTypeID == LeadTypeEnum.OrderConfirmations) %>'>
                        <ItemTemplate>
                            <%# Eval("LeadRouting.Percentage")%>% - <%# Eval("LeadPartnerName") %>
                        </ItemTemplate>
                        <SeparatorTemplate>
                            <br/>
                        </SeparatorTemplate>
                    </asp:Repeater>
                </td>
                <td>
                    <asp:Repeater runat="server" ID="rDeclines" DataSource='<%# ((IEnumerable<LeadRoutingView>)Container.DataItem).Where(i => i.LeadRouting.LeadTypeID == LeadTypeEnum.CancellationsDeclines) %>'>
                        <ItemTemplate>
                            <%# Eval("LeadRouting.Percentage")%>% - <%# Eval("LeadPartnerName") %>
                        </ItemTemplate>
                        <SeparatorTemplate>
                            <br/>
                        </SeparatorTemplate>
                    </asp:Repeater>
                </td>
                <td><a href='javascript:leadSettings(<%# Eval("Key.Key") %>, "<%# Eval("Key.Value") %>")' class="editIcon">Edit Settings</a></td>
            </tr>
            </ItemTemplate>
        </asp:Repeater>
    </table>
    </form>
	<div class="space">
	</div>
</div>
</asp:Content>
