<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ClientFulfillmentService.aspx.cs"
    Inherits="TrimFuel.Web.RapidApp.AjaxControls.ClientFulfillmentService" %>

<form id="from1" runat="server">
<h1>
    <%# HeaderTitle %></h1>
<asp:textbox id="tbShipperSettingID" text='<%# FulfillmentSettingsProp.ShipperSettingID %>' runat="server" style="display: none;" />
<asp:hiddenfield runat="server" id="hdnShipperID" value="<%# ShipperID %>" />
<asp:hiddenfield runat="server" id="hdnShipperName" value="<%# ShipperName %>" />
<asp:textbox id="tbClientID" text='<%# TPClientID %>' runat="server" style="display: none;" />
<table cellspacing="1" id="table1" class="rapidapp-alternate">
    <tr>
        <td width="30%">
            Company Specific Contact Name
        </td>
        <td>
            <asp:textbox id="tbName" text='<%# FulfillmentSettingsProp.CompanyName %>' runat="server" />
        </td>
    </tr>
    <tr>
        <td>
            Company Specific Contact Phone
        </td>
        <td>
            <asp:textbox id="tbPhone" text='<%# FulfillmentSettingsProp.CompanyPhone %>' runat="server" />
        </td>
    </tr>
    <tr>
        <td>
            Company Specific Contact Email Address
        </td>
        <td>
            <asp:textbox id="tbEmail" text='<%# FulfillmentSettingsProp.CompanyEmail %>' runat="server" />
        </td>
    </tr>
</table>
<asp:placeholder runat="server" visible='<%#ConfigFields.Count > 0 ? true : false %>'>
<h1 class="margintop10">Configuration</h1>
<table cellspacing="1" id="table2" class="rapidapp-alternate">
    <asp:repeater id="rConfig" runat="server" datasource='<%#ConfigFields %>'>
        <ItemTemplate>
            <tr>
                <td width="30%">
                    <%# Eval("Key") %>
                </td>
                <td>
                    <asp:TextBox name='<%# Eval("Key") %>' text='<%# Eval("Value") %>' runat="server" />
                </td>
            </tr>
        </ItemTemplate>
    </asp:repeater>
</table>
</asp:placeholder>
<asp:PlaceHolder runat="server" Visible='<%# !IsNew %>'>
<h1 class="margintop10">
    Product Groups Settings</h1>
   <a href="javascript:void(0)" onclick="$('#addProductRow').show();" style="font-size: 12px;" class="addNewIcon">Add</a>
   <div style="height:10px;"></div>
<table cellspacing="1" class="rapidapp-alternate" style="font-size: 12px; width: 100%;">
    <tr>
        <td style="width:30%;">
            <strong>Product</strong>
        </td>
        <td>
            <strong>Actions</strong>
        </td>
    </tr>
    <asp:repeater id="rProducts" runat="server" datasource='<%# ShipperProducts %>' OnItemCommand="rProducts_ItemCommand">
        <ItemTemplate>
            <tr>
                <td>
                    <%#Eval("Product")%>
                </td>
                <td>
                    <asp:LinkButton runat="server" ID="lbDelete" Text="Delete" CssClass="confirm removeIcon" CommandName="delete" CommandArgument='<%# Eval("ProductID") %>'></asp:LinkButton>
                </td>
            </tr>
        </ItemTemplate>
    </asp:repeater>
    <tr id="addProductRow" style="display:none;">
        <td>
            <asp:DropDownList id="dpdProducts" runat="server" datasource='<%# Products %>' datatextfield="ProductName" datavaluefield="ProductID" />
        </td>
        <td>
            <asp:LinkButton runat="server" ID="lbAddProduct" Text="Save" CssClass="saveIcon" CommandName="add" onclick="btnAddProduct_Click" ></asp:LinkButton>
            <a href="javascript:void(0);" onclick="$('#addProductRow').hide();" Class="cancelIcon" >Cancel</a>
        </td>
    </tr>
