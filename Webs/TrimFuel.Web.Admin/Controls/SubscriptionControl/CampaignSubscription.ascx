<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CampaignSubscription.ascx.cs" Inherits="TrimFuel.Web.Admin.Controls.SubscriptionControl.CampaignSubscription" %>
<%@ Register Assembly="TrimFuel.Web.UI" Namespace="TrimFuel.Web.UI.Specialized" TagPrefix="cc1" %>
<%@ Register src="Subscription.ascx" tagname="Subscription" tagprefix="uc1" %>
<div class="module" style="border:0 !important;">
<h2>Subscription</h2>
<table class="subscription-control editForm" id="<%# GenerateID %>">
    <uc1:Subscription ID="Subscription1" runat="server" />
</table>
<h2>Trial</h2>
<table class="editForm" id="subscription-trial-products">
    <tr><td>Price: (e.g. 1.95)</td><td>
        <asp:TextBox runat="server" ID="tbPrice" style="width:40px;" MaxLength="6"></asp:TextBox>&nbsp;&nbsp;
    </td>
    <td>Trial Period:</td><td>
        <asp:TextBox runat="server" ID="tbTrialInterim" style="width:40px;"></asp:TextBox> days
    </td></tr>
    <asp:PlaceHolder runat="server" ID="phProducts">
    <tr><td style="width:70px;" valign="top">Products</td><td colspan="3" nowrap>
        <a href="javascript:addProduct()" class="addNewIcon">Add Product</a>
        <div id="products">
	        <div style="margin:5px;width:100%;display:none;" id="product-prototype">
		        <input type="text" name="quantity" id="quantity-prototype" style="width: 30px;" class="xnarrow prototype" maxlength="2" value="1"/>&nbsp;
		        <select name="inventory" id="inventory-prototype" style="width:150px;" class="prototype">
			        <option value="">-- Select --</option>
			        <%# InventoryOptionList("") %>
		        </select>
		        <a href="#" onclick="return removeProduct(this);" class="removeIcon shipping-dependent">Remove</a>
	        </div>
	        <asp:Repeater runat="server" ID="rSelectedInventories" DataSource="<%# SelectedInventoryList %>">
	            <ItemTemplate>
		            <div style="margin:5px;width:100%;">
			            <input type="text" name="quantity" style="width: 30px;" class="xnarrow" maxlength="2" value='<%# Eval("Value") %>'/>&nbsp;
			            <select name="inventory" style="width:150px;">
				            <option value="">-- Select --</option>
				            <%# InventoryOptionList(Convert.ToString(Eval("Key"))) %>
			            </select>
			            <a href="#" onclick="return removeProduct(this);" class="removeIcon">Remove</a>
		            </div>
	            </ItemTemplate>
	        </asp:Repeater>
	        <div class="clear"></div>
        </div>
    </td></tr>
    </asp:PlaceHolder>
</table>
</div>
<div class="clear"></div>
<script type="text/javascript">
    var newSubscriptionFormId = '#subscription-trial-products';
    $(document).ready(function () {
        obtainSubscriptionControl("<%# GenerateID %>");
    });

    function setIDs(selector) {
        $(selector).find("input[name|=quantity]").attr("id", "quantity" + randomString());
        $(selector).find("select[name|=inventory]").attr("id", "inventory" + randomString());
    }

    function removeProduct(el) {
        $(el).parent().remove();
        return false;
    }

    function randomString() {
        var ABC, temp, randomID, a;
        ABC = "0123456789";
        temp = ""
        for (a = 0; a < 6; a++) {
            randomID = Math.random();
            randomID = Math.floor(randomID * ABC.length);
            temp += ABC.charAt(randomID);
        }
        return temp;
    }

    function addProduct() {
        var added = $(newSubscriptionFormId + " #products #product-prototype").clone().attr("id", "").appendTo($(newSubscriptionFormId + " #products")).show();
        setIDs(added);
        added.find(".donotvalidate").removeClass("donotvalidate");
        added.find(".prototype").removeClass("prototype");
    }
</script>