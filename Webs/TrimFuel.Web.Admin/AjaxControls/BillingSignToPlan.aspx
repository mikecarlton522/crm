<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BillingSignToPlan.aspx.cs" Inherits="TrimFuel.Web.Admin.AjaxControls.BillingSignToPlan" %>
<div class="module">
    <h2>Loyalty Plan</h2>
    <form runat="server" ID="form1">
    <table class="editForm">
        <tr><td>Loyalty Plan</td><td><asp:DropDownList runat="server" ID="ddlSubscriptionPlanList" class="validate[required]" DataTextField="SubscriptionPlanName" DataValueField="SubscriptionPlanID"></asp:DropDownList></td></tr>
        <tr><td colspan="2" align="right"><asp:Button runat="server" ID="btnSignUp" Text="Sign Up" onclick="btnSignUp_Click" /> </td></tr>
    </table>
    </form>
    <div id="errorMsg" style="max-width:400px;">
    <asp:PlaceHolder runat="server" ID="phError">    
    <div class='space'></div>
    <span class='small-alert'>Can't sign user. Error occurred while billing.</span>
    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" ID="phSuccess">
    <div class='space'></div>
    <span>User was successfuly signed to plan.</span>
    </asp:PlaceHolder>
    </div>
</div>