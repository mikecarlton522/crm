<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ErrorsLog.aspx.cs" Inherits="TrimFuel.Web.Admin.EditForms.ErrorsLog_" %>

<table width="100%">
    <tr>
        <td>
            ErrorsLogID:
        </td>
        <td>
            <%= ErrorsLog.ErrorsLogID %>
        </td>
    </tr>
    <tr>
        <td>Error Date:</td>
        <td><%= ErrorsLog.ErrorDate.ToString("yyyy-MM-dd HH:mm") %></td>
    </tr>
    <tr>
        <td>
            Application:
        </td>
        <td>
            <%= ErrorsLog.Application %>
        </td>
    </tr>
    <tr>
        <td>
            Class Name:
        </td>
        <td>
            <%= ErrorsLog.ClassName %>
        </td>
    </tr>
    <tr>
        <td valign="top">
            Brief Error Text
        </td>
        <td>
            <textarea cols="50" rows="5" readonly="readonly"><%= ErrorsLog.BriefErrorText %></textarea>
        </td>
    </tr>
    <tr>
        <td valign="top">
            Error Text
        </td>
        <td>
            <textarea cols="50" rows="10" readonly="readonly"><%= ErrorsLog.ErrorText %></textarea>
        </td>
    </tr>
</table>
