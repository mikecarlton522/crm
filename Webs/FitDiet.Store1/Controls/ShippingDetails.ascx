<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ShippingDetails.ascx.cs"
    Inherits="FitDiet.Store1.Controls.ShippingDetails" %>
<%@ Register TagPrefix="gen" Assembly="TrimFuel.Business" Namespace="TrimFuel.Business.Controls" %>
<%@ Register TagPrefix="uc" TagName="CartMenu" Src="~/Controls/CartMenu.ascx" %>
<asp:PlaceHolder runat="server" ID="phScript">

    <script type="text/javascript">
        function ValidatePhone() {
            return (/^\d{3}$/.test($("#<%# tbPhone1.ClientID %>").val()) &&
            /^\d{3}$/.test($("#<%# tbPhone2.ClientID %>").val()) &&
            /^\d{4}$/.test($("#<%# tbPhone3.ClientID %>").val()));
        }
        function onCountryChanged(country) {
            if (country == "US") {
                $("#customer #billing #stateEx").hide().find("[class*=validate]").addClass("donotvalidate");
                $("#customer #billing #zipEx").hide().find("[class*=validate]").addClass("donotvalidate");
                $("#customer #billing #phoneEx").hide().find("[class*=validate]").addClass("donotvalidate");
                $("#customer #billing #stateUS").show().find("[class*=validate]").removeClass("donotvalidate");
                $("#customer #billing #zipUS").show().find("[class*=validate]").removeClass("donotvalidate");
                $("#customer #billing #phoneUS").show().find("[class*=validate]").removeClass("donotvalidate");
            }
            else {
                $("#customer #billing #stateEx").show().find("[class*=validate]").removeClass("donotvalidate");
                $("#customer #billing #zipEx").show().find("[class*=validate]").removeClass("donotvalidate");
                $("#customer #billing #phoneEx").show().find("[class*=validate]").removeClass("donotvalidate");
                $("#customer #billing #stateUS").hide().find("[class*=validate]").addClass("donotvalidate");
                $("#customer #billing #zipUS").hide().find("[class*=validate]").addClass("donotvalidate");
                $("#customer #billing #phoneUS").hide().find("[class*=validate]").addClass("donotvalidate");
            }
        }
        $(document).ready(function() {
            $("#customer #billing #<%# ddlCountry.ClientID %>").change(function() {
                onCountryChanged($("#customer #billing #<%# ddlCountry.ClientID %>").val());
            });
            onCountryChanged($("#customer #billing #<%# ddlCountry.ClientID %>").val());
        });
    </script>

