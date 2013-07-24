<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UnsentShipments.aspx.cs"
    Inherits="TrimFuel.Web.Admin.AjaxControls.UnsentShipments" %>

<%@ Register Assembly="TrimFuel.Web.UI" Namespace="TrimFuel.Web.UI.Specialized" TagPrefix="cc1" %>
<script type="text/javascript">
    $(document).ready(function () {
        initMultilevelTablesShipments();
        $('#ddlShipperAction').val(0);
        ChangeShipperAction($('#ddlShipperAction'));
        SetToggles($("#shippments-content"));
        $(".editMode").hide();
    });

    function ShipmentsActionApply() {
        $('#processingShippments').show();
        $('.errorMsgWithoutLoading').hide();
    }


    function editClick(el) {
        cancelEditClick();
        var tr = $(el).closest('tr');
        $(tr).find('.viewMode').hide();
        $(tr).find('.editMode').show();
        var saleID = $(tr).find('#hdnSaleID').val();
        var sku = $(tr).find('#lblSKU').html();
        $(tr).find('#hdnSaleID').attr("id", "editSaleID");
        $(tr).find('#lblSKU').attr("id", "editlblSKU");
        var tdArray = $(tr).find('td');
        var templateObj = $('#template');
        templateObj.find('#teditProductSKUDDL').clone().attr("id", "editProductSKUDDL").addClass("validate[required]").appendTo(tdArray[8]).val(sku);
    }

    function cancelEditClick() {
        $('.viewMode').show();
        $('.editMode').hide();
        $('#editProductSKUDDL').remove();
        $('#editSaleID').attr("name", "hdnSaleID").attr("id", "hdnSaleID");
        $('#editlblSKU').attr("id", "lblSKU");
    }

    function saveClick() {
        //saving using ajax
        $.ajax({
            type: 'POST',
            url: "/dotNet/services/UnsentShipmentsGroup.aspx",
            data: "action=save&editProductSKUDDL=" + $("#editProductSKUDDL").val() + "&editSaleID=" + $("#editSaleID").val(),
            success: function (data) {
                $("#editlblSKU").html(data);
            },
            error: function (data) {
                alert(data);
            },
            complete: function () {
                $('.viewMode').show();
                $('.editMode').hide();
                $('#editlblSKU').attr("id", "lblSKU");
                $('#editProductSKUDDL').remove();
                $('#editSaleID').attr("name", "hdnSaleID").attr("id", "hdnSaleID");
            }
        });
    }
</script>
<form runat="server">
<div style="display: none;" id="template">
    <cc1:ProductCodeDDL runat="server" ID="teditProductSKUDDL" />
