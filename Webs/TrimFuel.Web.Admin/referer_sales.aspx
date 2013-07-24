<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/Admin.Master" AutoEventWireup="true" CodeBehind="referer_sales.aspx.cs" Inherits="TrimFuel.Web.Admin.referer_sales" %>
<%@ Register src="~/Controls/RefererSales.ascx" tagname="RefererSales" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphScript" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphStyle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
<div id="toggle" class="section">
	<a href="#">
	<h1>Referer Sales</h1>
	</a>
</div>
<div class="data">
    <uc1:RefererSales ID="RefererSales1" runat="server" />
	<div class="clear">
	</div>
</div>
</asp:Content>
