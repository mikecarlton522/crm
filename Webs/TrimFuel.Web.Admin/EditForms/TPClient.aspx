<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TPClient.aspx.cs" Inherits="TrimFuel.Web.Admin.EditForms.TPClient_" %>

<input type="hidden" name="action" value="process" />
<div class="module" style="width: 95%">
    <h2>
        Edit Client</h2>
    <table class="editForm" border="0" cellspacing="1" cellpadding="0">
        <tr class="header">
            <td>
                Name
            </td>
            <td>
                <%= TPClient.Name%>
            </td>
        </tr>
        <tr>
            <td>
                Triangle Fulfillment
            </td>
            <td>
                <input type="checkbox" name="triangleFulfillment" id="triangleFulfillment" <%= TPClient.TriangleFulfillment == true ? "checked='checked'" : "" %> />
            </td>
        </tr>
        <tr>
            <td>
                TriangleCRM
            </td>
            <td>
                <input type="checkbox" name="triangleCRM" id="triangleCRM" <%= TPClient.TriangleCRM == true ? "checked='checked'" : "" %> />
            </td>
        </tr>
        <tr>
            <td>
                Integrated Telephony Services
            </td>
            <td>
                <input type="checkbox" name="telephonyServices" id="telephonyServices" <%= TPClient.TelephonyServices == true ? "checked='checked'" : "" %> />
            </td>
        </tr>
        <tr>
            <td>
                Call Center Services
            </td>
            <td>
                <input type="checkbox" name="callCenterServices" id="callCenterServices" <%= TPClient.CallCenterServices == true ? "checked='checked'" : "" %> />
            </td>
        </tr>
        <tr>
            <td>
                Technology Services
            </td>
            <td>
                <input type="checkbox" name="technologyServices" id="technologyServices" <%= TPClient.TechnologyServices == true ? "checked='checked'" : "" %> />
            </td>
        </tr>
        <tr>
            <td>
                Media & Design Services
            </td>
            <td>
                <input type="checkbox" name="mediaServices" id="mediaServices" <%= TPClient.MediaServices == true ? "checked='checked'" : "" %> />
            </td>
        </tr>
    </table>
</div>