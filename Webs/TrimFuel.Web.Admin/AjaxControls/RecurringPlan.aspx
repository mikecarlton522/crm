<%@ Page EnableViewState="true" Language="C#" AutoEventWireup="true" CodeBehind="RecurringPlan.aspx.cs" Inherits="TrimFuel.Web.Admin.AjaxControls.RecurringPlan_" %>
<%@ Register assembly="TrimFuel.Web.UI" namespace="TrimFuel.Web.UI.Specialized" tagprefix="cc1" %>
<%@ Register src="../Controls/RecurringPlanCycleList.ascx" tagname="RecurringPlanCycleList" tagprefix="uc1" %>
<div class="module" style="width:98%">
    <div style="width: 270px; border-right: solid 1px #EEE;float:left;">
    <form id="form1" runat="server">    
    <h2><%# (RecurringPlan.RecurringPlanID != null ? "Edit Plan '" + RecurringPlan.Name + "'" : "New Plan")%></h2>
    <asp:HiddenField runat="server" ID="hdnRecurringPlanID" Value='<%# RecurringPlan.RecurringPlanID %>'></asp:HiddenField>
    <table class="editForm">
        <tr><td>Product:</td><td><cc1:ProductDDL ID="ProductDDL1" runat="server" SelectedValue='<%# RecurringPlan.ProductID %>' class="validate[required]" style="width:150px;"></cc1:ProductDDL></td></tr>
        <tr><td>Name:</td><td><asp:TextBox runat="server" ID="tbName" Text='<%# RecurringPlan.Name %>' class="validate[required]"></asp:TextBox></td></tr>
        <tr><td colspan="2">
            <asp:Button runat="server" ID="btnSave" Text="Save" onclick="btnSave_Click" />
            <input type="button" value="Cancel" onclick="if (hideRecurringPlan) hideRecurringPlan();" />
        </td></tr>
    </table>    
    </form>
    <div class='space'></div>
    <div id="errorMsg" style="max-width:500px;">
    <asp:PlaceHolder runat="server" ID="phError">    
        <span class='small-alert'>Can't save plan.</span>
    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" ID="phSuccess">
        <span>Plan was successfully saved.</span>
    </asp:PlaceHolder>        
    </div>
    </div>    
    <asp:PlaceHolder runat="server" visible='<%# RecurringPlan.RecurringPlanID != null %>'>
    <div style="padding-left:30px;float:left;">
        <h2>Plan Cycles&nbsp;&nbsp;&nbsp;<a href="javascript:void(0)" onclick="editPlanCycle(<%# SelectedRecurringPlanID %>, '');" class='addNewIcon' style="font-weight:normal">Add Cycle</a></h2>	        
        <uc1:RecurringPlanCycleList ID="RecurringPlanCycleList1" runat="server" ProposedProductID ='<%# ProposedProductID %>' RecurringPlanID='<%# SelectedRecurringPlanID %>'  />
    </div>
    </asp:PlaceHolder>        
</div>


