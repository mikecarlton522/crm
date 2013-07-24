<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/Admin.Master" AutoEventWireup="true" CodeBehind="management_custom_billing_form.aspx.cs" Inherits="TrimFuel.Web.Admin.management_custom_billing_form" %>
<%@ Register TagPrefix="gen" Assembly="TrimFuel.Business" Namespace="TrimFuel.Business.Controls" %>
<%@ Register assembly="TrimFuel.Web.UI" namespace="TrimFuel.Web.UI.Specialized" tagprefix="cc1" %>
<%@ Register src="Controls/SubscriptionControl/NewSubscriptionControl.ascx" tagname="NewSubscriptionControl" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphScript" runat="server">
<script type="text/javascript" src="/js/dropdowntree.js"></script>
<script type="text/javascript" src="/js/subscriptionControl.js"></script>
<script type="text/javascript">
    function ValidateCreditCard() {
        return Mod10($("#<%# tbCreditCardNumber.ClientID %>").val());
    }
    function onCountryChanged(country, postfix) {
        if (!postfix)
            postfix = "";
        if (country == "US") {
            $("#stateEx" + postfix).hide().find("[class*=validate]").addClass("donotvalidate");
            $("#zipEx" + postfix).hide().find("[class*=validate]").addClass("donotvalidate");
            $("#phoneEx" + postfix).hide().find("[class*=validate]").addClass("donotvalidate");
            $("#stateUS" + postfix).show().find("[class*=validate]").removeClass("donotvalidate");
            $("#zipUS" + postfix).show().find("[class*=validate]").removeClass("donotvalidate");
            $("#phoneUS" + postfix).show().find("[class*=validate]").removeClass("donotvalidate");
        }
        else {
            $("#stateEx" + postfix).show().find("[class*=validate]").removeClass("donotvalidate");
            $("#zipEx" + postfix).show().find("[class*=validate]").removeClass("donotvalidate");
            $("#phoneEx" + postfix).show().find("[class*=validate]").removeClass("donotvalidate");
            $("#stateUS" + postfix).hide().find("[class*=validate]").addClass("donotvalidate");
            $("#zipUS" + postfix).hide().find("[class*=validate]").addClass("donotvalidate");
            $("#phoneUS" + postfix).hide().find("[class*=validate]").addClass("donotvalidate");
        }
    }
    function CopyToBillingAddress() {
        $("#<%# tbFirstNameB.ClientID %>").val($("#<%# tbFirstName.ClientID %>").val());
        $("#<%# tbLastNameB.ClientID %>").val($("#<%# tbLastName.ClientID %>").val());
        $("#<%# tbAddress1B.ClientID %>").val($("#<%# tbAddress1.ClientID %>").val());
        $("#<%# tbAddress2B.ClientID %>").val($("#<%# tbAddress2.ClientID %>").val());
        $("#<%# ddlCountryB.ClientID %>").val($("#<%# ddlCountry.ClientID %>").val());
        $("#<%# tbCityB.ClientID %>").val($("#<%# tbCity.ClientID %>").val());
        $("#<%# ddlStateB.ClientID %>").val($("#<%# ddlState.ClientID %>").val());
        $("#<%# tbStateExB.ClientID %>").val($("#<%# tbStateEx.ClientID %>").val());
        $("#<%# tbZipB.ClientID %>").val($("#<%# tbZip.ClientID %>").val());
        $("#<%# tbZipExB.ClientID %>").val($("#<%# tbZipEx.ClientID %>").val());
        $("#<%# tbPhone1B.ClientID %>").val($("#<%# tbPhone1.ClientID %>").val());
        $("#<%# tbPhone2B.ClientID %>").val($("#<%# tbPhone2.ClientID %>").val());
        $("#<%# tbPhone3B.ClientID %>").val($("#<%# tbPhone3.ClientID %>").val());
        $("#<%# tbPhoneExB.ClientID %>").val($("#<%# tbPhoneEx.ClientID %>").val());
        $("#<%# tbEmailB.ClientID %>").val($("#<%# tbEmail.ClientID %>").val());
    }
    $(document).ready(function () {
        $("#<%# ddlCountry.ClientID %>").change(function () {
            onCountryChanged($("#<%# ddlCountry.ClientID %>").val());
        });
        onCountryChanged($("#<%# ddlCountry.ClientID %>").val());
        $("#<%# ddlCountryB.ClientID %>").change(function () {
            onCountryChanged($("#<%# ddlCountryB.ClientID %>").val(), "B");
        });
        onCountryChanged($("#<%# ddlCountryB.ClientID %>").val(), "B");
        $("#<%# chbCopyToBilling.ClientID %>").click(function () {
            if (this.checked) {
                CopyToBillingAddress();
            }
        });
        $("#aspnetForm").validationEngine({ inline: true, scroll: false });
    });
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphStyle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
<div class="data">
<form runat="server">
<div class="module">
    <h2>Shipping Address</h2>
    <table border="0" cellspacing="0" cellpadding="3" class="editForm">
        <tr><td>First Name:</td><td><asp:TextBox runat="server" ID="tbFirstName" MaxLength="100" CssClass="validate[custom[FirstName]]"></asp:TextBox></td></tr>
        <tr><td>Last Name:</td><td><asp:TextBox runat="server" ID="tbLastName" MaxLength="100" CssClass="validate[custom[LastName]]"></asp:TextBox></td></tr>
        <tr><td>Address 1:</td><td><asp:TextBox runat="server" ID="tbAddress1" MaxLength="50" CssClass="validate[custom[Address]]"></asp:TextBox></td></tr>
        <tr><td>Address 2:</td><td><asp:TextBox runat="server" ID="tbAddress2" MaxLength="50"></asp:TextBox></td></tr>
        <tr><td>City:</td><td><asp:TextBox runat="server" ID="tbCity" MaxLength="50" CssClass="validate[custom[City]]"></asp:TextBox></td></tr>
        <tr><td>Country:</td><td><gen:DDLCountry runat="server" ID="ddlCountry" style="width:150px;"></gen:DDLCountry></td></tr>
        <tr id="stateUS"><td>State:</td><td><gen:DDLStateFullName runat="server" ID="ddlState" style="width:150px;"></gen:DDLStateFullName></td></tr>
        <tr id="stateEx"><td>State/Province:</td><td><asp:TextBox runat="server" ID="tbStateEx" MaxLength="50"></asp:TextBox></td></tr>
        <tr id="zipUS"><td>Zip:</td><td><asp:TextBox runat="server" ID="tbZip" MaxLength="5" CssClass="validate[custom[Zip]] x-short"></asp:TextBox></td></tr>
        <tr id="zipEx"><td>Postal Code:</td><td><asp:TextBox runat="server" ID="tbZipEx" MaxLength="50"></asp:TextBox></td></tr>
        <tr id="phoneUS"><td>Phone:</td><td>( <asp:TextBox runat="server" ID="tbPhone1" style="width: 38px;" MaxLength="3"></asp:TextBox> ) <asp:TextBox runat="server" ID="tbPhone2" style="width: 38px;" MaxLength="3"></asp:TextBox> - <asp:TextBox runat="server" ID="tbPhone3" style="width: 46px;" MaxLength="4"></asp:TextBox></td></tr>
        <tr id="phoneEx"><td>Phone:</td><td><asp:TextBox runat="server" ID="tbPhoneEx" MaxLength="50"></asp:TextBox></td></tr>
        <tr><td>Email:</td><td><asp:TextBox runat="server" ID="tbEmail" MaxLength="50" CssClass="validate[custom[Email]]"></asp:TextBox></td></tr>
        <tr><td></td><td><asp:CheckBox runat="server" ID="chbCopyToBilling" Text="Copy to Billing Address" /></td></tr>
    </table>
