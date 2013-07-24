<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AlsoBought.ascx.cs"
    Inherits="BeautyTruth.Store1.Controls.AlsoBought" %>
<%@ Register TagPrefix="uc" TagName="Product" Src="~/Controls/Product.ascx" %>
<div class="<%#GridClass %>" id="suggested">
    <h1>
        Customers Also Bought...</h1>
    <asp:Repeater ID="rProducts" runat="server" DataSource="<%# Products %>">
        <ItemTemplate>
            <uc:Product runat="server" AdditionalClass='<%#DataBinder.Eval(Container.DataItem,"Value")%>' ProductCode='<%#DataBinder.Eval(Container.DataItem,"Key")%>' />
        </ItemTemplate>
    </asp:Repeater>
</div>
