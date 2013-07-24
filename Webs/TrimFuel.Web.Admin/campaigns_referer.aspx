<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/Admin.Master" AutoEventWireup="true" CodeBehind="campaigns_referer.aspx.cs" Inherits="TrimFuel.Web.Admin.campaigns_referer" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphScript" runat="server">
<script type="text/javascript" language=javascript>
    function editReferer(Id)
    {
        editForm("EditForms/Referer.aspx?id=" +Id, 400, "campaigns_referer.aspx");
    }

    function newReferer()
    {
        editForm("EditForms/Referer.aspx", 400, "campaigns_referer.aspx");
    }
    function createRefererCode(obj1, obj2) {
        var ABC, temp, randomID, a;
        ABC = "0123456789";
        temp = ""
        for (a = 0; a < 4; a++) {
            randomID = Math.random();
            randomID = Math.floor(randomID * ABC.length - 1);
            temp += ABC.charAt(randomID);
        }
        obj2.value = obj1.value + temp;
    }
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphStyle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
<div id="toggle" class="section">
	<a href="#">
	<h1>Referers</h1>
	</a>
	<div>
		<div class="module">
			<h2>Create Referer</h2>
			<input type="button" value="Create" onclick="newReferer()" />
		</div>
		<div class="clear"></div>
		<div class="data">
            <table class="process-offets sortable add-csv-export" border="0" cellspacing="1" width="100%">
                <tr class="header">
                    <td>ID</td>
                    <td>Name</td>
                    <td>Company</td>
                    <td>Address1</td>
                    <td>Address2</td>
                    <td>City</td>
                    <td>State</td>
                    <td>Zip</td>
                    <td>Country</td>
                    <td>Referer Code</td>
                    <td></td>
                </tr>
                <asp:Repeater runat="server" ID="rReferers">
                    <ItemTemplate>
                        <tr>
                            <td><a target="_blank" href='referer_sales.aspx?refererId=<%# Eval("RefererID") %>'><%# Eval("RefererID") %></a></td>
                            <td><%# DataBinder.Eval(Container.DataItem, "FirstName") %> <%# DataBinder.Eval(Container.DataItem, "LastName") %></td>
                            <td><%# DataBinder.Eval(Container.DataItem, "Company") %></td>
                            <td><%# DataBinder.Eval(Container.DataItem, "Address1") %></td>
                            <td><%# DataBinder.Eval(Container.DataItem, "Address2") %></td>
                            <td><%# DataBinder.Eval(Container.DataItem, "City") %></td>
                            <td><%# DataBinder.Eval(Container.DataItem, "State") %></td>
                            <td><%# DataBinder.Eval(Container.DataItem, "Zip") %></td>
                            <td><%# DataBinder.Eval(Container.DataItem, "Country") %></td>
                            <td><%# DataBinder.Eval(Container.DataItem, "RefererCode") %></td>
                            <td><a href="javascript:editReferer(<%# DataBinder.Eval(Container.DataItem, "RefererID") %>)" class="editIcon">Edit</a></td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>
		</div>
		<div class="clear">
		</div>
	</div>
</div>
</asp:Content>
