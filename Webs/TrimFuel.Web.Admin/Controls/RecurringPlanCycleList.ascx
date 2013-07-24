<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RecurringPlanCycleList.ascx.cs" Inherits="TrimFuel.Web.Admin.Controls.RecurringPlanCycleList" %>
<%@ Register Assembly="TrimFuel.Web.UI" Namespace="TrimFuel.Web.UI" TagPrefix="cc1" %>
<%@ Register src="RecurringPlanCycle.ascx" tagname="RecurringPlanCycle" tagprefix="uc1" %>
<style type="text/css">
    .plan-cycle-list 
    {
    	width:100%;    	
    	font-size:11px;
    }
    .plan-cycle
    {
    	float:left;
    	width:160px;
    	height:300px;
    }
    .plan-cycle-number
    {
    	width: 100px;
    	height: 20px;
    	font-size: 14;
    	font-weight: bold;
    	text-align:center;
    }
    .plan-cycle-view
    {
    	width:160px;
    	height:153px;
    }
    .plan-cycle-bottom
    {
    	width:160px;
    	height:125px;
    }
    .plan-cycle-view .plan-cycle-description
    {
    	background-color: #F8F8F8;
    	border: solid 1px grey;
    	cursor: pointer;
    	width:98px;
    	height:153px;    	
    }
    .plan-cycle-bottom .plan-cycle-description
    {
    	width:100px;
    	height:125px;    	
    }
    .plan-cycle-description
    {
    	float:left;
    	overflow: hidden;
    }
    .plan-cycle-billing
    {
    	width:98px;
    	height:30px;    	
    	padding: 5px 0px 5px 5px;
    }
    .plan-cycle-shipments
    {
    	width:98px;
    	height:50px;    	
    	padding: 5px 0px 5px 5px;
    }
    .plan-cycle-view .follow
    {
    	background: url(/images/white-arrow-right.png) no-repeat scroll left center;
    }
    .plan-cycle-view .last
    {
    	background: url(/images/white-arrow-rightEnd.png) no-repeat scroll left center;
    }
    .plan-cycle-view .recurring
    {
        height: 155px;
    	background: url(/images/white-arrow-right-top-corner.png) no-repeat scroll left bottom;
    }
    .plan-cycle-bottom .first
    {
    	background: url(/images/white-arrow-right-top.png) no-repeat scroll left center;
    }
    .plan-cycle-bottom .last
    {
    	background: url(/images/white-arrow-right-bottom-corner.png) no-repeat scroll left center;
    }
    .plan-cycle-bottom .recurring
    {
    	background: url(/images/white-arrow-horiz.png) repeat-x scroll left center;
    }
    .plan-cycle-interim
    {
    	float:left;
    	width:60px;
    	height:125px;    	    	
    	text-align:center;
    }
    .plan-cycle-view .plan-cycle-interim div
    {
    	padding-top:54px;
    	margin-left:-5px;
    }
    .hover 
    {
    	background-color: #DDD !important;
    }
</style>
<script type="text/javascript">
    $(document).ready(function() {
        $('.plan-cycle-view .plan-cycle-description').mouseover(function() {
            $(this).addClass('hover');
        });

        $('.plan-cycle-view .plan-cycle-description').mouseout(function() {
            $(this).removeClass('hover');
        });

        $('.plan-cycle-view .plan-cycle-description').click(function() {
            editPlanCycle(<%# RecurringPlanID %>, $(this).parent().parent().attr("recurringPlanCycleID"));            
        });
    });
</script>
<div class="plan-cycle-list">
    <asp:Repeater runat="server" ID="rPlanCycleList">
        <ItemTemplate>                        
            <div class="plan-cycle" id="plan-cycle-<%# Eval("Cycle.RecurringPlanCycleID") %>" recurringPlanCycleID='<%# Eval("Cycle.RecurringPlanCycleID") %>'>
                <div class="plan-cycle-number">#<%# Eval("Cycle.Cycle")%></div>
                <div class="plan-cycle-view">
                    <uc1:RecurringPlanCycle ID="RecurringPlanCycle1" runat="server" ViewMode="View" ProposedProductID='<%# ProposedProductID %>' PlanCycle='<%# Container.DataItem %>' LastOne='<%# IsLast(Convert.ToInt32(Eval("Cycle.Cycle"))) %>' />
                </div>
                <div class="plan-cycle-bottom">
                    <asp:PlaceHolder runat="server" Visible='<%# Convert.ToBoolean(Eval("Cycle.Recurring")) %>'>
                        <cc1:If ID="If1" runat="server" Condition='<%# IsFirstRecurring(Convert.ToInt32(Eval("Cycle.Cycle"))) %>'>
                            <div class="plan-cycle-description first">
                            </div>
                        </cc1:If>
                        <cc1:If ID="If2" runat="server" Condition='<%# !IsFirstRecurring(Convert.ToInt32(Eval("Cycle.Cycle"))) %>'>
                            <div class="plan-cycle-description recurring">
                            </div>
                        </cc1:If>
                        <cc1:If ID="If3" runat="server" Condition='<%# IsLast(Convert.ToInt32(Eval("Cycle.Cycle"))) %>'>
                            <div class="plan-cycle-interim last">
                            </div>
                        </cc1:If>
                        <cc1:If ID="If4" runat="server" Condition='<%# !IsLast(Convert.ToInt32(Eval("Cycle.Cycle"))) %>'>
                            <div class="plan-cycle-interim recurring">
                            </div>
                        </cc1:If>
                    </asp:PlaceHolder>
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>
    <!--
    <div class="plan-cycle" id="plan-cycle-">
        <div class="plan-cycle-number">New Cycle</div>
        <div class="plan-cycle-view"><uc1:RecurringPlanCycle ID="RecurringPlanCycleNew" runat="server" /></div>
        <div class="plan-cycle-edit"></div>
    </div>    
    -->
</div>