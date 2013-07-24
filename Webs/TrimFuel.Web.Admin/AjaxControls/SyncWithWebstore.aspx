<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SyncWithWebstore.aspx.cs" Inherits="TrimFuel.Web.Admin.AjaxControls.SyncWithWebstore" EnableViewState="true" %>
<%@ Register Assembly="TrimFuel.Web.UI" Namespace="TrimFuel.Web.UI" TagPrefix="gen" %>
<form id="formSyncWithWebstore" runat="server">
<% if ((ToDeleteCheckBoxList.Items.Count > 0) && (ToAddCheckBoxList.Items.Count > 0))
   {%>
<table width="100%">    
    <tr>        
        <td>
            <table class="process-offets sortable">
                <tr class="header">                    
                    <td><asp:label ID="ToDeleteLabel" runat="server"></asp:label></td>
                </tr>
                <tr>
                    <td>
                        <asp:checkboxlist runat="server" ID="ToDeleteCheckBoxList"></asp:checkboxlist>
                    </td>
                </tr>
            </table>
        </td>
        <td>
            <table class="process-offets sortable">
                <tr class="header">
                    <td><asp:label ID="ToAddLabel" runat="server"></asp:label></td>
                </tr>
                <tr>
                    <td>
                        <asp:checkboxlist runat="server" ID="ToAddCheckBoxList"></asp:checkboxlist>
                    </td>
                </tr>
            </table>
        </td>
    </tr>   
</table>
<asp:button ID="ConfirmButton" runat="server" text="Confirm" 
    onclick="ConfirmButton_Click" />
    <% }
   else
   {%>
   <asp:label ID="LabelOK" runat="server"></asp:label>
   <%
        
   } %>
</form>
