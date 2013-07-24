<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="reports_fulfilment_export.aspx.cs"
    MasterPageFile="~/Controls/Admin.Master" Inherits="TrimFuel.Web.Admin.reports_fulfilment_export" %>
<%@ Import Namespace="TrimFuel.Business.Utils" %>
<%@ Register TagPrefix="uc1" TagName="DateFilter" Src="~/Controls/DateFilter.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphScript" runat="server">
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
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphStyle" runat="server">
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
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <form runat="server" id="frmMain">
        <div id="toggle" class="section">
             <a href="#">
	            <h1>Quick View</h1>
	        </a>
	        <uc1:DateFilter ID="DateFilter1" runat="server" />
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
                EmptyDataText=""
                CssClass="process-offets sortable add-csv-export" GridLines="None" CellSpacing="1" CellPadding="0">
                <HeaderStyle CssClass="header" />
                <Columns>
                    <asp:TemplateField>
                        <ItemStyle Wrap="false" />
                        <ItemTemplate>
                            <%# Eval("SaleID")%>       
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemStyle Wrap="false" />
                        <ItemTemplate>
                            <%# Eval("BillingID")%>       
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemStyle Wrap="false" />
                        <ItemTemplate>
                            <%# Eval("CPF")%>       
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemStyle Wrap="false" />
                        <ItemTemplate>
                            <%# Eval("OrderDate")%>   
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemStyle Wrap="false" />
                        <ItemTemplate>
                            <%# Eval("OrderTime")%>   
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField>
                        <ItemStyle Wrap="false" />
                        <ItemTemplate>
                            <%# Eval("FirstName")%>  
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemStyle Wrap="false" />
                        <ItemTemplate>
                            <%# Eval("LastName")%>  
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemStyle Wrap="false" />
                        <ItemTemplate>
                            <%# Eval("Address1") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemStyle Wrap="false" />
                        <ItemTemplate>
                            <%# Eval("Address2")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemStyle Wrap="false" />
                        <ItemTemplate>
                             <%# Eval("Numero")%>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField>
                        <ItemStyle Wrap="false" />
                        <ItemTemplate>
                             <%# Eval("Complemento")%>
                        </ItemTemplate>
                    </asp:TemplateField>                   
                    <asp:TemplateField>
                        <ItemStyle Wrap="false" />
                        <ItemTemplate>
                            <%# Eval("City")%>  
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemStyle Wrap="false" />
                        <ItemTemplate>
                           <%# Eval("State")%>  
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <%# Eval("Zip")%>  
                        </ItemTemplate>
                    </asp:TemplateField>
                     <asp:TemplateField>
                        <ItemStyle Wrap="false" />
                        <ItemTemplate>
                             <%# Eval("Bairro")%>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField>
                        <ItemStyle Wrap="false" />
                        <ItemTemplate>
                            <%# Eval("Country")%>  
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemStyle Wrap="false" />
                        <ItemTemplate>
                            <%# Eval("Phone")%>  
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemStyle Wrap="false" />
                        <ItemTemplate>
                            <%# Eval("ProductCode")%>  
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemStyle Wrap="false" />
                        <ItemTemplate>
                            <%# Eval("TotalQty")%> 
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemStyle Wrap="false" />
                        <ItemTemplate>
                            <%# Utility.FormatCurrency(Convert.ToDecimal(Eval("ProductTotal")), Eval("HtmlSymbol").ToString())%>                             
                        </ItemTemplate>
                    </asp:TemplateField>
                    
                     <asp:TemplateField>
                        <ItemStyle Wrap="false" />
                        <ItemTemplate>
                            <%# Utility.FormatCurrency(Convert.ToDecimal(Eval("ProductShipping")), Eval("HtmlSymbol").ToString())%>  
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemStyle Wrap="false" />
                        <ItemTemplate>
                            <%# Utility.FormatCurrency(Convert.ToDecimal(Eval("GrandTotal")), Eval("HtmlSymbol").ToString())%>                        
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemStyle Wrap="false" />
                        <ItemTemplate>
                             <%# Container.DataItemIndex + 1 %>
                        </ItemTemplate>
                    </asp:TemplateField>
                     <asp:TemplateField>
                        <ItemTemplate>
                            <%# Eval("Email")%>  
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemStyle Wrap="false" />
                        <ItemTemplate>
                            <%# Eval("IP")%>  
                        </ItemTemplate>
                    </asp:TemplateField>
                    
                    <asp:TemplateField>
                        <ItemStyle Wrap="false" />
                        <ItemTemplate>
                            <%# Eval("PackageCode")%>  
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemStyle Wrap="false" />
                        <ItemTemplate>
                            <%# Eval("PackageDescription")%>  
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemStyle Wrap="false" />
                        <ItemTemplate>
                            <%# Eval("Misc1")%>  
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemStyle Wrap="false" />
                        <ItemTemplate>
                            <%# Eval("Misc2")%>    
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemStyle Wrap="false" />
                        <ItemTemplate>
                            <%# Eval("Misc3")%>  
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField>
                        <ItemStyle Wrap="false" />
                        <ItemTemplate>
                            <%# Eval("ProductDescription")%>  
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemStyle Wrap="false" />
                        <ItemTemplate>
                            <%# Eval("Weight")%>  
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemStyle Wrap="false" />
                        <ItemTemplate>
                            <%# Utility.FormatCurrency(Convert.ToDecimal(Eval("CustomsPrice")), Eval("HtmlSymbol").ToString())%>                              
                        </ItemTemplate>
                    </asp:TemplateField>                                   
                </Columns>
            </asp:GridView>
        </div>
    </form>    
</asp:Content>
