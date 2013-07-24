<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LeftMenu.aspx.cs" Inherits="TrimFuel.Web.RapidApp.AjaxControls.LeftMenu" %>

<%@ Register Src="~/Controls/ClientsMenuItem.ascx" TagName="ClientMenuItem" TagPrefix="Uc" %>

<script type="text/javascript">
    $("form").ready(function() {
        $(".menu_show").click(function() {
            if ($(this).next().css('display') == 'none') {
                $(this).next().show();
            }
            else {
                $(this).next().hide();
            }
            return false;
        });
    });
</script>

<form id="Form1" runat="server">
<h1 class="first">
    Permission Level: <%# AdminMembership.CurrentAdminRole %></h1>
<Uc:ClientMenuItem ID="ClientMenuItem1" FolderName="Companies 0-9, A-H" Class="first"
    Data='<%#Clients1 %>' runat="server" />
<Uc:ClientMenuItem ID="ClientMenuItem2" FolderName="Companies I-P" Class="" Data='<%#Clients2 %>'
    runat="server" />
<Uc:ClientMenuItem ID="ClientMenuItem3" FolderName="Companies Q-Z" Class="" Data='<%#Clients3 %>'
    runat="server" />
<a href="#" class="menu_show">
    <h2>
        Services</h2>
</a>
<div>
    <asp:repeater id="rServicesTypes" runat="server">
                        <ItemTemplate>
                            <a href="#" class="menu_show">
                                <h3 class='tpclientservice_<%# Eval("Name").ToString().ToLower() %>'>
                                    <%# Eval("DisplayName") %></h3>
                            </a>
                            <div style="display: none;">
                                <asp:Repeater runat="server" DataSource='<%# Eval("Services") %>'>
                                    <ItemTemplate>
                                        <a href="#" onclick='showService(<%# Eval("ID")%>, <%# DataBinder.Eval(Container.Parent.Parent, "DataItem.Name", "\"{0}\"")%>, <%# Eval("ServiceName", "\"{0}\"")%>, this); return false;'>
                                            <h4 class='tpclientservice_<%# DataBinder.Eval(Container.Parent.Parent, "DataItem.Name").ToString().ToLower() %>'>
                                                <%# Eval("ServiceName")%></h4>
                                        </a>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </ItemTemplate>
                    </asp:repeater>
</div>
</form>
