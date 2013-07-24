<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProductImg.ascx.cs" Inherits="Fitdiet.Store1.Controls.ProductImg" %>
<%@ Import Namespace="Fitdiet.Store1.Logic" %>
<asp:PlaceHolder runat="server" ID="ph1" Visible='<%# ProductNumber == KnownProduct.A30DayTrialKit %>'><img src="images/30day.jpg" width="100" height="100"></asp:PlaceHolder>
<asp:PlaceHolder runat="server" ID="ph2" Visible='<%# ProductNumber == KnownProduct.B60DayStarterKit %>'><img src="images/60day.jpg" width="100" height="100"></asp:PlaceHolder>
<asp:PlaceHolder runat="server" ID="ph3" Visible='<%# ProductNumber == KnownProduct.FitHerbalEnergyFatBurnerCapsules %>'><img src="images/fir_herbal.jpg" width="100" height="100"></asp:PlaceHolder>