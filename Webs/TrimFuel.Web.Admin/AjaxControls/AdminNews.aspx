<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminNews.aspx.cs" Inherits="TrimFuel.Web.Admin.AjaxControls.AdminNews" EnableViewState="true" %>
<%@ Import Namespace="System.Data"%>
<%@ Register Assembly="TrimFuel.Web.UI" Namespace="TrimFuel.Web.UI" TagPrefix="gen" %>
<form id="formAdminNews" runat="server">
<div class="news-day">
    <asp:Repeater ID="rNews" runat="server">
        <ItemTemplate>
            <div class="news-day-date"><strong><%#Eval("createDT", "{0:dddd d MMMM}")%></div></strong>
                <asp:Repeater runat="server" datasource= '<%# Eval("news") %>' OnItemCommand="rNews_ItemCommand">
                    <ItemTemplate>
                        <div class="news-text">
                            <%#Eval("content")%>
                            <br />
                            <asp:LinkButton runat="server" ID="bRead" CommandName="Read" CommandArgument='<%#Eval("newsID")%>'>Hide</asp:LinkButton>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</div>
<asp:Label runat="server" ID="LabelNoItems" Text=""></asp:Label>
</form>
