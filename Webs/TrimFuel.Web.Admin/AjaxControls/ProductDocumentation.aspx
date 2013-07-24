<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProductDocumentation.aspx.cs"
    Inherits="TrimFuel.Web.Admin.AjaxControls.ProductDocumentation" %>

<script type="text/javascript">
    $(document).ready(function() {
        $(".editMode").hide();
    })
    function editClick(el) {
        var tr = $(el).closest('tr');
        $(tr).find('.editMode').show();
        $(tr).find('.viewMode').hide();
    }
    function cancelEditClick(el) {
        var tr = $(el).closest('tr');
        $(tr).find('.editMode').hide();
        $(tr).find('.viewMode').show();
    }
</script>

<form id="productEventForm" runat="server" style="height: 20px;">
<asp:hiddenfield id="hdnProductID" value='<%# ProductID %>' runat="server" />
<table width="600px">
    <tr class="header">
        <td style="width: 450px;">
            <strong>Documentation Link</strong>
        </td>
        <td>
            <strong>Actions</strong>
        </td>
    </tr>
    <tr valign="top">
        <td>
            <asp:label id="lblDocLink" text='<%# DocLink %>' runat="server" cssclass='viewMode' />
            <input id="txtDocLink" name="txtDocLink" value='<%# DocLink %>' class='editMode' style="width:350px;" />
        </td>
        <td>
            <%
                if (string.IsNullOrEmpty(DocLink))
                {
            %>
            <a href="javascript:void(0);" id="lbEdit" onclick="editClick(this)" class="addNewIcon viewMode">
                Add</a>
            <%
                }
                else
                {
            %>
            <a href="javascript:void(0);" id="A1" onclick="editClick(this)" class="editIcon viewMode">
                Edit</a>&nbsp;
            <asp:linkbutton runat="server" id="lbDelete" text="Delete" cssclass="confirm removeIcon viewMode"
                onclick="doc_Delete"></asp:linkbutton>
            <%
                }
            %>
            <asp:linkbutton runat="server" id="lbSave" text="Save" cssclass="saveIcon editMode"
                onclick="doc_Save" style="display: none;"></asp:linkbutton>
            &nbsp;<a href="javascript:void(0);" id="lbCancel" onclick="cancelEditClick(this);"
                class="cancelIcon editMode" style="display: none;">Cancel</a>
        </td>
    </tr>
</table>
<div class="space">
</div>
<div id="errorMsg">
    <asp:literal runat="server" id="Note"></asp:literal>
</div>
</form>
