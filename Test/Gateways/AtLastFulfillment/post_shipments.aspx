<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="post_shipments.aspx.cs" Inherits="Gateways.AtLastFulfillment.post_shipments" %>
<% if (true)
   { %><?xml version="1.0" encoding="utf-8"?><response>  <shipments>    <shipment id="<%= new Random().Next(100000, 999999) %>" orderID="67797" />  </shipments>  <warnings />  <errors /></response><% }
   else
   { %><?xml version="1.0" encoding="utf-8"?><response>  <warnings />  <errors>Test case. ALF post. Error</errors></response><% } %>