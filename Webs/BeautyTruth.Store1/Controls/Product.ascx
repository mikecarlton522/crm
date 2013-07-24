<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Product.ascx.cs" Inherits="BeautyTruth.Store1.Controls.Product" %>
<div class="grid_3 <%#AdditionalClass %>">
    <div class="item">
        <h1>
            <%#Title %></h1>
        <a href="product.aspx?productCode=<%#ProductCode %>" >
            <img src="<%#Photo %>" width="150" height="150" /></a>
        <p>
            <%#Description %>
        </p>
        <div class="button">
            <a href="product.aspx?productCode=<%#ProductCode %>">Get More Info</a>
        </div>
    </div>
</div>
