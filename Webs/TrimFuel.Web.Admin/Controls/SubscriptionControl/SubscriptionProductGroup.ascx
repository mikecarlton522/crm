<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SubscriptionProductGroup.ascx.cs" Inherits="TrimFuel.Web.Admin.Controls.SubscriptionControl.SubscriptionProductGroup" %>
<option value=''>-- Select --</option>
<asp:Repeater runat="server" ID="rProducts">    
    <ItemTemplate><option value='<%# Eval("ProductID") %>' <%# (new int?(Convert.ToInt32(Eval("ProductID"))) == SelectedProductID ? "selected" : "") %>><%# Eval("ProductName") %></option></ItemTemplate>
</asp:Repeater>