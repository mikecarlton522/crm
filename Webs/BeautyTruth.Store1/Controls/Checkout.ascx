<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Checkout.ascx.cs" Inherits="BeautyTruth.Store1.Controls.Checkout" %>
<%@ Register TagPrefix="gen" Assembly="TrimFuel.Business" Namespace="TrimFuel.Business.Controls" %>
<%@ Import Namespace="TrimFuel.Business.Controls" %>
<asp:PlaceHolder runat="server" ID="phScript">

    <script type="text/javascript">

        function ValidateCreditCard(source, arguments) {
            arguments.IsValid = Mod10($("#<%# tbCreditCardNumber.ClientID %>").val());
        }
        function ValidatePhone(source, arguments) {
            arguments.IsValid = (/^\d{3}$/.test($("#<%# tbPhone1.ClientID %>").val()) &&
            /^\d{3}$/.test($("#<%# tbPhone2.ClientID %>").val()) &&
            /^\d{4}$/.test($("#<%# tbPhone3.ClientID %>").val()));
        }
        function ValidateShippingPhone(source, arguments) {
            arguments.IsValid = (/^\d{3}$/.test($("#<%# tbShippingPhone1.ClientID %>").val()) &&
            /^\d{3}$/.test($("#<%# tbShippingPhone2.ClientID %>").val()) &&
            /^\d{4}$/.test($("#<%# tbShippingPhone3.ClientID %>").val()));
        }
        function SetValidator(validator, enable) {
            validator.enabled = enable;
            ValidatorUpdateDisplay(validator);
        }
        function onCountryChanged(country) {
            if (country == "US") {
                $("tr #stateEx").hide();
                $("tr #zipEx").hide();
                $("tr #phoneEx").hide();
                $("tr #stateUS").show();
                $("tr #zipUS").show();
                $("tr #phoneUS").show();

                SetValidator($("#<%# RequiredFieldValidatorZipEx.ClientID %>")[0], false);
                SetValidator($("#<%# RequiredFieldValidatorStateEx.ClientID %>")[0], false);
                SetValidator($("#<%# RegularExpressionValidatorZipEx.ClientID %>")[0], false);
                SetValidator($("#<%# RegularExpressionValidatorPhoneEx.ClientID %>")[0], false);
                SetValidator($("#<%# RequiredFieldValidatorPhoneEx.ClientID %>")[0], false);
                SetValidator($("#<%# RegularExpressionValidatorStateEx.ClientID %>")[0], false);

                SetValidator($("#<%# CustomFieldValidatorPhone.ClientID %>")[0], true);
                SetValidator($("#<%# RequiredFieldValidatorPhone.ClientID %>")[0], true);
                SetValidator($("#<%# RequiredFieldValidatorZip.ClientID %>")[0], true);
                SetValidator($("#<%# RegularExpressionValidatorZip.ClientID %>")[0], true);
            }
            else {
                $("tr #stateEx").show();
                $("tr #zipEx").show();
                $("tr #phoneEx").show();
                $("tr #stateUS").hide();
                $("tr #zipUS").hide();
                $("tr #phoneUS").hide();

                SetValidator($("#<%# RequiredFieldValidatorStateEx.ClientID %>")[0], true);
                SetValidator($("#<%# RequiredFieldValidatorZipEx.ClientID %>")[0], true);
                SetValidator($("#<%# RequiredFieldValidatorPhoneEx.ClientID %>")[0], true);
                SetValidator($("#<%# RegularExpressionValidatorZipEx.ClientID %>")[0], true);
                SetValidator($("#<%# RegularExpressionValidatorPhoneEx.ClientID %>")[0], true);
                SetValidator($("#<%# RegularExpressionValidatorStateEx.ClientID %>")[0], true);

                SetValidator($("#<%# RequiredFieldValidatorZip.ClientID %>")[0], false);
                SetValidator($("#<%# CustomFieldValidatorPhone.ClientID %>")[0], false);
                SetValidator($("#<%# RequiredFieldValidatorPhone.ClientID %>")[0], false);
                SetValidator($("#<%# RegularExpressionValidatorZip.ClientID %>")[0], false);
            }
        }
        function onShippingCheckboxChange() {
            var ch = document.getElementById("<%# cbUseShippingAsBilling.ClientID %>");
            if (ch.checked == false) {
                $("#shipping").show();

                SetValidator($("#<%# RegularExpressionValidatorShippingZip.ClientID %>")[0], true);
                SetValidator($("#<%# RegularExpressionValidatorShippingZip.ClientID %>")[0], true);

                SetValidator($("#<%# RegularExpressionValidatorShippingFirstName.ClientID %>")[0], true);
                SetValidator($("#<%# RequiredFieldValidatorShippingFirstName.ClientID %>")[0], true);

                SetValidator($("#<%# RegularExpressionValidatorShippingLastName.ClientID %>")[0], true);
                SetValidator($("#<%# RequiredFieldValidatorShippingLastName.ClientID %>")[0], true);

                SetValidator($("#<%# RequiredFieldValidatorShippingCity.ClientID %>")[0], true);
                SetValidator($("#<%# RegularExpressionValidatorShippingCity.ClientID %>")[0], true);

                SetValidator($("#<%# RequiredFieldValidatorShippingAddress1.ClientID %>")[0], true);
                SetValidator($("#<%# RegularExpressionValidatorShippingAddress1.ClientID %>")[0], true);

                SetValidator($("#<%# RegularExpressionValidatorShippingEmail.ClientID %>")[0], true);
                SetValidator($("#<%# RequiredFieldValidatorShippingEmail.ClientID %>")[0], true);

                SetValidator($("#<%# CustomFieldValidatorShippingPhone.ClientID %>")[0], true);
                SetValidator($("#<%# RequiredFieldValidatorShippingPhone.ClientID %>")[0], true);

                SetValidator($("#<%# RegularExpressionValidatorShippingPhoneEx.ClientID %>")[0], true);
                SetValidator($("#<%# RequiredFieldValidatorShippingPhoneEx.ClientID %>")[0], true);
                SetValidator($("#<%# RegularExpressionValidatorShippingStateEx.ClientID %>")[0], true);
                SetValidator($("#<%# RequiredFieldValidatorShippingStateEx.ClientID %>")[0], true);
                SetValidator($("#<%# RegularExpressionValidatorShippingZipEx.ClientID %>")[0], true);
                SetValidator($("#<%# RequiredFieldValidatorShippingZipEx.ClientID %>")[0], true);

                onShippingCountryChanged($("#validation #<%# ddlShippingCountry.ClientID %>").val());
            }
            else {
                $("#shipping").hide();

                SetValidator($("#<%# RegularExpressionValidatorShippingZip.ClientID %>")[0], false);
                SetValidator($("#<%# RequiredFieldValidatorShippingZip.ClientID %>")[0], false);

                SetValidator($("#<%# RegularExpressionValidatorShippingFirstName.ClientID %>")[0], false);
                SetValidator($("#<%# RequiredFieldValidatorShippingFirstName.ClientID %>")[0], false);

                SetValidator($("#<%# RegularExpressionValidatorShippingLastName.ClientID %>")[0], false);
                SetValidator($("#<%# RequiredFieldValidatorShippingLastName.ClientID %>")[0], false);

                SetValidator($("#<%# RequiredFieldValidatorShippingCity.ClientID %>")[0], false);
                SetValidator($("#<%# RegularExpressionValidatorShippingCity.ClientID %>")[0], false);

                SetValidator($("#<%# RegularExpressionValidatorShippingAddress1.ClientID %>")[0], false);
                SetValidator($("#<%# RequiredFieldValidatorShippingAddress1.ClientID %>")[0], false);

                SetValidator($("#<%# RegularExpressionValidatorShippingEmail.ClientID %>")[0], false);
                SetValidator($("#<%# RequiredFieldValidatorShippingEmail.ClientID %>")[0], false);

                SetValidator($("#<%# CustomFieldValidatorShippingPhone.ClientID %>")[0], false);
                SetValidator($("#<%# RequiredFieldValidatorShippingPhone.ClientID %>")[0], false);

                SetValidator($("#<%# RegularExpressionValidatorShippingPhoneEx.ClientID %>")[0], false);
                SetValidator($("#<%# RequiredFieldValidatorShippingPhoneEx.ClientID %>")[0], false);
                SetValidator($("#<%# RegularExpressionValidatorShippingStateEx.ClientID %>")[0], false);
                SetValidator($("#<%# RegularExpressionValidatorShippingZipEx.ClientID %>")[0], false);
                SetValidator($("#<%# RequiredFieldValidatorShippingZipEx.ClientID %>")[0], false);
                SetValidator($("#<%# RequiredFieldValidatorShippingStateEx.ClientID %>")[0], false);
            }
        }
        function onShippingCountryChanged(country) {
            if (country == "US") {
                $("tr #shippingStateEx").hide();
                $("tr #shippingZipEx").hide();
                $("tr #shippingPhoneEx").hide();
                $("tr #shippingState").show();
                $("tr #shippingZip").show();
                $("tr #shippingPhone").show();

                SetValidator($("#<%# RequiredFieldValidatorShippingZipEx.ClientID %>")[0], false);
                SetValidator($("#<%# RequiredFieldValidatorShippingStateEx.ClientID %>")[0], false);
                SetValidator($("#<%# RegularExpressionValidatorShippingZipEx.ClientID %>")[0], false);
                SetValidator($("#<%# RegularExpressionValidatorShippingStateEx.ClientID %>")[0], false);
                SetValidator($("#<%# RegularExpressionValidatorShippingPhoneEx.ClientID %>")[0], false);
                SetValidator($("#<%# RequiredFieldValidatorShippingPhoneEx.ClientID %>")[0], false);

                SetValidator($("#<%# CustomFieldValidatorShippingPhone.ClientID %>")[0], true);
                SetValidator($("#<%# RequiredFieldValidatorShippingPhone.ClientID %>")[0], true);
                SetValidator($("#<%# RequiredFieldValidatorShippingZip.ClientID %>")[0], true);
                SetValidator($("#<%# RegularExpressionValidatorShippingZipEx.ClientID %>")[0], true);
            }
            else {
                $("tr #shippingStateEx").show();
                $("tr #shippingZipEx").show();
                $("tr #shippingPhoneEx").show();
                $("tr #shippingState").hide();
                $("tr #shippingZip").hide();
                $("tr #shippingPhone").hide();

                SetValidator($("#<%# RequiredFieldValidatorShippingStateEx.ClientID %>")[0], true);
                SetValidator($("#<%# RequiredFieldValidatorShippingZipEx.ClientID %>")[0], true);
                SetValidator($("#<%# RegularExpressionValidatorShippingZipEx.ClientID %>")[0], true);
                SetValidator($("#<%# RegularExpressionValidatorShippingStateEx.ClientID %>")[0], true);
                SetValidator($("#<%# RegularExpressionValidatorShippingPhoneEx.ClientID %>")[0], true);
                SetValidator($("#<%# RequiredFieldValidatorShippingPhoneEx.ClientID %>")[0], true);

                SetValidator($("#<%# CustomFieldValidatorShippingPhone.ClientID %>")[0], false);
                SetValidator($("#<%# RequiredFieldValidatorShippingPhone.ClientID %>")[0], false);
                SetValidator($("#<%# RegularExpressionValidatorShippingZip.ClientID %>")[0], false);
                SetValidator($("#<%# RequiredFieldValidatorShippingZip.ClientID %>")[0], false);
            }
        }

        function onLoginSuccess(billing, registration, registrationInfo) {
            if (billing != null) {
                $("#billing #<%# tbFirstName.ClientID %>").val(billing.FirstName);
                $("#billing #<%# tbLastName.ClientID %>").val(billing.LastName);
                $("#billing #<%# tbAddress1.ClientID %>").val(billing.Address1);
                $("#billing #<%# tbAddress2.ClientID %>").val(billing.Address2);
                $("#billing #<%# tbCity.ClientID %>").val(billing.City);
                if (billing.Country == null || billing.Country == "" || billing.Country == "US") {
                    $("#billing #<%# ddlState.ClientID %>").val(billing.State);
                    $("#billing #<%# tbZip.ClientID %>").val(billing.Zip);
                    if (billing.PhoneCnt != null) {
                        $("#billing #<%# tbPhone1.ClientID %>").val(billing.PhoneCnt.Code);
                        $("#billing #<%# tbPhone2.ClientID %>").val(billing.PhoneCnt.Part1);
                        $("#billing #<%# tbPhone3.ClientID %>").val(billing.PhoneCnt.Part2);
                    }
                    $("#billing #<%# ddlCountry.ClientID %>").val("US");
                    onCountryChanged("US");
                }
                else {
                    $("#billing #<%# tbStateEx.ClientID %>").val(billing.State);
                    $("#billing #<%# tbZipEx.ClientID %>").val(billing.Zip);
                    $("#billing #<%# tbPhoneEx.ClientID %>").val(billing.Phone);
                    $("#billing #<%# ddlCountry.ClientID %>").val(billing.Country);
                    onCountryChanged(billing.Country);
                }
                $("#billing #<%# tbEmail.ClientID %>").val(billing.Email);
            }
            if (registration != null) {
                $("#<%# cbUseShippingAsBilling.ClientID %>").attr('checked', '');
                onShippingCheckboxChange();
                $("#shipping #<%# tbShippingFirstName.ClientID %>").val(registration.FirstName);
                $("#shipping #<%# tbShippingLastName.ClientID %>").val(registration.LastName);
                $("#shipping #<%# tbShippingAddress1.ClientID %>").val(registration.Address1);
                $("#shipping #<%# tbShippingAddress2.ClientID %>").val(registration.Address2);
                $("#shipping #<%# tbShippingCity.ClientID %>").val(registration.City);
                if (registrationInfo == null || registrationInfo.Country == "" || registrationInfo.Country == "US") {
                    $("#shipping #<%# shippingState.ClientID %>").val(registration.State);
                    $("#shipping #<%# tbShippingZip.ClientID %>").val(registration.Zip);
                    if (registration.PhoneCnt != null) {
                        $("#shipping #<%# tbShippingPhone1.ClientID %>").val(registration.PhoneCnt.Code);
                        $("#shipping #<%# tbShippingPhone2.ClientID %>").val(registration.PhoneCnt.Part1);
                        $("#shipping #<%# tbShippingPhone3.ClientID %>").val(registration.PhoneCnt.Part2);
                    }
                    $("#shipping #<%# ddlShippingCountry.ClientID %>").val("US");
                    onShippingCountryChanged("US");
                }
                else {
                    $("#shipping #<%# tbShippingStateEx.ClientID %>").val(registration.State);
                    $("#shipping #<%# tbShippingZipEx.ClientID %>").val(registration.Zip);
                    $("#shipping #<%# tbShippingPhoneEx.ClientID %>").val(registration.Phone);
                    $("#shipping #<%# ddlShippingCountry.ClientID %>").val(registrationInfo.Country);
                    onShippingCountryChanged(registrationInfo.Country);
                }
                $("#shipping #<%# tbShippingEmail.ClientID %>").val(registration.Email);
            }
            else {
                $("#<%# cbUseShippingAsBilling.ClientID %>").attr('checked', 'checked');
                onShippingCheckboxChange();
            }
        }
        function onLoginBtnClick() {
            $("#ajax-login-section #ajax-login-error-msg").show();
            ajaxLogin($("#ajax-username").val(), $("#ajax-password").val(), onLoginSuccess, onLoginError);
        }
        function onLoginError(errorMsg) {
            showLoginMessage(errorMsg);
        }
        function showLoginMessage(msg) {
            $("#ajax-login-section #ajax-login-error-msg").html(msg);
            $("#ajax-login-section #ajax-login-error-msg-container").show();
        }

        $(document).ready(function() {
            $("#validation #<%# ddlCountry.ClientID %>").change(function() {
                onCountryChanged($("#validation #<%# ddlCountry.ClientID %>").val());
            });
            onCountryChanged($("#validation #<%# ddlCountry.ClientID %>").val());

            $("#validation #<%# ddlShippingCountry.ClientID %>").change(function() {
                onShippingCountryChanged($("#validation #<%# ddlShippingCountry.ClientID %>").val());
            });
            onShippingCountryChanged($("#validation #<%# ddlShippingCountry.ClientID %>").val());

            $("#<%# cbUseShippingAsBilling.ClientID %>").change(function() {
                onShippingCheckboxChange();
            });
            onShippingCheckboxChange();
        });
    </script>

