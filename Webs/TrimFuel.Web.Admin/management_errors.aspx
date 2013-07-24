<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/Admin.Master" AutoEventWireup="true" CodeBehind="management_errors.aspx.cs"
    Inherits="TrimFuel.Web.Admin.management_errors" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphScript" runat="server">
<script type="text/javascript" language="javascript">
    function viewErrorDetails(Id) {
        popupControl2("error-details" + Id, "Error Details", 640, 520, "editForms/ErrorsLog.aspx?id=" + Id);
    }

    function groupDelete(identifier, type) {
        $.ajax({
            type: "POST",
            url: "management_errors.aspx/GroupDelete",
            data: "{identifier : " + identifier + ", type: " + type + "}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                window.location = 'management_errors.aspx';
            }
        });
    }

    function singleDelete(id) {
        $.ajax({
            type: "POST",
            url: "management_errors.aspx/SingleDelete",
            data: "{id : " + id + "}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                window.location = 'management_errors.aspx';
            }
        });
    }

    $(document).ready(function () {
        
        initMultilevelTables();
    });


    function initMultilevelTables() {
        $("table.multilevel").each(function () {
            var table = this;
            $(table).find("tr.master").click(function () {
                multilevelTableToggleDetail(table, this);
            });
        });
    }

    function multilevelTableToggleDetail(tableEl, masterEl) {
        var masterId = $(masterEl).attr("master-id");
        //alert(masterId);
        //Recursive hide children
        $(tableEl).find("tr.detail[detail-id=" + masterId + "]").filter(".master").filter(":visible").each(function () {
            multilevelTableHideDetail(tableEl, this)
        });
        $(tableEl).find("tr.detail[detail-id=" + masterId + "]").toggle();
    }

    function multilevelTableHideDetail(tableEl, masterEl) {
        var masterId = $(masterEl).attr("master-id");
        //alert(masterId);
        //Recursive hide children
        $(tableEl).find("tr.detail[detail-id=" + masterId + "]").filter(".master").filter(":visible").each(function () {
            multilevelTableHideDetail(tableEl, this)
        });
        $(tableEl).find("tr.detail[detail-id=" + masterId + "]").hide();
    }


    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphStyle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <form runat="server">
    <div class="module">
        <asp:Button OnCommand="DoErrorGroupAction" CommandName="application" ID="btApplication" runat="server" Text="Group By Application" />
        <asp:Button OnCommand="DoErrorGroupAction" CommandName="applicationID" runat="server" Text="Group By ApplicationID" />
        <asp:Button OnCommand="DoErrorGroupAction" CommandName="briefError" ID="btBriefError" runat="server" Text="Group By Brief Error Text" />
        <asp:Button OnCommand="DoErrorGroupAction" CommandName="className" ID="btClassName" runat="server" Text="Group By Class Name" />
    </div>
    <div class="clear">
    </div>
    <div class="data">
        <table class="process-offets multilevel sortable add-csv-export" border="0" cellspacing="1" width="100%">
            <tr class="header">
                <td>
                    Error ID
                </td>
                <td>
                    Date
                </td>
                <td>
                    Application
                </td>
                <td>
                    ApplicationID
                </td>
                <td>
                    Class Name
                </td>
                <td>
                    Brief Error Text
                </td>
                <td>
                    Details
                </td>
                <td>
                    Resolve
                </td>
            </tr>
            <asp:Literal ID="ltOutput" runat="server" />
        </table>
    </div>
    </form>
</asp:Content>
