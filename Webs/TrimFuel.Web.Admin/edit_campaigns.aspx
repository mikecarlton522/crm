<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/Admin.Master" AutoEventWireup="true"
    CodeBehind="edit_campaigns.aspx.cs" Inherits="TrimFuel.Web.Admin.edit_campaigns"
    ValidateRequest="false" AspCompat="true" %>

<%@ Register Assembly="TrimFuel.Web.UI" Namespace="TrimFuel.Web.UI.Specialized" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphScript" runat="server">
    <script type="text/javascript" src="/ckeditor/ckeditor.js"></script>
    <script type="text/javascript" src="/ckeditor/config.js?t=ABLC4TW"></script>
    <link rel="stylesheet" type="text/css" href="/ckeditor/skins/kama/editor.css?t=ABLC4TW">
    <script type="text/javascript" src="/ckeditor/lang/pl.js?t=ABLC4TW"></script>
    <script type="text/javascript" src="/ckeditor/plugins/styles/styles/default.js?t=ABLC4TW"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $("#tabs").tabs();

            var parentCampaignField = $('#parent-campaign-row');
            var upsell1NoSubscription = $('.upsell1-no-subscription');
            var upsell1Subscription = $('.upsell1-subscription');
            var upsell2NoSubscription = $('.upsell2-no-subscription');
            var upsell2Subscription = $('.upsell2-subscription');
            var upsell3NoSubscription = $('.upsell3-no-subscription');
            var upsell3Subscription = $('.upsell3-subscription');

            if ($('input[id$="cbSave"]:checked').val() == undefined) {
                parentCampaignField.hide();
            }

            if ($('input[id$="cbUpsell1PlanChange"]:checked').val() !== undefined) {
                upsell1NoSubscription.hide();
                upsell1Subscription.show();
            }

            if ($('input[id$="cbUpsell2PlanChange"]:checked').val() !== undefined) {
                upsell2NoSubscription.hide();
                upsell2Subscription.show();
            }

            if ($('input[id$="cbUpsell3PlanChange"]:checked').val() !== undefined) {
                upsell3NoSubscription.hide();
                upsell3Subscription.show();
            }

            $("input[id$='cbSave']").click(function () {
                if ($('input[id$="cbSave"]:checked').val() == undefined) {
                    parentCampaignField.hide();
                }
                else {
                    parentCampaignField.show();
                }
            });

            $("input[id$='cbUpsell1PlanChange']").click(function () {
                if ($('input[id$="cbUpsell1PlanChange"]:checked').val() == undefined) {
                    upsell1NoSubscription.show();
                    upsell1Subscription.hide();
                }
                else {
                    upsell1NoSubscription.hide();
                    upsell1Subscription.show();
                }
            });

            $("input[id$='cbUpsell2PlanChange']").click(function () {
                if ($('input[id$="cbUpsell2PlanChange"]:checked').val() == undefined) {
                    upsell2NoSubscription.show();
                    upsell2Subscription.hide();
                }
                else {
                    upsell2NoSubscription.hide();
                    upsell2Subscription.show();
                }
            });

            $("input[id$='cbUpsell3PlanChange']").click(function () {
                if ($('input[id$="cbUpsell3PlanChange"]:checked').val() == undefined) {
                    upsell3NoSubscription.show();
                    upsell3Subscription.hide();
                }
                else {
                    upsell3NoSubscription.hide();
                    upsell3Subscription.show();
                }
            });

            <% if(string.IsNullOrEmpty(Request["CampaignID"]))
               { %>

                inlineControl("../editForms/campaignsubscription.asp?cid=1", "bs-container");
            <%} else { %>
                inlineControl("../editForms/campaignsubscription.asp?cid=" + <%= Request["CampaignID"] %>, "bs-container");
            <%} %>
        });

       
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphStyle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <form runat="server">
    <table cellpadding="3">
        <tr>
            <td colspan="2">
                <asp:ValidationSummary ID="validationSummary" runat="server" />
                <asp:Label ID="statusMessage" runat="server" ForeColor="Green" />
            </td>
        </tr>
        <tr>
            <td style="width: 270px;">
                <span class="caption">Campaign Name:</span>
            </td>
            <td>
                <asp:TextBox ID="txtCampaignName" MaxLength="100" runat="server" Width="200"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvCampaignName" runat="server" ControlToValidate="txtCampaignName"
                    Display="None" ErrorMessage="Campaign name is required"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td style="width: 270px;">
                <span class="caption">Campaign URL:</span>
            </td>
            <td>
                <asp:TextBox ID="txtCampaignURL" runat="server" MaxLength="100" Width="200"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvCampaignURL" runat="server" ControlToValidate="txtCampaignURL"
                    Display="None" ErrorMessage="URL name is required"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
