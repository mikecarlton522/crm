<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RatingControl.ascx.cs"
    Inherits="BeautyTruth.Store1.Controls.RatingControl" %>
<% for (int i = 0; i < Convert.ToInt32(Rating); i++)
   { %>
<img src="images/star.png" width="16" height="16" />
<% } %>