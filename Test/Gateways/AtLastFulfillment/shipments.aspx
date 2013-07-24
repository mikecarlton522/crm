<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="shipments.aspx.cs" Inherits="Gateways.AtLastFulfillment.shipments" %>
<% if (true)
   { %><?xml version="1.0" encoding="utf-8"?><Shipments>  <Shipment>    <ShipmentStatus>SHIPPED</ShipmentStatus>    <Tracking>1234123412341234123412</Tracking>  </Shipment></Shipments><% }
   else
   { %><Shipments>  <Shipment>    <ShipmentStatus>....</ShipmentStatus>    <Tracking></Tracking>  </Shipment></Shipments><% } %>