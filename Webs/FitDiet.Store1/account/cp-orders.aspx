<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/Front.Master" AutoEventWireup="true" CodeBehind="cp-orders.aspx.cs" Inherits="Fitdiet.Store1.account.cp_orders" %>
<%@ Register src="../Controls/MenuControlPanel.ascx" tagname="MenuControlPanel" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">E-Cigs: My Orders</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphScript" runat="server">
<link rel="stylesheet" type="text/css" href="../css/control-panel.css" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
<div id="content">
  <div class="left">
    <h1>Customer &amp; Referral Control Panel</h1>
    <uc1:MenuControlPanel ID="MenuControlPanel1" runat="server" />
    <h2>My Orders</h2>
    <h3>Upcoming Shipments</h3>
    <div class="blueborder" style="border:1px solid #C3D9FF; width:100%;">
      <table width="100%" border="0" cellspacing="0">
        <tr class="header">
          <td>Upcoming Order Date</td>
          <td>Product To Be Shipped</td>
          <td>Estimated Shipment Date</td>
          <td>Price</td>
          <td>Shipping &amp; Handling</td>
        </tr>
        <asp:Repeater runat="server" ID="rPendingSales">
            <ItemTemplate>
                <tr class="white">
                  <td width="20%"><%# Convert.ToDateTime(Eval("CreateDT")).ToString("MMMM d, yyyy")%></td>
                  <td width="27%">
                    <asp:Repeater runat="server" ID="rSaleInventories" DataSource='<%# Eval("InventoryList") %>'>
                        <ItemTemplate>
                            <%# Eval("Product") %>&nbsp;
                            <%# (Convert.ToInt32(Eval("Quantity")) > 1) ? string.Format("({0} Items)", Eval("Quantity")) : ""%>
                        </ItemTemplate>
                        <SeparatorTemplate>
                            <br />
                        </SeparatorTemplate>
                    </asp:Repeater>
                  </td>
                  <td width="24%"><%# Convert.ToDateTime(Eval("ShippedDT")).ToString("MMMM d, yyyy")%></td>
                  <td width="12%"><%# (Eval("ChargeAmount") == null || Convert.ToDecimal(Eval("ChargeAmount")) == 0) ? "free" : Convert.ToDecimal(Eval("ChargeAmount")).ToString("c")%></td>
                  <td width="17%"></td>
                </tr>
            </ItemTemplate>
            <AlternatingItemTemplate>
                <tr>
                  <td width="20%"><%# Convert.ToDateTime(Eval("CreateDT")).ToString("MMMM d, yyyy")%></td>
                  <td width="27%">
                    <asp:Repeater runat="server" ID="rSaleInventories" DataSource='<%# Eval("InventoryList") %>'>
                        <ItemTemplate>
                            <%# Eval("Product") %>&nbsp;
                            <%# (Convert.ToInt32(Eval("Quantity")) > 1) ? string.Format("({0} Items)", Eval("Quantity")) : ""%>
                        </ItemTemplate>
                        <SeparatorTemplate>
                            <br />
                        </SeparatorTemplate>
                    </asp:Repeater>
                  </td>
                  <td width="24%"><%# Convert.ToDateTime(Eval("ShippedDT")).ToString("MMMM d, yyyy")%></td>
                  <td width="12%"><%# (Eval("ChargeAmount") == null || Convert.ToDecimal(Eval("ChargeAmount")) == 0) ? "free" : Convert.ToDecimal(Eval("ChargeAmount")).ToString("c")%></td>
                  <td width="17%"></td>
                </tr>
            </AlternatingItemTemplate>
        </asp:Repeater>
        <asp:PlaceHolder runat="server" ID="phNoPendingSales" Visible="false">
            <tr class="white">
              <td colspan="5">No orders found</td>
            </tr>
        </asp:PlaceHolder>
      </table>
    </div>
    <h3>Previous Orders</h3>
    <div style="border:1px solid #C3D9FF; width:100%;">
      <table width="100%" border="0" cellspacing="0">
        <tr class="header">
          <td>Order Date</td>
          <td>Product</td>
          <td>Shipment Date</td>
          <td>Price</td>
          <td>Actions</td>
        </tr>
        <asp:Repeater runat="server" ID="rShippedSales">
            <ItemTemplate>
                <tr class="white">
                  <td width="20%"><%# Convert.ToDateTime(Eval("CreateDT")).ToString("MMMM d, yyyy")%></td>
                  <td width="27%">
                    <asp:Repeater runat="server" ID="rSaleInventories" DataSource='<%# Eval("InventoryList") %>'>
                        <ItemTemplate>
                            <%# Eval("Product") %>&nbsp;
                            <%# (Convert.ToInt32(Eval("Quantity")) > 1) ? string.Format("({0} Items)", Eval("Quantity")) : ""%>
                        </ItemTemplate>
                        <SeparatorTemplate>
                            <br />
                        </SeparatorTemplate>
                    </asp:Repeater>
                  </td>
                  <td width="24%"><%# Convert.ToDateTime(Eval("ShippedDT")).ToString("MMMM d, yyyy")%></td>
                  <td width="12%"><%# (Eval("ChargeAmount") == null || Convert.ToDecimal(Eval("ChargeAmount")) == 0) ? "free" : Convert.ToDecimal(Eval("ChargeAmount")).ToString("c")%></td>
                  <td width="17%"></td>
                </tr>
            </ItemTemplate>
            <AlternatingItemTemplate>
                <tr>
                  <td width="20%"><%# Convert.ToDateTime(Eval("CreateDT")).ToString("MMMM d, yyyy")%></td>
                  <td width="27%">
                    <asp:Repeater runat="server" ID="rSaleInventories" DataSource='<%# Eval("InventoryList") %>'>
                        <ItemTemplate>
                            <%# Eval("Product") %>&nbsp;
                            <%# (Convert.ToInt32(Eval("Quantity")) > 1) ? string.Format("({0} Items)", Eval("Quantity")) : ""%>
                        </ItemTemplate>
                        <SeparatorTemplate>
                            <br />
                        </SeparatorTemplate>
                    </asp:Repeater>
                  </td>
                  <td width="24%"><%# Convert.ToDateTime(Eval("ShippedDT")).ToString("MMMM d, yyyy")%></td>
                  <td width="12%"><%# (Eval("ChargeAmount") == null || Convert.ToDecimal(Eval("ChargeAmount")) == 0) ? "free" : Convert.ToDecimal(Eval("ChargeAmount")).ToString("c")%></td>
                  <td width="17%"></td>
                </tr>
            </AlternatingItemTemplate>
        </asp:Repeater>
        <asp:PlaceHolder runat="server" ID="phNoShippedSales" Visible="false">
            <tr class="white">
              <td colspan="5">No orders found</td>
            </tr>
        </asp:PlaceHolder>
      </table>
    </div>
  </div>
</div>
<div style="clear:both;">
</div>
</asp:Content>
