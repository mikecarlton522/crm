<%@ Page Title="" Language="C#" MasterPageFile="~/admin/Controls/Admin.Master" AutoEventWireup="true" CodeBehind="Subscription.aspx.cs" Inherits="Securetrialoffers.admin.Subscription_" %>
<%@ Register src="Controls/EditForms/Subscription.ascx" tagname="Subscription" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphScript" runat="server">
<link href="css/validationEngine.jquery.css" rel="stylesheet" type="text/css"><script src="js/jquery.validationEngine.validationRules.js" type="text/javascript" language="javascript"></script><script src="js/jquery.validationEngine.js" type="text/javascript" language="javascript"></script> <script type="text/javascript">
    $(document).ready(function() {
        $("#aspnetForm").validationEngine({ inline: true, scroll: false })
    });
    function validateForm() {
        return $("#aspnetForm").validationEngine({ returnIsValid: true, inline: true, scroll: false });
    }</script></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphStyle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <form id="form1" runat="server">
        <table border="0">
            <tr>
                <td align="center">
                    <h2>Add Subscription</h2>
                </td>
            </tr>
            <asp:PlaceHolder runat="server" ID="phSuccess" Visible="false">
            <tr>
                <td>
                    <div style="color:Green;">Subscription was successfully added. SubscriptionID = <asp:Literal runat="server" ID="lSubscriptionID"></asp:Literal></div>
                </td>
            </tr>
            </asp:PlaceHolder>
            <asp:PlaceHolder runat="server" ID="phError" Visible="false">
            <tr>
                <td>
                    <div style="color:Red;">Error occued while trying to add new Subscription. Subscription was not added.</div>
                </td>
            </tr>
            </asp:PlaceHolder>
            <tr>
                <td>
                    <uc1:Subscription ID="Subscription1" runat="server" />
                </td>
            </tr>
            <tr>
                <td align="right">
                    <asp:Button runat="server" ID="bSave" Text="Add Subscription" 
                        onclick="bSave_Click" OnClientClick="return validateForm();" />
                </td>
            </tr>
        </table>                
    </form>
</asp:Content>
