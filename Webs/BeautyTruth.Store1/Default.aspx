<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="BeautyTruth.Store1._Default"
    MasterPageFile="~/Controls/Front.Master" %>

<%@ Register TagPrefix="uc" TagName="Footer" Src="~/Controls/Footer.ascx" %>
<%@ Register TagPrefix="uc" TagName="Product" Src="~/Controls/Product.ascx" %>
<asp:Content ContentPlaceHolderID="cphTitle" runat="server">
    Beauty &amp; Truth: Anti-Aging Wrinkle Creams & Skin Care for Face and Eye</asp:Content>
<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <div class="container_12">
        <div id="lightbox" class="grid_12">
            <img src="images/940.png" width="940" height="400" usemap="#shop" />
            <map name="shop">
                <area shape="rect" coords="40, 250, 160, 290" href="category.aspx">
            </map>
        </div>
        <asp:Repeater ID="rProducts" runat="server" DataSource="<%# Products %>">
            <ItemTemplate>
                <uc:Product runat="server" ProductCode='<%#Eval("ProductCode_")%>' />
            </ItemTemplate>
        </asp:Repeater>
    </div>
</asp:Content>
<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <uc:Footer WithSignIn="true" runat="server" />
</asp:Content>
