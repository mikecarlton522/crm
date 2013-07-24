<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Error_Page.aspx.cs" Inherits="Payment_test.Error_Page" ValidateRequest="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Error Page </title>
</head>
<body>
    <form id="form1" runat="server">
    <div style="margin: 0 10px 10px 0; padding: 10px; border: 1px dotted #a5a5a5; float: left;
        font-size: 12px; min-height: 47px;">
        <h1>
            Error: Required page parameters not sent. Please contact technical support.</h1>
        <asp:Label runat="server" ID="lblErrorMessage"></asp:Label>
    </div>
    </form>
</body>
</html>
