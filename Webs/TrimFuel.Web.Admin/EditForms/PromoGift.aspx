<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="_PromoGift.aspx.cs" Inherits="TrimFuel.Web.Admin.EditForms._PromoGift" %>

<form runat="server">
<input type="hidden" name="action" value="process" />
<input type="hidden" name="action" value="<%= giftNumber %>" />
<div class="module" style="width: 95%"<%= hide %>>
    <table>
        <tr>
            <td>
                Gift Number:
            </td>
            <td>
                <%= giftNumber %>
            </td>
        </tr>
        <tr>
            <td>
                Value:
            </td>
            <td>
                <input type="text" name="price" id="price" class="narrow validate[custom[Amount]]"
                    maxlength="8" value="<%= giftValue  %>" />
            </td>
        </tr>
        <tr>
            <td>
                Remaining Value:
            </td>
            <td>
                <input type="text" name="remainingPrice" id="remainingPrice" class="narrow validate[custom[Amount]]"
                    maxlength="8" value="<%= remainingGiftValue %>" />
            </td>
        </tr>
    </table>
    <div class="clear">
    </div>
    <input type="submit" value="Save" name="button" />
    &nbsp;&nbsp;&nbsp;
    <input type="submit" value="Delete" name="button" onclick="return confirm('Are you sure?');" />
    <br />
</div>
<span id="promo-status"></span>
<%= statusMessage %>
</form>
