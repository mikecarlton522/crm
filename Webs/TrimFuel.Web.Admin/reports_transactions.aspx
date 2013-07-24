<%@ Page Language="C#"  MasterPageFile="~/Controls/Admin.Master" AutoEventWireup="true" CodeBehind="reports_transactions.aspx.cs"
    Inherits="TrimFuel.Web.Admin.reports_transactions" %>

<%@ Register Src="Controls/DateFilter.ascx" TagName="DateFilter" TagPrefix="uc1" %>
<asp:content id="Content1" contentplaceholderid="cphScript" runat="server">

    <script type="text/javascript">
        $(document).ready(function () {
            $("#tabs").tabs({
                cookie: {
                    expires: 1
                }
            });
            initMultilevelTables();
        });

        function initMultilevelTables() {
            $("table.multilevel").each(function () {
                var table = this;
                $(table).find("tr.master").click(function () {
                    multilevelTableToggleDetail(table, this);
                });
            });
        }

        function multilevelTableToggleDetail(tableEl, masterEl) {
            var masterId = $(masterEl).attr("master-id");
            //alert(masterId);
            //Recursive hide children
            $(tableEl).find("tr.detail[detail-id=" + masterId + "]").filter(".master").filter(":visible").each(function () {
                multilevelTableHideDetail(tableEl, this)
            });
            $(tableEl).find("tr.detail[detail-id=" + masterId + "]").toggle();
        }

        function multilevelTableHideDetail(tableEl, masterEl) {
            var masterId = $(masterEl).attr("master-id");
            //alert(masterId);
            //Recursive hide children
            $(tableEl).find("tr.detail[detail-id=" + masterId + "]").filter(".master").filter(":visible").each(function () {
                multilevelTableHideDetail(tableEl, this)
            });
            $(tableEl).find("tr.detail[detail-id=" + masterId + "]").hide();
        }
    </script>

</asp:content>
<asp:content id="Content2" contentplaceholderid="cphStyle" runat="server">
    <style type="text/css">
        tr.level0 td
        {
            padding: 5px;
            background-color: #e4e4e4;
            font-weight: bold;
        }
        tr.level1 td
        {
            background-color: #ecf0f7;
        }
        tr.detail
        {
            display: none;
        }
        tr.level0 td
        {
        }
        tr.level1 td
        {
        }
        tr.level2 td
        {
        }
    </style>
</asp:content>
<asp:content id="Content3" contentplaceholderid="cphContent" runat="server">

    <form runat="server" id="frmMain">

        <div id="toggle" class="section">
            <a href="#">
                <h1>Quick View</h1>
            </a>
            <uc1:DateFilter ID="DateFilter1" runat="server" />
	        <a href="#">
	        <h1>Filters</h1>
	        </a>
	        <div>
	            <div class="module">
	                <h2>By Charge Type</h2>    
	                <asp:DropDownList runat="server" ID="ddlChargeType"  OnDataBound="ddlChargeType_DataBound"/>
	           
	            </div>
                <div class="module">
	                <h2>By Transaction Type</h2>    
	                <asp:DropDownList runat="server" ID="ddlTransactionType"  OnDataBound="ddlTransactionType_DataBound"/>
	           
	            </div>
                 <div class="module">
	                <h2>By Product Group</h2>    
	                <asp:DropDownList runat="server" ID="ddlProductGroup"  OnDataBound="ddlProductGroup_DataBound"/>
	           
	            </div>
                 <div class="module">
	                <h2>By MID</h2>    
	                <asp:DropDownList runat="server" ID="ddlMid"  OnDataBound="ddlMid_DataBound"/>
	           
	            </div>
	        </div>
             
        </div>
        <div id="buttons">
            <asp:Button runat="server" ID="btnGo" Text="Generate Report" OnClick="btnGo_Click" />
        </div>
        <div class="data">
        
    <asp:GridView runat="server" Width="100%" ID="GvReport" AutoGenerateColumns="false" ShowFooter="true"
        EmptyDataText="No transactions found for selected date range and filters" OnPageIndexChanging="gridView_PageIndexChanging"
        CssClass="process-offets sortable add-csv-export"  GridLines="None" CellSpacing="1" CellPadding="0" onrowdatabound="GvReport_RowDataBound">
              <pagersettings  Position="Top"  /> 
              <pagerstyle backcolor="white"
          height="30px"
          
          horizontalalign="Center"/>
        <HeaderStyle CssClass="header" />
        <Columns>
            <asp:TemplateField  HeaderText="Charge Type">
                <ItemStyle Wrap="false" />
                <ItemTemplate>
                    <%#Eval("ChargeType")%>                   
                </ItemTemplate>
                <FooterTemplate>
                    <asp:Label ID="lblTotal" runat="server"  Text="Total" Font-Bold="true"/>
                </FooterTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Count">
                <ItemStyle Wrap="false" />
                <ItemTemplate>
                   <asp:Label ID="lblCount" runat="server" Text='<%# Eval("Count")%>' /> 
                </ItemTemplate>
                <FooterTemplate>
                    <asp:Label ID="lblTotalCount" runat="server" />
                </FooterTemplate>
            </asp:TemplateField>
            
       </Columns>
    </asp:GridView>

</div>
    </form>
    
</asp:content>
