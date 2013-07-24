<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DropDownFreeItem.ascx.cs"
    Inherits="TrimFuel.Web.Admin.Controls.DropDownFreeItem" %>
<script type="text/javascript">
    <asp:Literal runat="server" ID="litProductProductCodeList" />

    $(document).ready(function(){
        $('#<%#ProductGroupID %>').change(changeFreeProduct);
        changeFreeProduct();
    });

    function changeFreeProduct() {
        $('#ddlFreeItems').html("<option value=''>-- Select --</option>");
        var selectedGroup = $('#<%#ProductGroupID %>').val();
        for (var item in productFreeList) {
            var productID = productFreeList[item].ProductID;
            if (selectedGroup == productID) {
                $('#ddlFreeItems').append("<option value='" + productFreeList[item].ExtraTrialShipID + "'>" + productFreeList[item].ProductName + "</option>");
            }
        }
    }
</script>
<select id="ddlFreeItems" name="ddlFreeItems" style="width:250px;">
    <option value="">-- Select --</option>
</select>
