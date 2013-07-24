<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QueuedTransactions.aspx.cs"
    Inherits="TrimFuel.Web.Admin.AjaxControls.QueuedTransactions" %>

<%@ Register Assembly="TrimFuel.Web.UI" Namespace="TrimFuel.Web.UI.Specialized" TagPrefix="cc1" %>
<script type="text/javascript">
    $(document).ready(function () {
        $('table.sortable').each(function () {
            sorttable.makeSortable(this);
        });
        initMultilevelTablesTransactions();
        $('#ddlMIDAction').val(0);
        ChangeMIDAction($('#ddlMIDAction'));
        SetToggles($("#transactions-content"));
    });

    function TransactionsActionApply() {
        $('#processingMIDs').show();
        $('.errorMsgWithoutLoading').hide();
    }
</script>
<form id="Form1" runat="server">
<asp:placeholder id="phResults2" runat="server">
    <div class="data-loading" id="processingMIDs" style="display:none;">
		Processing...<br>
		<img src="/images/loading.gif">
	</div>
    <asp:PlaceHolder runat="server" ID="phMessage2" Visible="false">
        <div class="errorMsgWithoutLoading" style="text-align: center !important;">
            The selected transactions were resent for processing
        </div>
    </asp:PlaceHolder>
     <asp:PlaceHolder runat="server" ID="phMessage3" Visible="false">
        <div class="errorMsgWithoutLoading" style="text-align: center !important;">
            The selected items have been removed from the list
        </div>
    </asp:PlaceHolder>
    <div class="data" style="display: block; padding-top:10px;" id="transactions-content">
        <div id="toggle" class="section">
	    <a href="#"><h1 style="text-align:left !important;">Group Action</h1> </a>
        <div class="module">
            <div>
                <table style="border:0;">
                    <tr>
                        <td colspan="2">
                            <strong style="text-decoration:underline;">Applies to all checked records:</strong>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Select action:
                        </td>
                        <td>
                            <select id="ddlMIDAction" onchange="ChangeMIDAction(this); return false;"
                                runat="server" Style="width: 200px;">
                                <option value="0" selected>Select Action</option>
                                <option value="1">Re-Attempt to the same MID</option>
                                <option value="2">Re-Attempt to a different MID</option>
                                <option value="3">Remove from list</option>
                            </select>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            MID:
                        </td>
                        <td>
                        <cc1:AssertigyMidDDL runat="server" ID="AssertigyMidDDL" Style="width: 200px;"/>
                        </td>
                    </tr>
                </table>
            </div>
            <div id="sameMID" style="display: none; float: right;">
                <asp:Button runat="server" ID="sendBills" Text="Apply Action" OnClick="btnResendBills_Click" OnClientClick="TransactionsActionApply();"/>
            </div>
            <div id="diffMID" style="display: none; float: right;">
                <asp:Button runat="server" ID="sendBillsToDifferentMID" Text="Apply Action" OnClick="btnSendBillsToDifferentMID_Click"
                    OnClientClick="TransactionsActionApply();" />
            </div>
            <div id="ignore" style="display: none; float: right;">
                <asp:Button runat="server" ID="addToIgnoreList" Text="Apply Action" OnClick="btnRemoveFromList_Click"
                    OnClientClick="TransactionsActionApply();" />
            </div>
        </div>
        <div class="clear">
        </div>
        <asp:Repeater ID="rBills" runat="server">
            <HeaderTemplate>
                <table class="process-offets sortable multilevel add-csv-export" cellspacing="1"
                    cellpadding="0" border="0" style="width: 100%;">
                    <thead>
                        <tr class="header">
                            <td>
                            </td>
                            <td class="sorttable_nosort" style="text-align: center;">
                                <input type="checkbox" id="selectAllBills" name="selectAllBills" onchange="selectBills(); return false;" />
                            </td>
                            <td>
                                BillingID
                            </td>
                            <td>
                                Sale Type
                            </td>
                            <td>
                                First Name
                            </td>
                            <td>
                                Last Name
                            </td>
                            <td style="width: 65px;">
                                Date
                            </td>
                            <td>
                                Amount
                            </td>
                            <td>
                                Reason
                            </td>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr class="subheader master level0" master-id="mid-<%# Eval("Key") %>">
                    <td style="width: 180px;">
                        <img id="img-mid-<%# Eval("Key") %>-Wrap" src="/images/icons/bullet_arrow_down.png" />
                        <img id="img-mid-<%# Eval("Key") %>-Unwrap" src="/images/icons/bullet_arrow_up.png"
                            style="display: none;" />
                        <strong>
                            <%# GetMIDNameById(Eval("Key").ToString())%></strong>
                    </td>
                    <td style="width: 30px;">
                    </td>
                    <td style="width: 70px;">
                    </td>
                    <td style="width: 70px;">
                    </td>
                    <td style="width: 100px;">
                    </td>
                    <td style="width: 100px;">
                    </td>
                    <td style="width: 70px;">
                        <span style="display: none;">
                            <%#DateTime.Now.ToShortDateString()%></span>
                    </td>
                    <td style="width: 70px;">
                    </td>
                    <td>
                    </td>
                </tr>
               <%-- <asp:Repeater ID="rBills" runat="server" DataSource='<%# Eval("Value") %>'>
                    <ItemTemplate>
                        <tr class="detail level1 mid-<%# DataBinder.Eval(Container.Parent.Parent, "DataItem.Key") %>" detail-id="mid-<%# DataBinder.Eval(Container.Parent.Parent, "DataItem.Key") %>">
                            <td>
                            </td>
                            <td class="sorttable_nosort" style="text-align: center;">
                                <input type="checkbox" id='<%#Eval("BillType").ToString() == "Rebill"
                                ? "rebillToSend" : "billToSend" %>' name='<%#Eval("BillType").ToString() == "Rebill"
                                    ? "rebillToSend" : "billToSend" %>' value='<%#Eval("ID")%>' />
                            </td>
                            <td>
                                <a href='https://<%# TrimFuel.Business.Config.Current.APPLICATION_ID %>/billing_edit.asp?id=<%#Eval("BillingID")%>'
                                    target="_blank">
                                    <%#Eval("BillingID")%></a>
                            </td>
                            <td>
                                <%#Eval("BillType")%>
                            </td>
                            <td>
                                <%#Eval("FirstName")%>
                            </td>
                            <td>
                                <%#Eval("LastName")%>
                            </td>
                            <td>
                                <%#Convert.ToDateTime(Eval("CreateDT")).ToString("MM/dd/yyyy")%>
                            </td>
                            <td>
                                <%#String.Format("${0:f2}", Eval("Amount"))%>
                            </td>
                            <td>
                                <span title="<%#Eval("Reason")%>">
                                    <%#Eval("ShortReason")%></span>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>--%>
            </ItemTemplate>
            <FooterTemplate>
                </tbody> 
                </table>
            </FooterTemplate>
        </asp:Repeater>
    </div>
    </div>
</asp:placeholder>
<div class="clear">
</div>
</form>
