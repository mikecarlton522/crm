<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="orderinputapi.aspx.cs" Inherits="Gateways.tsn" %>
<%Random rnd = new Random();%>
&result=0&orderid=<%=(int)(rnd.NextDouble() * 1000000.0)%>
