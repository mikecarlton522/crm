<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProductFlowWizard.aspx.cs" Inherits="TrimFuel.Web.Admin.AjaxControls.ProductFlowWizard" %>
<%@ Register Assembly="TrimFuel.Web.UI" Namespace="TrimFuel.Web.UI" TagPrefix="gen" %>
<script type="text/javascript">

    function EnableControls(enable, form, selector) {
        if (enable) {
            $(form + " " + selector).removeAttr('disabled');
            $(form + " " + selector).show();
        } else {
            $(form + " " + selector).attr('disabled', 'disabled');
            $(form + " " + selector).hide();
        }
    }

    function SelectAffiliate(affID) {
        EnableControls(false, "#step3-form", ".affiliate-dependent");
    }

    function CancelValidation(el) {
        $(el).addClass("confirm");
    }

    function addPartner1() {
        var added = $("#step5-form #leadpartners1 #partner1-prototype").clone().attr("id", "").appendTo($("#step5-form #leadpartners1")).show();
        $(added).find("select[name|=partner1]").attr("id", "partner1" + randomString());
        added.find(".donotvalidate").removeClass("donotvalidate");
        added.find(".prototype").removeClass("prototype");
    }

    function addPartner2() {
        var added = $("#step5-form #leadpartners2 #partner2-prototype").clone().attr("id", "").appendTo($("#step5-form #leadpartners2")).show();
        $(added).find("select[name|=partner2]").attr("id", "partner2" + randomString());
        added.find(".donotvalidate").removeClass("donotvalidate");
        added.find(".prototype").removeClass("prototype");
    }

    function addPartner3() {
        var added = $("#step5-form #leadpartners3 #partner3-prototype").clone().attr("id", "").appendTo($("#step5-form #leadpartners3")).show();
        $(added).find("select[name|=partner3]").attr("id", "partner3" + randomString());
        added.find(".donotvalidate").removeClass("donotvalidate");
        added.find(".prototype").removeClass("prototype");
    }

    function addAffiliate() {
        var added = $("#step3-form #affiliates #aff-prototype").clone().attr("id", "").appendTo($("#step3-form #affiliates")).show();
        $(added).find("select[name|=affiliate]").attr("id", "affiliate" + randomString());
        added.find(".donotvalidate").removeClass("donotvalidate");
        added.find(".prototype").removeClass("prototype");
    }

    function removePartner(el) {
        $(el).parent().remove();
        return false;
    }

    function removeAffiliate(el) {
        $(el).parent().remove();
        EnableControls(false, "#step3-form", ".affiliate-dependent");
        return false;
    }

    function randomString() {
        var ABC, temp, randomID, a;
        ABC = "0123456789";
        temp = ""
        for (a = 0; a < 6; a++) {
            randomID = Math.random();
            randomID = Math.floor(randomID * ABC.length);
            temp += ABC.charAt(randomID);
        }
        return temp;
    }

    function editAffiliate(el) {
        var dv = $(el).closest('div');
        var affID = $(dv).find("select[name*=affiliate]").val();
        if (affID != "" && affID != "undefined")
        {
            EnableControls(true, "#step3-form", ".affiliate-dependent");
            inlineControl("../../controls/affiliate.asp?affId=" + affID, "edit-affiliate", null,
                function() {
                    $("#pixels-header").remove();
                    $("#affiliate-data").remove();
                    $("#subaff-list").remove();

                    $(document).ready(
                        function() {
                            $("#tabs").tabs({
                                cookie: true,
                                selected: $("#hdnSelectedTab").val(),
                                select: function(event, ui) {
                                    $("#hdnSelectedTab").val(ui.index);
                                }
                            });
                        });

                });
        }
    }

    function newAffiliate(el) {
        EnableControls(false, "#step3-form", ".affiliate-dependent");
        popupControl2("popup-new-affiliate", "New Affiliate", 600, 500, "../../controls/affiliate.asp", null,
            function() {
                $(document).ready(
                    function() {
                        $("#tabs").tabs({
                            cookie: true,
                            selected: $("#hdnSelectedTab").val(),
                            select: function(event, ui) {
                                $("#hdnSelectedTab").val(ui.index);
                            }
                        });
                    });

            },
            function() {
                var dv = $(el).closest('div');
                $(dv).find("select[name*=affiliate]").val("101");
            }
        );
    }

    function cancelClick() {
	    if (confirm("Are you sure you want to cancel?")) {
	        inlineControl("ajaxControls/ProductFlowManager.aspx?productId=" + <%= Request["productId"] %>, "right", null, null);
	    }
        return false;
    }


    function loadEmailTypeList() {
        $("#campaign-email-type-list-container").html("<img src=\"../images/loading2.gif\"/>");        var url = "ajaxControls/CampaignEmailTypeList.aspx?productID=" + <%= ProductID %> + "&campaignID=" + <%= CampaignID %>;
        inlineControl(url, "campaign-email-type-list-container");
    }        function onCreateEmail(dynamicEmailTypeID, hours, giftCertificateDynamicEmail_StoreID) {        var url = "ajaxControls/CampaignDynamicEmail.aspx?productID=" + <%= ProductID %> + "&campaignID=" + <%= CampaignID %> + "&dynamicEmailTypeID=" + dynamicEmailTypeID + "&hours=" + hours + "&storeID=" + giftCertificateDynamicEmail_StoreID;
		popupControl2("popup-dynamic-email-container", "Dynamic Email", 930, 600, url, null, null,
            function() {
                loadEmailTypeList();
            });
    }        function onEditEmail(dynamicEmailID) {        var url = "ajaxControls/CampaignDynamicEmail.aspx?productID=" + <%= ProductID %> + "&campaignID=" + <%= CampaignID %> + "&dynamicEmailID=" + dynamicEmailID;
		popupControl2("popup-dynamic-email-container", "Dynamic Email", 930, 600, url, null, null,
            function() {
                loadEmailTypeList();
            });
    }

    $(document).ready(function () {
        <% if(string.IsNullOrEmpty(Request["campaignID"]))
            { %>
            inlineControl("../../editForms/campaignsubscription.asp?cProductId=" + <%= Request["productID"] %>, "bs-container", null,
                function() {
                    $("#campaign-subscription-product").hide();
                });
        <%} else { %>
            inlineControl("../../editForms/campaignsubscription.asp?cId=" + <%= Request["campaignID"] %>, "bs-container", null,
                function() {
                    $("#campaign-subscription-product").hide();
                });
        <%} %>

        loadEmailTypeList();

        $("#<%# cbExternal.ClientID %>").click(function () {
            EnableControls(this.checked, "#step1-form", ".hostingtype-dependent");
        });
        
        EnableControls(<% if(CampaignView.Campaign.IsExternal == true) {%>true<%} else { %>false<%} %>, "#step1-form", ".hostingtype-dependent");
        EnableControls(false, "#step3-form", ".affiliate-dependent");
    });
