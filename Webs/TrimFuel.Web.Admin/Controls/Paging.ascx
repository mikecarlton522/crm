<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Paging.ascx.cs" Inherits="TrimFuel.Web.Admin.Controls.Paging" %>
<div id="paging">
    <script type="text/javascript" language="javascript">
        function beforeChangePage(el) {
            $('#hdnCurrentPage').val($(el).html());
        }
    </script>
    <h1>
        Records Found:
        <%#TotalNumberOfRecords%></h1>
    <input type="hidden" name="hdnCurrentPage" id="hdnCurrentPage" value="<%# CurrentPage %>" />
    <div style="text-align: center;">
        <asp:Repeater runat="server" ID="rPaging" DataSource="<%# Pages %>">
            <ItemTemplate>
                <asp:LinkButton ID="LinkButton1" runat="server" OnClick="btnGoToPage_Click" OnClientClick="beforeChangePage(this);"
                    Text='<%# Eval("Key") %>' Enabled='<%# Eval("Value") %>' CssClass="submit" />
            </ItemTemplate>
        </asp:Repeater>
    </div>
    <div class="clear">
    </div>
</div>
