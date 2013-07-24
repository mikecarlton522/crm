<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CategoryMenu.ascx.cs"
    Inherits="BeautyTruth.Store1.Controls.CategoryMenu" %>
<div class="grid_3" id="menu">
    <asp:Repeater ID="rCategories" runat="server" DataSource='<%# CategoriesEx %>'>
        <ItemTemplate>
            <h1>
                <a href='category.aspx?categoryID=<%#Eval("Key.ProductCategoryID") %>'>
                    <%#Eval("Key.CategoryName")%></a></h1>
            <ul>
                <asp:Repeater ID="rProducts" runat="server" DataSource='<%#Eval("Value") %>'>
                    <ItemTemplate>
                        <li><a href='product.aspx?productCode=<%# Eval("ProductCode_")%>'><%#Eval("Title") %></a></li>
                    </ItemTemplate>
                </asp:Repeater>
            </ul>
        </ItemTemplate>
    </asp:Repeater>
</div>
