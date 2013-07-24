<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Subscription.ascx.cs" Inherits="Securetrialoffers.admin.Controls.EditForms.Subscription_" %>
<%@ Register assembly="TrimFuel.Web.UI" namespace="TrimFuel.Web.UI.Specialized" tagprefix="cc1" %>
<table border="0" cellspacing="20">
    <tr valign="top">
        <td>
            <b>Product Info</b>
            <table border="0">
                <tr>
                    <td>Product</td>
                    <td>
                        <cc1:ProductDDL ID="ddlProduct" runat="server" Width="180" CssClass="validate[required]"></cc1:ProductDDL>
                    </td>
                </tr>
                <tr>
                    <td>Product Name</td>
                    <td>
                        <asp:TextBox ID="tbProductName" runat="server" Width="180"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>Product Code</td>
                    <td>
                        <cc1:ProductCodeDDL ID="ddlProductCode" runat="server" Width="180" CssClass="validate[required]"></cc1:ProductCodeDDL>
                    </td>
                </tr>
                <tr>
                    <td>SKU2</td>
                    <td>
                        <cc1:ProductCodeDDL ID="ddlSKU2" runat="server" Width="180" CssClass="validate[required]"></cc1:ProductCodeDDL>
                    </td>
                </tr>
                <tr>
                    <td>Quantity</td>
                    <td>
                        <asp:TextBox ID="tbQuantity" runat="server" Width="100" CssClass="validate[custom[Quantity]]"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>Display Name</td>
                    <td>
                        <asp:TextBox ID="tbDisplayName" runat="server" Width="180"></asp:TextBox>
                    </td>
                </tr>
            </table>
            <br />
            <b>Settings</b>
            <table border="0">
                <tr>
                    <td>Parent Subscription</td>
                    <td>
                        <cc1:SubscriptionDDL ID="ddlParentSubscription" runat="server" Width="180">
                        </cc1:SubscriptionDDL>
                    </td>
                </tr>
                <tr>
                    <td>Selectable</td>
                    <td>
                        <asp:CheckBox ID="chbSelectable" runat="server"  />
                    </td>
                </tr>
                <tr>
                    <td>Recurring</td>
                    <td>
                        <asp:CheckBox ID="chbRecurring" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td>Ship First Rebill</td>
                    <td>
                        <asp:CheckBox ID="chbShipFirstRebill" runat="server" />
                    </td>
                </tr>
            </table>
        </td>
        <td valign="top">
            <b>Interim Settings</b>
            <table border="0">
                <tr>
                    <td>Initial Interim (days)</td>
                    <td>
                        <asp:TextBox ID="tbInitialInterim" runat="server" Width="100" CssClass="validate[custom[Numeric]]"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>Initial Shipping ($)</td>
                    <td>
                        <asp:TextBox ID="tbInitialShipping" runat="server" Width="100" CssClass="validate[custom[Amount]]"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>Initial Bill Amount ($)</td>
                    <td>
                        <asp:TextBox ID="tbInitialBillAmount" runat="server" Width="100" CssClass="validate[custom[Amount]]"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>Save Shipping ($)</td>
                    <td>
                        <asp:TextBox ID="tbSaveShipping" runat="server" Width="100" CssClass="validate[custom[Amount]]"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>Save Billing ($)</td>
                    <td>
                        <asp:TextBox ID="tbSaveBilling" runat="server" Width="100" CssClass="validate[custom[Amount]]"></asp:TextBox>
                    </td>
                </tr>
                <tr><td colspan="2" style="height:10px;"></td></tr>
                <tr>
                    <td>Second Interim (days)</td>
                    <td>
                        <asp:TextBox ID="tbSecondInterim" runat="server" Width="100" CssClass="validate[custom[Numeric]]"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>Second Shipping ($)</td>
                    <td>
                        <asp:TextBox ID="tbSecondShipping" runat="server" Width="100" CssClass="validate[custom[Amount]]"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>Second Bill Amount ($)</td>
                    <td>
                        <asp:TextBox ID="tbSecondBillAmount" runat="server" Width="100" CssClass="validate[custom[Amount]]"></asp:TextBox>
                    </td>
                </tr>
                <tr><td colspan="2" style="height:10px;"></td></tr>
                <tr>
                    <td>Regular Interim (days)</td>
                    <td>
                        <asp:TextBox ID="tbRegularInterim" runat="server" Width="100" CssClass="validate[custom[Numeric]]"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>Regular Shipping ($)</td>
                    <td>
                        <asp:TextBox ID="tbRegularShipping" runat="server" Width="100" CssClass="validate[custom[Amount]]"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>Regular Bill Amount ($)</td>
                    <td>
                        <asp:TextBox ID="tbRegularBillAmount" runat="server" Width="100" CssClass="validate[custom[Amount]]"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <b>Description</b>
            <asp:TextBox ID="tbDescription" runat="server" Width="100%"></asp:TextBox>        
        </td>
    </tr>
</table>

