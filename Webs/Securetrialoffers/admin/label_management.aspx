<%@ Page Title="" Language="C#" MasterPageFile="~/admin/Controls/Admin.Master" AutoEventWireup="true" CodeBehind="label_management.aspx.cs" Inherits="Securetrialoffers.admin.label_management" %>
<%@ Register src="Controls/TagList.ascx" tagname="TagList" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphScript" runat="server">
<script type="text/javascript" src="js/billing-tag.js"></script>
<script type="text/javascript">
    $(document).ready(function() {
        obtainTagList();
    });
</script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphStyle" runat="server">
<link rel="stylesheet" href="css/billing-tag.css" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<h2>Label Management</h2>
<uc1:TagList ID="TagList1" runat="server" />
</asp:Content>
