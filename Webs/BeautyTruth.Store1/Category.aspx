<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Category.aspx.cs" Inherits="BeautyTruth.Store1.Category"
    MasterPageFile="~/Controls/Front.Master" %>

<%@ Register TagPrefix="uc" TagName="Footer" Src="~/Controls/Footer.ascx" %>
<%@ Register TagPrefix="uc" TagName="CategoryMenu" Src="~/Controls/CategoryMenu.ascx" %>
<%@ Register TagPrefix="uc" TagName="CategoryPager" Src="~/Controls/CategoryPager.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    Beauty &amp; Truth: Anti-Aging Wrinkle Creams & Skin Care for Face and Eye</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <div class="container_12">
        <uc:CategoryMenu ID="CategoryMenu" runat="server" />
        <div class="grid_9 sort">
            <div>
                <h1>
                    Sort By:</h1>
                <asp:DropDownList ID="dpdSort" OnSelectedIndexChanged="dpdSort_SelectedIndexChanged"
                    runat="server" AutoPostBack="true">
                    <asp:ListItem Value="0" Text="Choose Sort" />
                    <asp:ListItem Value="1" Text="Date - New to Old" />
                    <asp:ListItem Value="2" Text="Date - Old to New" />
                    <asp:ListItem Value="3" Text="Price - High to Low" />
                    <asp:ListItem Value="4" Text="Price - Low to High" />
                    <asp:ListItem Value="5" Text="Name - A to Z" />
                    <asp:ListItem Value="6" Text="Name - Z to A" />
                </asp:DropDownList>
            </div>
        </div>
        <div class="grid_9" id="breadcrumb">
            <ul>
                <li><a href="default.aspx">Home</a></li>
                <li>&raquo;</li>
                <li><a href="category.aspx">Products</a></li>
                <li>&raquo;</li>
                <li><a href="category.aspx?categoryID=<%#CategoryID %>" class="highlight"><%# CategoryName %></a></li>
            </ul>
        </div>
        <uc:CategoryPager ID="CategoryPager" ViewAll="<%# ViewAll %>" CurrentPage="<%# CurrentPage %>" CountOnPage="9"
            Products="<%#Products %>" SortType="<%# SortType %>" CategoryID="<%# CategoryID %>" runat="server" />
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphFooter" runat="server">
    <uc:Footer WithSignIn="true" runat="server" />
</asp:Content>
