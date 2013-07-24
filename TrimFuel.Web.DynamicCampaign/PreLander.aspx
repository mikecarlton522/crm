<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PreLander.aspx.cs" Inherits="TrimFuel.Web.DynamicCampaign.PreLander" MasterPageFile="~/Masterpages/Dynamic.Master"  %>
<%@ MasterType VirtualPath="~/MasterPages/Dynamic.Master"  %>
<asp:Content ID="Content1" ContentPlaceHolderID="cpHead" runat="server">    
<script type="text/javascript" src="js/billing.js"></script>
<script type="text/javascript" src="js/date.js"></script>  
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cpContent" runat="server">
    <asp:Literal ID="_html" runat="server" />
</asp:Content>
