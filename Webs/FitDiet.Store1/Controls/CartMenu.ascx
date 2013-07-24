<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CartMenu.ascx.cs" Inherits="FitDiet.Store1.Controls.CartMenu" %>
<table width="697" border="0" align="center" cellpadding="0" cellspacing="0" class="numbering_bg">
    <tr runat="server" id="trLinks">
        <td width="45" align="center">
            <img src="images/one.gif" alt="" width="19" height="19" style="padding-top:2px;" />
        </td>
        <td width="162" align="center" >
            <asp:PlaceHolder runat="server" ID="phShoppingCart" >
            </asp:PlaceHolder>
        </td>
        <td width="19" align="center">
            <img src="images/two.gif" alt="" width="19" height="19" style="padding-top:2px;" />
        </td>
        <td width="162" align="center">
            <asp:PlaceHolder runat="server" ID="phShippingDet" >
            </asp:PlaceHolder>
        </td>
        <td width="19" align="center">
            <img src="images/three.gif" alt="" width="19" height="19" style="padding-top:2px;" />
        </td>
        <td width="162" align="center">
            <asp:PlaceHolder runat="server" ID="phBillingDet" >
            </asp:PlaceHolder>
        </td>
        <td width="19" align="center">
            <img src="images/four.gif" alt="" width="19" height="19" style="padding-top:2px;" />
        </td>
        <td width="162" align="center">
            <asp:PlaceHolder runat="server" ID="phPlaceMyOrder" >
            </asp:PlaceHolder>
        </td>
    </tr>
</table>
