<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DropDownUpsell.ascx.cs" Inherits="TrimFuel.Web.Admin.Controls.DropDownUpsell" %>
<%@ Import Namespace="TrimFuel.Business" %>
<%@ Register Assembly="TrimFuel.Web.UI" Namespace="TrimFuel.Web.UI" TagPrefix="cc1" %>
<asp:DropDownList runat="server" ID="ddlUpsellTypes">
    <asp:ListItem Value="">-- Select --</asp:ListItem>    
</asp:DropDownList>
<asp:DropDownList runat="server" ID="ddlUpsellTypes_STO_Ecigs" Visible='<%# (ProductID == 10 || ProductID == 14 || ProductID == 15 || ProductID == 20) && (Config.Current.APPLICATION_ID == "dashboard.trianglecrm.com" || Config.Current.APPLICATION_ID == "trimfuel.localhost") %>'>
    <asp:ListItem Value="">-- Select --</asp:ListItem>    
    <asp:ListItem Value="-1">-- E-Cig Starter Kits --</asp:ListItem>
    <asp:ListItem Value="161" Enabled="false">Starter Kit Tobacco @ $4.95</asp:ListItem>
    <asp:ListItem Value="158" Enabled="false">Starter Kit Tobacco @ $9.99</asp:ListItem>
    <asp:ListItem Value="127">Starter Kit Tobacco @ $29.99</asp:ListItem>
    <asp:ListItem Value="128">Starter Kit Menthol @ $29.99</asp:ListItem>
    <asp:ListItem Value="-2">-- E-Cigs Tobacco Refill Packs --</asp:ListItem>
    <asp:ListItem Value="129">1x High Nicotine Tobacco @ $19.99</asp:ListItem>
    <asp:ListItem Value="130">2x High Nicotine Tobacco @ $29.99</asp:ListItem>
    <asp:ListItem Value="131">2x Med Nicotine Tobacco @ $29.99</asp:ListItem>
    <asp:ListItem Value="132">2x Low Nicotine Tobacco @ $29.99</asp:ListItem>
    <asp:ListItem Value="133">2x No Nicotine Tobacco @ $29.99</asp:ListItem>
    <asp:ListItem Value="-3">-- E-Cigs Menthol Refill Packs --</asp:ListItem>
    <asp:ListItem Value="134">2x High Nicotine Menthol @ $29.99</asp:ListItem>
    <asp:ListItem Value="135">2x Low Nicotine Menthol @ $29.99</asp:ListItem>
    <asp:ListItem Value="-4">-- E-Cigs Accessories --</asp:ListItem>
    <asp:ListItem Value="137">Wall Charger @ $9.99</asp:ListItem>
    <asp:ListItem Value="136">Wall Charger @ $4.95</asp:ListItem>
    <asp:ListItem Value="139">Car Charger @ $9.99</asp:ListItem>
    <asp:ListItem Value="138">Car Charger @ $4.95</asp:ListItem>
    <asp:ListItem Value="100">E-Cig USB Adapter @ $4.95</asp:ListItem>
    <asp:ListItem Value="140">E-Cig Extra Battery @ $9.99</asp:ListItem>
    <asp:ListItem Value="163">E-Cig Carrying Case Black @ $14.99</asp:ListItem>
    <asp:ListItem Value="93" Enabled="false">E-Cig Carrying Case Red @ $9.99</asp:ListItem>
</asp:DropDownList>