</div>
<div class="module">
    <h2>Billing Address</h2>
    <table border="0" cellspacing="0" cellpadding="3" class="editForm">
        <tr><td>First Name:</td><td><asp:TextBox runat="server" ID="tbFirstNameB" MaxLength="100" CssClass="validate[custom[FirstName]]"></asp:TextBox></td></tr>
        <tr><td>Last Name:</td><td><asp:TextBox runat="server" ID="tbLastNameB" MaxLength="100" CssClass="validate[custom[LastName]]"></asp:TextBox></td></tr>
        <tr><td>Address 1:</td><td><asp:TextBox runat="server" ID="tbAddress1B" MaxLength="50" CssClass="validate[custom[Address]]"></asp:TextBox></td></tr>
        <tr><td>Address 2:</td><td><asp:TextBox runat="server" ID="tbAddress2B" MaxLength="50"></asp:TextBox></td></tr>
        <tr><td>City:</td><td><asp:TextBox runat="server" ID="tbCityB" MaxLength="50" CssClass="validate[custom[City]]"></asp:TextBox></td></tr>
        <tr><td>Country:</td><td><gen:DDLCountry runat="server" ID="ddlCountryB" style="width:150px;"></gen:DDLCountry></td></tr>
        <tr id="stateUSB"><td>State:</td><td><gen:DDLStateFullName runat="server" ID="ddlStateB" style="width:150px;"></gen:DDLStateFullName></td></tr>
        <tr id="stateExB"><td>State/Province:</td><td><asp:TextBox runat="server" ID="tbStateExB" MaxLength="50"></asp:TextBox></td></tr>
        <tr id="zipUSB"><td>Zip:</td><td><asp:TextBox runat="server" ID="tbZipB" MaxLength="5" CssClass="validate[custom[Zip]] x-short"></asp:TextBox></td></tr>
        <tr id="zipExB"><td>Postal Code:</td><td><asp:TextBox runat="server" ID="tbZipExB" MaxLength="50"></asp:TextBox></td></tr>
        <tr id="phoneUSB"><td>Phone:</td><td>( <asp:TextBox runat="server" ID="tbPhone1B" style="width: 38px;" MaxLength="3"></asp:TextBox> ) <asp:TextBox runat="server" ID="tbPhone2B" style="width: 38px;" MaxLength="3"></asp:TextBox> - <asp:TextBox runat="server" ID="tbPhone3B" style="width: 46px;" MaxLength="4"></asp:TextBox></td></tr>
        <tr id="phoneExB"><td>Phone:</td><td><asp:TextBox runat="server" ID="tbPhoneExB" MaxLength="50"></asp:TextBox></td></tr>
        <tr><td>Email:</td><td><asp:TextBox runat="server" ID="tbEmailB" MaxLength="50" CssClass="validate[custom[Email]]"></asp:TextBox></td></tr>
    </table>
