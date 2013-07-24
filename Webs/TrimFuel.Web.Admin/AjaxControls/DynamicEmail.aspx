<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DynamicEmail.aspx.cs" Inherits="TrimFuel.Web.Admin.AjaxControls.DynamicEmail_"
    EnableViewState="false" ValidateRequest="false" %>

<div class="module">
    <h2>
        <%# (DynamicEmail.DynamicEmailID == null) ? "Add " : "Edit " %>
        <%# Product.ProductName %>
        <%# DynamicEmailType.DisplayName %>
        <%# (GiftCertificateDynamicEmail_Store != null) ? "(" + GiftCertificateDynamicEmail_Store.Name + ")" : ""%>
        <%# (DynamicEmail.Days > 0) ? "(" + DynamicEmail.Days.ToString() + " hour(s))" : ""%>
        Email</h2>
    <form runat="server" id="form1">
    <asp:hiddenfield runat="server" id="hdnDynamicEmailID" value='<%# DynamicEmail.DynamicEmailID %>'></asp:hiddenfield>
    <table class="editForm">
        <tr>
            <td>
                From Name
            </td>
            <td>
                <asp:textbox id="tbFromName" runat="server" cssclass="xxwide validate[required]"
                    width="300" text='<%# DynamicEmail.FromName %>' tabindex="1"></asp:textbox>
            </td>
            <td>
                Active
            </td>
            <td>
                <asp:checkbox runat="server" id="chbActive" checked='<%# DynamicEmail.Active %>'
                    tabindex="4"></asp:checkbox>
            </td>
        </tr>
        <tr>
            <td>
                From Address
            </td>
            <td>
                <asp:textbox id="tbFromAddress" runat="server" cssclass="xxwide validate[custom[Email]]"
                    width="300" text='<%# DynamicEmail.FromAddress %>' tabindex="2"></asp:textbox>
            </td>
            </td>
            <td style="display: none;">
                Landing
            </td>
            <td style="display: none;">
                <asp:textbox id="tbLanding" runat="server" cssclass="xxwide" width="300" text='<%# DynamicEmail.Landing %>'
                    tabindex="5"></asp:textbox>
        </tr>
        <tr>
            <td>
                Subject
            </td>
            <td>
                <asp:textbox id="tbSubject" runat="server" cssclass="xxwide validate[required]" width="300"
                    text='<%# DynamicEmail.Subject %>' tabindex="3"></asp:textbox>
            </td>
            <td style="display: none;">
                Landing Link
            </td>
            <td style="display: none;">
                <asp:textbox id="tbLandingLink" runat="server" cssclass="xxwide" width="300" text='<%# DynamicEmail.LandingLink %>'
                    tabindex="6"></asp:textbox>
            </td>
        </tr>
        <asp:placeholder runat="server" id="phShowHours" visible='<%# AllowHours %>'>        
        <tr><td>Hours</td><td><asp:TextBox ID="tbHours" runat="server" CssClass="xxwide validate[custom[Numeric]]" Width="300" Text='<%# (DynamicEmail.Days == -1 ? "" : DynamicEmail.Days.ToString()) %>' TabIndex="3"></asp:TextBox></td>
            <td></td><td></td></tr>
        </asp:placeholder>
        <asp:placeholder runat="server" id="phShowReplacementVariables" visible='<%# DynamicEmailType.DynamicEmailTypeID == TrimFuel.Model.Enums.DynamicEmailTypeEnum.OrderConfirmation %>'>
        <tr><td colspan="4">
            <a href="javascript:void(0)" onclick="jQuery('#email-replacement-variables').slideToggle();" class="infoIcon">Replacement Variables</a>
            <div class="space"></div>
            <div id="email-replacement-variables" style="display:none;">
            <ul class="big-item-list2" style="max-width:250px;float:left;">
                <li><span><b>##FNAME##</b> - First name</span></li>
                <li><span><b>##LNAME##</b> - Last name</span></li>
                <li><span><b>##ADD1##</b> - Address 1 line</span></li>
                <li><span><b>##ADD2##</b> - Address 2 line</span></li>
                <li><span><b>##CITY##</b> - City</span></li>
                <li><span><b>##STATE##</b> - State</span></li>
                <li><span><b>##ZIP##</b> - Zip code</span></li>
                <li><span><b>##PHONE##</b> - Phone</span></li>
                <li><span><b>##EMAIL##</b> - Email address</span></li>
                <li><span><b>##SHIPPING_FNAME##</b> - Shipping First name</span></li>
                <li><span><b>##SHIPPING_LNAME##</b> - Shipping Last name</span></li>
                <li><span><b>##SHIPPING_ADD1##</b> - Shipping Address 1 line</span></li>
                <li><span><b>##SHIPPING_ADD2##</b> - Shipping Address 2 line</span></li>
                <li><span><b>##SHIPPING_CITY##</b> - Shipping City</span></li>
                <li><span><b>##SHIPPING_STATE##</b> - Shipping State</span></li>
                <li><span><b>##SHIPPING_ZIP##</b> - Shipping Zip code</span></li>
                <li><span><b>##SHIPPING_PHONE##</b> - Shipping Phone</span></li>
                <li><span><b>##SHIPPING_EMAIL##</b> - Shipping Email address</span></li>
            </ul>
            <ul class="big-item-list2" style="max-width:250px;float:left;margin-left:20px;">
                <li><span><b>##CARDTYPE##</b> - Credit Card type</span></li>
                <li><span><b>##LAST4##</b> - Last 4 digits of Credit Card</span></li>
                <li><span><b>##MID##</b> - MID descriptor</span></li>
                <li><span><b>##PRODUCT_AMOUNT##</b> - Product amount</span></li>
                <li><span><b>##SH_AMOUNT##</b> - Shipping amount</span></li>
                <li><span><b>##TOTAL_AMOUNT##</b> - Total amount</span></li>
                <li><span><b>##ORDER_DATE##</b> - Order date</span></li>
                <li><span><b>##PRODUCT_NAME##</b> - Product Group Name</span></li>
            </ul>
            <ul class="big-item-list2" style="max-width:250px;float:left;margin-left:20px;">
                <li><span><b>##DESCRIPTION##</b> - Description</span></li>
                <li><span><b>##BILLINGID##</b> - Billing ID</span></li>
                <li><span><b>##PASSWORD##</b> - Password</span></li>
                <li><span><b>##INTERNAL_ID##</b> - Internal or Customer ID</span></li>
                <li><span><b>##REFERER_CODE##</b> - Code of sale referer</span></li>
                <li><span><b>##OWN_REFERER_CODE##</b> - Customer referer code</span></li>
                <li><span><b>##CANCELCODE##</b> - Subscription cancel code</span></li>
                <li><span><b>##REACTIVATION_LINK##</b> - Subscription reactivation link</span></li>
            </ul>
            </div>
        </td></tr>
        </asp:placeholder>
        <asp:placeholder runat="server" id="phShowReplacementVariablesAbandons" visible='<%# DynamicEmailType.DynamicEmailTypeID == TrimFuel.Model.Enums.DynamicEmailTypeEnum.Abandons %>'>
        <tr><td colspan="4">
            <a href="javascript:void(0)" onclick="jQuery('#email-replacement-variables').slideToggle();" class="infoIcon">Replacement Variables</a>
            <div class="space"></div>
            <div id="email-replacement-variables" style="display:none;">
            <ul class="big-item-list2" style="max-width:250px;float:left;">
                <li><span><b>##FNAME##</b> - First name</span></li>
                <li><span><b>##LNAME##</b> - Last name</span></li>
                <li><span><b>##ADD1##</b> - Address 1 line</span></li>
                <li><span><b>##ADD2##</b> - Address 2 line</span></li>
                <li><span><b>##CITY##</b> - City</span></li>
                <li><span><b>##STATE##</b> - State</span></li>
                <li><span><b>##ZIP##</b> - Zip code</span></li>
                <li><span><b>##PHONE##</b> - Phone</span></li>
                <li><span><b>##EMAIL##</b> - Email address</span></li>
                <li><span><b>##PASSWORD##</b> - Password</span></li>
            </ul>
            </div>
        </td></tr>
        </asp:placeholder>
        <asp:placeholder runat="server" id="phShowReplacementVariablesRMA" visible='<%# DynamicEmailType.DynamicEmailTypeID == TrimFuel.Model.Enums.DynamicEmailTypeEnum.RMA %>'>
        <tr>
        <td colspan="4">
            <a href="javascript:void(0)" onclick="jQuery('#email-replacement-variables').slideToggle();" class="infoIcon">Replacement Variables</a>
            <div class="space"></div>
            <div id="email-replacement-variables" style="display:none;">
            <ul class="big-item-list2" style="max-width:250px;float:left;">
                <li><span><b>##FNAME##</b> - First name</span></li>
                <li><span><b>##LNAME##</b> - Last name</span></li>
                <li><span><b>##ADD1##</b> - Address 1 line</span></li>
                <li><span><b>##ADD2##</b> - Address 2 line</span></li>
                <li><span><b>##CITY##</b> - City</span></li>
                <li><span><b>##STATE##</b> - State</span></li>
                <li><span><b>##ZIP##</b> - Zip code</span></li>
                <li><span><b>##PHONE##</b> - Phone</span></li>
                <li><span><b>##EMAIL##</b> - Email address</span></li>
                <li><span><b>##PASSWORD##</b> - Password</span></li>
                <li><span><b>##RMA##</b> - RMA code</span></li>
            </ul>
            </div>
        </td>
        </tr>
        </asp:placeholder>
        <tr>
            <td colspan="4">
                <asp:textbox runat="server" id="tbBody" textmode="MultiLine" cssclass="xxwide" width="530"
                    height="360" text='<%# DynamicEmail.Content %>' tabindex="7"></asp:textbox>
            </td>
        </tr>
        <tr>
            <td colspan="4" align="right">
                <div id="dynamicemail-btn-group-testemail" style="display: none;">
                    Please type target Email here:
                    <asp:textbox id="tbTestEmail" runat="server" cssclass="xwide validate[custom[Email]]"
                        text='dtcoder@gmail.com'></asp:textbox>
                    <asp:button id="bSendTestEmail" runat="server" onclientclick='<%# "CKEDITOR.instances[\"" + tbBody.ClientID + "\"].updateElement();" %>'
                        onclick="bSendTestEmail_Click" text="Send" />
                    <input type="button" onclick="$('#dynamicemail-btn-group-main').show();$('#dynamicemail-btn-group-testemail').hide();"
                        value="Cancel" />
                </div>
                <div id="dynamicemail-btn-group-main">
                    <input type="button" onclick="$('#dynamicemail-btn-group-main').hide();$('#dynamicemail-btn-group-testemail').show();"
                        value="Test Email" />
                    <asp:button id="bSave" runat="server" text="Save" onclick="bSave_Click" onclientclick='<%# "CKEDITOR.instances[\"" + tbBody.ClientID + "\"].updateElement();" %>' />
                </div>
            </td>
        </tr>
    </table>
    </form>
    <asp:placeholder runat="server" id="phMessage" visible="false">
    <div id="errorMsg">Email was successfully updated</div>
    </asp:placeholder>
    <asp:placeholder runat="server" id="phMessage2" visible="false">
    <div id="errorMsg">Test Email was sent</div>
    </asp:placeholder>
    <asp:placeholder runat="server" id="phMessage3" visible="false">
    <div id="errorMsg">Error occured while sending Email: <asp:Literal runat="server" ID="lSendEmailError" /></div>
    </asp:placeholder>
</div>
<script type="text/javascript">
    $(document).ready(function () {
        if (CKEDITOR) {
            if (CKEDITOR.instances['<%= tbBody.ClientID %>']) {
                delete CKEDITOR.instances['<%= tbBody.ClientID %>'];
            }
            CKEDITOR.replace('<%= tbBody.ClientID %>', { width: 870, height: 320, resize_enabled: false })
		.on("instanceReady", fixCkeStyles);
        }
    });

    function fixCkeStyles() {
        $("#cke_<%= tbBody.ClientID %>").find("table.cke_editor").css("border-width", "0").after("<div class='clear'></div>");
    }
</script>
