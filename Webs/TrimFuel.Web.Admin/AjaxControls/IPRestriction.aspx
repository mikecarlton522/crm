<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="IPRestriction.aspx.cs"
    Inherits="TrimFuel.Web.Admin.AjaxControls.IPRestriction" %>

<script type="text/javascript">
    $(document).ready(function () {
        AllowAllClick();
        $(".editMode").hide();
    });

    function AllowAllClick(item) {
        if ($("#chbAllowAll").attr("checked") == true) {
            $('#existingIPs').hide();
        }
        else {
            $('#existingIPs').show();
        }
    }

    function editClick(el) {
        cancelEditClick();
        var tr = $(el).closest('tr');
        $(tr).find('.viewMode').hide();
        $(tr).find('.editMode').show();
        $(tr).find('#hdnIPRestrictionID').attr("name", "lblIPRestrictionID").attr("id", "lblIPRestrictionID");
        var ip = $(tr).find('#lblIP').html();
        var tdArray = $(tr).find('td');
        var templateObj = $('#template');
        templateObj.find('#teditTbIP').clone().attr("name", "editTbIP").attr("id", "editTbIP").appendTo(tdArray[0]).val(ip);
    }
    function cancelEditClick() {
        $('.viewMode').show();
        $('.editMode').hide();
        $('#editTbIP').remove();
        $('#lblIPRestrictionID').attr("name", "hdnIPRestrictionID").attr("id", "hdnIPRestrictionID");
    }
</script>
<div style="max-height: 600px; overflow-y: auto">
    <form runat="server">
    <div id="template" style="display: none;">
        <asp:textbox id="teditTbIP" clientidmode="static" runat="server" />
    </div>
    <table>
        <tr>
            <td>
                Allow All IPs
            </td>
            <td>
                <asp:checkbox id="chbAllowAll" clientidmode="Static" runat="server" onclick="AllowAllClick();" />
            </td>
            <td>
                <asp:button text="Save" runat="server" id="assertSave" onclick="SaveAllowChanges_Click" />
            </td>
        </tr>
    </table>
    <div id="existingIPs">
        <div class="space">
        </div>
        <table>
            <tr>
                <td colspan="2">
                    <asp:textbox id="txtNewIP" runat="server" style="width: 280px;" />
                </td>
                <td>
                    <asp:button text="Add" runat="server" id="assertAdd" onclick="AddNew_Click" />
                </td>
            </tr>
        </table>
        <div class="space">
        </div>
        <table class="process-offets" border="0" cellspacing="1" style="width: 360px;" >
            <tr class="header">
                <td>
                    Allowed IP Address
                </td>
                <td>
                    Actions
                </td>
            </tr>
            <asp:repeater id="rIPList" runat="server" onitemcommand="rIPList_ItemCommand">
        <ItemTemplate>
            <tr>
                <td>
                    <input id="hdnIPRestrictionID" name="hdnIPRestrictionID" type="hidden" value='<%#Eval("IPRestrictionID") %>' />
                    <span id="lblIP" class="viewMode" ><%# Eval("IP") %></span>
                </td>
                <td>
                    <a href="javascript:void(0);" ID="lbEdit" onclick="editClick(this)" class="editIcon viewMode" >Edit</a>
                    <asp:LinkButton runat="server" ID="lbHideShow" Text='Remove' CssClass="confirm viewMode removeIcon" CommandName='remove' CommandArgument='<%#Eval("IPRestrictionID") %>' ></asp:LinkButton>
                    <asp:LinkButton runat="server" ID="lbSave" Text="Save" CssClass="submit saveIcon editMode" style="display:none;" OnClick="btnEditIP_Click" ></asp:LinkButton>
                    <a href="javascript:void(0);" ID="lbCancel" onclick="cancelEditClick(this);" class="cancelIcon editMode" style="display:none;">Cancel</a>
                </td>
            </tr>
        </ItemTemplate>
    </asp:repeater>
        </table>
    </div>
    <div class="space">
    </div>
    <div id="errorMsg">
        <asp:literal runat="server" id="Note"></asp:literal>
    </div>
    </form>
</div>
