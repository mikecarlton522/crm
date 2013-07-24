<%@ Page Title="" Language="C#" MasterPageFile="~/Gateway.Master" AutoEventWireup="true" CodeBehind="BadCustomer.aspx.cs" Inherits="Gateways.BadCustomer" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphSuccess" runat="server"><?xml version="1.0" encoding="UTF-8"?>
    <API version="1.0">
        <transactionId>7562a132-497d-26aa-ac14-4c7eb47ad881</transactionId>
        <error>0</error>
        <found>0</found>
        <result>No Results</result>
    </API>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphError" runat="server"><?xml version="1.0" encoding="UTF-8"?>
    <API version="1.0">
        <transactionId>7562a132-497d-26aa-ac14-4c7eb47ad881</transactionId>
        <error>2</error>
        <found>0</found>
        <result>Test generic error</result>
    </API>
</asp:Content>
