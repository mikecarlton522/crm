<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/Admin.Master" AutoEventWireup="true" CodeBehind="reports_chargeback.aspx.cs" Inherits="TrimFuel.Web.Admin.reports_chargeback" %>
<%@ Register src="Controls/DateFilter.ascx" tagname="DateFilter" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphScript" runat="server">
<script type="text/javascript" language="javascript">
    function RemoveUser(bId) {
        $.ajax({
            type: "POST",
            url: "reports_chargeback.aspx/RemoveUser",
            data: "{billingId : " + bId + "}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function(msg) {
            }
        });
    }
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphStyle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
<form runat="server" id="frmMain">
<div id="toggle" class="section">
	<a href="#">
	<h1>Quick View</h1>
	</a>
	<uc1:DateFilter ID="DateFilter1" runat="server" />
	<a href="#">
	<h1>Refine by Status</h1>
	</a>
	<div>
	    <div class="module">
	        <h2>Status</h2>    
	        <asp:DropDownList runat="server" ID="ddlStatus" OnDataBound="ddlStatus_DataBound" />
	    </div>
	</div>
</div>
<div id="buttons">
    <asp:Button runat="server" ID="btnGo" Text="Generate Report" 
        onclick="btnGo_Click" />
</div>
<div id="load">
    <img src="../images/loading2.gif" style="display:none;" alt="Please wait" />
</div>
<div class="data">
    <asp:GridView runat="server" Width="100%" ID="GvReport" AutoGenerateColumns="false" AllowPaging="false"
        EmptyDataText="No Chargebacks Found for Selected Date Range"
        CssClass="process-offets sortable add-csv-export" GridLines="None" CellSpacing="1" CellPadding="0">
        <HeaderStyle CssClass="header" />
        <Columns>
            <asp:TemplateField>
                <ItemStyle Wrap="false" />
                <ItemTemplate>
                    <a href='../billing_edit.asp?id=<%#Eval("BillingID")%>' target="_blank"><%#Eval("BillingID")%></a>                    
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemStyle Wrap="false" />
                <ItemTemplate>
                    <%#ShowDate(Eval("ChargeDate"))%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemStyle Wrap="false" />
                <ItemTemplate>
                    <%#Eval("ChildMID")%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemStyle Wrap="false" />
                <ItemTemplate>
                    <%#Eval("UserName")%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemStyle Wrap="false" />
                <ItemTemplate>
                    <%#ShowCC(Eval("CreditCard"))%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemStyle Wrap="false" />
                <ItemTemplate>
                    <%#Eval("CaseNumber")%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemStyle Wrap="false" />
                <ItemTemplate>
                    <%#ShowAmount(Eval("Amount"))%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <%#Eval("Description")%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemStyle Wrap="false" />
                <ItemTemplate>
                    <%#ShowDate(Eval("PostDT"))%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <%#Eval("DisplayName")%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemStyle Wrap="false" />
                <ItemTemplate>
                    <%#Eval("DisputeSentDT")%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemStyle Wrap="false" />
                <ItemTemplate>
                    <a href='reports_chargeback.aspx?bID=<%#Eval("BillingID")%><%# (Eval("ChargeHistoryID") is DBNull) ? "" : string.Format("&chID={0}&scID={1}", Eval("ChargeHistoryID"), Eval("SaleChargebackID")) %>' class="wordIcon">Create Doc</a>
                    &nbsp;&nbsp;
                    <a href="#" id='userRemove<%#Eval("BillingID")%>' onclick='javascript:$(this).parent().parent().hide(); RemoveUser(<%#Eval("BillingID")%>)' class="removeIcon">Remove</a>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</div>
</form>
</asp:Content>
