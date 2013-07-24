<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QueuedTransactionsGroup.aspx.cs"
    Inherits="TrimFuel.Web.Admin.services.QueuedTransactionsGroup" %>

<asp:repeater id="rBills" runat="server">
    <ItemTemplate>
        <tr class="detail level1 mid-<%# MidID %>" detail-id="mid-<%# MidID %>" style="display:table-row;">
            <td>
            </td>
            <td class="sorttable_nosort" style="text-align: center;">
                <input type="checkbox" id='<%#Eval("BillType").ToString() == "Rebill"
                ? "rebillToSend" : "billToSend" %>' name='<%#Eval("BillType").ToString() == "Rebill"
                    ? "rebillToSend" : "billToSend" %>' value='<%#Eval("ID")%>' />
            </td>
            <td>
                <a href='https://<%# TrimFuel.Business.Config.Current.APPLICATION_ID %>/billing_edit.asp?id=<%#Eval("BillingID")%>'
                    target="_blank">
                    <%#Eval("BillingID")%></a>
            </td>
            <td>
                <%#Eval("BillType")%>
            </td>
            <td>
                <%#Eval("FirstName")%>
            </td>
            <td>
                <%#Eval("LastName")%>
            </td>
            <td>
                <%#Convert.ToDateTime(Eval("CreateDT")).ToString("MM/dd/yyyy")%>
            </td>
            <td>
                <%#String.Format("${0:f2}", Eval("Amount"))%>
            </td>
            <td>
                <span title="<%#Eval("Reason")%>">
                    <%#Eval("ShortReason")%></span>
            </td>
        </tr>
    </ItemTemplate>
</asp:repeater>
