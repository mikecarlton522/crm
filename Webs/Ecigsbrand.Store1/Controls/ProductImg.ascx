<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProductImg.ascx.cs" Inherits="Ecigsbrand.Store1.Controls.ProductImg" %>
<%@ Import Namespace="Ecigsbrand.Store1.Logic" %>
<asp:PlaceHolder runat="server" ID="ph1" Visible='<%# ProductNumber == KnownProduct.StarterKit_OneTimeOrder_OriginalFlavor %>'><img src="images/cart-starter-kit.png" width="100" height="100"></asp:PlaceHolder>
<asp:PlaceHolder runat="server" ID="ph2" Visible='<%# ProductNumber == KnownProduct.StarterKit_OneTimeOrder_MentholFlavor %>'><img src="images/cart-starter-kit.png" width="100" height="100"></asp:PlaceHolder>
<asp:PlaceHolder runat="server" ID="ph3" Visible='<%# ProductNumber == KnownProduct.StarterKit_TrialOrder_OriginalFlavor %>'><img src="images/cart-starter-kit.png" width="100" height="100"></asp:PlaceHolder>
<asp:PlaceHolder runat="server" ID="ph4" Visible='<%# ProductNumber == KnownProduct.StarterKit_TrialOrder_MentholFlavor %>'><img src="images/cart-starter-kit.png" width="100" height="100"></asp:PlaceHolder>
<asp:PlaceHolder runat="server" ID="ph5" Visible='<%# ProductNumber == KnownProduct.OriginalFlavor_StandardNicotine %>'><img src="images/cart-cartridge-tobacco.png" width="100" height="100"></asp:PlaceHolder>
<asp:PlaceHolder runat="server" ID="ph6" Visible='<%# ProductNumber == KnownProduct.OriginalFlavor_MediumNicotine %>'><img src="images/cart-cartridge-tobacco.png" width="100" height="100"></asp:PlaceHolder>
<asp:PlaceHolder runat="server" ID="ph7" Visible='<%# ProductNumber == KnownProduct.OriginalFlavor_LowNicotine %>'><img src="images/cart-cartridge-tobacco.png" width="100" height="100"></asp:PlaceHolder>
<asp:PlaceHolder runat="server" ID="ph9" Visible='<%# ProductNumber == KnownProduct.NoNicFlavor %>'><img src="images/cart-cartridge-tobacco.png" width="100" height="100"></asp:PlaceHolder>
<asp:PlaceHolder runat="server" ID="ph8" Visible='<%# ProductNumber == KnownProduct.MentholFlavor_StandardNicotine %>'><img src="images/cart-cartridge-menthol.png" width="100" height="100"></asp:PlaceHolder>
<asp:PlaceHolder runat="server" ID="ph12" Visible='<%# ProductNumber == KnownProduct.MentholFlavor_LowNicotine %>'><img src="images/cart-cartridge-menthol.png" width="100" height="100"></asp:PlaceHolder>
<asp:PlaceHolder runat="server" ID="ph10" Visible='<%# ProductNumber == KnownProduct.CarCharger %>'><img src="images/cart-car-charger.png" width="100" height="100"></asp:PlaceHolder>
<asp:PlaceHolder runat="server" ID="ph11" Visible='<%# ProductNumber == KnownProduct.WallCharger %>'><img src="images/cart-wall-charger.png" width="100" height="100"></asp:PlaceHolder>
<asp:PlaceHolder runat="server" ID="ph13" Visible='<%# ProductNumber == KnownProduct.Battery %>'><img width="100" height="100"></asp:PlaceHolder>