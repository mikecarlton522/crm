<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="msc.aspx.cs" Inherits="Gateways.msc" %>
<% if (true)
   { %>
success=Yes&message=Test+text+test+text&customerid=1111111&leadID=0&orderID=2222222&authCode=123123&responseCode=&referenceID=10083884
<% }
   else
   { %>
success=No&message=bfirstname+field+is+required%3bblastname+field+is+required%3bbaddress1+field+is+required%3bbcity+field+is+required%3bbstate+field+is+required%3bbzipcode+field+is+required&customerid=0&leadID=0&orderID=0&authCode=&responseCode=
<% } %>

