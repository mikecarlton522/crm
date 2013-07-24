<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="product_manager.aspx.cs"
    Inherits="TrimFuel.Web.Admin.product_manager" MasterPageFile="~/Controls/Admin.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphScript" runat="server">

    <script type="text/javascript" src="/ckeditor/ckeditor.js"></script>

    <script type="text/javascript">

        function loadMenu() {
            inlineControl("ajaxControls/ProductMenu.aspx", "left", null, function() { SetToggles("#left"); });
        }

        $(document).ready(function() {
            loadMenu();
        });

        function showSetting(type, productId, productName, typeName) {
            $("#right").html("<img src='../images/loading2.gif' />");
            var url = "";
            if (type == "email") {
                url = "ajaxControls/ProductEmailManager.aspx?productId=" + productId;
            }
            if (type == "lead") {
                url = "ajaxControls/ProductLeadManager.aspx?productId=" + productId;
            }
            if (type == "shipper") {
                url = "ajaxControls/ProductShipperManager.aspx?productId=" + productId;
            }
            if (type == "mid") {
                url = "ajaxControls/ProductMIDManager.aspx?productId=" + productId;
            }
            if (type == "subscription") {
                url = "/controls/subscription_manager.asp?staticProductId=" + productId;
            }
            if (type == "routing") {
                url = "/controls/product_domain_routing_manager.asp?staticProductId=" + productId;
            }
            if (type == "flow") {
                url = "ajaxControls/ProductFlowManager.aspx?productId=" + productId;
            }
            if (type == "events") {
                url = "ajaxControls/ProductEventsManager.aspx?productId=" + productId;
            }
            if (type == "docs") {
                url = "ajaxControls/ProductDocumentation.aspx?productId=" + productId;
            }
            if (type == "inventory") {
                url = "ajaxControls/ProductListManager.aspx?productId=" + productId;
            }
            inlineControl(url, "right", null,
            function() {
                $("#right").prepend("<h2>" + productName + " " + typeName + "</h2>");
            });
        }
        function showProduct(productId, productName) {
            inlineControl("ajaxControls/ProductManager.aspx?productId=" + productId, "right", loadMenu,
            function() {
                $("#right").prepend("<h2>" + productName + "</h2>");
            });
        }
        function addProduct() {
            inlineControl("ajaxControls/ProductManager.aspx", "right", loadMenu,
            function() {
                $("#right").prepend("<h2>Add Product Group</h2>");
            });
        }
        function createProductGroup() {
            inlineControl("ajaxControls/CreateNewProductGroup.aspx", "right", loadMenu,
            function () {
                $("#right").prepend("<h2>Create New Product Group Based On</h2>");
            });
        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphStyle" runat="server">
    <link href="../css/productManager.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <div id="toggle" class="section">
        <a href="#">
            <h1>
                Product Group Manager</h1>
        </a>
        <div class="data productManager">
            <table style="width: 100%; border: 0;">
                <tr>
                    <td id="left">
                    </td>
                    <td id="right">
                    </td>
                </tr>
            </table>
        </div>
    </div>
</asp:Content>