</div>
<asp:placeholder id="phResults1" runat="server">
    <div class="data-loading" id="processingShippments" style="display:none;">
		Processing...<br>
		<img src="/images/loading.gif">
	</div>
    <asp:PlaceHolder runat="server" ID="phMessage1" Visible="false">
        <div class="errorMsgWithoutLoading" style="text-align: center !important;">
            The shipment(s) were successfully resent to the selected shipper
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" ID="phMessage2" Visible="false">
        <div class="errorMsgWithoutLoading" style="text-align: center !important;">
            The shipment(s) were reattempted, Please review the table below for any errors
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" ID="phMessage3" Visible="false">
        <div class="errorMsgWithoutLoading" style="text-align: center !important;">
            The selected items have been removed from the list
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" ID="phMessage4" Visible="false">
        <div class="errorMsgWithoutLoading" style="text-align: center !important;">
            There was an error while trying to remove the selected item from the list - Please try again later or contact the tech department
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" ID="phMessage5" Visible="false">
        <div class="errorMsgWithoutLoading" style="text-align: center !important;">
            The selected items have been removed from the list and are marked as sent
        </div>
    </asp:PlaceHolder>
    <div class="data" style="display: block; padding-top:10px;" id="shippments-content">
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
                            <select id="ddlShipperAction" onchange="ChangeShipperAction(this); return false;"
                                runat="server" Style="width: 200px;">
                                <option value="0" selected>Select Action</option>
                                <option value="1">Send to the same shipper</option>
                                <option value="2">Send to a different shipper</option>
                                <option value="3">Remove from list</option>
                                <option value="4">Mark as sent</option>
                            </select>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Shipper:
                        </td>
                        <td>
                            <cc1:ShipperDDL runat="server" ID="ShipperDDL" Style="width: 200px;" />
                        </td>
                    </tr>
                </table>
            </div>
            <div id="sameShipper" style="display: none; float: right;">
                <asp:Button runat="server" ID="sendShippments" Text="Apply Action" OnClick="btnResendShippments_Click"
                   OnClientClick="ShipmentsActionApply();" />
            </div>
            <div id="diffShipper" style="display: none; float: right;">
                <asp:Button runat="server" ID="sendShippmentsToDifferentShipper" Text="Apply Action"
                    OnClick="btnSendShippmentsToDifferentShipper_Click" OnClientClick="ShipmentsActionApply();" />
            </div>
            <div id="asSent" style="display: none; float: right;">
                <asp:Button runat="server" ID="markAsSent" Text="Apply Action"
                    OnClick="btnMarkAsSent_Click" OnClientClick="ShipmentsActionApply();" />
            </div>
            <div id="asSentWithNote" style="display: none; float: right;">
                <asp:Button runat="server" ID="markAsSentWithNote" Text="Apply Action"
                    OnClick="btnMarkAsSentWithNote_Click" OnClientClick="ShipmentsActionApply();" />
            </div>
        </div>
        <div class="clear">
        </div>
        <asp:Repeater ID="rShipers" runat="server">
            <HeaderTemplate>
                <table class="process-offets sortable multilevel add-csv-export" cellspacing="1"
                    cellpadding="0" border="0" style="width: 100%;">
                    <thead>
                        <tr class="header">
                            <th>
                            </th>
                            <th class="sorttable_nosort" style="text-align: center;">
                                <input type="checkbox" id="selectAll" name="selectAll" onchange="selectShippments(); return false;" />
                            </th>
                            <th>
                                BillingID
                            </th>
                            <th>
                                SaleID
                            </th>
                            <th>
                                Date
                            </th>
                            <th>
                                First Name
                            </th>
                            <th>
                                Last Name
                            </th>
                            <th>
                                Reason
                            </th>
                            <th>
                                SKU
                            </th>
                            <th class="sorttable_nosort">
                                Actions
                            </th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr class="subheader master level0" master-id="shipper-<%# Eval("Key") %>">
                    <td style="width: 180px;">
                        <img id="img-shipper-<%# Eval("Key") %>-Wrap" src="/images/icons/bullet_arrow_down.png" />
                        <img id="img-shipper-<%# Eval("Key") %>-Unwrap" src="/images/icons/bullet_arrow_up.png"
                            style="display: none;" />
                        <strong>
                            <%# GetShipperNameById(Eval("Key").ToString())%></strong>
                    </td>
                    <td style="width: 30px;">
                    </td>
                    <td style="width: 45px;">
                    </td>
                    <td style="width: 45px;">
                    </td>
                    <td style="width: 65px;">
                        <span style="display: none;">
                            <%#DateTime.Now.ToShortDateString()%></span>
                    </td>
                    <td style="width: 80px;">
                    </td>
                    <td style="width: 80px;">
                    </td>
                    <td>
                    </td>
                    <td style="width: 300px;">
                    </td>
                    <td style="width: 110px;">
                    </td>
                </tr>
                <%--<asp:Repeater ID="rShipments" runat="server" DataSource='<%# Eval("Value") %>'>
                    <ItemTemplate>
                        <tr class="detail level1 shipper-<%# DataBinder.Eval(Container.Parent.Parent, "DataItem.Key") %>" detail-id="shipper-<%# DataBinder.Eval(Container.Parent.Parent, "DataItem.Key") %>">
                            <td>
                            </td>
                            <td class="sorttable_nosort" style="text-align: center;">
                                <input type="checkbox" id="shippmentToSend" name="shippmentToSend" value='<%#Eval("ID")%>' />
                            </td>
                            <td>
                                <a href='https://<%# TrimFuel.Business.Config.Current.APPLICATION_ID %>/billing_edit.asp?id=<%#Eval("BillingID")%>'
                                    target="_blank">
                                    <%#Eval("BillingID")%></a>
                            </td>
                            <td>
                                <%#Eval("ID")%>
                            </td>
                            <td>
                                <%#Convert.ToDateTime(Eval("CreateDT")).ToString("MM/dd/yyyy")%>
                            </td>
                            <td>
                                <%#Eval("FirstName")%>
                            </td>
                            <td>
                                <%#Eval("LastName")%>
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
                </tbody> </table>
            </FooterTemplate>
        </asp:Repeater>
    </div>
    </div>
</asp:placeholder>
<div class="clear">
</div>
</form>
