<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ClientOutbound.aspx.cs"
    Inherits="TrimFuel.Web.RapidApp.AjaxControls.ClientOutbound" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="TrimFuel.Model.Views" %>
<%@ Import Namespace="TrimFuel.Model.Enums" %>

<script type="text/javascript">
    function leadSettings(productId, productName) {
        popupControl2("popup-lead-settings-" + productId, productName + " Lead Routing", 500, 400, "ajaxControls/ProductLeadManager.aspx?productId=" + productId + "&ClientId=<%# TPClientID %>", null, null, function() {
            if (showOutboundSettings) {
                showOutboundSettings(<%# TPClientID %>);
            }
        });
    }
</script>

<form id="Form1" runat="server">
<asp:hiddenfield runat="server" id="hdnTPClientID" value="<%# TPClientID %>" />
<h1>
    Lead Routing</h1>
<table cellspacing="1" class="rapidapp-alternate" style="font-size: 12px;">
    <tr class="header">
        <td>
            <Strong>Product</Strong>
        </td>
        <td>
            <strong>Abandons</strong>
        </td>
        <td>
            <strong>Order Confirmations</strong>
        </td>
        <td>
            <strong>Cancellations/Declines</strong>
        </td>
        <td>
            <strong>Actions</strong>
        </td>
    </tr>
    <asp:repeater runat="server" id="rProducts">
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
        </asp:repeater>
</table>
</form>
