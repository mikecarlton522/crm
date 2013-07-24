<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CalendarData.aspx.cs" Inherits="TrimFuel.Web.Admin.AjaxControls.CalendarData" %>
<%@ Import Namespace="TrimFuel.Web.Admin.Logic" %>
<asp:Repeater runat="server" ID="rInvoices">
<ItemTemplate>
    '<%# Convert.ToDateTime(Eval("Invoice.CreateDT")).ToString("M/d/yyyy") %>~
    <asp:Repeater runat="server" ID="rSales" DataSource='<%# Eval("SaleList") %>'>
        <ItemTemplate>
            <div style="width:100%">
                <div style="float:left;"><b><%# OrderHelper.ShowSaleType(Container.DataItem) %><br /></b></div>                    
                <div style="float:right;"><%# OrderHelper.ShowSalePrice(Container.DataItem) %></div>
                <div class="space"><%# FixString(OrderHelper.ShowSaleDescription(Container.DataItem))%></div>                    
            </div>
        </ItemTemplate>
    </asp:Repeater>            
    ~~~',
</ItemTemplate>
</asp:Repeater>
<asp:Repeater runat="server" ID="rPlannedInvoices">
<ItemTemplate>
    '<%# Convert.ToDateTime(Eval("Invoice.CreateDT")).ToString("M/d/yyyy") %>~
    <asp:Repeater runat="server" ID="rSales" DataSource='<%# Eval("SaleList") %>'>
        <ItemTemplate>
            <div style="width:100%">
                <div style="float:left;"><b>Planned <%# OrderHelper.ShowSaleType(Container.DataItem) %><br /></b></div>                    
                <div style="float:right;"><%# OrderHelper.ShowSalePrice(Container.DataItem) %></div>
                <div class="space"><%# FixString(OrderHelper.ShowSaleDescription(Container.DataItem))%></div>                    
            </div>
        </ItemTemplate>
    </asp:Repeater>            
    ~~~',
</ItemTemplate>
</asp:Repeater>
