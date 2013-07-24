<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/Admin.Master" AutoEventWireup="true"
    CodeBehind="management_coupons.aspx.cs" Inherits="TrimFuel.Web.Admin.management_coupons" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphScript" runat="server">
    <script type="text/javascript" language="javascript">
        function editCoupon(Id) {
            editForm("EditForms/Coupon.aspx?id=" + Id, 550, "management_coupons.aspx");
        }

        function createCoupon() {
            editForm("EditForms/Coupon.aspx", 550, "management_coupons.aspx");
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphStyle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <div class="module">
        <h2>
            Create Coupon</h2>
        <input type="button" value="Create" onclick="createCoupon()" />
    </div>
    <div class="clear">
    </div>
    <div class="data">
        <form runat="server">
        <table class="process-offets sortable add-csv-export" border="0" cellspacing="1"
            width="100%">
            <tr class="header">
                <td>
                    Coupon ID
                </td>
                <td>
                    Product ID
                </td>
                <td>
                    Code
                </td>
                <td>
                    Discount
                </td>
                <td>
                    New Price
                </td>
                <td>
                </td>
            </tr>
            <asp:Repeater ID="rCoupons" runat="server" OnItemCommand="DoCouponAction">
                <ItemTemplate>
                    <tr>
                        <td>
                            <%# Eval("CouponID") %>
                        </td>
                        <td>
                            <%# DataBinder.Eval(Container.DataItem, "ProductID")%>
                        </td>
                        <td>
                            <%# DataBinder.Eval(Container.DataItem, "Code")%>
                        </td>
                        <td>
                            <%# DataBinder.Eval(Container.DataItem, "Discount")%>
                        </td>
                        <td>
                            <%# DataBinder.Eval(Container.DataItem, "NewPrice")%>
                        </td>
                        <td>
                            <a href="javascript:editCoupon(<%# DataBinder.Eval(Container.DataItem, "CouponID") %>)">
                                Edit</a> &nbsp;&nbsp;|&nbsp;&nbsp;
                            <asp:LinkButton OnClientClick="return confirm('Are you certain you want to delete this coupon?');" CausesValidation="False" runat="server"
                                ID="lbDelete" CommandName="delete" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "CouponID") %>'>Delete</asp:LinkButton>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>
        </form>
    </div>
</asp:Content>
