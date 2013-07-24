<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CampaignCoupons.aspx.cs" Inherits="TrimFuel.Web.Admin.EditForms.CampaignCoupons" %>

<div id="coupon-form">
    <form runat="server">
    <input type="hidden" name="campaignID" id="campaignID" value="<%=Request["id"]%>" />
    <asp:placeholder id="ph1" runat="server" visible="false">
    <div class="module" style="width: 85%">
    <% if (Coupon != null)
       { %>
        <input type="hidden" name="couponID" id="couponID" value="<%=Coupon.CouponID%>" />
        <h2>Edit Coupon</h2>
    <% }
       else
       { %>
       <h2>Create Coupon</h2>
        <% } %>
                <table>
                    <tr>
                        <td>
                            Product
                        </td>
                        <td>
                            <select name="productId" id="productId">
                                <asp:repeater id="rProducts" runat="server">
                                <ItemTemplate>
                                        <option value="<%# Eval("ProductID") %>"><%# Eval("ProductName") %></option>                                   
                                    </ItemTemplate>
                    </asp:repeater>
                            </select>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Coupon Code
                        </td>
                        <td>
                            <input type="text" name="code" id="code" maxlength="50" value='<%=Coupon == null ? "" : Coupon.Code%>' />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Coupon Type
                        </td>
                        <td>
                            <input type="radio" id="toggleNewPrice" name="couponType" value="price" />
                            New Price
                            <input type="radio" id="toggleDiscount" name="couponType" value="discount" checked="checked" />
                            Discount
                        </td>
                    </tr>
                    <tr id="couponTypeToggle-discount" class="toHide">
                        <td>
                            Discount
                        </td>
                        <td>
                            <input type="text" name="discount" id="discount" class="narrow" maxlength="8" value='<%=Coupon == null ? "" : Coupon.Discount.ToString()%>'  />
                            %
                        </td>
                    </tr>
                    <tr id="couponTypeToggle-price" class="toHide" style="display: none;">
                        <td>
                            New Price
                        </td>
                        <td>
                            $
                            <input type="text" name="newPrice" id="newPrice" class="narrow" maxlength="8" value='<%=Coupon == null ? "" : Coupon.NewPrice.ToString()%>'  />
                        </td>
                    </tr>
                </table>
                <div class="clear">
                </div>
                <asp:button id="btCreateCoupon" runat="server" onclick="SaveCoupon" text="Save" />
                <asp:button id="btCancelCreateCoupon" runat="server" onclick="CancelSaveCoupon" text="Cancel" />
                <br />
                <asp:literal id="ltCreateCouponConfirm" runat="server" />
            </div>
    </asp:placeholder>
    <asp:placeholder id="ph2" runat="server">
    <div id="tabs">
        <ul>
            <li><a href="#tabs-1">This campaign</a></li>
            <li><a href="#tabs-2">All coupons</a></li>
        </ul>
        <div id="tabs-1">
            <div class="module" style="width: 85%">
                <h2>
                    Manage Coupons for
                    <%= Campaign.DisplayName %></h2>
                <a href="javascript:addCoupon()" class="addNewIcon">Add Coupon</a>
                <div class="toolbox" id="coupons">
                    <asp:literal id="ltCoupons" runat="server"></asp:literal>
                    <div id="coupons-prototype" style="display: none; margin-bottom: 4px;">
                        <select name="coupon" style="float: left;">
                            <option>-- Select --</option>
                            <asp:repeater id="rCoupons" runat="server">
                    <ItemTemplate>
                        <option value="<%# Eval("CouponID")  %>"><%# Eval("Code") %> (<%# CurrencyOrPercentage(Eval("Discount"), Eval("NewPrice")) %>)</option>
                    </ItemTemplate>
                </asp:repeater>
                        </select>
                        <a href="#" onclick="return removeCoupon(this);" class="removeIcon" style="float: right;">Remove</a>
                        <div class="clear">
                        </div>
                    </div>
                </div>
                <asp:button id="btManageCoupons" runat="server" onclick="SaveCampaignCoupons" text="Save" />
                &nbsp;
                <asp:literal id="ltManageCouponsConfirm" runat="server" />
            </div>
            <div class="clear">
            </div>
        </div>
        <div id="tabs-2">
        <div class="module">
            <asp:Button ID="btPrepareCreateCoupon" CommandName="create" OnCommand="DoCouponAction" runat="server" Text="Create New Coupon" />
        </div>
            <table width="95%">
                <tr class="header">
                    <td>
                        ID
                    </td>
                    <td>
                        Code
                    </td>
                    <td>
                        Value
                    </td>
                    <td>
                        Actions
                    </td>
                </tr>
                <asp:repeater id="rCouponTable" runat="server" onitemcommand="DoCouponAction">
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("CouponID")  %></td>
                                <td><%# Eval("Code") %></td>
                                <td><%# CurrencyOrPercentage(Eval("Discount"), Eval("NewPrice")) %></td>
                                <td>
                                    <asp:LinkButton ID="lnkEditCoupon" runat="server" Text="Edit" CommandName="edit" CommandArgument='<%# Eval("CouponID")  %>'></asp:LinkButton>
                                    &nbsp;
                                    <asp:LinkButton CssClass="confirm" Title="Warning: this action will delete the coupon from all flows." ID="lnkDeleteCoupon" runat="server" Text="Delete" CommandName="delete" CommandArgument='<%# Eval("CouponID")  %>'></asp:LinkButton>
                                </td>
                            </tr>
                    </ItemTemplate>
                    </asp:repeater>
            </table>
            <div class="clear">
            </div>
        </div>
    </div>
    </asp:placeholder>
    </form>
</div>
<script type="text/javascript">
    $(document).ready(function () {
        $('#tabs').tabs();

        $(function () {
            $("[name=couponType]").click(function () {
                $('.toHide').hide();
                $("#couponTypeToggle-" + $(this).val()).show();
            });
        });
    });

    function addCoupon() {
        $("#coupons-prototype").clone().attr("id", "").appendTo($("#coupons")).show();
    }

    function removeCoupon(el) {
        $(el).parent().remove();
        return false;
    }
</script>
