<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EmailFile.ascx.cs" Inherits="Gateways.EmailFile" %>
<html>
    <head>
        <title>
        </title>
    </head>
    <body>
    <h4>From: <%# FromName %>&lt;<%# FromAddress %>&gt;</h4>
    <h4>To: <%# ToName %>&lt;<%# ToAddress %>&gt;</h4>
    <h4>Subject: <%# Subject %></h4>
    <hr />
    <%# Body %>
    </body>
</html>