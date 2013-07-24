<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="orderinputapi.aspx.cs" Inherits="Gateways.TSN.orderinputapi" %>
<% if (true)
   { %>&result=0&orderid=<%= new Random().Next(100000, 999999) %><% }
   else
   { %>&result=7&orderid=<% } %>