<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UnsentShipmentsGroup.aspx.cs"
    Inherits="TrimFuel.Web.Admin.services.UnsentShipmentsGroup" %>

<asp:repeater id="rShipments" runat="server">
    <ItemTemplate>
        <tr class="detail level1 shipper-<%# ShipperID %>" style="display:table-row;" detail-id="shipper-<%# ShipperID %>">
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
                <%# Eval("ID")%>
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
                <span title="<%#Eval("Reason")%>"> <%#Eval("ShortReason")%></span>
            </td>
            <td>
                <span id="lblSKU" class="viewMode" ><%#Eval("SKU")%></span>
            </td>
            <td>
                <input id="hdnSaleID" name="hdnSaleID" type="hidden" value='<%#Eval("ID") %>' />
                <a href="javascript:void();" onclick="editClick(this); return false;" class="editIcon viewMode">Edit</a>
                <a href="javascript:void();" style="display:none;" onclick="cancelEditClick(this); return false;" class="cancelIcon editMode">Cancel</a>
                <a href="javascript:void();" id="lbSave" class="submit saveIcon editMode" style="display:none;" onclick="saveClick(); return false;" >Save</a>
            </td>
        </tr>
    </ItemTemplate>
</asp:repeater>
