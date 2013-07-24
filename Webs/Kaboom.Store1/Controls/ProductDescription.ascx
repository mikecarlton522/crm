<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProductDescription.ascx.cs" Inherits="Kaboom.Store1.Controls.ProductDescription" %>
<%@ Import Namespace="Kaboom.Store1.Logic" %>
<asp:PlaceHolder runat="server" ID="ph1" Visible='<%# ProductNumber == KnownProduct.KaboomCombo_1x2_30_Trial %>'>
    <asp:PlaceHolder runat="server" Visible='<%# Type == DescriptionType.Promotion %>'>
        <span class="up">Kaboom Ultra Combo Trial</span><br />
        Try the Kaboom Ultra Combo at Home for 12 Days.<br />
        <br />
        - 
        Trial-Size Action Strips (2 Strips)<br />
        - 
        Kaboom Daily (30 Capsules)
    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" Visible='<%# Type == DescriptionType.Cart %>'>
        Kaboom Ultra Combo Trial<br/>
        <br />
        - 
        Trial-Size Action Strips (2 Strips)<br />
        - 
        Kaboom Daily (30 Capsules)
    </asp:PlaceHolder>    
</asp:PlaceHolder>
<asp:PlaceHolder runat="server" ID="ph2" Visible='<%# ProductNumber == KnownProduct.KaboomCombo_1x12_60_Upsell %>'>
    <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%# Type == DescriptionType.Promotion %>'>
        <span class="up">Kaboom Ultra Combo - Save 20%</span><br />
        Combine the amazing power of both Kaboom Action Strips with Daily Formula. Feel the surge of sexual energy when you want.<br />
        <br />
        - 
        Action Strips (12 Strips)<br />
        - 
        Kaboom Daily (60 Capsules)
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%# Type == DescriptionType.Cart %>'>
        Kaboom Ultra Combo<br/>
        <br />
        - 
        Action Strips (12 Strips)<br />
        - 
        Kaboom Daily (60 Capsules)
    </asp:PlaceHolder>    
</asp:PlaceHolder>
<asp:PlaceHolder runat="server" ID="ph3" Visible='<%# ProductNumber == KnownProduct.KaboomStrips_1x12_Upsell %>'>
    <asp:PlaceHolder ID="PlaceHolder3" runat="server" Visible='<%# Type == DescriptionType.Promotion %>'>
        <span class="up">Kaboom Action Strips - 12 Action Strips</span><br />
        Give your body an instant jolt of sexual arousal. Used minutes before sex, this one of a kind herbal formula tastes great, dissolves quickly, and works fast providing fuller and harder erections with sustained stamina.
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="PlaceHolder4" runat="server" Visible='<%# Type == DescriptionType.Cart %>'>
        Kaboom Action Strips - 12 Action Strips<br/>
    </asp:PlaceHolder>    
</asp:PlaceHolder>
<asp:PlaceHolder runat="server" ID="ph4" Visible='<%# ProductNumber == KnownProduct.KaboomDaily_1x60_Upsell %>'>
    <asp:PlaceHolder ID="PlaceHolder5" runat="server" Visible='<%# Type == DescriptionType.Promotion %>'>
        <span class="up">Kaboom Daily - 60 Capsules</span><br />
        100% all-natural formula that promotes healthy male hormonal balance and sexual stamina in a convenient once-daily capsule. This advanced herbal formula helps build and sustain optimal male hormone levels leading to increased energy, vitality, and sexual performance.
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="PlaceHolder6" runat="server" Visible='<%# Type == DescriptionType.Cart %>'>
        Kaboom Daily - 60 Capsules<br/>
    </asp:PlaceHolder>    
</asp:PlaceHolder>
<asp:PlaceHolder runat="server" ID="ph5" Visible='<%# ProductNumber == KnownProduct.KaboomDaily_1x30_Upsell %>'>
    <asp:PlaceHolder ID="PlaceHolder7" runat="server" Visible='<%# Type == DescriptionType.Promotion %>'>
        <span class="up">Kaboom Daily - 30 Capsules</span><br />
        Same amazing Kaboom Daily Formula in a smaller travel-size container. Always be ready for Action when the time comes!
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="PlaceHolder8" runat="server" Visible='<%# Type == DescriptionType.Cart %>'>
        Kaboom Daily - 30 Capsules<br/>
    </asp:PlaceHolder>    
</asp:PlaceHolder>