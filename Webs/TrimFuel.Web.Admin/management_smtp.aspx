<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="management_smtp.aspx.cs"
    Inherits="TrimFuel.Web.Admin.management_smtp" MasterPageFile="~/Controls/Admin.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphScript" runat="server">

    <script type="text/javascript" language="javascript">
        function editSMTP() {
            popupControl2("smtp-popup", "SMTP Settings", 550, 300, "EditForms/SMTP.aspx", null, null, function() { document.location = "management_smtp.aspx";s });
        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphStyle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <asp:PlaceHolder runat="server" ID="phDefault">
        <div class="data">
            <table class="process-offets" border="0" cellspacing="1" width="70%">
                <tr class="header">
                    <td>
                        UserName
                    </td>
                    <td>
                        Password
                    </td>
                    <td>
                        Server
                    </td>
                    <td>
                        Port
                    </td>
                    <td>
                        Enable SSL
                    </td>
                    <td>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td>
                    </td>
                    <td>
                        relay.jangosmtp.net
                    </td>
                    <td>
                    </td>
                    <td>
                    </td>
                    <td>
                        <a href="javascript:editSMTP();" class="editIcon">Edit</a>
                    </td>
                </tr>
            </table>
            <div class="space">
            </div>
            <h2>
                Default SMTP Settings Enabled</h2>
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" ID="phCurrent">
        <div class="data">
            <form id="Form1" runat="server">
            <table class="process-offets" border="0" cellspacing="1" width="70%">
                <tr class="header">
                    <td>
                        UserName
                    </td>
                    <td>
                        Password
                    </td>
                    <td>
                        Server
                    </td>
                    <td>
                        Port
                    </td>
                    <td>
                        Enable SSL
                    </td>
                    <td>
                    </td>
                </tr>
                <asp:Repeater ID="rSMTP" runat="server">
                    <ItemTemplate>
                        <tr>
                            <td>
                                <%# Eval("UserName") %>
                            </td>
                            <td>
                                <%# Eval("Password") %>
                            </td>
                            <td>
                                <%# Eval("Server") %>
                            </td>
                            <td>
                                <%# Eval("Port") %>
                            </td>
                            <td>
                                <%# Convert.ToBoolean(Eval("EnableSSL")) ? "yes" : "no" %>
                            </td>
                            <td>
                                <a href="javascript:editSMTP();" class="editIcon">Edit</a>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>
            </form>
        </div>
    </asp:PlaceHolder>
</asp:Content>
