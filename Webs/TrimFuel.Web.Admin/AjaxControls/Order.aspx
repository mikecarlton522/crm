<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Order.aspx.cs" Inherits="TrimFuel.Web.Admin.AjaxControls.Order" %>

<%@ Register Src="../Controls/Address.ascx" TagName="Address" TagPrefix="uc1" %>
<%@ Register Src="../Controls/DropDownUpsell.ascx" TagName="DropDownUpsell" TagPrefix="uc2" %>
<%@ Register Src="../Controls/CreditCard.ascx" TagName="CreditCard" TagPrefix="uc3" %>
<%@ Register Src="../Controls/SubscriptionControl/NewSubscriptionControl.ascx" TagName="Subscription"
    TagPrefix="uc4" %>
<%@ Register Assembly="TrimFuel.Web.UI" Namespace="TrimFuel.Web.UI" TagPrefix="cc1" %>
<%@ Register Assembly="TrimFuel.Web.UI" Namespace="TrimFuel.Web.UI.Specialized" TagPrefix="cc2" %>
<%@ Register Assembly="TrimFuel.Web.Admin" Namespace="TrimFuel.Web.Admin.Logic" TagPrefix="cc3" %>
<form id="form1" runat="server">
<script type="text/javascript">
    <asp:Literal runat="server" ID="litProductProductCodeList" />

    $(document).ready(function(){
        changeProductGroup();
    });

function changeProductCode(id, price)
{
    var selectedGroup = $('#<%#DropDownProduct.ClientID %>').val();
    var selectedCode, selectedPrice;
    selectedCode = $('#' + id).val();
    selectedPrice = $("#" + price);
    selectedPrice.val(0);
    for(var item in productGroupList)
    {
        if (selectedCode == productGroupList[item].ProductCode && selectedGroup == productGroupList[item].ProductID)
        {
            selectedPrice.val(productGroupList[item].Price);
            break;
        }
    }
}

function changeProductGroup()
{
    var selectedGroup = $('#<%#DropDownProduct.ClientID %>').val();
    $("#txtPriceUpsell1").val('');
    $("#txtPriceUpsell2").val('');
    $("#txtPriceUpsell3").val('');

    $('#ddlUpsell1').html("<option value=''>-- Select --</option>");
    $('#ddlUpsell2').html("<option value=''>-- Select --</option>");
    $('#ddlUpsell3').html("<option value=''>-- Select --</option>");

    for(var item in productGroupList)
    {        
        var productID = productGroupList[item].ProductID;
        if(productID == selectedGroup)
        {
            $('#ddlUpsell1').append("<option value='" + productGroupList[item].ProductCode + "'>" + productGroupList[item].ProductName + "</option>");
            $('#ddlUpsell2').append("<option value='" + productGroupList[item].ProductCode + "'>" + productGroupList[item].ProductName + "</option>");
            $('#ddlUpsell3').append("<option value='" + productGroupList[item].ProductCode + "'>" + productGroupList[item].ProductName + "</option>");
        }
    }
}
</script>
<cc1:If ID="ifCustomerExists" runat="server">
    <div>
        <div>
            <div class="module">
                <h2>
                    Shipping Address</h2>
                <table border="0" cellspacing="0" cellpadding="3" class="editForm">
                    <uc1:Address ID="shippingAddress" runat="server" />
                </table>
            </div>
            <div class="module">
                <h2>
                    Billing Address</h2>
                <table border="0" cellspacing="0" cellpadding="3" class="editForm">
                    <uc1:Address ID="billingAddress" runat="server" />
                </table>
            </div>
            <div style="float: left">
                <div class="module">
                    <h2>
                        Credit Card</h2>
                    <table border="0" cellspacing="0" cellpadding="3" class="editForm">
                        <uc3:CreditCard ID="creditCard" runat="server" />
                    </table>
                </div>
                <div class="clear">
                </div>
                <div class="module" style="width: 500px;">
                    <h2>
                        New Subscription</h2>
                    <uc4:Subscription ID="NewSubscriptionControl1" runat="server" />
                    <div class="clear">
                    </div>
                    <h2>
                        Single Items</h2>
                    <table border="0" cellspacing="0" cellpadding="3" class="editForm">
                        <tr>
                            <td>
                                Product Group
                            </td>
                            <td>
                                <cc2:ProductDDL ID="DropDownProduct" runat="server" onchange="changeProductGroup();" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Single Item 1
                            </td>
                            <td>
                                <select ID="ddlUpsell1" name="ddlUpsell1" Style="width: 250px;" onchange="changeProductCode('ddlUpsell1', 'txtPriceUpsell1');">
                                    <option Value="">-- Select --</option>
                                </select>
                                &nbsp;&nbsp;Price&nbsp;&nbsp;<asp:textbox id="txtPriceUpsell1" runat="server" cssclass="xnarrow" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Single Item 2
                            </td>
                            <td>
                                <select ID="ddlUpsell2" name="ddlUpsell2" Style="width: 250px;" onchange="changeProductCode('ddlUpsell2', 'txtPriceUpsell2');" >
                                    <option Value="">-- Select --</option>
                                </select>
                                &nbsp;&nbsp;Price&nbsp;&nbsp;<asp:textbox runat="server" id="txtPriceUpsell2" cssclass="xnarrow" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Single Item 3
                            </td>
                            <td>
                                <select ID="ddlUpsell3" name="ddlUpsell3" Style="width: 250px;" onchange="changeProductCode('ddlUpsell3', 'txtPriceUpsell3');" >
                                    <option Value="">-- Select --</option>
                                </select>
                                &nbsp;&nbsp;Price&nbsp;&nbsp;<asp:textbox runat="server" id="txtPriceUpsell3" cssclass="xnarrow" />
                            </td>
                        </tr>
                    </table>
                </div>
                <div class="clear">
                </div>
                <div class="module">
                    <h2>
                        Referer</h2>
                    <table border="0" cellspacing="0" cellpadding="3" class="editForm">
                        <tr>
                            <td>
                                Referer
                            </td>
                            <td>
                                <asp:textbox runat="server" id="tbRefererCode"></asp:textbox>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
        <div class="clear">
        </div>
        <cc3:Error ID="Error1" Type="Error" runat="server"></cc3:Error>
        <div style="float: right;">
            <asp:button runat="server" id="btnPlaceOrder" text="Place Order" onclick="btnPlaceOrder_Click" />
        </div>
    </div>
</cc1:If>
<cc1:If ID="ifCustomerDoesntExist" runat="server">
    Customer not found
</cc1:If>
</form>
