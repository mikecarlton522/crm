<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/Admin.Master" AutoEventWireup="true" CodeBehind="management_label.aspx.cs" Inherits="TrimFuel.Web.Admin.management_label" %>
<%@ Register src="Controls/TagList.ascx" tagname="TagList" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphScript" runat="server">
<script type="text/javascript" src="../js/billing-tag.js"></script>
<script type="text/javascript">
    $(document).ready(function() {
        obtainTagList();
    });
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphStyle" runat="server">
<link rel="stylesheet" href="../css/billing-tag.css" type="text/css" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
	<div id="toggle" class="section">		
		<a href="#"><h1>User Tag List</h1></a>
		<div class="data">		    
        <uc1:TagList ID="TagList1" runat="server" />
		</div>
	</div>
</asp:Content>
