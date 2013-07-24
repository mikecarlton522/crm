<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProductEventsManager.aspx.cs"
    Inherits="TrimFuel.Web.Admin.AjaxControls.ProductEventsManager" %>

<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>

<script type="text/javascript">
    function addEvent(productID) {
        popupControl2("product-event-add", "Add product event", 500, 300, "AjaxControls/ProductEvent.aspx?productID=" + productID, null, null,
        function() {
            $("#right").html("<img src='../images/loading2.gif' />");
            var url = "ajaxControls/ProductEventsManager.aspx?productId=" + productID;
            inlineControl(url, "right", null,
            function() {
                $("#right").prepend("<h2><%# ProductName %> Events</h2>");
            })
        });
    }
    function editEvent(productID, productEventID) {
        popupControl2("product-event-add", "Add product event", 500, 300,
        "AjaxControls/ProductEvent.aspx?productID=" + productID + "&productEventID=" + productEventID, null, null,
        function() {
            $("#right").html("<img src='../images/loading2.gif' />");
            var url = "ajaxControls/ProductEventsManager.aspx?productId=" + productID;
            inlineControl(url, "right", null,
            function() {
                $("#right").prepend("<h2><%# ProductName %> Events</h2>");
            })
        });
    }
</script>

<form id="formProductEvents" runat="server">
<asp:hiddenfield id="hdnProductID" value='<%# ProductID %>' runat="server" />
<a href="javascript:void(0)" style="font-size: 12px;" onclick="addEvent(<%# ProductID %>)"
    class="addNewIcon">Add</a>
<div style="height: 10px;">
</div>
<table cellspacing="1" class="process-offets sortable" style="margin-left: 0;">
    <tr class="header">
        <td>
            <strong>Event Type</strong>
        </td>
        <td>
            <strong>URL</strong>
        </td>
        <td>
            <strong>Actions</strong>
        </td>
    </tr>
    <asp:repeater id="rProductEvents" runat="server" onitemcommand="rProductEvents_ItemCommand"
        onitemdatabound="rProductEvents_ItemDataBound" datasource="<%# ProductEvents %>">
        <ItemTemplate>
            <tr>
                <td>
                    <asp:Label id="lblEventType" runat="server" Text='<%# Convert.ToInt32(Eval("EventTypeID"))==1 ? "Registration" : "Order Confirmation" %>' />
                </td>
                <td>
                    <asp:Label id="lblURL" runat="server" Text='<%# Eval("URl") %>' />
                </td>
                <td>
                    <a href="javascript:void(0);" ID="lbEdit" onclick='editEvent(<%# ProductID %>, <%# Eval("ProductEventID")%>)' class="editIcon" >Edit</a>
                    <asp:LinkButton runat="server" ID="lbDelete" Text="Delete" CssClass="confirm removeIcon" CommandName="delete" CommandArgument='<%# Eval("ProductEventID") %>'></asp:LinkButton>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            <tr>
                <td colspan="3">
                    <asp:Label ID="lblEmptyData" Text="No Events Set" runat="server" Visible="false"/>
                </td>
            </tr>
        </FooterTemplate>
    </asp:repeater>
</table>
<div class="space">
</div>
<div id="errorMsg">
    <asp:literal runat="server" id="Note"></asp:literal>
</div>
</form>
