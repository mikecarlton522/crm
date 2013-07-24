<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ClientProfileItem.aspx.cs"
    Inherits="TrimFuel.Web.RapidApp.AjaxControls.ClientProfileItem" EnableViewState="false" %>

<script type="text/javascript">
    $("#form1").ready(function() {
        alternate('table');
        alternate('table1');
        alternate('table2');
        alternate('table3');
        ShowProductGroups();
    });

    function ShowProductGroups() {        
        var clientID = <%# TPClientID %>;
        inlineControl("ajaxControls/ClientProductGroups.aspx?clientId=" + clientID, "product-groups", null, function(){
            alternateTables('product-groups');
        });
    }
    function ShowMessagePopup(id, type){
        popupControl2('mailnote-view-' + id, "View " + type, 300, 300, "ajaxControls/ViewMessage.aspx?ID=" + id + "&type=" + type);
    }
</script>

<h1><%# TPClient.Name %> Profile</h1>
<form id="form1" runat="server" action="AjaxControls/ClientProfileItem.aspx">
<table cellspacing="1" id="table">
    <tr style="display: none;">
        <td>
            <asp:textbox id="tbTPClientID" runat="server" text='<%# TPClientProp.TPClientID %>'></asp:textbox>
            <asp:textbox id="tbTPClientSettingID" runat="server" text='<%# TPClientProp.TPClientSettingID %>'></asp:textbox>
        </td>
    </tr>
    <tr>
        <td width="30%">
            Legal Business Name
        </td>
        <td>
            <asp:textbox id="tbName" runat="server" text='<%# TPClientProp.LegalBusinessName %>'
                tabindex="1"></asp:textbox>
        </td>
    </tr>
    <tr>
        <td>
            DBA Name
        </td>
        <td>
            <asp:textbox id="tbDBAName" runat="server" text='<%# TPClientProp.DBAName %>' tabindex="2"></asp:textbox>
        </td>
    </tr>
    <tr>
        <td>
            Contact Name
        </td>
        <td>
            <asp:textbox id="tbContactName" runat="server" text='<%# TPClientProp.ContactName %>'
                tabindex="3"></asp:textbox>
        </td>
    </tr>
    <tr>
        <td>
            Contact Phone
        </td>
        <td>
            <asp:textbox id="tbContactPhone" runat="server" text='<%# TPClientProp.ContactPhone %>'
                tabindex="4"></asp:textbox>
        </td>
    </tr>
    <tr>
        <td>
            Email Address (Username)
        </td>
        <td>
            <asp:textbox id="tbUserName" runat="server" text='<%# TPClientProp.Username %>' tabindex="5"></asp:textbox>
        </td>
    </tr>
    <tr>
        <td>
            Password
        </td>
        <td>
            <asp:textbox id="tbPassword" runat="server" text='<%# TPClientProp.Password %>' tabindex="6"></asp:textbox>
        </td>
    </tr>
    <tr>
        <td>
            Secondary Contact Name
        </td>
        <td>
            <asp:textbox id="tbSecondaryName" runat="server" text='<%# TPClientProp.SecondaryContactName %>'
                tabindex="7"></asp:textbox>
        </td>
    </tr>
    <tr style="display:none;">
        <td>
            Last Login Date
        </td>
        <td>
            <span class="left">
                <%# DateTime.Now %></span>
        </td>
    </tr>
    <tr>
        <td>
            Product Offered
        </td>
        <td>
            <asp:textbox id="tbProductOffered" runat="server" text='<%# TPClientProp.ProductOffered %>'
                tabindex="8"></asp:textbox>
        </td>
    </tr>
    <tr>
        <td>
            Billing Model
        </td>
        <td>
            <asp:dropdownlist id="sBillingModel" runat="server" selectedindex='<%# TPClientProp.BillingModel ? 1 : 0 %>'>
                <asp:ListItem Text="Continuity" Value="0" runat="server" />
                <asp:ListItem Text="Straight Sale" Value="1" runat="server" />
            </asp:dropdownlist>
        </td>
    </tr>
    <tr>
        <td>
            Customer Service Phone Number
        </td>
        <td>
            <asp:textbox id="tbServicePhone" runat="server" text='<%# TPClientProp.CustomerServicePhoneNumber %>'
                tabindex="9"></asp:textbox>
        </td>
    </tr>
    <tr>
        <td>
            Marketing Page URL
        </td>
        <td>
            <asp:textbox id="tbURL" runat="server" text='<%# TPClientProp.MarketingPageURL %>'
                tabindex="10"></asp:textbox>
        </td>
    </tr>
    <tr>
        <td>
            Open Sales Regions
        </td>
        <td>
            <asp:textbox id="tbSalesRegion" runat="server" text='<%# TPClientProp.OpenSalesRegions %>'
                tabindex="11"></asp:textbox>
        </td>
    </tr>
