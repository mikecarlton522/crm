<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProductEvent.aspx.cs" Inherits="TrimFuel.Web.Admin.AjaxControls.ProductEvent" %>

<form id="productEventForm" runat="server">
<asp:hiddenfield id="hdnProductEventID" value='<%# ProductEventID %>' runat="server" />
<asp:hiddenfield id="hdnProductID" value='<%# ProductID %>' runat="server" />
<table width="100%">
    <tr valign="top">
        <td>
            <span>Product</span>
        </td>
        <td>
            <asp:label id="lblProduct" text='<%# ProductName %>' runat="server" />
        </td>
    </tr>
    <tr>
        <td>
            <span>Event Type</span>
        </td>
        <td>
            <asp:dropdownlist id="dpEventTypes" runat="server" datasource='<%# EventTypes %>'
                datatextfield="Key" datavaluefield="Value" selectedvalue='<%# ProductEventProp.EventTypeID.ToString() %>' />
        </td>
    </tr>
    <tr>
        <td>
            <span>URL</span>
        </td>
        <td>
            <asp:textbox id="tbURL" runat="server" class="xxwide" text='<%# ProductEventProp.URl %>' />
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <a href="javascript:void(0)" onclick="jQuery('#url-replacement-variables').slideToggle();"
                class="infoIcon">Use the following variables for URL</a>
            <div class="space">
            </div>
            <div id="url-replacement-variables" style="display: none;">
                <ul class="big-item-list2" style="max-width: 250px; float: left;">
                    <li><span><b>#EMAIL#</b> - Email</span></li>
                    <li><span><b>#ZIP#</b> - Zip code</span></li>
                    <li><span><b>#FIRSTNAME#</b> - Frist Name</span></li>
                    <li><span><b>#LASTNAME#</b> - Last Name</span></li>
                    <li><span><b>#ADDRESS#</b> - Address</span></li>
                    <li><span><b>#CITY#</b> - City</span></li>
                    <li><span><b>#STATE#</b> - State</span></li>
                    <li><span><b>#TELEPHONE#</b> - Telephone</span></li>                    
                    <li><span><b>#PURCHASE_DATE#</b> - Purchase date</span></li>
                    <li><span><b>#BILLING_ID#</b> - Billing ID</span></li>
                    <li><span><b>#AFFILIATE#</b> - Affiliate</span></li>
                    <li><span><b>#SUBAFFILIATE#</b> - Subaffiliate</span></li>
                    <li><span><b>#IP#</b> - IP</span></li>
                </ul>
            </div>
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <div align="right">
                <asp:button text="Save Changes" runat="server" id="assertSave" onclick="SaveChanges_Click" />
            </div>
        </td>
    </tr>
</table>
<div id="errorMsg">
    <asp:literal runat="server" id="Note"></asp:literal>
</div>
</form>