</asp:PlaceHolder>
<div id="validation">
    <div class="container_12">
        <div class="grid_6">
            <div class="cart">
                <table width="100%" id="ajax-login-section">
                    <tr>
                        <th colspan="2">
                            Save Time and Sign In (Not Required)
                        </th>
                    </tr>
                    <tr>
                        <td width="35%">
                            Login (Email Address)
                        </td>
                        <td width="65%">
                            <input type="text" maxlength="50" id="ajax-username" name="username" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Password
                        </td>
                        <td>
                            <input type="password" maxlength="50" id="ajax-password" name="password" />
                            &nbsp;<a href="#">Forgot Password?</a>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <div class="button">
                                <a href="javascript:void();" onclick="onLoginBtnClick();">Login</a>
                            </div>
                        </td>
                    </tr>
                    <tr id="ajax-login-error-msg-container" style="display: none;">
                        <td colspan="2" align="center" class="last">
                            <span class="error" id="ajax-login-error-msg"></span>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <div class="grid_4">
            <div class="cart">
                <table width="100%">
                    <tr>
                        <th>
                            Need Help?
                        </th>
                    </tr>
                    <tr>
                        <td>
                            <strong>Call Customer Service at 1-888-555-5555</strong><br />
                            Monday to Saturday 24 Hours<br />
                            Sunday from 8AM - 6PM CST
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
    <div class="container_12">
        <div class="grid_6">
            <h1>
                Account Information</h1>
            <div class="cart color1">
                <table width="100%" id="billing">
                    <tr>
                        <th colspan="2">
                            Billing Address
                        </th>
                    </tr>
                    <tr>
                        <td colspan="2">
                            This is the address your credit card company has on file.
                        </td>
                    </tr>
                    <tr>
                        <td width="35%">
                            First Name *
                        </td>
                        <td width="65%">
                            <asp:TextBox runat="server" ID="tbFirstName" TabIndex="1" MaxLength="50" />
                            <asp:RegularExpressionValidator runat="server" ID="vFirstName" ValidationExpression="[a-zA-Z_\-\.\,\(\)\s]{2,}$"
                                ValidationGroup="billingGroup" ControlToValidate="tbFirstName" Display="Dynamic"
                                ErrorMessage="<div class='error'>Please enter valid first name</div>" EnableClientScript="true" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ValidationGroup="billingGroup"
                                ControlToValidate="tbFirstName" Display="Dynamic" ErrorMessage="<div class='error'>Please enter valid first name</div>"
                                EnableClientScript="true" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Last Name *
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="tbLastName" TabIndex="2" MaxLength="50" />
                            <asp:RegularExpressionValidator runat="server" ID="vLastName" ValidationExpression="[a-zA-Z_\-\.\,\(\)\s]{2,}$"
                                ValidationGroup="billingGroup" ControlToValidate="tbLastName" Display="Dynamic"
                                ErrorMessage="<div class='error'>Please enter valid last name</div>" EnableClientScript="true" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ValidationGroup="billingGroup"
                                ControlToValidate="tbLastName" Display="Dynamic" ErrorMessage="<div class='error'>Please enter valid last name</div>"
                                EnableClientScript="true" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Address Line One *
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="tbAddress1" TabIndex="3" MaxLength="100" />
                            <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator2" ValidationExpression="[a-zA-Z_0-9\#\-\.\,\(\)\s]{2,}$"
                                ValidationGroup="billingGroup" ControlToValidate="tbAddress1" Display="Dynamic"
                                ErrorMessage="<div class='error'>Please enter valid address</div>" EnableClientScript="true" />
                            <asp:RequiredFieldValidator ValidationGroup="billingGroup" ControlToValidate="tbAddress1"
                                Display="Dynamic" ErrorMessage="<div class='error'>Please enter valid address</div>"
                                EnableClientScript="true" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Address Line Two
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="tbAddress2" TabIndex="4" MaxLength="100" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Country *
                        </td>
                        <td>
                            <label>
                                <gen:DDLCountry runat="server" ID="ddlCountry" TabIndex="5" Style="width: 170px;" />
                            </label>
                        </td>
                    </tr>
                    <tr id="stateUS">
                        <td>
                            State *
                        </td>
                        <td>
                            <label>
                                <gen:DDLStateFullName runat="server" ID="ddlState" TabIndex="6" Style="width: 170px;" />
                            </label>
                        </td>
                    </tr>
                    <tr id="stateEx">
                        <td>
                            State/Province *
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="tbStateEx" TabIndex="7" MaxLength="50" />
                            <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidatorStateEx"
                                ValidationExpression="[a-zA-Z_0-9\#\-\.\,\(\)\s]{2,}$" ValidationGroup="billingGroup"
                                ControlToValidate="tbStateEx" Display="Dynamic" ErrorMessage="<div class='error'>Please enter valid state</div>"
                                EnableClientScript="true" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidatorStateEx" ValidationGroup="billingGroup"
                                ControlToValidate="tbStateEx" Display="Dynamic" ErrorMessage="<div class='error'>Please enter valid state</div>"
                                EnableClientScript="true" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            City/Town *
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="tbCity" TabIndex="8" MaxLength="50" />
                            <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator3" ValidationExpression="[a-zA-Z_0-9\-\.\,\(\)\s]{2,}$"
                                ValidationGroup="billingGroup" ControlToValidate="tbCity" Display="Dynamic" ErrorMessage="<div class='error'>Please enter valid city/town</div>"
                                EnableClientScript="true" />
                            <asp:RequiredFieldValidator ValidationGroup="billingGroup" ControlToValidate="tbCity"
                                Display="Dynamic" ErrorMessage="<div class='error'>Please enter valid city/town</div>"
                                EnableClientScript="true" runat="server" />
                        </td>
                    </tr>
                    <tr id="zipUS">
                        <td>
                            Zip Code *
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="tbZip" TabIndex="9" MaxLength="5" />
                            <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidatorZip"
                                ValidationExpression="\d{5}$" ValidationGroup="billingGroup" ControlToValidate="tbZip"
                                Display="Dynamic" ErrorMessage="<div class='error'>Please enter valid zip code</div>"
                                EnableClientScript="true" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidatorZip" ValidationGroup="billingGroup"
                                ControlToValidate="tbZip" Display="Dynamic" ErrorMessage="<div class='error'>Please enter valid zip code</div>"
                                EnableClientScript="true" runat="server" />
                        </td>
                    </tr>
                    <tr id="zipEx">
                        <td>
                            Zip Code *
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="tbZipEx" TabIndex="10" MaxLength="50" />
                            <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidatorZipEx"
                                ValidationExpression="\d{10}$" ValidationGroup="billingGroup" ControlToValidate="tbZipEx"
                                Display="Dynamic" ErrorMessage="<div class='error'>Please enter valid zip code</div>"
                                EnableClientScript="true" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidatorZipEx" ValidationGroup="billingGroup"
                                ControlToValidate="tbZipEx" Display="Dynamic" ErrorMessage="<div class='error'>Please enter valid zip code</div>"
                                EnableClientScript="true" runat="server" />
                        </td>
                    </tr>
                    <tr id="phoneUS">
                        <td>
                            Phone Number *
                        </td>
                        <td colspan="2">
                            (
                            <asp:TextBox runat="server" ID="tbPhone1" TabIndex="11" Style="width: 30px;" MaxLength="3" />
                            )
                            <asp:TextBox runat="server" ID="tbPhone2" TabIndex="12" Style="width: 30px;" MaxLength="3" />
                            -
                            <asp:TextBox runat="server" ID="tbPhone3" TabIndex="13" Style="width: 40px;" MaxLength="4" />
                            <asp:CustomValidator ID="CustomFieldValidatorPhone" ValidationGroup="billingGroup"
                                ControlToValidate="tbPhone3" Display="Dynamic" ClientValidationFunction="ValidatePhone"
                                ErrorMessage="<div class='error'>Please enter valid phone</div>" EnableClientScript="true"
                                runat="server" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidatorPhone" ValidationGroup="billingGroup"
                                ControlToValidate="tbPhone3" Display="Dynamic" ErrorMessage="<div class='error'>Please enter valid phone</div>"
                                EnableClientScript="true" runat="server" />
                        </td>
                    </tr>
                    <tr id="phoneEx">
                        <td>
                            Phone Number *
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="tbPhoneEx" TabIndex="14" MaxLength="50" />
                            <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidatorPhoneEx"
                                ValidationExpression="\d{2,}$" ValidationGroup="billingGroup" ControlToValidate="tbPhoneEx"
                                Display="Dynamic" ErrorMessage="<div class='error'>Please enter valid phone</div>"
                                EnableClientScript="true" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidatorPhoneEx" ValidationGroup="billingGroup"
                                ControlToValidate="tbPhoneEx" Display="Dynamic" ErrorMessage="<div class='error'>Please enter valid phone</div>"
                                EnableClientScript="true" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Email Address *
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="tbEmail" TabIndex="15" MaxLength="50" />
                            <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator1" ValidationExpression="[a-zA-Z_0-9\.\-]+@[a-zA-Z_0-9\-\.]+[a-zA-Z0-9]{2,4}"
                                ValidationGroup="billingGroup" ControlToValidate="tbEmail" Display="Dynamic"
                                ErrorMessage="<div class='error'>Please enter valid email</div>" EnableClientScript="true" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator4" ValidationGroup="billingGroup"
                                ControlToValidate="tbEmail" Display="Dynamic" ErrorMessage="<div class='error'>Please enter valid email</div>"
                                EnableClientScript="true" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Shipping Address *
                        </td>
                        <td>
                            <asp:CheckBox name="checkbox" ID="cbUseShippingAsBilling" Checked="true" runat="server"
                                TabIndex="16" />
                            Use this address as my shipping address.
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <div class="grid_6">
            <h1>
                &nbsp;</h1>
            <div class="cart color1" id="shipping" style="display: none;">
                <table width="100%">
                    <tr>
                        <th colspan="2">
                            Shipping Address
                        </th>
                    </tr>
                    <tr>
                        <td colspan="2">
                            Ship to this address.
                        </td>
                    </tr>
                    <tr>
                        <td width="35%">
                            First Name *
                        </td>
                        <td width="65%">
                            <asp:TextBox runat="server" ID="tbShippingFirstName" TabIndex="17" MaxLength="50" />
                            <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidatorShippingFirstName"
                                ValidationExpression="[a-zA-Z_\-\.\,\(\)\s]{2,}$" ValidationGroup="billingGroup"
                                ControlToValidate="tbShippingFirstName" Display="Dynamic" ErrorMessage="<div class='error'>Please enter valid first name</div>"
                                EnableClientScript="true" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidatorShippingFirstName" ValidationGroup="billingGroup"
                                ControlToValidate="tbShippingFirstName" Display="Dynamic" ErrorMessage="<div class='error'>Please enter valid first name</div>"
                                EnableClientScript="true" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Last Name *
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="tbShippingLastName" TabIndex="18" MaxLength="50" />
                            <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidatorShippingLastName"
                                ValidationExpression="[a-zA-Z_\-\.\,\(\)\s]{2,}$" ValidationGroup="billingGroup"
                                ControlToValidate="tbShippingLastName" Display="Dynamic" ErrorMessage="<div class='error'>Please enter valid last name</div>"
                                EnableClientScript="true" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidatorShippingLastName" ValidationGroup="billingGroup"
                                ControlToValidate="tbShippingLastName" Display="Dynamic" ErrorMessage="<div class='error'>Please enter valid last name</div>"
                                EnableClientScript="true" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Address Line One *
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="tbShippingAddress1" TabIndex="19" MaxLength="100" />
                            <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidatorShippingAddress1"
                                ValidationExpression="[a-zA-Z_0-9\#\-\.\,\(\)\s]{2,}$" ValidationGroup="billingGroup"
                                ControlToValidate="tbShippingAddress1" Display="Dynamic" ErrorMessage="<div class='error'>Please enter valid address</div>"
                                EnableClientScript="true" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidatorShippingAddress1" ValidationGroup="billingGroup"
                                ControlToValidate="tbShippingAddress1" Display="Dynamic" ErrorMessage="<div class='error'>Please enter valid address</div>"
                                EnableClientScript="true" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Address Line Two
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="tbShippingAddress2" TabIndex="20" MaxLength="100" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Country *
                        </td>
                        <td>
                            <label>
                                <gen:DDLCountry runat="server" ID="ddlShippingCountry" TabIndex="21" Style="width: 170px;" />
                            </label>
                        </td>
                    </tr>
                    <tr id="shippingState">
                        <td>
                            State *
                        </td>
                        <td>
                            <label>
                                <gen:DDLStateFullName runat="server" ID="shippingState" TabIndex="22" Style="width: 170px;" />
                            </label>
                        </td>
                    </tr>
                    <tr id="shippingStateEx">
                        <td>
                            State/Province *
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="tbShippingStateEx" TabIndex="23" MaxLength="50" />
                            <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidatorShippingStateEx"
                                ValidationExpression="[a-zA-Z_0-9\#\-\.\,\(\)\s]{2,}$" ValidationGroup="billingGroup"
                                ControlToValidate="tbShippingStateEx" Display="Dynamic" ErrorMessage="<div class='error'>Please enter valid state</div>"
                                EnableClientScript="true" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidatorShippingStateEx" ValidationGroup="billingGroup"
                                ControlToValidate="tbShippingStateEx" Display="Dynamic" ErrorMessage="<div class='error'>Please enter valid state</div>"
                                EnableClientScript="true" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            City/Town *
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="tbShippingCity" TabIndex="24" MaxLength="50" />
                            <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidatorShippingCity"
                                ValidationExpression="[a-zA-Z_0-9\-\.\,\(\)\s]{2,}$" ValidationGroup="billingGroup"
                                ControlToValidate="tbShippingCity" Display="Dynamic" ErrorMessage="<div class='error'>Please enter valid city/town</div>"
                                EnableClientScript="true" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidatorShippingCity" ValidationGroup="billingGroup"
                                ControlToValidate="tbShippingCity" Display="Dynamic" ErrorMessage="<div class='error'>Please enter valid city/town</div>"
                                EnableClientScript="true" runat="server" />
                        </td>
                    </tr>
                    <tr id="shippingZip">
                        <td>
                            Zip Code *
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="tbShippingZip" TabIndex="25" MaxLength="5" />
                            <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidatorShippingZip"
                                ValidationExpression="\d{5}$" ValidationGroup="billingGroup" ControlToValidate="tbShippingZip"
                                Display="Dynamic" ErrorMessage="<div class='error'>Please enter valid zip code</div>"
                                EnableClientScript="true" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidatorShippingZip" ValidationGroup="billingGroup"
                                ControlToValidate="tbShippingZip" Display="Dynamic" ErrorMessage="<div class='error'>Please enter valid zip code</div>"
                                EnableClientScript="true" runat="server" />
                        </td>
                    </tr>
                    <tr id="shippingZipEx">
                        <td>
                            Zip Code *
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="tbShippingZipEx" TabIndex="26" MaxLength="50" />
                            <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidatorShippingZipEx"
                                ValidationExpression="\d{5}$" ValidationGroup="billingGroup" ControlToValidate="tbShippingZip"
                                Display="Dynamic" ErrorMessage="<div class='error'>Please enter valid zip code</div>"
                                EnableClientScript="true" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidatorShippingZipEx" ValidationGroup="billingGroup"
                                ControlToValidate="tbShippingZipEx" Display="Dynamic" ErrorMessage="<div class='error'>Please enter valid zip code</div>"
                                EnableClientScript="true" runat="server" />
                        </td>
                    </tr>
                    <tr id="shippingPhone">
                        <td>
                            Phone Number *
                        </td>
                        <td colspan="2">
                            (
                            <asp:TextBox runat="server" ID="tbShippingPhone1" TabIndex="27" Style="width: 30px;"
                                MaxLength="3" />
                            )
                            <asp:TextBox runat="server" ID="tbShippingPhone2" TabIndex="28" Style="width: 30px;"
                                MaxLength="3" />
                            -
                            <asp:TextBox runat="server" ID="tbShippingPhone3" TabIndex="29" Style="width: 40px;"
                                MaxLength="4" />
                            <asp:CustomValidator ID="CustomFieldValidatorShippingPhone" ValidationGroup="billingGroup"
                                ControlToValidate="tbShippingPhone3" Display="Dynamic" ClientValidationFunction="ValidateShippingPhone"
                                ErrorMessage="<div class='error'>Please enter valid phone</div>" EnableClientScript="true"
                                runat="server" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidatorShippingPhone" ValidationGroup="billingGroup"
                                ControlToValidate="tbShippingPhone3" Display="Dynamic" ErrorMessage="<div class='error'>Please enter valid phone</div>"
                                EnableClientScript="true" runat="server" />
                        </td>
                    </tr>
                    <tr id="shippingPhoneEx">
                        <td>
                            Phone Number *
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="tbShippingPhoneEx" TabIndex="30" MaxLength="50" />
                            <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidatorShippingPhoneEx"
                                ValidationExpression="\d{2,}$" ValidationGroup="billingGroup" ControlToValidate="tbShippingPhoneEx"
                                Display="Dynamic" ErrorMessage="<div class='error'>Please enter valid phone</div>"
                                EnableClientScript="true" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidatorShippingPhoneEx" ValidationGroup="billingGroup"
                                ControlToValidate="tbShippingPhoneEx" Display="Dynamic" ErrorMessage="<div class='error'>Please enter valid phone</div>"
                                EnableClientScript="true" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Email Address *
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="tbShippingEmail" TabIndex="31" MaxLength="50" />
                            <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidatorShippingEmail"
                                ValidationExpression="[a-zA-Z_0-9\.\-]+@[a-zA-Z_0-9\-\.]+[a-zA-Z0-9]{2,4}" ValidationGroup="billingGroup"
                                ControlToValidate="tbShippingEmail" Display="Dynamic" ErrorMessage="<div class='error'>Please enter valid email</div>"
                                EnableClientScript="true" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidatorShippingEmail" ValidationGroup="billingGroup"
                                ControlToValidate="tbShippingEmail" Display="Dynamic" ErrorMessage="<div class='error'>Please enter valid email</div>"
                                EnableClientScript="true" runat="server" />
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
    <asp:PlaceHolder runat="server" ID="phPassword" Visible="<%# Referer == null %>">
        <div class="container_12">
            <div class="grid_6">
                <div class="cart color1">
                    <table width="100%">
                        <tr>
                            <th colspan="2">
                                Create a Password for Faster Checkout
                            </th>
                        </tr>
                        <tr>
                            <td width="35%">
                                Password *
                            </td>
                            <td width="65%">
                                <asp:TextBox ID="tbPassword" runat="server" TabIndex="32" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Password Again *
                            </td>
                            <td>
                                <asp:TextBox ID="tbPasswordAgain" runat="server" TabIndex="33" />
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" ID="phCreditCardInformation" Visible="<%# IsGiftCertificateValidate == false %>">
        <div class="container_12">
            <div class="grid_6">
                <h1>
                    Payment Information</h1>
                <div class="cart color1">
                    <table width="100%">
                        <tr>
                            <th colspan="2">
                                Payment Method
                            </th>
                        </tr>
                        <tr>
                            <td>
                                We Accept
                            </td>
                            <td>
                                <img src="images/Visa.gif" width="50" height="30" />
                                <img src="images/MasterCard.gif" width="50" height="30" />
                                <img src="images/Amex.gif" width="50" height="30" />
                            </td>
                        </tr>
                        <tr>
                            <td width="35%">
                                Card Type *
                            </td>
                            <td width="65%">
                                <gen:DDLPaymentType runat="server" ID="ddlPaymentType" TabIndex="34" ToolTip="Please Choose Credit Card Type" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Card Number *
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="tbCreditCardNumber" TabIndex="35" ToolTip="Please Enter Valid Credit Card Number"
                                    MaxLength="16" />
                                <asp:CustomValidator ID="CustomValidator1" ValidationGroup="billingGroup" ControlToValidate="tbCreditCardNumber"
                                    Display="Dynamic" ClientValidationFunction="ValidateCreditCard" ErrorMessage="<div class='error'>Please enter valid credit card number</div>"
                                    EnableClientScript="true" runat="server" />
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator5" ValidationGroup="billingGroup"
                                    ControlToValidate="tbCreditCardNumber" Display="Dynamic" ErrorMessage="<div class='error'>Please enter valid credit card number</div>"
                                    EnableClientScript="true" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Expiry Date *
                            </td>
                            <td>
                                <gen:DDLMonth runat="server" ID="ddlExpireMonth" TabIndex="36" ToolTip="Please Choose Credit Card Expiration Month"
                                    Style="width: 75px;" />
                                <gen:DDLYear runat="server" ID="ddlExpireYear" TabIndex="37" ToolTip="Please Choose Credit Card Expiration Year"
                                    Style="width: 60px;" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                CVV *
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="tbCreditCardCVV" TabIndex="38" ToolTip="Please Enter Valid CVV"
                                    Style="width: 50px;" MaxLength="5" />
                                &nbsp; <a href="javascript: void(0)" tabindex="39" onclick="window.open('images/cart-cvv.png','buttonwin','height=350,width=520,scrollbars=yes,toolbar=no,location=no,screenX=100,screenY=20,top=20,left=100','fullscreen=no'); 
            return false;">
                                    What's This?</a>
                                <asp:RegularExpressionValidator ID="CustomValidator2" ValidationGroup="billingGroup"
                                    ControlToValidate="tbCreditCardCVV" Display="Dynamic" ErrorMessage="<div class='error'>Please enter valid CVV</div>"
                                    ValidationExpression="[0-9]{3}" EnableClientScript="true" runat="server" />
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator6" ValidationGroup="billingGroup"
                                    ControlToValidate="tbCreditCardCVV" Display="Dynamic" ErrorMessage="<div class='error'>Please enter valid CVV</div>"
                                    EnableClientScript="true" runat="server" />
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
    </asp:PlaceHolder>
</div>
