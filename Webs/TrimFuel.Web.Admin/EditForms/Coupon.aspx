<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Coupon.aspx.cs" Inherits="TrimFuel.Web.Admin.EditForms.Coupon_" %>

<input type="hidden" name="action" value="process" />
<div class="module" style="width: 95%">
    <% if (Coupon.CouponID != null)
       { %>
    <h2>
        Edit Coupon</h2>
    <% }
       else
       { %>
    <h2>
        Create Coupon</h2>
    <% } %>
    <table class="editForm" border="0" cellspacing="1" cellpadding="0">
    	    <tr>
		    <td>Coupon ID</td>
		    <td colspan="2">
    <% if (Coupon.CouponID != null) { %>				    
		        <%= Coupon.CouponID %>
		        <input type="hidden" name="id" id="id" value='<%= Coupon.CouponID %>' />
    <% } else { %>
                N/A
    <% } %>
		    </td>
	    </tr>
        <tr>
            <td>
                Product
            </td>
            <td>
                <select name="productId">
                    <asp:Repeater ID="rProducts" runat="server">
                        <ItemTemplate>
                            <option value="<%# Eval("ProductID") %>"<%# Convert.ToInt32(Eval("ProductID")) == Coupon.ProductID ? " selected" : "" %>><%# Eval("ProductName") %></option>
                        </ItemTemplate>
                    </asp:Repeater>
                </select>
            </td>
        </tr>
        <tr>
        <td>Code</td>
        <td><input type="text" name="code" id="code" class="validate[custom[Coupon]]" maxlength="50" value="<%= Coupon.Code %>" /></td>
        </tr>
        <tr>
            <td>
                Discount
            </td>
            <td>
                <input type="text" name="discount" id="discount" class="narrow validate[custom[Amount?]]" maxlength="8" value="<%= Coupon.Discount.ToString()  %>" />
            </td>
        </tr>
        <tr>
            <td>
                New Price
            </td>
            <td>
                <input type="text" name="newPrice" id="newPrice" class="narrow validate[custom[Amount?]]" maxlength="8" value="<%= Coupon.NewPrice.ToString()  %>" />
            </td>
        </tr>
    </table>