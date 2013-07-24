<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProductFlowManagerAdvanced.aspx.cs" Inherits="TrimFuel.Web.Admin.AjaxControls.ProductFlowManagerAdvanced" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Register assembly="TrimFuel.Web.UI" namespace="TrimFuel.Web.UI" tagprefix="cc1" %>
<script type="text/javascript">
    $(document).ready(function () {
        $(".editMode").hide();
        $("#tabs-managers").tabs({ cookie: true });
    })
    function editClick(el) {
        cancelEditClick();
        var tr = $(el).closest('tr');
        $(tr).find('.viewMode').hide();
        $(tr).find('.editMode').show();
        var subscrID = $(tr).find('#hdnSelectedSubscriptionID').val();
        $(tr).find('#hdnCampaignID').attr("name", "lblCampaignID").attr("id", "lblCampaignID");
        var campName = $(tr).find('#lblCampaignName').html();
        var url = $(tr).find('#lblURL').html();
        var tdArray = $(tr).find('td');
        var templateObj = $('#template')
        templateObj.find('#teditTbCampaignName').clone().attr("name", "editTbCampaignName").attr("id", "editTbCampaignName").addClass("validate[required]").appendTo(tdArray[1]).val(campName);
        templateObj.find('#teditdpdrSubscriptions').clone().attr("name", "editdpdrSubscriptions").attr("id", "editdpdrSubscriptions").addClass("validate[required]").appendTo(tdArray[2]).val(subscrID);
        templateObj.find('#teditTbURL').clone().attr("name", "editTbURL").attr("id", "editTbURL").addClass("validate[required]").appendTo(tdArray[3]).val(url);
    }
    function cancelEditClick() {
        $('.viewMode').show();
        $('.editMode').hide();
        $('#editdpdrSubscriptions').remove();
        $('#editTbCampaignName').remove();
        $('#editTbURL').remove();
        $('#lblCampaignID').attr("name", "hdnCampaignID").attr("id", "hdnCampaignID");
    }
    function addFlow() {
        $('#tbCampaignName').addClass("validate[required]");
        $('#dpdrSubscriptions').addClass("validate[required]");
        $('#tbURL').addClass("validate[required]");
        $('#addFlowRow').show();
    }
    function cancelFlow() {
        $('#addFlowRow').hide();
        $('#tbCampaignName').removeClass("validate[required]");
        $('#dpdrSubscriptions').removeClass("validate[required]");
        $('#tbURL').removeClass("validate[required]");
    }

</script>
<form id="formProductFlow" runat="server">
<div id="tabs-managers">
    <ul>
        <li><a href="#manager-tab-1">Campaigns</a></li>
        <li><a href="#manager-tab-2">Campaigns Advanced</a></li>
    </ul>
    <div id="manager-tab-1">        
        <div id="template" style="display: none;">
            <asp:dropdownlist clientidmode="static" id="teditdpdrSubscriptions" runat="server"
                datasource='<%# Subscriptions %>' datatextfield="Value" datavaluefield="Key" />
            <asp:textbox id="teditTbCampaignName" clientidmode="static" runat="server" />
            <asp:textbox id="teditTbURL" clientidmode="static" runat="server" />
        </div>
        <asp:hiddenfield id="hdnProductID" value='<%# ProductID %>' runat="server" />
        <a href="javascript:void(0)" onclick="addFlow();" style="font-size: 12px;" class="addNewIcon">
            Add</a>
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
            <tr id="addFlowRow" style="display: none;">
                <td>
                </td>
                <td>
                    <asp:textbox clientidmode="static" id="tbCampaignName" runat="server" />
                </td>
                <td>
                    <asp:dropdownlist clientidmode="static" id="dpdrSubscriptions" runat="server" datasource='<%# Subscriptions %>'
                        datatextfield="Value" datavaluefield="Key" />
                </td>
                <td>
                    <asp:textbox clientidmode="static" id="tbURL" runat="server" />
                </td>
                <td>
                </td>
                <td>
                    <asp:linkbutton runat="server" id="lbAddMID" text="Save" cssclass="submit saveIcon"
                        onclick="btnAddFlow_Click"></asp:linkbutton>
                    <a href="javascript:void(0);" onclick="cancelFlow();" class="cancelIcon">Cancel</a>
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
                            <a href="javascript:void(0);" ID="lbEdit" onclick="editClick(this)" class="editIcon viewMode" >Edit</a>
                            <asp:LinkButton runat="server" ID="lbHideShow" Text='<%# Convert.ToBoolean(Eval("Active")) ? "Disable" : "Enable" %>' 
                                CssClass="confirm viewMode" CommandName='<%# Convert.ToBoolean(Eval("Active")) ? "hide" : "show" %>' CommandArgument='<%#Eval("CampaignID") %>' ></asp:LinkButton>
                            <asp:LinkButton runat="server" ID="lbSave" Text="Save" CssClass="submit saveIcon editMode" style="display:none;" OnClick="btnSave_Click" ></asp:LinkButton>
                            <a href="javascript:void(0);" ID="lbCancel" onclick="cancelEditClick(this);" class="cancelIcon editMode" style="display:none;">Cancel</a>
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
    </div>
    <div id="manager-tab-2">
        <a href="javascript:void(0)" style="font-size: 12px;" onclick="" class="addNewIcon">Add</a>
        <div class="space"></div>
        <table cellspacing="1" class="process-offets sortable">
            <tr class="header">
                <td>CampaignID</td>
                <td>Campaign</td>
                <td>Trial</td>
                <td>Subscription</td>
                <td>URL</td>
                <td>Create Date</td>
                <td>Actions</td>
            </tr>
            <asp:Repeater runat="server" ID="rCampaigns">
            <ItemTemplate>
            <tr class="header">
                <td><%# Eval("Campaign.CampaignID") %></td>
                <td><%# Eval("Campaign.DisplayName") %></td>
                <td><%# ShowTrial(Container.DataItem) %></td>
                <td><uc1:RecurringPlanBriefView2 ID="RecurringPlanBriefView21" runat="server" RecurringPlan='<%# Eval("RecurringPlan") %>' /></td>
                <td><%# Eval("Campaign.URL") %></td>
                <td><%# Eval("Campaign.CreateDT") %></td>
                <td>
                    <a href="javascript:void(0);" onclick="editCampaign(<%# Eval("Campaign.CampaignID") %>)" class="editIcon" >Edit</a>
                    <asp:LinkButton runat="server" ID="lbHideShow" Text='<%# Convert.ToBoolean(Eval("Active")) ? "Disable" : "Enable" %>' 
                                CssClass="confirm viewMode" CommandName='<%# Convert.ToBoolean(Eval("Active")) ? "hide" : "show" %>' CommandArgument='<%#Eval("Campaign.CampaignID") %>' ></asp:LinkButton>
                </td>
            </tr>
            </ItemTemplate>
            </asp:Repeater>
            <cc1:If ID="If1" runat="server" Condition="<%# rCampaigns.Items.Count == 0 %>">
            <tr>
                <td colspan="7">No records</td>
            </tr>
            </cc1:If>
        </table>
        <div class="space">
        </div>
    </div>
</div>
</form>
