<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RecurringPlanDelete.aspx.cs" Inherits="TrimFuel.Web.Admin.AjaxControls.RecurringPlanDelete" %>
<div class='space'></div>
<div id="errorMsg" style="max-width:500px;">
<asp:PlaceHolder runat="server" ID="phError">    
    <span class='small-alert'><%# ErrorMessage %></span>
</asp:PlaceHolder>
<asp:PlaceHolder runat="server" ID="phSuccess">
    <span>Plan was successfully deleted.</span>
</asp:PlaceHolder>        
</div>
