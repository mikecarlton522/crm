<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ClientsMenuItem.ascx.cs"
    Inherits="TrimFuel.Web.RapidApp.Controls.ClientsMenuItem" %>
<%@ Register Src="~/Controls/ClientServiceItem.ascx" TagName="ClientServiceItem"
    TagPrefix="Uc" %>
<a href="#" class="menu_show">
    <h2 class="<%# Class %>">
        <%# FolderName %></h2>
</a>
<div>
    <asp:Repeater runat="server" DataSource='<%# Data %>'>
        <ItemTemplate>
            <a href='#' onclick='showClientProfile(<%# Eval("Key") %>, this); return false;'>
                <h3 class="company">
                    <%# Eval("Value") %></h3>
            </a>
            <div style="display: none">
                <Uc:ClientServiceItem ID="ClientServiceItem2" TPClientID='<%# Eval("Key") %>' ServiceType='fulfillment' ServiceTypeDisplayName="Fulfillment"
                    runat="server" />
                <Uc:ClientServiceItem ID="ClientServiceItem1" TPClientID='<%# Eval("Key") %>' ServiceType='gateway' ServiceTypeDisplayName="Gateway"
                    runat="server" />
                <Uc:ClientServiceItem ID="ClientServiceItem3" TPClientID='<%# Eval("Key") %>' ServiceType='outbound' ServiceTypeDisplayName="Call Center Outbound"
                    runat="server" />
            </div>
        </ItemTemplate>
    </asp:Repeater>
</div>
