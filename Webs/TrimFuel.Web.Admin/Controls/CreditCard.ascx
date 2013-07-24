<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CreditCard.ascx.cs" Inherits="TrimFuel.Web.Admin.Controls.CreditCard_" %>
<%@ Register Assembly="TrimFuel.Business" Namespace="TrimFuel.Business.Controls"
    TagPrefix="cc1" %>
<tr><td>Credit Card Number:</td><td><asp:TextBox runat="server" ID="tbCreditCardNumber" MaxLength="16" CssClass="validate[custom[CreditCard]]"></asp:TextBox></td></tr>
<tr><td>Security Code:</td><td><asp:TextBox runat="server" ID="tbCreditCardCVV" style="width:50px;" MaxLength="5" CssClass=""></asp:TextBox></td></tr>
<tr><td>Expiration (Month/Year):</td><td><cc1:DDLMonth ID="ddlMonth" runat="server" CssClass="validate[custom[Numeric]]"></cc1:DDLMonth> / <cc1:DDLYear ID="ddlYear" CssClass="validate[custom[Numeric]]" runat="server"></cc1:DDLYear></td></tr>
<script type="text/javascript">
    function ValidateCreditCard() {
        return Mod10($("#<%# tbCreditCardNumber.ClientID %>").val());
    }
</script>
