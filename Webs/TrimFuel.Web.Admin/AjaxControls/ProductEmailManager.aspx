<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProductEmailManager.aspx.cs" Inherits="TrimFuel.Web.Admin.AjaxControls.ProductEmailManager" EnableViewState="false" %>
<div id="product-email-type-list-container" style="float:left; width:400px;"><img src="../images/loading2.gif" /></div>
<div id="dynamic-email-edit-container" style="float:left; margin-left: 20px;"></div>
<script type="text/javascript">
    $(document).ready(function() {
        loadEmailTypeList();
    });

    function loadEmailTypeList() {
        var url = "ajaxControls/ProductEmailTypeList.aspx?productID=" + <%# ProductID %>;
        inlineControl(url, "product-email-type-list-container");
    }        function onCreateEmail(dynamicEmailTypeID, hours, giftCertificateDynamicEmail_StoreID) {        $("#dynamic-email-edit-container").html("<img src=\"../images/loading2.gif\"/>");        var url = "ajaxControls/DynamicEmail.aspx?productID=" + <%# ProductID %> + "&dynamicEmailTypeID=" + dynamicEmailTypeID + "&hours=" + hours + "&storeID=" + giftCertificateDynamicEmail_StoreID;
        inlineControl(url, "dynamic-email-edit-container", loadEmailTypeList);
    }        function onEditEmail(dynamicEmailID) {        $("#dynamic-email-edit-container").html("<img src=\"../images/loading2.gif\"/>");        var url = "ajaxControls/DynamicEmail.aspx?dynamicEmailID=" + dynamicEmailID;
        inlineControl(url, "dynamic-email-edit-container", loadEmailTypeList);
    }</script>