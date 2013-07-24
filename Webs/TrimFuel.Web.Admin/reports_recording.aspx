<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="reports_recording.aspx.cs"
    MasterPageFile="~/Controls/Admin.Master" Inherits="TrimFuel.Web.Admin.reports_recording" %>

<%@ Register Src="Controls/Paging.ascx" TagName="Paging" TagPrefix="uc1" %>
<%@ Register Src="Controls/DateFilter.ascx" TagName="DateFilter" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphScript" runat="server">
    <script type="text/javascript">
        function playCall(callID, contactID, el) {
            popupControl2("popup-call-" + callID, "Call Recording", 320, 100, "/dotNet/AjaxControls/CallPlayer.aspx?contactID=" + contactID + "&callID=" + callID, null, null,
            function () {
                destroyPlayer(callID)
            });
        }

        function destroyPlayer(callID) {
            $("#mPlayer-" + callID).stop();
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphStyle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <form runat="server" id="frmMain">
    <div id="toggle" class="section">
        <table border="0" cellspacing="0" cellpadding="3" class="editForm" style="float: right;
            border: 0;">
            <tbody>
                <tr>
                    <td>
                        Records on page
                    </td>
                    <td>
                        <select name="ddlPageRecords" id="ddlPageRecords" runat="server">
                            <option value="50">50</option>
                            <option value="100">100</option>
                            <option value="200" selected="">200</option>
                            <option value="0">All</option>
                        </select>
                    </td>
                </tr>
            </tbody>
        </table>
        <a href="#">
            <h1>
                Quick View</h1>
        </a>
        <uc1:DateFilter ID="DateFilter1" runat="server" />
    </div>
    <div id="buttons">
        <asp:Button runat="server" ID="btnGo" Text="Generate Report" OnClick="btnGo_Click" />
    </div>
    <div id="load">
        <img src="../images/loading2.gif" style="display: none;" alt="Please wait" />
    </div>
    <div class="data" style="display: none" id="data" runat="server">
        <%--        <div id="paging" runat="server">
            <script type="text/javascript" language="javascript">
                function beforeChangePage(el) {
                    debugger;
                    $('#hdnCurrentPage').val($(el).html());
                }
            </script>
            <h1>
                Records Found:
                <%#TotalNumberOfRecords%></h1>
            <input type="hidden" name="hdnCurrentPage" id="hdnCurrentPage" value="<%# CurrentPage %>" />
            <div style="text-align: center;">
                <asp:Repeater runat="server" ID="rPaging" DataSource="<%# Pages %>">
                    <ItemTemplate>
                        <asp:LinkButton runat="server" OnClick="btnGoToPage_Click" OnClientClick="beforeChangePage(this);"
                            Text='<%# Eval("Key") %>' Enabled='<%# Eval("Value") %>' CssClass="submit" />
                    </ItemTemplate>
                </asp:Repeater>
            </div>
            <div class="clear">
            </div>
    </div>--%>
        <uc1:Paging ID="Paging" runat="server" CountOnPage="<%# CountOnPage%>" OnGoToPageClicked="btnGoToPage_Click"
            TotalNumberOfRecords="<%#TotalNumberOfRecords %>" />
        <asp:Repeater runat="server" ID="rRecording">
            <HeaderTemplate>
                <table class="process-offets sortable add-csv-export" cellspacing="1" cellpadding="0"
                    border="0" style="width: 100%;">
                    <tr class="header">
                        <td>
                            #
                        </td>
                        <td>
                            Billing ID
                        </td>
                        <td>
                            First Name
                        </td>
                        <td>
                            Last Name
                        </td>
                        <td>
                            Phone
                        </td>
                        <td>
                            Last Call Date
                        </td>
                        <td>
                            # Of Calls
                        </td>
                        <td>
                            Last Product
                        </td>
                        <td class="sorttable_nosort">
                        </td>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td>
                        <%#Eval("RowNumber")%>
                    </td>
                    <td>
                        <a href='../billing_edit.asp?id=<%#Eval("BillingID")%>' target="_blank">
                            <%#Eval("BillingID")%></a>
                    </td>
                    <td>
                        <%#Eval("FirstName")%>
                    </td>
                    <td>
                        <%#Eval("LastName")%>
                    </td>
                    <td>
                        <%#Eval("Phone")%>
                    </td>
                    <td>
                        <%#Eval("CallDate")%>
                    </td>
                    <td>
                        <%#Eval("NumberOfCalls")%>
                    </td>
                    <td>
                        <%#Eval("CustomerProduct")%>
                    </td>
                    <td>
                        <a name="play" href="javascript:void(0)" onclick="playCall(<%#Eval("CallID")%>, '<%#Eval("ExternalCallID")%>', this)"
                            class="playIcon">Play</a>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </table>
            </FooterTemplate>
        </asp:Repeater>
    </div>
    </form>
</asp:Content>
