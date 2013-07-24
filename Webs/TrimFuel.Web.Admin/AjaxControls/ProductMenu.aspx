<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProductMenu.aspx.cs" Inherits="TrimFuel.Web.Admin.AjaxControls.ProductMenu" %>

<asp:repeater id="rProducts" datasource='<%# Products %>' runat="server">
    <ItemTemplate>
        <div class="section" id="toggle">
            <a class="productGroupIcon" href='#' onclick='showProduct(<%# Eval("ProductID") %>, <%# Eval("ProductName", "\"{0}\"") %>); return false;'>
                <h2>
                    <%# Eval("ProductName") %> (<%# Eval("ProductID")%>)</h2>
            </a>
            <div class="hidden">
                <div class="settings">
                    <a class="mid" href='javascript:showSetting("mid", <%# Eval("ProductID") %>, <%# Eval("ProductName", "\"{0}\"") %>, "MID Management");'>
                        <h2 style="font-weight: normal;">
                            MID Management</h2>
                    </a><a class="shipper" href='javascript:showSetting("shipper", <%# Eval("ProductID") %>, <%# Eval("ProductName", "\"{0}\"") %>, "Shipper Management");'>
                        <h2 style="font-weight: normal;">
                            Shipper Management</h2>
                    </a><a class="subscription" href='javascript:showSetting("subscription", <%# Eval("ProductID") %>, <%# Eval("ProductName", "\"{0}\"") %>, "Subscription Management");'>
                        <h2 style="font-weight: normal;">
                            Subscription Management</h2>
                    </a><a class="routing" href='javascript:showSetting("routing", <%# Eval("ProductID") %>, <%# Eval("ProductName", "\"{0}\"") %>, "Traffic Routing");'>
                        <h2 style="font-weight: normal;">
                            Traffic Routing</h2>
                    </a><a class="flow" href='javascript:showSetting("flow", <%# Eval("ProductID") %>, <%# Eval("ProductName", "\"{0}\"") %>, "Campaign Management");'>
                        <h2 style="font-weight: normal;">
                            Campaign Management</h2>
                    </a><a class="email" href='javascript:showSetting("email", <%# Eval("ProductID") %>, <%# Eval("ProductName", "\"{0}\"") %>, "Email Management");'>
                        <h2 style="font-weight: normal;">
                            Email Management</h2>
                    </a><a class="lead" href='javascript:showSetting("lead", <%# Eval("ProductID") %>, <%# Eval("ProductName", "\"{0}\"") %>, "Lead Management");'>
                        <h2 style="font-weight: normal;">
                            Lead Management</h2>
                    </a><a class="events" href='javascript:showSetting("events", <%# Eval("ProductID") %>, <%# Eval("ProductName", "\"{0}\"") %>, "Events");'>
                        <h2 style="font-weight: normal;">
                            Events</h2>
                    </a><a class="inventory" href='javascript:showSetting("inventory", <%# Eval("ProductID") %>, <%# Eval("ProductName", "\"{0}\"") %>, "Products");'>
                        <h2 style="font-weight: normal;">
                            Product List</h2>
                    </a><a class="docs" href='javascript:showSetting("docs", <%# Eval("ProductID") %>, <%# Eval("ProductName", "\"{0}\"") %>, "Documentation Link");'>
                        <h2 style="font-weight: normal;">
                            Documentation Link</h2>
                    </a>
                </div>
                <div class="clear" style="height: 10px;">
                </div>
            </div>
        </div>
    </ItemTemplate>
</asp:repeater>
<div class="section">
    <a href='#' class="productGroupIconAdd" onclick='addProduct(); return false;'>
        <h2>
            Add Product Group</h2>
    </a>
</div>
<div class="section">
    <a href='#' class="productGroupIconAdd" onclick='createProductGroup(); return false;'>
        <h2>
            Create New Product Group Based On</h2>
    </a>
</div>
