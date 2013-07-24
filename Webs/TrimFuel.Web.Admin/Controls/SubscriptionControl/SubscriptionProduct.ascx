<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SubscriptionProduct.ascx.cs" Inherits="TrimFuel.Web.Admin.Controls.SubscriptionControl.SubscriptionProduct" %>
<option value=''>-- Select --</option>
<asp:Repeater runat="server" ID="rProducts">    
    <ItemTemplate><option value='<%# Eval("GroupProductSKU_") %>' <%# (Convert.ToString(Eval("GroupProductSKU_")) == SelectedGroupProductSKU && SelectedGroupProductSKU != ""  ? "selected" : "") %> <%# (Convert.ToString(Eval("GroupProductSKU_")) == ""  ? "disabled='disabled'" : "") %>><%# Eval("GroupProductName") %></option></ItemTemplate>
</asp:Repeater>