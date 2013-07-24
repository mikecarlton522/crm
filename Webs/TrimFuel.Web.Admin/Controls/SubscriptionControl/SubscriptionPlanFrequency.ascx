<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SubscriptionPlanFrequency.ascx.cs" Inherits="TrimFuel.Web.Admin.Controls.SubscriptionControl.SubscriptionPlanFrequency" %>
<option value=''>-- Select --</option>
<asp:Repeater runat="server" ID="rProducts">    
    <ItemTemplate><option value='<%# Eval("Value") %>' <%# ((int?)(Eval("Value")) == SelectedPlanFrequency  ? "selected" : "") %>><%# Eval("Value") %></option></ItemTemplate>
</asp:Repeater>