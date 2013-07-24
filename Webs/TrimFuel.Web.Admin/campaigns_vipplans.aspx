<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/Admin.Master" AutoEventWireup="true" CodeBehind="campaigns_vipplans.aspx.cs"
    Inherits="TrimFuel.Web.Admin.campaigns_vipplans" %>

<%@ Register Src="Controls/VIPPlan.ascx" TagName="VIPPlan" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphScript" runat="server">
    <script type="text/javascript">
    
    function editPlan(id) {
        if (id == "") {
            if ($("#vip-plan-container-").length == 0) {
                $("#plan-list").prepend("<div id='vip-plan-container-' class='vip-plan-container'></div>");
            } else {
                return;
            }
        }
        inlineControl("ajaxControls/VipPlan.aspx?mode=edit&planid=" + id, "vip-plan-container-" + id, function() {
            if (id == "")
            {
                if ($("#vip-plan-container-" + id).find("table.vip-plan").length > 0) {                
                    id = $("#vip-plan-container-" + id).find("table.vip-plan").attr("vip-plan-id");
                    $("#vip-plan-container-").attr("id", "vip-plan-container-" + id);
                    editPlan(id);
                } else {
                    $("#vip-plan-container-" + id).remove();
                }
            }
        });
    }

    $(document).ready(function() { 
    
        $('table.vip-plan tr.header').live('click', function () {
            $(this).siblings().toggle();
        });
    
        <asp:Repeater runat="server" ID="rpPlanList2">
            <ItemTemplate>
                editPlan('<%# Eval("SubscriptionPlanID") %>');
            </ItemTemplate>
        </asp:Repeater>
    });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphStyle" runat="server">
    <style type="text/css">
        #plan-list a
        {
            float: right;
            margin-left: 10px;
        }
        #plan-list label
        {
            float: left;
            width: 100px;
            padding: 5px;
        }
        
        #plan-list .edit
        {
            padding: 3px 0px;
        }
        
        #plan-list .edit select, #plan-list .edit input
        {
            width: 150px;
        }
        
        #plan-list .action-name
        {
            font-weight: bold;
            padding: 6px 3px;
        }
        
        .vip-plan-container
        {
            float: left;
            padding: 5px;
            width: 300px;
        }
        
        .vip-plan-container table
        {
            width: 100%;
        }
    </style>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <form id="form1" runat="server">
    <div id="toggle" class="section">
        <a href="#">
            <h1>
                Plan List</h1>
        </a>
    </div>
    <div class="data">
        <div class="toolBox">
            <a href="javascript:void(0)" onclick="editPlan('')" class="addNewIcon">Add Plan</a>
        </div>
        <div id="plan-list">
            <asp:Repeater runat="server" ID="rpPlanList">
                <ItemTemplate>
                    <div id='vip-plan-container-<%# Eval("SubscriptionPlanID") %>' class="vip-plan-container">
                        <asp:PlaceHolder runat="server" ID="phComment" Visible="true">
                            <uc1:VIPPlan ID="VIPPlan1" runat="server" Plan="<%# Container.DataItem %>" AllPlanItems='<%# AllPlanItems %>' AllPlanItemActions='<%# AllPlanItemActions %>'
                                ViewMode="NotEditable" />
                            <div class="space">
                            </div>
                            <!--
                <a href="javascript:void(0)" onclick="editPlan('<%# Eval("SubscriptionPlanID") %>')" class="editIcon">Modify Plan</a>
                -->
                        </asp:PlaceHolder>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
        <div class="space">
        </div>
    </div>
    </form>
</asp:Content>
