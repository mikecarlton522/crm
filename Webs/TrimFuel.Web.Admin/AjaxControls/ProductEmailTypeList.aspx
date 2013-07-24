<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProductEmailTypeList.aspx.cs" Inherits="TrimFuel.Web.Admin.AjaxControls.ProductEmailTypeList" EnableViewState="true" %>
<%@ Register Assembly="TrimFuel.Web.UI" Namespace="TrimFuel.Web.UI" TagPrefix="gen" %>
<form id="formProductEmailTypeList" runat="server">
<table class="process-offets sortable" width="100%">
    <tr class="header">
        <td>Email Type</td>
        <td>Tasks</td>
    </tr>
    <asp:Repeater runat="server" ID="rEmailType" 
        onitemcommand="rEmailType_ItemCommand">        
        <ItemTemplate>
        <tr <%# Eval("DynamicEmailID") != null ? Convert.ToBoolean(Eval("Active")) ?  "" : "class='hiddenrow'" : "class='disabled'" %>>
            <td><%# Eval("DisplayName") %><%# (!string.IsNullOrEmpty(Convert.ToString(Eval("CustomName")))) ? "(" + Eval("CustomName") + ")" : "" %><%# (Convert.ToInt32(Eval("Hours")) > 0) ? "(" + Eval("Hours") + " hour(s))" : "" %></td>
            <td nowrap>
                <gen:If runat="server" Condition='<%# Eval("DynamicEmailID") != null %>'>
                    <a href='javascript:onEditEmail(<%# Eval("DynamicEmailID") %>)' class="editIcon">Edit</a>
                    <gen:If runat="server" Condition='<%# Convert.ToBoolean(Eval("Active")) %>'>
                        &nbsp;&nbsp;<asp:LinkButton runat="server" ID="bInactive" CssClass="cancelIcon confirm" CommandName="Inactive" CommandArgument='<%# Eval("DynamicEmailID")%>'>Inactivate</asp:LinkButton>
                    </gen:If>
                    <gen:If ID="If2" runat="server" Condition='<%# !Convert.ToBoolean(Eval("Active")) %>'>
                        &nbsp;&nbsp;<asp:LinkButton runat="server" ID="bActive" CssClass="saveIcon confirm" CommandName="Active" CommandArgument='<%# Eval("DynamicEmailID")%>'>Activate</asp:LinkButton>
                    </gen:If>
                    <gen:If ID="If3" runat="server" Condition='<%# Convert.ToInt32(Eval("EmailCount")) == 0 %>'>
                        &nbsp;&nbsp;<asp:LinkButton runat="server" ID="bRemove" CssClass="removeIcon confirm" CommandName="Remove" CommandArgument='<%# Eval("DynamicEmailID")%>'>Delete</asp:LinkButton>
                    </gen:If>
                </gen:If>
                <gen:If ID="If1" runat="server" Condition='<%# Eval("DynamicEmailID") == null %>'>
                    <a href='javascript:onCreateEmail(<%# Eval("DynamicEmailTypeID") %>, <%# Eval("Hours") %>, "<%# Eval("GiftCertificateDynamicEmail_StoreID") %>")' class="addNewIcon">Create</a>
                </gen:If>
            </td>
        </tr>
        </ItemTemplate>
    </asp:Repeater>
</table>
</form>