</asp:PlaceHolder>
<table width="900" border="0" align="center" cellpadding="0" cellspacing="0">
    <tr>
        <td>
            &nbsp;
        </td>
    </tr>
    <tr>
        <td valign="top">
            <table width="900" border="0" cellspacing="0" cellpadding="0">
                <tr>
                    <td width="489" valign="top" class="main_heading">
                        SHIPPING DETAILS
                    </td>
                </tr>
                <tr>
                    <td valign="top" style="padding-top: 15px;">
                        <table width="900" border="0" cellspacing="0" cellpadding="0">
                            <tr>
                                <td width="697" valign="top">
                                    <table width="697" border="0" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td valign="middle" class="numbering_bg">
                                                <uc:CartMenu ID="cartMenu" runat="server" OnStepChanged="ChangeStep_Click" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:PlaceHolder runat="server" ID="phError">
                                                    <div id="error" class="validation-error">
                                                        <asp:Literal runat="server" ID="lError" />
                                                    </div>
                                                </asp:PlaceHolder>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center" valign="top" style="padding-top: 12px;">
                                                <div id="customer">
                                                    <div id="billing">
                                                        <table width="100%" border="0" cellspacing="0" cellpadding="5">
                                                            <tr>
                                                                <td width="40%" class="copy12grey1">
                                                                    <b>First name:</b>
                                                                </td>
                                                                <td width="60%" class="last">
                                                                    <label>
                                                                        <asp:TextBox runat="server" ID="tbFirstName" TabIndex="6" MaxLength="50" CssClass="validate[custom[FirstName]]" />
                                                                    </label>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td class="copy12grey1">
                                                                    <b>Last name:</b>
                                                                </td>
                                                                <td class="last">
                                                                    <label>
                                                                        <asp:TextBox runat="server" ID="tbLastName" TabIndex="7" MaxLength="50" CssClass="validate[custom[LastName]]" />
                                                                    </label>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td class="copy12grey1">
                                                                    <b>Address line 1:</b>
                                                                </td>
                                                                <td class="last">
                                                                    <label>
                                                                        <asp:TextBox runat="server" ID="tbAddress1" TabIndex="8" MaxLength="50" CssClass="validate[custom[Address]]" />
                                                                    </label>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td class="copy12grey1">
                                                                    <b>Address line 2:</b>
                                                                </td>
                                                                <td class="last">
                                                                    <label>
                                                                        <asp:TextBox runat="server" ID="tbAddress2" TabIndex="9" MaxLength="50" />
                                                                    </label>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td class="copy12grey1">
                                                                    <b>City:</b>
                                                                </td>
                                                                <td class="last">
                                                                    <label>
                                                                        <asp:TextBox runat="server" ID="tbCity" TabIndex="10" MaxLength="50" CssClass="validate[custom[City]]" />
                                                                    </label>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td class="copy12grey1">
                                                                    <b>Country:</b>
                                                                </td>
                                                                <td class="last">
                                                                    <label>
                                                                        <gen:DDLCountry runat="server" ID="ddlCountry" TabIndex="11" Style="width: 170px;">
                                                                        </gen:DDLCountry>
                                                                    </label>
                                                                </td>
                                                            </tr>
                                                            <tr id="stateUS">
                                                                <td class="copy12grey1">
                                                                    <b>State:</b>
                                                                </td>
                                                                <td class="last">
                                                                    <label>
                                                                        <gen:DDLStateFullName runat="server" ID="ddlState" TabIndex="12" Style="width: 170px;">
                                                                        </gen:DDLStateFullName>
                                                                    </label>
                                                                </td>
                                                            </tr>
                                                            <tr id="stateEx">
                                                                <td class="copy12grey1">
                                                                    <b>State/Province:</b>
                                                                </td>
                                                                <td class="last">
                                                                    <label>
                                                                        <asp:TextBox runat="server" ID="tbStateEx" TabIndex="12" MaxLength="50" />
                                                                    </label>
                                                                </td>
                                                            </tr>
                                                            <tr id="zipUS">
                                                                <td class="copy12grey1">
                                                                    <b>Zip code:</b>
                                                                </td>
                                                                <td class="last">
                                                                    <asp:TextBox runat="server" ID="tbZip" TabIndex="13" MaxLength="5" CssClass="validate[custom[Zip]] x-short" />
                                                                </td>
                                                            </tr>
                                                            <tr id="zipEx">
                                                                <td class="copy12grey1">
                                                                    <b>Postal code:</b>
                                                                </td>
                                                                <td class="last">
                                                                    <asp:TextBox runat="server" ID="tbZipEx" TabIndex="13" MaxLength="50" />
                                                                </td>
                                                            </tr>
                                                            <tr id="phoneUS">
                                                                <td class="copy12grey1">
                                                                    <b>Phone number:</b>
                                                                </td>
                                                                <td class="last">
                                                                    (
                                                                    <asp:TextBox runat="server" ID="tbPhone1" TabIndex="14" Style="width: 30px;" MaxLength="3" />
                                                                    )
                                                                    <asp:TextBox runat="server" ID="tbPhone2" TabIndex="15" Style="width: 30px;" MaxLength="3" />
                                                                    -
                                                                    <asp:TextBox runat="server" ID="tbPhone3" TabIndex="16" Style="width: 40px;" MaxLength="4"
                                                                        CssClass="validate[funcCall[ValidatePhone]]" />
                                                                </td>
                                                            </tr>
                                                            <tr id="phoneEx">
                                                                <td class="copy12grey1">
                                                                    <b>Phone number:</b>
                                                                </td>
                                                                <td class="last">
                                                                    <asp:TextBox runat="server" ID="tbPhoneEx" TabIndex="14" MaxLength="50" />
                                                                </td>
                                                            </tr>
                                                            <tr class="item bottom">
                                                                <td class="copy12grey1">
                                                                    <b>Email eddress:</b>
                                                                </td>
                                                                <td class="last">
                                                                    <label>
                                                                        <asp:TextBox runat="server" ID="tbEmail" TabIndex="17" MaxLength="50" CssClass="validate[custom[Email]]" />
                                                                    </label>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </div>
                                                    <div style="clear: both;">
                                                    </div>
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="green_tr">
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
