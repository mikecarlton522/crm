<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EmailForm.aspx.cs" Inherits="TrimFuel.Web.Admin.AjaxControls.EmailForm"
    ValidateRequest="false" %>

<div class="module">
    
    <h2>
        <%# DynamicEmailType.DisplayName %>
        <form runat="server" id="form1">
        <asp:hiddenfield runat="server" id="hdnDynamicEmailID" value='<%# DynamicEmail.DynamicEmailID %>'></asp:hiddenfield>
        <asp:hiddenfield runat="server" id="hdnBillingID" value='<%# BillingID %>'></asp:hiddenfield>
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
                </td>
                <td>
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
            <tr>
                <td colspan="4">
                    <asp:textbox runat="server" id="tbBody" textmode="MultiLine" cssclass="xxwide" width="530"
                        height="360" text='<%# Body %>' tabindex="7"></asp:textbox>
                </td>
            </tr>
            <tr>
                <td colspan="4" align="right">
                    <div id="dynamicemail-btn-group-main">
                        <asp:button id="bSendEmail" runat="server" onclientclick='<%# "CKEDITOR.instances[\"" + tbBody.ClientID + "\"].updateElement();" %>'
                            onclick="bSendEmail_Click" text="Send" />
                        <%--<asp:button id="bCancel" runat="server" text="Cancel" onclick="bCancel_Click" />--%>
                    </div>
                </td>
            </tr>
        </table>
        </form>
        <asp:placeholder runat="server" id="phMessage2" visible="false">
    <div id="errorMsg">Email was sent</div>
    </asp:placeholder>
        <asp:placeholder runat="server" id="phMessage3" visible="false">
    <div id="errorMsg">Error occured while sending Email: <asp:Literal runat="server" ID="lSendEmailError" /></div>
    </asp:placeholder>
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
</div>