</table>
</asp:PlaceHolder>
<h1 class="margintop10">Fees</h1>
<table cellspacing="1" id="table" class="rapidapp-alternate">
    <tr>
        <td>
            <strong>Fee Type</strong>
        </td>
        <td>
            <strong>Retail</strong>
        </td>
        <td>
            <strong>Cost</strong>
        </td>
    </tr>
    <tr>
        <td width="30%">
            Shipment
        </td>
        <td width="35%">
            <asp:textbox id="tbShipmentFeeRetail" cssclass="currency" text='<%# FulfillmentSettingsProp.ShipmentFeeRetail %>'
                runat="server" />
            Per Action
        </td>
        <td width="35%">
            <asp:textbox id="tbShipmentFee" cssclass="currency" text='<%# FulfillmentSettingsProp.ShipmentFee %>'
                runat="server" />
            Per Action
        </td>
    </tr>
    <tr>
        <td>
            Shipment (Per SKU)
        </td>
        <td>
            <asp:textbox id="tbShipmentSKUFeeRetail" cssclass="currency" text='<%# FulfillmentSettingsProp.ShipmentSKUFeeRetail %>'
                runat="server" />
            Per Action
        </td>
        <td>
            <asp:textbox id="tbShipmentSKUFee" cssclass="currency" text='<%# FulfillmentSettingsProp.ShipmentSKUFee %>'
                runat="server" />
            Per Action
        </td>
    </tr>
    <tr>
        <td>
            Setup Fee
        </td>
        <td>
            <asp:textbox id="tbSetupFeeRetail" cssclass="currency" text='<%# FulfillmentSettingsProp.SetupFeeRetail %>'
                runat="server" />
            One Time
        </td>        
        <td>
            <asp:textbox id="tbSetupFee" cssclass="currency" text='<%# FulfillmentSettingsProp.SetupFee %>'
                runat="server" />
            One Time
        </td>
    </tr>
    <tr>
        <td>
            Returns
        </td>
        <td>
            <asp:textbox id="tbReturnsFeeRetail" cssclass="currency" text='<%# FulfillmentSettingsProp.ReturnsFeeRetail %>'
                runat="server" />
            Per Action
        </td>        
        <td>
            <asp:textbox id="tbReturnsFee" cssclass="currency" text='<%# FulfillmentSettingsProp.ReturnsFee %>'
                runat="server" />
            Per Action
        </td>
    </tr>
    <tr>
        <td>
            Kitting and Asembly
        </td>
        <td>
            <asp:textbox id="tbKittingFeeRetail" cssclass="currency" text='<%# FulfillmentSettingsProp.KittingAndAsemblyFeeRetail %>'
                runat="server" />
            Per Action
        </td>        
        <td>
            <asp:textbox id="tbKittingFee" cssclass="currency" text='<%# FulfillmentSettingsProp.KittingAndAsemblyFee %>'
                runat="server" />
            Per Action
        </td>
    </tr>
    <tr>
        <td>
            Custom Development
        </td>
        <td>
            <asp:textbox id="tbCustomDevelopmentFeeRetail" cssclass="currency" text='<%# FulfillmentSettingsProp.CustomDevelopmentFeeRetail %>'
                runat="server" />
            Per Hour
        </td>        
        <td>
            <asp:textbox id="tbCustomDevelopmentFee" cssclass="currency" text='<%# FulfillmentSettingsProp.CustomDevelopmentFee %>'
                runat="server" />
            Per Hour
        </td>
    </tr>
    <tr>
        <td>
            Special Labor
        </td>
        <td>
            <asp:textbox id="tbSpecialLaborFeeRetail" cssclass="currency" text='<%# FulfillmentSettingsProp.SpecialLaborFeeRetail %>'
                runat="server" />
            Per Hour
        </td>        
        <td>
            <asp:textbox id="tbSpecialLaborFee" cssclass="currency" text='<%# FulfillmentSettingsProp.SpecialLaborFee %>'
                runat="server" />
            Per Hour
        </td>
    </tr>
</table>
<div id="bottom">
    <div class="left">
        <asp:PlaceHolder runat="server" id="lSaved">
           Saved <%# DateTime.Now %> by <%# AdminMembership.CurrentAdmin.DisplayName %>
        </asp:PlaceHolder>
    </div>
    <div class="right" onclick="setTimeout(function(){updateMenu();},1000);">
        <asp:button type="submit" name="button7" id="button7" runat="server" text="Save Changes"
            onclick="SaveChanges_Click" />
    </div>
</div>
</form>
