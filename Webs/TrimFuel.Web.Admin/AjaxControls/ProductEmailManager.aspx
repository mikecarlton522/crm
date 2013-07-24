﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProductEmailManager.aspx.cs" Inherits="TrimFuel.Web.Admin.AjaxControls.ProductEmailManager" EnableViewState="false" %>
<div id="product-email-type-list-container" style="float:left; width:400px;"><img src="../images/loading2.gif" /></div>
<div id="dynamic-email-edit-container" style="float:left; margin-left: 20px;"></div>
<script type="text/javascript">
    $(document).ready(function() {
        loadEmailTypeList();
    });

    function loadEmailTypeList() {
        var url = "ajaxControls/ProductEmailTypeList.aspx?productID=" + <%# ProductID %>;
        inlineControl(url, "product-email-type-list-container");
    }
        inlineControl(url, "dynamic-email-edit-container", loadEmailTypeList);
    }
        inlineControl(url, "dynamic-email-edit-container", loadEmailTypeList);
    }