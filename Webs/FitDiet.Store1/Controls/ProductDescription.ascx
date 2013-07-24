<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProductDescription.ascx.cs" Inherits="Fitdiet.Store1.Controls.ProductDescription" %>
<%@ Import Namespace="Fitdiet.Store1.Logic" %>
<asp:PlaceHolder runat="server" ID="ph1" Visible='<%# ProductNumber == KnownProduct.A30DayTrialKit %>'>
    Fit Diet <span style="font-weight:normal">30-DAY TRIAL KIT</span>
</asp:PlaceHolder>
<asp:PlaceHolder runat="server" ID="ph2" Visible='<%# ProductNumber == KnownProduct.B60DayStarterKit %>'>
    Fit Diet <span style="font-weight:normal">60-DAY STARTER KIT</span>
</asp:PlaceHolder>
<asp:PlaceHolder runat="server" ID="ph3" Visible='<%# ProductNumber == KnownProduct.FitHerbalEnergyFatBurnerCapsules %>'>
    Fit Diet <span style="font-weight:normal">FIT™ HERBAL ENERGY FAT BURNER CAPSULES</span><br/><br />    
</asp:PlaceHolder>