<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Product.aspx.cs" Inherits="BeautyTruth.Store1.Product"
    MasterPageFile="~/Controls/Front.Master" %>

<%@ Register TagPrefix="uc" TagName="Footer" Src="~/Controls/Footer.ascx" %>
<%@ Register TagPrefix="uc" TagName="CategoryMenu" Src="~/Controls/CategoryMenu.ascx" %>
<%@ Register TagPrefix="uc" TagName="AlsoBought" Src="~/Controls/AlsoBought.ascx" %>
<%@ Register TagPrefix="uc" TagName="ProductReviews" Src="~/Controls/ProductReviews.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    Beauty &amp; Truth: Anti-Aging Wrinkle Creams & Skin Care for Face and Eye</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphScript" runat="server">

    <script type="text/javascript">
        $(document).ready(function() {
            $("a#product_large_img_<%#ProductCode %>").fancybox();
        });
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <div class="container_12">
        <uc:CategoryMenu ID="CategoryMenu" runat="server" />
        <div class="grid_9" id="breadcrumb">
            <ul>
                <li><a href="default.aspx">Home</a></li>
                <li>&raquo;</li>
                <li><a href="category.aspx">Products</a></li>
                <li>&raquo;</li>
                <li><a href="category.aspx?categoryID=<%#CategoryID %>">
                    <%# CategoryName %></a></li>
                <li>&raquo;</li>
                <li><a href="product.aspx?productCode=<%#ProductCode %>" class="highlight">
                    <%# ProductTitle%></a></li>
            </ul>
        </div>
        <div class="grid_9 sort">
            <div>
                <h1>
                    &laquo; <a href="javascript:history.back();">Back</a></h1>
            </div>
        </div>
        <div class="grid_9">
            <div class="grid_9 alpha" id="product">
                <div class="grid_3 alpha">
                    <div id="image">
                        <a href="<%# LargePhoto %>" id="product_large_img_<%#ProductCode %>">
                            <img src="<%# Photo %>" width="150" height="150" />
                        </a>
                    </div>
                </div>
                <div class="grid_6 omega">
                    <div id="logo">
                        <img src="images/logo-black.png" width="270" height="34" />
                    </div>
                    <h1>
                        <%#ProductTitle %></h1>
                    <p>
                        <%#Description%>
                    </p>
                </div>
            </div>
            <div class="grid_9 alpha">
                <div class="cart">
                    <table width="100%">
                        <tr>
                            <th width="20%">
                                Order Type
                            </th>
                            <th width="14%">
                                Price
                            </th>
                            <th width="20%">
                                Availability
                            </th>
                            <th width="22%">
                                Quantity
                            </th>
                            <th width="24%">
                                Action
                            </th>
                        </tr>
                        <asp:Repeater ID="rProducts" runat="server" DataSource="<%# Products %>">
                            <ItemTemplate>
                                <tr id="product-el-<%#Eval("ProductCodeID") %>">
                                    <td>
                                        <input type="hidden" id="product-id-<%#Eval("ProductCodeID") %>" value="product_<%#Eval("ProductCodeID") %>" />
                                        One Time Order
                                    </td>
                                    <td>
                                        <%# FormatPrice(ProductCodeRetailPrice)%>
                                    </td>
                                    <td>
                                        In Stock!
                                    </td>
                                    <td>
                                        <input name="quantity" id="product-count-<%#Eval("ProductCodeID") %>" type="text"
                                            size="2" maxlength="2" value="1" />
                                    </td>
                                    <td>
                                        <div class="button">
                                            <a href="#shoppingCartPopup" id="add-product-<%#Eval("ProductCodeID") %>" class="copy12orange">
                                                <strong>Add to Cart</strong></a>

                                            <script type="text/javascript">
                                                $(document).ready(function() {
                                                    $('#add-product-<%#Eval("ProductCodeID") %>').fancybox({
                                                        'titlePosition': 'inside',
                                                        'transitionIn': 'none',
                                                        'transitionOut': 'none'
                                                    });
                                                });
                                            </script>

                                        </div>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                        <% 
                            if (Subscription.SubscriptionID != null)
                            {
                        %>
                        <tr id="product-el-s">
                            <td>
                                <input type="hidden" id="product-id-s" value="subscription_<%# Subscription.SubscriptionID %>" />
                                <%# Subscription.InitialInterim%>
                                Day Trial
                            </td>
                            <td>
                                <%# FormatPrice((Subscription.InitialBillAmount ?? 0) + (Subscription.InitialShipping ?? 0))%>
                            </td>
                            <td>
                                In Stock!
                            </td>
                            <td>
                                <input name="quantity" id="product-count-s" type="text" size="2" maxlength="2" value="1"
                                    disabled="disabled" readonly="readonly" />
                                Limit of One
                            </td>
                            <td>
                                <div class="button">
                                    <a href="#shoppingCartPopup" id="add-product-s" class="copy12orange"><strong>Add to
                                        Cart</strong></a>

                                    <script type="text/javascript">
                                        $(document).ready(function() {
                                            $('#add-product-s').fancybox({
                                                'titlePosition': 'inside',
                                                'transitionIn': 'none',
                                                'transitionOut': 'none'
                                            });
                                        });
                                    </script>

                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="5" style="color: #777777;">
                                <%# Subscription.InitialInterim%>
                                Day Trial: Unless you call to cancel, in
                                <%# Subscription.InitialInterim%>
                                days you will be charged
                                <%# FormatPrice((Subscription.SecondBillAmount ?? 0) + (Subscription.SecondShipping ?? 0))%>
                                for your trial and every
                                <%# Subscription.RegularInterim%>
                                days thereafter you will be sent a full size bottle of
                                <%#ProductTitle %>
                                for only
                                <%# FormatPrice(Subscription.RegularBillAmount ?? 0) %>
                                +
                                <%# FormatPrice(Subscription.RegularShipping ?? 0)%>
                                S/H. To modify your order at anytime call 1-866-830-2464.
                            </td>
                        </tr>
                        <% 
                            } 
                        %>
                    </table>
                </div>
                <% 
                    if (SalesContext != null && SalesContext.Count > 0)
                    {
                %>
                <div id="tabs">
                    <ul>
                        <asp:Repeater runat="server" DataSource="<%# SalesContext %>">
                            <ItemTemplate>
                                <li><a href='#tab_<%# DataBinder.Eval(Container,"ItemIndex") %>'>
                                    <%# Eval("Key") %></a></li>
                            </ItemTemplate>
                        </asp:Repeater>
                    </ul>
                    <asp:Repeater ID="Repeater1" runat="server" DataSource="<%# SalesContext %>">
                        <ItemTemplate>
                            <div id='tab_<%# DataBinder.Eval(Container,"ItemIndex") %>'>
                                <h1>
                                    <%# Eval("Key") %></h1>
                                <%# Eval("Value") %>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
                <% 
                    } 
                %>
            </div>
            <uc:AlsoBought ID="AlsoBought" GridClass="grid_9 alpha" CountToShow="3" ProductCode="<%#ProductCode %>"
                runat="server" />
            <uc:ProductReviews runat="server" WebStoreProductID="<%# WebStoreProductID %>" />
        </div>
    </div>
    <div style="display: none;">
        <div id="shoppingCartPopup" align="center">
            <p>
                Product was added to your shopping cart.</p>
            <table width="80%">
                <tr>
                    <td>
                        <span class="back button inline\"><a href="javascript:void();" onclick="$.fancybox.close();"
                            class="buttoncolor1">&laquo; Keep Shopping</a></span>
                    </td>
                    <td align="right">
                        <span class="checkout button inline"><a href="#">Go To Shopping Cart &raquo;</a></span>
                    </td>
                </tr>
            </table>
        </div>
    </div>
</asp:Content>
<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <uc:Footer WithSignIn="true" runat="server" />
</asp:Content>
