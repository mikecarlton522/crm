<%@ Page Title="" Language="C#" MasterPageFile="~/admin/Controls/Admin.Master" AutoEventWireup="true" CodeBehind="referer_management.aspx.cs" Inherits="Securetrialoffers.admin.referer_management" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphScript" runat="server">
<link href="css/validationEngine.jquery.css" rel="stylesheet" type="text/css"><script type="text/javascript" src="js/tabber-minimized.js"></script>
<script type="text/javascript" src="js/scripts.js"></script>
<script src="js/jquery.validationEngine.validationRules.js" type="text/javascript" language="javascript"></script><script src="js/jquery.validationEngine.js" type="text/javascript" language="javascript"></script><script type="text/javascript" language=javascript>
    function editReferer(Id)
    {
        editForm("EditForms/Referer.aspx?id=" +Id, 400, "referer_management.aspx");
    }

    function newReferer()
    {
        editForm("EditForms/Referer.aspx", 400, "referer_management.aspx");
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
<style type="text/css">
.sortable tr:hover 
{
    background-color: Silver;
}
</style>    
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div style="width:1000px;text-align:left;">
<hr />
<a href="javascript:newReferer()">Add New Referer</a>
<hr />
<table border="0" cellspacing="1" width="90%" class="sortable">
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
                <td><%# DataBinder.Eval(Container.DataItem, "RefererID") %></td>
                <td><%# DataBinder.Eval(Container.DataItem, "FirstName") %> <%# DataBinder.Eval(Container.DataItem, "LastName") %></td>
                <td><%# DataBinder.Eval(Container.DataItem, "Company") %></td>
                <td><%# DataBinder.Eval(Container.DataItem, "Address1") %></td>
                <td><%# DataBinder.Eval(Container.DataItem, "Address2") %></td>
                <td><%# DataBinder.Eval(Container.DataItem, "City") %></td>
                <td><%# DataBinder.Eval(Container.DataItem, "State") %></td>
                <td><%# DataBinder.Eval(Container.DataItem, "Zip") %></td>
                <td><%# DataBinder.Eval(Container.DataItem, "Country") %></td>
                <td><%# DataBinder.Eval(Container.DataItem, "RefererCode") %></td>
                <td><a href="javascript:editReferer(<%# DataBinder.Eval(Container.DataItem, "RefererID") %>)">Edit</a></td>
            </tr>
        </ItemTemplate>
    </asp:Repeater>
</table>
</div>
</asp:Content>
