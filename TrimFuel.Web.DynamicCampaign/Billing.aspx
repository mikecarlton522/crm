<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Billing.aspx.cs" Inherits="TrimFuel.Web.DynamicCampaign.BillingPage" MasterPageFile="~/Masterpages/Dynamic.Master" %>
<%@ MasterType VirtualPath="~/MasterPages/Dynamic.Master"  %>
<asp:Content ID="Content1" ContentPlaceHolderID="cpHead" runat="server">  
<script type="text/javascript" src="js/billing.js"></script>
<script type="text/javascript" src="js/date.js"></script>  
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cpContent" runat="server">
    <asp:Literal ID="html" runat="server" /> 
</asp:Content>
