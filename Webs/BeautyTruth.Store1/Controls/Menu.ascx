<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Menu.ascx.cs" Inherits="BeautyTruth.Store1.Controls.Menu" %>
<div class="container_12">
    <div class="grid_12" id="subheader">
        <div id="left">
            <p>
                <strong>FREE Standard Shipping</strong> on orders of $75 or more to USA and Canada!</p>
        </div>
        <div id="right">
            <span id="twitter" style="width: 98px;"><a href="https://twitter.com/share" class="twitter-share-button"
                data-count="horizontal" data-via="FindBeautyTruth">Tweet</a><script type="text/javascript"
                    src="//platform.twitter.com/widgets.js"></script>

            </span><span id="facebook" style="width: 90px;">
                <iframe src="//www.facebook.com/plugins/like.php?href=https%3A%2F%2Fwww.facebook.com%2FFindBeautyAndTruth&amp;send=false&amp;layout=button_count&amp;width=450&amp;show_faces=false&amp;action=like&amp;colorscheme=light&amp;font&amp;height=21&amp;appId=165260010222496"
                    scrolling="no" frameborder="0" style="border: none; overflow: hidden; width: 90px;
                    height: 21px;" allowtransparency="true"></iframe>
            </span>
        </div>
    </div>
</div>
<div class="container_12">
    <div class="grid_12" id="nav">
        <ul>
            <li><a href="<%# RealativePathPrefix %>default.aspx">Home</a></li>
            <li><a href="<%# RealativePathPrefix %>category.aspx">Skincare</a></li>
            <li><a href="<%# RealativePathPrefix %>category.aspx?categoryID=3">Beauty Bundles</a></li>
            <li class="separator"></li>
            <li><a href="<%# RealativePathPrefix %>philosophy.aspx">Our Philosophy</a></li>
            <%--<li><a href="<%# RealativePathPrefix %>testimonial.aspx">Customer Testimonials</a></li>--%>
        </ul>
    </div>
</div>