</script>

<form runat="server" ID="form1">
<div style="width:100%;height=100%;">
    <asp:Wizard ID="wSteps" runat="server" ActiveStepIndex="0" DisplaySideBar="false" DisplayCancelButton="true" style="border:none;" height="100%" width="100%"
        onactivestepchanged="OnActiveStepChanged" onnextbuttonclick="OnNextButtonClick" onfinishbuttonclick="OnFinishButtonClick">
        <HeaderTemplate>
                <table class="editForm">
                    <tr>
                        <td>
                            <asp:Image id="imgHeader" runat="server" AlternateText="" ImageUrl="images/Step1.png" />
                        </td>
                        <td>
                            <strong>
                                <asp:Label id="lblHeader" runat="server" style="font-size: 16px;" Text="Step 1: New Campaign" />
                            </strong>
                        </td>
                    </tr>
                </table>
        </HeaderTemplate>
        <StartNavigationTemplate>
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CausesValidation="false" 
                OnClientClick="return cancelClick();" />
            <asp:Button ID="StartNextButton" runat="server" CommandName="MoveNext" Text="Next" />
        </StartNavigationTemplate>
        <StepNavigationTemplate>
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CausesValidation="false" 
                OnClientClick="return cancelClick();" />
            <asp:Button ID="StepPreviousButton" runat="server" CommandName="MovePrevious" Text="Back" />
            <asp:Button ID="StepNextButton" runat="server" CommandName="MoveNext" Text="Next" />
        </StepNavigationTemplate>
        <FinishNavigationTemplate>
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CausesValidation="false" 
                OnClientClick="return cancelClick();" />
            <asp:Button ID="FinishPreviousButton" runat="server" CommandName="MovePrevious" Text="Back" />
            <asp:Button ID="FinishButton" runat="server" CommandName="MoveComplete" Text="Save" />
        </FinishNavigationTemplate>
        <WizardSteps>
            <asp:WizardStep ID="wStep1" runat="server" Title="Step 1: New Campaign" StepType="Start">
                <div style="width:700px; height:550px;">
                <table class="editForm" id="step1-form" width="100%">
                    <tr>
                        <td>
                            <span class="caption" width="200px">Enter/Edit Campaign Name:</span>
                        </td>
                        <td>
                            <asp:TextBox id="txtCampaignName" width="300px" MaxLength="100" runat="server" class="validate[required]"></asp:TextBox>
                        </td>
                    </tr>
                    <tr class="subheader">
                        <td>
                            <span class="caption" width="200px">Hosting: External</span>
                        </td>
                        <td>
                            <asp:CheckBox id="cbExternal" runat="server"/>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <a class="caption hostingtype-dependent" width="200px">URL:</a>
                        </td>
                        <td>
                            <asp:TextBox id="txtCampaignURL" CssClass="hostingtype-dependent" runat="server" MaxLength="255" width="100%"></asp:TextBox>
                        </td>
                    </tr>
                </table>
                </div>
            </asp:WizardStep>
            <asp:WizardStep ID="wStep2" Title="Step 2: Select Products/Plans" runat="server" StepType="Step">
                <div style="width:700px; height:550px;">
                <table class="editForm">
                    <tr class="subheader">
                        <td colspan="2">
                            <span class="caption" width="100%">Part 1: Select a recurring plan. This campaign will assign the user to this plan.</span>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <div id="bs-container" style="width:100%; height:100%;"/>
                        </td>
                    </tr>
                    <tr class="subheader">
                        <td colspan="2">
                            <span class="caption" width="100%">Part 2: Select a Shipper/Fulfillment Site. If you don't see your shipper below, please contact your Triangle representative.</span>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <span class="caption" width="100px">Shipper:</span>
                        </td>
                        <td>
                            <asp:DropDownList id="ddShipper" width="250px" runat="server" DataTextField="Name"
                                DataValueField="ShipperID" AppendDataBoundItems="true">
                                <asp:ListItem Text="--- Select ---" Value=""></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                </div>
            </asp:WizardStep>
            <asp:WizardStep ID="wStep3" Title="Step 3: Affiliates & Options" runat="server" StepType="Step">
                <div style="width:700px; height:550px; overflow:auto; overflow-x:hidden;">
                <table class="editForm" id="step3-form" width="100%">
                    <tr class="subheader">
                        <td style="width:200px;">Affiliates & Pixels</td>
                        <td>
                            <a href="javascript:addAffiliate()" class="addNewIcon" style="float:right;">Add Affiliate</a>
                        </td>
                    </tr>
                    <tr>
                        <td></td>
                        <td nowrap="nowrap">
                            <div id="affiliates">
	                            <div style="margin:5px;width:100%;display:none;" id="aff-prototype">
		                            <select name="affiliateid" id="affiliateid-prototype" style="width:200px;" onchange="javascript:SelectAffiliate(this.value);" class="donotvalidate prototype">
			                            <option value="">-- Select --</option>
			                            <%# AffiliateList("")%>
		                            </select>
		                            <a href="javascript:void(0)" onclick="return editAffiliate(this);" class="editIcon">Edit Pixels</a>
		                            <a href="javascript:void(0)" onclick="return newAffiliate(this);" class="addNewIcon">New</a>
		                            <a href="javascript:void(0)" onclick="return removeAffiliate(this);" class="removeIcon">Remove</a>
	                            </div>
                                <asp:Literal id="ltAffiliate" runat="server"/>
                            </div>
                        </td>
                    </tr>
                   <tr class="subheader affiliate-dependent">
                        <td style="width:200px;">Edit Pixels:</td>
                        <td>
                        </td>
                    </tr>
                    <tr class="affiliate-dependent">
                        <td colspan="2">
                            <div id="edit-affiliate" style="width:100%; height:100%; overflow:auto; overflow-x:hidden;"/>
                        </td>
                    </tr>
                    <!--tr class="subheader">
                        <td colspan="2">
                            <span class="caption" width="100%">Campaign Options</span>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <span class="caption" width="200px">Enable Risk Scoring:</span>
                        </td>
                        <td>
                            <asp:CheckBox id="cbRiskScoring" runat="server" Checked="false"/>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <span class="caption" width="200px">Enable Dupe Checking:</span>
                        </td>
                        <td>
                            <asp:CheckBox id="cbDupeChecking" runat="server" Checked="false" />
                        </td>
                    </tr-->
                 </table>
                </div>
            </asp:WizardStep>
            <asp:WizardStep ID="wStep4" Title="Step 4: Email Settings" runat="server" StepType="Step">
                <div style="width:700px; height:550px; overflow:auto; overflow-x:hidden;">
                <table class="editForm" id="step4-form" width="100%">
                    <tr>
                        <td colspan="2">
                            <div id="campaign-email-type-list-container" style="width:100%; height:100%;"/>
                        </td>
                    </tr>
                </table>
                </div>
            </asp:WizardStep>
            <asp:WizardStep ID="wStep5" Title="Step 5: Lead Routing" runat="server" StepType="Step">
                <div style="width:700px; height:550px; overflow:auto; overflow-x:hidden;">
                <table class="editForm" id="step5-form" width="100%">
                    <tr class="subheader"><td style="width:200px;">Abandons</td>
                        <td>
                            <a href="javascript:addPartner1()" class="addNewIcon" style="float:right;">Add Routing Rule for Abandons</a>
                        </td>
                    </tr>
                    <tr>
                        <td></td>
                        <td nowrap="nowrap">
                            <div id="leadpartners1">
	                            <div style="margin:5px;width:100%;display:none;" id="partner1-prototype">
			                        <input type="text" name="percentage1" class="xnarrow" maxlength="3" value="0"/>%&nbsp;&nbsp;
		                            <select name="partnerid1" id="partnerid1-prototype" style="width:200px;" class="donotvalidate prototype">
			                            <option value="">-- Select --</option>
			                            <%# LeadPartnerList("")%>
		                            </select>
		                            <a href="#" onclick="return removePartner(this);" class="removeIcon">Remove</a>
	                            </div>
                                <asp:Literal id="ltPartner1" runat="server"/>
                            </div>
                        </td>
                    </tr>
                    <tr class="subheader"><td style="width:200px;">Order Confirmations</td>
                        <td>
                            <a href="javascript:addPartner2()" class="addNewIcon" style="float:right;">Add Routing Rule for New Orders</a>
                        </td>
                    </tr>
                    <tr>
                        <td></td>
                        <td nowrap="nowrap">
                            <div id="leadpartners2">
	                            <div style="margin:5px;width:100%;display:none;" id="partner2-prototype">
			                        <input type="text" name="percentage2" class="xnarrow" maxlength="3" value="0"/>%&nbsp;&nbsp;
		                            <select name="partnerid2" id="partnerid2-prototype" style="width:200px;" class="donotvalidate prototype">
			                            <option value="">-- Select --</option>
			                            <%# LeadPartnerList("")%>
		                            </select>
		                            <a href="#" onclick="return removePartner(this);" class="removeIcon">Remove</a>
	                            </div>
                                <asp:Literal id="ltPartner2" runat="server"/>
                            </div>
                        </td>
                    </tr>
                    <tr class="subheader"><td style="width:200px;">Cancellations/Declines</td>
                        <td>
                            <a href="javascript:addPartner3()" class="addNewIcon" style="float:right;">Add Routing Rule for Cancels/Declines</a>
                        </td>
                    </tr>
                    <tr>
                        <td></td>
                        <td nowrap="nowrap">
                            <div id="leadpartners3">
	                            <div style="margin:5px;width:100%;display:none;" id="partner3-prototype">
			                        <input type="text" name="percentage3" class="xnarrow" maxlength="3" value="0"/>%&nbsp;&nbsp;
		                            <select name="partnerid3" id="partnerid3-prototype" style="width:200px;" class="donotvalidate prototype">
			                            <option value="">-- Select --</option>
			                            <%# LeadPartnerList("")%>
		                            </select>
		                            <a href="#" onclick="return removePartner(this);" class="removeIcon">Remove</a>
	                            </div>
                                <asp:Literal id="ltPartner3" runat="server"/>
                            </div>
                        </td>
                    </tr>
                </table>
                </div>
            </asp:WizardStep>
            <asp:WizardStep ID="wStep6" Title="Step 6: Confirmation" runat="server" StepType="Finish">
                <div style="width:700px; height:550px; overflow:auto; overflow-x:hidden;">
                <table class="editForm" width="100%">
                    <tr class="subheader">
                        <td>
                            <strong><span class="caption" width="200px">Campaign Name:</span></strong>
                        </td>
                        <td>
                            <asp:Label id="lblCampaignName" runat="server" width="100%" style="font-weight: normal;"/>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <strong><span class="caption" width="200px">External Hosting Provider:</span></strong>
                        </td>
                        <td>
                            <asp:Label id="lblCampaignURL" runat="server" width="100%" style="font-weight: normal;"/>
                        </td>
                    </tr>
                    <tr class="subheader">
                        <td>
                            <strong><span class="caption" width="200px">Product:</span></strong>
                        </td>
                        <td>
                            <asp:Label id="lblProduct" runat="server" width="100%" style="font-weight: normal;"/>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <strong><span class="caption" width="200px">Product Type:</span></strong>
                        </td>
                        <td>
                            <asp:Label id="lblSubscription" runat="server" width="100%" style="font-weight: normal;"/>
                        </td>
                    </tr>
                    <tr class="subheader">
                        <td>
                            <strong><span class="caption" width="200px">Shipper:</span></strong>
                        </td>
                        <td>
                            <asp:Label id="lblShipper" runat="server" width="100%" style="font-weight: normal;"/>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <strong><span class="caption" width="200px">Affiliates:</span></strong>
                        </td>
                        <td>
                            <asp:Label id="lblAffiliates" runat="server" width="100%" style="font-weight: normal;"/>
                        </td>
                    </tr>
                    <tr class="subheader">
                        <td>
                            <strong><span class="caption" width="200px">Transactional Emails:</span></strong>
                        </td>
                        <td>
                            <asp:Label id="lblEmails" runat="server" width="100%" style="font-weight: normal;"/>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <strong><span class="caption" width="200px">Abandons:</span></strong>
                        </td>
                        <td>
                            <asp:Label id="lblAbandons" runat="server" width="100%" style="font-weight: normal;"/>
                        </td>
                    </tr>
                    <tr class="subheader">
                        <td>
                            <strong><span class="caption" width="200px">Order Confirmations:</span></strong>
                        </td>
                        <td>
                            <asp:Label id="lblConfirmations" runat="server" width="100%" style="font-weight: normal;"/>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <strong><span class="caption" width="200px">Cancellations:</span></strong>
                        </td>
                        <td>
                            <asp:Label id="lblCancellations" runat="server" width="100%" style="font-weight: normal;"/>
                        </td>
                    </tr>
                </table>
                </div>
            </asp:WizardStep>
        </WizardSteps>
    </asp:Wizard>
    <div id="errorMsg" style="max-width:500px;">
        <asp:PlaceHolder runat="server" ID="phError">    
            <span class='small-alert'>Can't save Campaign.</span>
        </asp:PlaceHolder>
        <asp:PlaceHolder runat="server" ID="phSuccess">
            <span>Campaign was successfully saved.</span>
        </asp:PlaceHolder>        
    </div>
</div>
</form>
