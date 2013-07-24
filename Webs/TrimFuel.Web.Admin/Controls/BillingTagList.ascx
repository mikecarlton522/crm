<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BillingTagList.ascx.cs" Inherits="TrimFuel.Web.Admin.Controls.BillingTagList" %>
<asp:Repeater runat="server" ID="rTag">
    <ItemTemplate>
        <%# DataBinder.Eval(Container.DataItem, "TagValue") %>
    </ItemTemplate>
    <SeparatorTemplate>,&nbsp;</SeparatorTemplate>
</asp:Repeater>
<%# (rTag.Items.Count == 0) ? "-" : ""%>