</table>
<h1 class="margintop10">Billing API Credentials</h1>
<table cellspacing="1" id="table3">
    <tr>
        <td width="30%">
            Username
        </td>
        <td>
            <asp:textbox id="tbBillingAPIUsername" runat="server" text='<%# TPClient.Username %>'
                tabindex="12"></asp:textbox>
        </td>
    </tr>
    <tr>
        <td>
            Password
        </td>
        <td>
            <asp:textbox id="tbBillingAPIPassword" runat="server" text='<%# TPClient.Password %>'
                tabindex="13"></asp:textbox>
        </td>
    </tr>
</table>
<div id="bottom">
    <div class="left">
        <asp:PlaceHolder runat="server" id="lSaved">
        Saved <%# DateTime.Now %> by <%# AdminMembership.CurrentAdmin.DisplayName %>
        </asp:PlaceHolder>
    </div>
    <div class="right">
        <asp:button onclick="bSaveSettings_Click" id="button7" text="Save Changes" style="height: 26px"
            runat="server" />
    </div>
</div>
<h1 class="margintop10">Product Groups</h1>
<span id="product-groups"></span>
<h1 class="margintop10">Internal Notes</h1>
<table cellspacing="1" id="table1">
    <tr>
        <td>
            <p>
                Add Note
            </p>
            <p class="margintop10">
                <textarea name="textarea" rows="5" id="txtNote" runat="server"></textarea>
            </p>
            <p class="margintop10">
              <asp:button onclick="bSendNote_Click" id="btnNote" text="Submit" runat="server" />
                <%--<input type="submit" name="button" id="button" value="Submit" />--%>
            </p>
        </td>
    </tr>
    <asp:repeater id="rNotes" runat="server" DataSource='<%# TPClientNotesProp %>'>
        <ItemTemplate>
            <tr>
                <td>
                    <a href='javascript:ShowMessagePopup(<%# Eval("TPClientNoteID") %>, "Note")'>View Note Added <%# Eval("CreateDT") %> by <%# Eval("AdminName") %></a>
                </td>
            </tr>
        </ItemTemplate>
    </asp:repeater>
</table>
<h1 class="margintop10">
    Outgoing Messages</h1>
<table cellspacing="1" id="table2">
    <tr>
        <td>
            <p>
                Email Message
            </p>
            <p class="margintop10">
                <textarea name="textarea" rows="5" id="txtMessage" runat="server" ></textarea>
            </p>
            <p class="margintop10">
                From:
                <asp:TextBox id="tbFrom" runat="server" Text='' />
            </p>            
            <p class="margintop10">
                To:
                <asp:TextBox id="tbTo" runat="server" Text='' />
                <asp:button onclick="bSendEmail_Click" id="btnEmail" text="Send" runat="server" />
                <%--<input name="input11" type="text" id="input11" value="john@biggerbidder.com" />--%>
                <%--<input type="submit" name="button" id="button" value="Send" />--%>
            </p>
        </td>
    </tr>
    <asp:repeater id="rEmails" runat="server" DataSource='<%# TPClientEmailsProp %>'>
        <ItemTemplate>
            <tr>
                <td>
                    <a href='javascript:ShowMessagePopup(<%# Eval("TPClientEmailID") %>, "Mail")'>View Message Delivered to <%# Eval("To") %> on <%# Eval("CreateDT") %> by <%# Eval("AdminName") %></a>
                </td>
            </tr>
        </ItemTemplate>
    </asp:repeater>
</table>
</form>
