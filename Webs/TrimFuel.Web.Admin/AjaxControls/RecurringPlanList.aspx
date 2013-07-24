<%@ Page EnableViewState="false" Language="C#" AutoEventWireup="true" CodeBehind="RecurringPlanList.aspx.cs" Inherits="TrimFuel.Web.Admin.AjaxControls.RecurringPlanList" %>
<%@ Register src="../Controls/RecurringPlanBriefView2.ascx" tagname="RecurringPlanBriefView2" tagprefix="uc1" %>
<form id="form1" runat="server">
<table class="sortable" border="0" cellpadding="0" cellspacing="1" width="100%">
	<thead>
	<tr class="header">
        <td rowspan="1" style="width:50px;">Plan ID</td>
		<td rowspan="1">Product</td>
		<td rowspan="1">Friendly Name</td>
		<td rowspan="1">Cycles</td>
		<td rowspan="1"></td>
	</tr>
	</thead>
	<tbody>
	<asp:Repeater runat="server" ID="rpSubscriptions">
	<ItemTemplate>
	    <tr id="recurring-plan_<%# Eval("RecurringPlanID")%>" <%# (SelectedRecurringPlanID == Convert.ToInt32(Eval("RecurringPlanID")) ? "class='selected'" : "") %> >
		    <td><%# Eval("RecurringPlanID")%></td>
		    <td><%# Eval("ProductName")%></td>
		    <td><%# Eval("Name")%></td>
		    <td><uc1:RecurringPlanBriefView2 ID="RecurringPlanBriefView21" runat="server" RecurringPlan='<%# Container.DataItem %>' /></td>
		    <td><a href="javascript:editRecurringPlan(<%# Eval("RecurringPlanID")%>, this)" class="editIcon">Edit</a>&nbsp;&nbsp;
                <a href="javascript:deleteRecurringPlan(<%# Eval("RecurringPlanID")%>)" class="removeIcon">Delete</a>&nbsp;&nbsp;
            </td>
	    </tr>	
	</ItemTemplate>
    <AlternatingItemTemplate>
	    <tr id="recurring-plan_<%# Eval("RecurringPlanID")%>" <%# (SelectedRecurringPlanID == Convert.ToInt32(Eval("RecurringPlanID")) ? "class='selected offset'" : "class='offset'") %> >
		    <td><%# Eval("RecurringPlanID")%></td>
		    <td><%# Eval("ProductName")%></td>
		    <td><%# Eval("Name")%></td>
		    <td><uc1:RecurringPlanBriefView2 ID="RecurringPlanBriefView21" runat="server" RecurringPlan='<%# Container.DataItem %>' /></td>
		    <td><a href="javascript:editRecurringPlan(<%# Eval("RecurringPlanID")%>, this)" class="editIcon">Edit</a>&nbsp;&nbsp;
                <a href="javascript:deleteRecurringPlan(<%# Eval("RecurringPlanID")%>)" class="removeIcon">Delete</a>&nbsp;&nbsp;
            </td>
	    </tr>	
    </AlternatingItemTemplate>
	</asp:Repeater>
	<asp:PlaceHolder runat="server" Visible='<%# rpSubscriptions.Items.Count == 0 %>'>
        <tr><td colspan="5">No records found</td></tr>
	</asp:PlaceHolder>
	</tbody>
</table>
</form>
