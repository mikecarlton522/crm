var shoppingCart = shoppingCart || {};
shoppingCart.options = {
    shoppingCartCookieName: "shoppingCart"
};
shoppingCart.obtainProductElement = function(el, idProductID, idProductCount, idAddButton, idShoppingCartCount, shoppingCartUrl) {
    $(el).find("[id|=" + idAddButton + "]").click(function() {
        var productID = $(el).find("[id|=" + idProductID + "]").val();
        if (!productID) {
            productID = $(el).find("[id|=" + idProductID + "]").attr("value");
        }
        var productCount = $(el).find("[id|=" + idProductCount + "]").val();
        if (!productCount) {
            productCount = $(el).find("[id|=" + idProductCount + "]").attr("value");
        }
        if (!productCount) {
            productCount = 1;
        }
        var productLimit = $(el).find("[id|=" + idProductCount + "]").attr("product-limit");
        if (!productLimit) {
            productLimit = 1000000;
        }
        
        if (shoppingCart.getProductCount(productID) >= productLimit) {
            shoppingCart.showExceededMsg(shoppingCartUrl, productLimit);
        }
        else {
            shoppingCart.addProduct(productID, productCount, function() {
                $("#" + idShoppingCartCount).html(shoppingCart.productCount);
                shoppingCart.showAddedMsg(shoppingCartUrl);
            });
        }
        return false;
    });
};
shoppingCart.addProduct = function(productID, count, productAddedCallback) {
    var productCookie = $.cookie(shoppingCart.options.shoppingCartCookieName);
    var productList = productCookie ? productCookie.split(/,/) : new Array();
    count = parseInt(count);
    if (typeof (count) == "number") {
        for (var i = 0; i < count; i++) {
            productList.push(productID);
            $.cookie(shoppingCart.options.shoppingCartCookieName, productList, { path: "/", expires: 365 });
        }
        if (productAddedCallback) {
            productAddedCallback();
        }
    }
};
shoppingCart.getProductCount = function(productID) {
    var productCookie = $.cookie(shoppingCart.options.shoppingCartCookieName);
    var productList = productCookie ? productCookie.split(/,/) : new Array();
    var res = 0;
    for (var key in productList) {
        if (productList[key] == productID) {
            res = res + 1;
        }
    }
    return res;
};
shoppingCart.productCount = function() {
    var productCookie = $.cookie(shoppingCart.options.shoppingCartCookieName);
    var productList = productCookie ? productCookie.split(/,/) : new Array();
    return productList.length;
};
shoppingCart.createMsg = function(msgId, msg, shoppingCartUrl) {
    //    var dlg = $("<div id=\"" + msgId + "\"><p>" +
    //        msg + "</p>" +
    //        "<table width=\"100%\"><tr>" +
    //        "<td><span class=\"back button inline\"><a href=\"#\" class=\"buttoncolor1\">&laquo; Keep Shopping</a></span></td>" +
    //        "<td align=\"right\"><span class=\"checkout button inline\"><a href=\"#\">Go To Shopping Cart &raquo;</a></span></td>" +
    //        "</tr></table></div>")
    //        .appendTo(document.body)
    //        .dialog({
    //            autoOpen: false,
    //            draggable: false,
    //            position: "center",
    //            resizable: false,
    //            height: 90,
    //            width: 350,
    //            modal: true
    //        });
    var dlg = $("#shoppingCartPopup");
    dlg.find(".back > a").click(function() {
        dlg.dialog('close');
        return false;
    });
    dlg.find(".checkout > a").click(function() {
        document.location = shoppingCartUrl;
        return false;
    });
    return dlg;
};
shoppingCart.showAddedMsg = function(shoppingCartUrl) {
    if ($("#product-added-msg").length == 0) {
        shoppingCart.createMsg("product-added-msg", "Product was added to your shopping cart.", shoppingCartUrl);
    }
    $("#product-added-msg").dialog("open");
};
shoppingCart.showExceededMsg = function(shoppingCartUrl, limit) {
    if ($("#product-exceed-msg").length == 0) {
        shoppingCart.createMsg("product-exceed-msg", "Sorry, but only " + limit + " this product per order is allowed. Product is already added to your card.", shoppingCartUrl);
    }
    $("#product-exceed-msg").dialog("open");
};
$(document).ready(function() {
    $("[id|=product-el]").each(function() {
        shoppingCart.obtainProductElement(this, "product-id", "product-count", "add-product", "shopping-cart-count", "cart.aspx");
    });
});