<%--            <td style="width: 270px;">
                &nbsp;
            </td>--%>
            <td colspan="2">
                <div id="bs-container" style="height: 220px; overflow: hidden;">
                </div>
            </td>
        </tr>
        <tr>
            <td style="width: 270px;">
                <%--<span class="caption">Redirect to STO.com Secure Billing:</span>--%>
                <span class="caption">Redirect to the Secure billing:</span>
            </td>
            <td>
                <asp:DropDownList ID="ddlRedirectURL" runat="server">
                    <asp:ListItem Text="Stay on same domain" Value="" />
                    <asp:ListItem Text="Securetrialoffers.com" Value="http://Securetrialoffers.com" />
                    <asp:ListItem Text="secure.tripayments.com" Value="https://secure.tripayments.com" />
                    <asp:ListItem Text="secure.safepaymentsonline.com" Value="https://secure.safepaymentsonline.com" />
                </asp:DropDownList>
                <%--<asp:CheckBox ID="cbRedirect" runat="server" Checked="true" />--%>
            </td>
        </tr>
        <tr>
            <td style="width: 270px;">
                <span class="caption">Merchant Application:</span>
            </td>
            <td>
                <asp:CheckBox ID="cbMerchant" runat="server" Checked="true" />
            </td>
        </tr>
        <tr>
            <td style="width: 270px;">
                <span class="caption">Save Campaign</span>
            </td>
            <td>
                <asp:CheckBox ID="cbSave" runat="server" Checked="true" />
            </td>
        </tr>
        <tr>
            <td style="width: 270px;">
                <span class="caption">Externally Hosted</span>
            </td>
            <td>
                <asp:CheckBox ID="cbExternal" runat="server" Checked="true" />
            </td>
        </tr>
        <tr id="parent-campaign-row">
            <td style="width: 270px;">
                <span class="caption">Parent Campaign:</span>
            </td>
            <td>
                <asp:DropDownList ID="ddParentCampaign" runat="server" DataTextField="DisplayName"
                    DataValueField="CampaignID" AppendDataBoundItems="true">
                    <asp:ListItem Text="--- Select ---" Value=""></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td valign="top" style="width: 270px;">
                <span class="caption">HTML:</span>
            </td>
            <td>
                <div id="tabs">
                    <ul>
                        <li><a href="#tabs-1">PreLander</a></li>
                        <li><a href="#tabs-2">Lander</a></li>
                        <li><a href="#tabs-3">Billing</a></li>
                        <li><a href="#tabs-4">Upsell 1</a></li>
                        <li><a href="#tabs-5">Upsell 2</a></li>
                        <li><a href="#tabs-6">Upsell 3</a></li>
                        <li><a href="#tabs-7">Confirmation</a></li>
                    </ul>
                    <div id="tabs-1">
                        <table>
                            <tr>
                                <td>
                                    <span class="caption">Title:</span>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtPreLanderTitle" MaxLength="100" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td valign="top">
                                    <span class="caption">Header HTML:</span>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtPreLanderHeaderHTML" TextMode="MultiLine" runat="server" Style="width: 1200px;
                                        height: 200px;"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td valign="top">
                                    <span class="caption">Page HTML:</span>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtPreLanderPageHTML" TextMode="MultiLine" runat="server" Style="width: 1200px;
                                        height: 600px;"></asp:TextBox>
                                    <script type="text/javascript">
                                        CKEDITOR.replace('ctl00_cphContent_txtPreLanderPageHTML');
                                    </script>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="tabs-2">
                        <table>
                            <tr>
                                <td>
                                    <span class="caption">Title:</span>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtLandingTitle" MaxLength="100" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td valign="top">
                                    <span class="caption">Header HTML:</span>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtLandingHeaderHTML" TextMode="MultiLine" runat="server" Style="width: 1200px;
                                        height: 200px;"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td valign="top">
                                    <span class="caption">Page HTML:</span>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtLandingPageHTML" TextMode="MultiLine" runat="server" Style="width: 1200px;
                                        height: 600px;"></asp:TextBox>
                                    <script type="text/javascript">
                                        CKEDITOR.replace('ctl00_cphContent_txtLandingPageHTML');
                                    </script>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="tabs-3">
                        <table>
                            <tr>
                                <td>
                                    <span class="caption">Title:</span>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtBillingTitle" MaxLength="100" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td valign="top">
                                    <span class="caption">Header HTML:</span>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtBillingHeaderHTML" TextMode="MultiLine" runat="server" Style="width: 1200px;
                                        height: 200px;"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td valign="top">
                                    <span class="caption">Page HTML:</span>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtBillingPageHTML" TextMode="MultiLine" runat="server" Style="width: 1200px;
                                        height: 600px;"></asp:TextBox>
                                    <script type="text/javascript">
                                        CKEDITOR.replace('ctl00_cphContent_txtBillingPageHTML');
                                    </script>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="tabs-4">
                        <table>
                            <tr>
                                <td>
                                    <span class="caption">Title:</span>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtUpsell1Title" MaxLength="100" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr class="upsell1-no-subscription">
                                <td>
                                    <span class="caption">Product Code:</span>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtUpsell1Code" MaxLength="20" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <span class="caption">Upsell is plan change:</span>
                                </td>
                                <td>
                                    <asp:CheckBox ID="cbUpsell1PlanChange" runat="server"></asp:CheckBox>
                                </td>
                            </tr>
                            <tr class="upsell1-subscription" style="display: none;">
                                <td>
                                    <span class="caption">Subscription:</span>
                                </td>
                                <td>
                                    <cc1:SubscriptionDDL ID="ddlSubscriptionUpsell1" runat="server" />
                                </td>
                            </tr>
                            <tr class="upsell1-no-subscription">
                                <td>
                                    <span class="caption">Price:</span>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtUpsell1Price" MaxLength="20" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr class="upsell1-no-subscription">
                                <td>
                                    <span class="caption">Quantity:</span>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtUpsell1Quantity" MaxLength="20" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td valign="top">
                                    <span class="caption">Header HTML:</span>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtUpsell1HeaderHTML" TextMode="MultiLine" runat="server" Style="width: 1200px;
                                        height: 200px;"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td valign="top">
                                    <span class="caption">Page HTML:</span>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtUpsell1PageHTML" TextMode="MultiLine" runat="server" Style="width: 1200px;
                                        height: 600px;"></asp:TextBox>
                                    <script type="text/javascript">
                                        CKEDITOR.replace('ctl00_cphContent_txtUpsell1PageHTML');
                                    </script>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="tabs-5">
                        <table>
                            <tr>
                                <td>
                                    <span class="caption">Title:</span>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtUpsell2Title" MaxLength="100" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr class="upsell2-no-subscription">
                                <td>
                                    <span class="caption">Product Code:</span>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtUpsell2Code" MaxLength="20" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <span class="caption">Upsell is plan change:</span>
                                </td>
                                <td>
                                    <asp:CheckBox ID="cbUpsell2PlanChange" runat="server"></asp:CheckBox>
                                </td>
                            </tr>
                            <tr class="upsell2-subscription" style="display: none;">
                                <td>
                                    <span class="caption">Subscription:</span>
                                </td>
                                <td>
                                    <cc1:SubscriptionDDL ID="ddlSubscriptionUpsell2" runat="server" />
                                </td>
                            </tr>
                            <tr class="upsell2-no-subscription">
                                <td>
                                    <span class="caption">Price:</span>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtUpsell2Price" MaxLength="20" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr class="upsell2-no-subscription">
                                <td>
                                    <span class="caption">Quantity:</span>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtUpsell2Quantity" MaxLength="20" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td valign="top">
                                    <span class="caption">Header HTML:</span>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtUpsell2HeaderHTML" TextMode="MultiLine" runat="server" Style="width: 1200px;
                                        height: 200px;"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td valign="top">
                                    <span class="caption">Page HTML:</span>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtUpsell2PageHTML" TextMode="MultiLine" runat="server" Style="width: 1200px;
                                        height: 600px;"></asp:TextBox>
                                    <script type="text/javascript">
                                        CKEDITOR.replace('ctl00_cphContent_txtUpsell2PageHTML');
                                    </script>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="tabs-6">
                        <table>
                            <tr>
                                <td>
                                    <span class="caption">Title:</span>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtUpsell3Title" MaxLength="100" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr class="upsell3-no-subscription">
                                <td>
                                    <span class="caption">Product Code:</span>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtUpsell3Code" MaxLength="20" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <span class="caption">Upsell is plan change:</span>
                                </td>
                                <td>
                                    <asp:CheckBox ID="cbUpsell3PlanChange" runat="server"></asp:CheckBox>
                                </td>
                            </tr>
                            <tr class="upsell3-subscription" style="display: none;">
                                <td>
                                    <span class="caption">Subscription:</span>
                                </td>
                                <td>
                                    <cc1:SubscriptionDDL ID="ddlSubscriptionUpsell3" runat="server" />
                                </td>
                            </tr>
                            <tr class="upsell3-no-subscription">
                                <td>
                                    <span class="caption">Price:</span>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtUpsell3Price" MaxLength="20" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr class="upsell3-no-subscription">
                                <td>
                                    <span class="caption">Quantity:</span>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtUpsell3Quantity" MaxLength="20" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td valign="top">
                                    <span class="caption">Header HTML:</span>
                                </td>
                                <td valign="top">
                                    <asp:TextBox ID="txtUpsell3HeaderHTML" TextMode="MultiLine" runat="server" Style="width: 1200px;
                                        height: 200px;"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td valign="top">
                                    <span class="caption">Page HTML:</span>
                                </td>
                                <td valign="top">
                                    <asp:TextBox ID="txtUpsell3PageHTML" TextMode="MultiLine" runat="server" Style="width: 1200px;
                                        height: 600px;"></asp:TextBox>
                                    <script type="text/javascript">
                                        CKEDITOR.replace('ctl00_cphContent_txtUpsell3PageHTML');
                                    </script>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="tabs-7">
                        <table>
                            <tr>
                                <td>
                                    <span class="caption">Title:</span>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtConfirmationTitle" MaxLength="100" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td valign="top">
                                    <span class="caption">Header HTML:</span>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtConfirmationHeaderHTML" TextMode="MultiLine" runat="server" Style="width: 1200px;
                                        height: 200px;"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td valign="top">
                                    <span class="caption">Page HTML:</span>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtConfirmationPageHTML" TextMode="MultiLine" runat="server" Style="width: 1200px;
                                        height: 600px;"></asp:TextBox>
                                    <script type="text/javascript">
                                        CKEDITOR.replace('ctl00_cphContent_txtConfirmationPageHTML');
                                    </script>
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
            <td>
                <asp:Button ID="btSave" runat="server" Text="Save" OnClick="Save" />&nbsp;&nbsp;&nbsp;
                <button onclick="window.location='management_campaigns.aspx';return false;">
                    Back to Campaigns</button>
            </td>
        </tr>
    </table>
    </form>
</asp:Content>
