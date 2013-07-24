<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NetworkMerchants.aspx.cs" Inherits="Gateways.NetworkMerchants" %>
<% if (true)
   { %>
response=1&responsetext=SUCCESS&authcode=123456&transactionid=281719471&avsresponse=&cvvresponse=M&orderid=&type=sale&response_code=100
<% }
   else
   { %>
response=2&responsetext=Pick Up Card L&authcode=123456&transactionid=281719471&avsresponse=&cvvresponse=M&orderid=&type=sale&response_code=100
<% } %>

