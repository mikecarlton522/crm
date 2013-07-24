<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/Admin.Master" AutoEventWireup="true" 
    CodeBehind="management_campaigns_advanced.aspx.cs" Inherits="TrimFuel.Web.Admin.management_campaigns_advanced" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphScript" runat="server">
    <script type="text/javascript" language="javascript">
        function editTermsConditions(Id) {
            editForm("editForms/CampaignTerm.aspx?id=" + Id, 600, "/dotnet/management_campaigns_advanced.aspx");
        }

        function editCoupons(Id) {
            popupControl2("campaign-coupons-" + Id, "Edit Coupons", 640, 750, "editForms/CampaignCoupons.aspx?id=" + Id);
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphStyle" runat="server">
    <style type="text/css">
        .campaign-container { width: 310px; padding: 0; overflow: hidden; white-space: nowrap; }
        .campaign-container .campaign-title { background-color: #c2d2e8; padding: 5px; }
        .campaign-container .campaign-img { padding: 5px; text-align: center; }
        .campaign-container .campaign-body { padding: 5px; }
        
    </style>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
    <form id="Form1" runat="server">

    <div class="module">
    <br />
        <asp:LinkButton ID="lbHideArchived" OnClick="HideArchivedCampaigns" runat="server">Hide Archived Campaigns</asp:LinkButton>&nbsp;&nbsp;|&nbsp;&nbsp;
        <asp:LinkButton ID="lbShowArchived" OnClick="ShowArchivedCampaigns" runat="server">Show Archived Campaigns</asp:LinkButton>&nbsp;&nbsp;|&nbsp;&nbsp;
        <a href="edit_campaigns_advanced.aspx">Create New Campaign</a>
        <br /><br />
    </div>
    <div class="module">
    <br />
        <asp:Button ID="btnDuplicate" runat="server" Text="Create new Campaign Based on"
            OnClick="Duplicate" />&nbsp;
            <asp:DropDownList ID="ddlCampaigns" runat="server" DataTextField="DisplayName" DataValueField="CampaignID"
            Width="250px" />
            <br />
    </div>

    <div class="clear"></div>

    <div id="toggle" class="section">
        <a href="#"><h1>Merchant Applications</h1></a>
        <div>
            <asp:Repeater ID="rMerchantApplications" runat="server" OnItemCommand="DoCampaignAction">
                <ItemTemplate>
                    <div class="module campaign-container">
                        <div class="campaign-title">
                            <strong><%# DataBinder.Eval(Container.DataItem, "DisplayName") %></strong>
                        </div>
                        <div class="campaign-img">
                            <img src='<%# GetScreenshot((int)DataBinder.Eval(Container.DataItem, "CampaignID")) %>' height="200" width="300" alt="<%# DataBinder.Eval(Container.DataItem, "DisplayName") %>">
                        </div>
                        <div class="campaign-body">
                            Campaign ID: <%# DataBinder.Eval(Container.DataItem, "CampaignID") %><br />
                            Campaign URL: <a href="<%# GetURL(DataBinder.Eval(Container.DataItem, "URL")) %>" target="_blank"><%# DataBinder.Eval(Container.DataItem, "URL") %></a><br />
                            Corporation: <%# DataBinder.Eval(Container.DataItem, "Corporation")%><br />
                            Customer Service Phone: <%# DataBinder.Eval(Container.DataItem, "Phone")%><br />
                            Support e-mail: <%# DataBinder.Eval(Container.DataItem, "Email") %><br />
                            Registrations: <%# DataBinder.Eval(Container.DataItem, "RegistrationCount") %><br />
                        </div>
                        <div class="campaign-body">
                            <a href="edit_campaigns_advanced.aspx?CampaignID=<%# DataBinder.Eval(Container.DataItem, "CampaignID") %>">Edit</a>
                            &nbsp;&nbsp;|&nbsp;&nbsp;
                            <a href="javascript:void(0);" onclick="return editTermsConditions(<%# DataBinder.Eval(Container.DataItem, "CampaignID") %>);">Terms</a>
                            &nbsp;&nbsp;|&nbsp;&nbsp;
                            <a href="javascript:void(0);" onclick="return editCoupons(<%# DataBinder.Eval(Container.DataItem, "CampaignID") %>);">Coupons</a>
                            &nbsp;&nbsp;|&nbsp;&nbsp;
                            <asp:LinkButton ID="lbArchive" OnClientClick="return confirm('Are you sure? This setting can be undone by going to Show Archived Campaigns.');" CausesValidation="False" CommandName="archive" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "CampaignID") %>' runat="server">Archive</asp:LinkButton>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
        <div class="clear"></div>
        <a href="#"><h1>Available Campaigns</h1></a>
        <div>
            <asp:Repeater ID="rAvailableCampaigns" runat="server" OnItemCommand="DoCampaignAction">
                <ItemTemplate>
                    <div class="module campaign-container">
                        <div class="campaign-title">
                            <strong><%# DataBinder.Eval(Container.DataItem, "DisplayName") %></strong>
                        </div>
                        <div class="campaign-img">
                            <img src='<%# GetScreenshot((int)DataBinder.Eval(Container.DataItem, "CampaignID")) %>' height="200" width="300" alt="<%# DataBinder.Eval(Container.DataItem, "DisplayName") %>">
                        </div>
                        <div class="campaign-body">
                            Campaign ID: <%# DataBinder.Eval(Container.DataItem, "CampaignID") %><br />
                            Campaign URL: <a href="<%# GetURL(DataBinder.Eval(Container.DataItem, "URL")) %>" target="_blank"><%# DataBinder.Eval(Container.DataItem, "URL") %></a><br />
                            Corporation: <%# DataBinder.Eval(Container.DataItem, "Corporation")%><br />
                            Customer Service Phone: <%# DataBinder.Eval(Container.DataItem, "Phone")%><br />
                            Support e-mail: <%# DataBinder.Eval(Container.DataItem, "Email") %><br />
                            Registrations: <%# DataBinder.Eval(Container.DataItem, "RegistrationCount") %><br />
                        </div>
                        <div class="campaign-body">
                            <a href="edit_campaigns_advanced.aspx?CampaignID=<%# DataBinder.Eval(Container.DataItem, "CampaignID") %>">Edit</a>
                            &nbsp;&nbsp;|&nbsp;&nbsp;
                            <a href="javascript:void(0);" onclick="return editTermsConditions(<%# DataBinder.Eval(Container.DataItem, "CampaignID") %>);">Terms</a>
                            &nbsp;&nbsp;|&nbsp;&nbsp;
                            <a href="javascript:void(0);" onclick="return editCoupons(<%# DataBinder.Eval(Container.DataItem, "CampaignID") %>);">Coupons</a>
                            &nbsp;&nbsp;|&nbsp;&nbsp;
                            <asp:LinkButton ID="lbArchive" OnClientClick="return confirm('Are you sure? This setting can be undone by going to Show Archived Campaigns.');" CausesValidation="False" CommandName="archive" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "CampaignID") %>' runat="server">Archive</asp:LinkButton>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>
    </form>
</asp:Content>
