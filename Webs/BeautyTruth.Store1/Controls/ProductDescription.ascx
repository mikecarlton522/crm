<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProductDescription.ascx.cs" Inherits="Fitdiet.Store1.Controls.ProductDescription" %>
<%@ Import Namespace="BeautyTruth.Store1.Logic" %>
<asp:PlaceHolder runat="server" ID="ph1" Visible='true'>
    <span style="font-weight:normal"><%#Description %></span>
</asp:PlaceHolder>
