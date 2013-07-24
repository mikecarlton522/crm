<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ClientGateway.aspx.cs"
    Inherits="TrimFuel.Web.RapidApp.AjaxControls.ClientGateway" %>

<script type="text/javascript">
    function editProduct(productID) {
        var clientID = <%# TPClientID %>;        
        var title = (productID == '' ? "New Product" : "Edit Product");
        popupControl2('product-edit-' + productID, title, 400, 400, "ajaxControls/ClientProductMerchantAccountEdit.aspx?clientId=" + clientID + "&productID=" + productID, function(){                                    
            if (showGatewaySettings) {
                showGatewaySettings(clientID);
            }
        });
    }
</script>
<form id="form1" runat="server">
<h1>
    Product Groups Settings</h1>
<table class="rapidapp-alternate" cellspacing="1">
    <tr>
        <td>
            <strong>Product Group</strong>
        </td>
        <td>
            <strong>Currency</strong>
        </td>
        <td>
            <strong>MIDs</strong>
        </td>
        <td>
            <strong>Actions</strong>
        </td>
    </tr>
    <asp:repeater id="rProducts" runat="server">
        <ItemTemplate>
            <tr>
                <td>
                    <%#Eval("ProductName")%>
                </td>
                <td>
                    <%# GetCurrency(Convert.ToInt32(Eval("ProductID"))) %>
                </td>                
                <td>
                    <asp:Repeater runat="server" datasource='<%# GetMerchantAccounts(Convert.ToInt32(Eval("ProductID"))) %>'>
                        <ItemTemplate>
                            <%#Eval("DisplayName")%> (<%#Eval("MID")%>)<br/>
                        </ItemTemplate>
                    </asp:Repeater>
                </td>
                <td>
                    <a onclick="editProduct(<%# Eval("ProductID") %>); return false;" class="editIcon" href="#">Edit</a>
                </td>
            </tr>
        </ItemTemplate>
    </asp:repeater>
</table>
<div style="height: 10px;">
</div>
<h1>
    Gateway Settings</h1>
<table class="rapidapp-alternate" id="table" cellspacing="1">
    <tr>
        <td>
            <strong>Gateway Name</strong>
        </td>
        <td>
            <strong>Status</strong>
        </td>
        <td>
            <strong>Actions</strong>
        </td>
    </tr>
    <asp:repeater id="rGateways" runat="server" onitemcommand="rGateways_ItemCommand">
        <ItemTemplate>
            <tr>
                <td>
                    <%#Eval("GatewayIntegrated")%> - <%#Eval("CompanyName")%>
                </td>
                <td>
                    <%#Convert.ToBoolean(Eval("Deleted")) ? "Deleted" : (Convert.ToBoolean(Eval("Active")) ? "Active" : "Inactive")%>
                </td>
                <td>
                        <asp:LinkButton ID="lbDelete" title='You are about to delete the gateway. Click "Ok" to complete the action or Cancel to go back.' CssClass="confirm removeIcon" CommandName="delete" CommandArgument='<%# Eval("NMICompanyID") %>' Text="Delete" runat="server" Visible='<%# !Convert.ToBoolean(Eval("Deleted")) %>'  onclientclick="setTimeout(function(){updateMenu();},1000);" />             
                </td>
            </tr>            
        </ItemTemplate>
    </asp:repeater>
</table>
<div id="bottom">
    <div class="left">
        <asp:placeholder runat="server" id="lSaved">
           Saved <%# DateTime.Now %> by <%# AdminMembership.CurrentAdmin.DisplayName %>
        </asp:placeholder>
    </div>
    <div class="center" style="text-align: center;">
        <asp:placeholder runat="server" id="lMIDsError" visible="false">
           <span style="color:Red;">Gateway cannot be deleted because it has active MID(s)</span>
        </asp:placeholder>
    </div>
</div>
</form>
