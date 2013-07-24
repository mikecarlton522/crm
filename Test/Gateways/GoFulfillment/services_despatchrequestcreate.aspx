<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="services_despatchrequestcreate.aspx.cs" Inherits="Gateways.GoFulfillment.services_despatchrequestcreate" %>
<% if (true)
   { %><DocumentElement>
  <Result>
    <Result>true</Result>
    <Message>Despatch Request created.</Message>
    <DocumentId><%= new Random().Next(100000, 999999) %></DocumentId>
  </Result>
</DocumentElement><% }
   else
   { %><DocumentElement>
  <Result>
    <Result>false</Result>
    <Message>Sorry, but there was an error. (Could not populate the products (Product not found in DB (Sequence contains no elements)))</Message>
    <DocumentId>0</DocumentId>
  </Result>
</DocumentElement><% } %>