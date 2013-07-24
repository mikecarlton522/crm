<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CategoryPager.ascx.cs"
    Inherits="BeautyTruth.Store1.Controls.CategoryPager" %>
<%@ Register TagPrefix="uc" TagName="Product" Src="~/Controls/Product.ascx" %>
<div class="grid_9 index">
    <div id="toUppendUp" runat="server">
    </div>
</div>
<div class="grid_9" id="products">
    <asp:ListView ID="lvProducts" runat="server">
        <LayoutTemplate>
            <asp:PlaceHolder runat="server" ID="itemPlaceholder"></asp:PlaceHolder>
        </LayoutTemplate>
        <ItemTemplate>
            <uc:Product AdditionalClass='<%#DataBinder.Eval(Container.DataItem,"Value")%>' ProductCode='<%#DataBinder.Eval(Container.DataItem,"Key")%>'
                runat="server" />
        </ItemTemplate>
    </asp:ListView>
</div>
<div class="grid_9 index" style="margin-left: 250px;">
    <div id="toUppendDown" runat="server">
    </div>
</div>