</div>
<div class="module">
    <h2>Credit Card</h2>
    <table border="0" cellspacing="0" cellpadding="3" class="editForm">
        <tr><td>Credit Card Number:</td><td><asp:TextBox runat="server" ID="tbCreditCardNumber" MaxLength="16" CssClass="validate[custom[CreditCard]]"></asp:TextBox></td></tr>
        <tr><td>Security Code:</td><td><asp:TextBox runat="server" ID="tbCreditCardCVV" style="width:50px;" MaxLength="5" CssClass=""></asp:TextBox></td></tr>
        <tr><td>Expiration (Month/Year):</td><td><asp:TextBox runat="server" ID="tbExpMonth" style="width:30px;" MaxLength="2" CssClass="validate[custom[Numeric]]"></asp:TextBox> / <asp:TextBox runat="server" ID="tbExpYear" style="width:30px;" MaxLength="2" CssClass="validate[custom[Numeric]]"></asp:TextBox></td></tr>
    </table>
</div>
<div class="module">
    <h2>Affiliate</h2>
    <table border="0" cellspacing="0" cellpadding="3" class="editForm">
        <tr><td>Affiliate:</td><td><cc1:AffiliateDDL ID="ddlAffiliate" style="width:150px;" runat="server"></cc1:AffiliateDDL></td></tr>
        <tr><td>SubAffiliate:</td><td><asp:TextBox runat="server" ID="tbSubAffiliate"></asp:TextBox></td></tr>
    </table>
</div>
<div class="module">
    <h2>Subscription:</h2>
    <uc1:NewSubscriptionControl ID="NewSubscriptionControl1" runat="server" />
</div>
<div id="buttons">
    <asp:Button runat="server" ID="btnPlaceOrder" Text="Place Order" onclick="btnPlaceOrder_Click" />
</div>
<div class="clear"></div>
<asp:PlaceHolder runat="server" ID="phError">
<div id="errorMsg" style="max-width:600px;"><asp:Literal runat="server" ID="tbError"></asp:Literal></div>
</asp:PlaceHolder>
</form>
<div class="space"></div>
</div>
</asp:Content>
