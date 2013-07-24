<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Referer.aspx.cs" Inherits="Securetrialoffers.admin.EditForms.Referer_" %>
<input type="hidden" name="action" value="process" />
<div class="module" style="width: 95%">
    <% if (Referer.RefererID != null) { %>
    <h2>Edit Referer</h2>
    <% } else { %>
    <h2>Create Referer</h2>
    <% } %>
    <table border="0" cellspacing="0" cellpadding="3">
	    <tr>
		    <td>Referer ID</td>
		    <td colspan="2">
    <% if (Referer.RefererID != null) { %>				    
		        <%= Referer.RefererID %>
		        <input type="hidden" name="id" id="id" value='<%= Referer.RefererID %>' />
    <% } else { %>
                N/A
    <% } %>
		    </td>
	    </tr>
	    <tr>
		    <td>First Name</td>
		    <td colspan="2"><input name="firstname" id="firstname" value='<%= Referer.FirstName %>' maxlength="30" title="Please Enter First Name" tabindex="1" type="text" class="validate[custom[FirstName]]"></td>
	    </tr>
	    <tr>
		    <td>Last Name</td>
		    <td colspan="2"><input name="lastname" id="lastname" value='<%= Referer.LastName %>' maxlength="30" title="Please Enter Last Name" tabindex="2" type="text" class="validate[custom[LastName]]"></td>
	    </tr>
	    <tr>
		    <td>Company</td>
		    <td colspan="2"><input name="company" id="company" value='<%= Referer.Company %>' maxlength="50" title="Please Enter Company" tabindex="3" type="text"></td>
	    </tr>
	    <tr>
		    <td>Address Line 1</td>
		    <td colspan="2"><input name="address1" id="address1" value='<%= Referer.Address1 %>' maxlength="30" title="Please Enter Address Line 1" tabindex="4" type="text" class="validate[custom[Address]]"></td>
	    </tr>
	    <tr>
		    <td>Address Line 2</td>
		    <td colspan="2"><input name="address2" id="address2" value='<%= Referer.Address2 %>' maxlength="30" title="Please Enter Address Line 2" tabindex="5" type="text"></td>
	    </tr>
	    <tr>
		    <td>City</td>
		    <td colspan="2"><input name="city" id="city" value='<%= Referer.City %>' maxlength="30" title="Please Enter City" tabindex="6" type="text" class="validate[custom[City]]"></td>
	    </tr>
	    <tr>
		    <td>State</td>
		    <td colspan="2"><input name="state" id="state" value='<%= Referer.State %>' maxlength="30" title="Please Enter State" tabindex="7" type="text"></td>
	    </tr>
	    <tr>
		    <td>Zip Code</td>
		    <td colspan="2"><input name="zip" id="zip1" value='<%= Referer.Zip %>' maxlength="5" title="Please Enter Zip Code" tabindex="8" type="text" class="validate[custom[Zip]]"></td>
	    </tr>
	    <tr>
		    <td>Country</td>
		    <td colspan="2"><input name="country" id="country" value='<%= Referer.Country %>' maxlength="30" title="Please Enter State" tabindex="9" type="text"></td>
	    </tr>
	    <tr>
		    <td>Referer Code</td>
		    <td><input name="refererCode" id="refererCode" value='<%= Referer.RefererCode %>' maxlength="50" title="Please Enter Referer Code" tabindex="10" type="text" class="validate[custom[RefererCode]]"></td>
		    <td><input type='button' onclick='createRefererCode(document.getElementById("lastname"), document.getElementById("refererCode"));' value='Generate' /></td>
	    </tr>
	    <tr>
		    <td>Parent Referer</td>
		    <td colspan="2">
                <select name="parentReferer" id="parentReferer" title="Please Select Parent Referer" tabindex="11">
                <option value="">-- Select --</option>
                  <%= TrimFuel.Business.Utils.HtmlHelper.DDL(GetRefererList(), (Referer.ParentRefererID != null) ? Referer.ParentRefererID.Value.ToString() : null)%>
                </select>
		    </td>
	    </tr>
    </table>
</div>