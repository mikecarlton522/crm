<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProductDescription.ascx.cs" Inherits="Ecigsbrand.Store1.Controls.ProductDescription" %>
<%@ Import Namespace="Ecigsbrand.Store1.Logic" %>
<asp:PlaceHolder runat="server" ID="ph1" Visible='<%# ProductNumber == KnownProduct.StarterKit_OneTimeOrder_OriginalFlavor %>'>
    E-Cigs Starter Kit<br/><br />
    <span style="font-weight:normal">Original Tobacco Flavor</span>
</asp:PlaceHolder>
<asp:PlaceHolder runat="server" ID="ph2" Visible='<%# ProductNumber == KnownProduct.StarterKit_OneTimeOrder_MentholFlavor %>'>
    E-Cigs Starter Kit<br/><br />
    <span style="font-weight:normal">Minty Menthol Flavor</span>
</asp:PlaceHolder>
<asp:PlaceHolder runat="server" ID="ph3" Visible='<%# ProductNumber == KnownProduct.StarterKit_TrialOrder_OriginalFlavor %>'>
    E-Cigs Starter Kit with Refill Membership<br/><br />
    <span style="font-weight:normal">Original Tobacco Flavor</span><br/><br />
    <span style="font-weight:normal">15 Day Trial Membership Program: Unless you call to cancel, in 15 days and every 30 days thereafter, you will be sent 10 refill packs (equivalent to 20 packs of tobacco cigarettes) for only $79.99 plus S&H. To
        modify your order at anytime call 1-866-830-2464. Limit one per customer.</span>
</asp:PlaceHolder>
<asp:PlaceHolder runat="server" ID="ph4" Visible='<%# ProductNumber == KnownProduct.StarterKit_TrialOrder_MentholFlavor %>'>
    E-Cigs Starter Kit with Refill Membership<br/><br />
    <span style="font-weight:normal">Minty Menthol Flavor</span><br /><br />
    <span style="font-weight:normal">15 Day Trial Membership Program: Unless you call to cancel, in 15 days and every 30 days thereafter, you will be sent 10 refill packs (equivalent to 20 packs of tobacco cigarettes) for only $79.99 plus S&H. To
        modify your order at anytime call 1-866-830-2464. Limit one per customer.</span>
</asp:PlaceHolder>
<asp:PlaceHolder runat="server" ID="ph5" Visible='<%# ProductNumber == KnownProduct.OriginalFlavor_StandardNicotine %>'>
    Original Tobacco Flavor<br/><br />
    <span style="font-weight:normal">Standard Nicotine</span><br/>
    <span style="font-weight:normal">Box of 10 Cartridges</span>
</asp:PlaceHolder>
<asp:PlaceHolder runat="server" ID="ph6" Visible='<%# ProductNumber == KnownProduct.OriginalFlavor_MediumNicotine %>'>
    Original Tobacco Flavor<br/><br />
    <span style="font-weight:normal">Medium Nicotine</span><br/>
    <span style="font-weight:normal">Box of 10 Cartridges</span>
</asp:PlaceHolder>
<asp:PlaceHolder runat="server" ID="ph7" Visible='<%# ProductNumber == KnownProduct.OriginalFlavor_LowNicotine %>'>
    Original Tobacco Flavor<br/><br />
    <span style="font-weight:normal">Low Nicotine</span><br/>
    <span style="font-weight:normal">Box of 10 Cartridges</span>
</asp:PlaceHolder>
<asp:PlaceHolder runat="server" ID="ph8" Visible='<%# ProductNumber == KnownProduct.MentholFlavor_StandardNicotine %>'>
    Minty Menthol Flavor<br/><br />
    <span style="font-weight:normal">Standard Nicotine</span><br/>
    <span style="font-weight:normal">Box of 10 Cartridges</span>
</asp:PlaceHolder>
<asp:PlaceHolder runat="server" ID="ph12" Visible='<%# ProductNumber == KnownProduct.MentholFlavor_LowNicotine %>'>
    Minty Menthol Flavor<br/><br />
    <span style="font-weight:normal">Low Nicotine</span><br/>
    <span style="font-weight:normal">Box of 10 Cartridges</span>
</asp:PlaceHolder>
<asp:PlaceHolder runat="server" ID="ph9" Visible='<%# ProductNumber == KnownProduct.NoNicFlavor %>'>
    Original Tobacco Flavor<br/><br />
    <span style="font-weight:normal">No Nicotine</span><br/>
    <span style="font-weight:normal">Box of 10 Cartridges</span>
</asp:PlaceHolder>
<asp:PlaceHolder runat="server" ID="ph10" Visible='<%# ProductNumber == KnownProduct.CarCharger %>'>
    E-Cigarette Car Charger
</asp:PlaceHolder>
<asp:PlaceHolder runat="server" ID="ph11" Visible='<%# ProductNumber == KnownProduct.WallCharger %>'>
    E-Cigarette Wall Charger
</asp:PlaceHolder>
<asp:PlaceHolder runat="server" ID="ph13" Visible='<%# ProductNumber == KnownProduct.Battery %>'>
    E-Cigarette Battery<br /><br />
    <span style="font-weight:normal">High-Capacity Lithium-Ion Battery</span>
</asp:PlaceHolder>
