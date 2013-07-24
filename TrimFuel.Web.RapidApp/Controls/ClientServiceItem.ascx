<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ClientServiceItem.ascx.cs"
    Inherits="TrimFuel.Web.RapidApp.Controls.ClientServiceItem" %>
<a href="#" class="menu_show">
    <h4 class='tpclientservice_<%# ServiceType.ToLower() %>'>
        <%# ServiceTypeDisplayName%></h4>
</a>
<div style="display: none;">
    <a href="#" onclick="showClientServiceType(<%# TPClientID %>, '<%# ServiceType.ToLower() %>', this); return false;">
        <h4 class="service">General Settings</h4>
    </a>
    <asp:Repeater ID="rf2" runat="server" DataSource='<%# Services %>'>
        <ItemTemplate>
            <a href="#" onclick='showClientService(<%# TPClientID %>, <%# "\"" + ServiceType + "\"" %> ,<%# Eval("ServiceName", "\"{0}\"") %>, <%# Eval("ID") %>, <%# Eval("BaseServiceName", "\"{0}\"") %>, this); return false;'>
                <h4 class='tpclientservice_<%# ServiceType.ToLower() %> service'>
                    <%# Eval("DisplayName") %></h4>
            </a>
        </ItemTemplate>
    </asp:Repeater>
    <a href="#" onclick='addService(<%# TPClientID %>, <%# "\"" + ServiceType + "\"" %>, this); return false;'>
        <h4 class="add service">
            Add Service</h4>
    </a>
</div>
