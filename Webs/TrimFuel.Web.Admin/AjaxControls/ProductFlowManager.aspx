<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProductFlowManager.aspx.cs"
    Inherits="TrimFuel.Web.Admin.AjaxControls.ProductFlowManager" %>

<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<script type="text/javascript">
    $(document).ready(function () {
        $(".editMode").hide();
    })
    function addFlowWizard(productID) {
        var url = "ajaxControls/ProductFlowWizard.aspx?productID=" + productID;
        inlineControl(url, "right", null, null);
    }
    function editFlowWizard(el, productID) {
        var tr = $(el).closest('tr');
        var campaignID = $(tr).find('#hdnCampaignID').val();
        var campaignName = $(tr).find('#lblCampaignName').html();
        var url = "ajaxControls/ProductFlowWizard.aspx?productID=" + productID + "&campaignID=" + campaignID;
        inlineControl(url, "right", null, null);
    }
</script>
<form id="formProductFlow" runat="server">
<div id="template" style="display: none;">
    <asp:dropdownlist clientidmode="static" id="teditdpdrSubscriptions" runat="server"
        datasource='<%# Subscriptions %>' datatextfield="Value" datavaluefield="Key" />
    <asp:textbox id="teditTbCampaignName" clientidmode="static" runat="server" />
    <asp:textbox id="teditTbURL" clientidmode="static" runat="server" />
</div>
<asp:hiddenfield id="hdnProductID" value='<%# ProductID %>' runat="server" />
<a href="javascript:void(0)" onclick="addFlowWizard(<%# ProductID %>);" style="font-size: 12px;"
    class="addNewIcon">Add</a>&nbsp;&nbsp;|&nbsp;&nbsp;
<asp:LinkButton ID="lbShowExternal" OnClick="ShowExternalCampaigns" runat="server">Show Externally Hosted Campaigns</asp:LinkButton>&nbsp;&nbsp;|&nbsp;&nbsp;
<asp:LinkButton ID="lbHideExternal" OnClick="HideExternalCampaigns" runat="server">Hide Externally Hosted campaigns</asp:LinkButton>&nbsp;&nbsp;|&nbsp;&nbsp;
<div style="height: 10px;">
</div>
<table cellspacing="1" class="process-offets sortable" style="margin-left: 0;">
    <tr class="header">
        <td>
            <strong>CampaignID</strong>
        </td>
        <td>
            <strong>Campaign</strong>
        </td>
        <td>
            <strong>Subscription</strong>
        </td>
        <td>
            <strong>URL</strong>
        </td>
        <td>
            <strong>Create Date</strong>
        </td>
        <td>
            <strong>Actions</strong>
        </td>
    </tr>
    <asp:repeater id="rFlow" runat="server" onitemcommand="rFlow_ItemCommand">
        <ItemTemplate>
            <tr <%# !Convert.ToBoolean(Eval("Active")) ? "class='hiddenrow offset'" : "" %>>
                <td>
                    <span><%# Eval("CampaignID")%></span>
                </td>
                <td>
                    <input id="hdnCampaignID" name="hdnCampaignID" type="hidden" value='<%#Eval("CampaignID") %>' />
                    <span id="lblCampaignName" class="viewMode" ><%# Eval("DisplayName") %></span>
                </td>
                <td id="subscr">
                    <input id="hdnSelectedSubscriptionID" name="hdnSelectedSubscriptionID" type="hidden" value='<%#Eval("SubscriptionID") %>' />
                    <asp:Label class="viewMode" id="lblSubscriptionName" runat="server" Text='<%# Subscriptions.Where(u => u.Key == Eval("SubscriptionID").ToString()).Count() == 0 ? "-- Not Set --" : Subscriptions.Where(u => u.Key == Eval("SubscriptionID").ToString()).SingleOrDefault().Value%>' />
                </td>
                <td>
                    <span id="lblURL" class="viewMode" ><%# Eval("URL") %></span>
                </td>
                <td>
                    <asp:Label id="lblCreateDT" runat="server" Text='<%# Eval("CreateDT") %>' />
                </td>
                <td>
                    <a href="javascript:void(0);" ID="lbEdit" onclick="editFlowWizard(this, <%# ProductID %>)" class="editIcon viewMode" >Edit</a>
                    <asp:LinkButton runat="server" ID="lbHideShow" Text='<%# Convert.ToBoolean(Eval("Active")) ? "Disable" : "Enable" %>' 
                        CssClass="confirm viewMode" CommandName='<%# Convert.ToBoolean(Eval("Active")) ? "hide" : "show" %>' CommandArgument='<%#Eval("CampaignID") %>' ></asp:LinkButton>
                </td>
            </tr>
        </ItemTemplate>
    </asp:repeater>
</table>
<div class="space">
</div>
<div id="errorMsg">
    <asp:literal runat="server" id="Note"></asp:literal>
</div>
</form>
