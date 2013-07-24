<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/Admin.Master" AutoEventWireup="true"
    CodeBehind="management_campaigns.aspx.cs" Inherits="TrimFuel.Web.Admin.management_campaigns" %>
<%@ Register Assembly="TrimFuel.Web.UI" Namespace="TrimFuel.Web.UI" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphScript" runat="server">
    <script type="text/javascript" language="javascript">
        function editTermsConditions(Id) {
            editForm("editForms/CampaignTerm.aspx?id=" + Id, 600, "/dotnet/management_campaigns.aspx");
        }

        function editCoupons(Id) {
            popupControl2("campaign-coupons-" + Id, "Edit Coupons", 640, 750, "editForms/CampaignCoupons.aspx?id=" + Id);
        }

        $(document).ready(function () {
            $("#tabs-managers").tabs({ cookie: true });
        });
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
<cc1:If ID="If1" runat="server" Condition='<%# TrimFuel.Business.Config.Current.APPLICATION_ID == TrimFuel.Model.Enums.ApplicationEnum.TriangleCRM || TrimFuel.Business.Config.Current.APPLICATION_ID == TrimFuel.Model.Enums.ApplicationEnum.LocalhostTriangleCRM %>'>
<div id="tabs-managers" class="data">
<ul>
    <li><a href="#manager-tab-1">Campaign Manager</a></li>
    <li><a href="#manager-tab-2">Campaign Manager (Advanced)</a></li>
</ul>
<div id="manager-tab-1">
</cc1:If>
    <div class="module">
        <br />
        <a href="edit_campaigns.aspx">Create New Campaign</a>
        <br />
    </div>
    <div class="module">
        <br />
        <asp:LinkButton ID="lbHideArchived" OnClick="HideArchivedCampaigns" runat="server">Hide Archived Campaigns</asp:LinkButton>&nbsp;&nbsp;|&nbsp;&nbsp;
        <asp:LinkButton ID="lbShowArchived" OnClick="ShowArchivedCampaigns" runat="server">Show Archived Campaigns</asp:LinkButton>&nbsp;&nbsp;|&nbsp;&nbsp;
        <asp:LinkButton ID="lbHideExternal" OnClick="HideExternalCampaigns" runat="server">Hide Externally Hosted Campaigns</asp:LinkButton>&nbsp;&nbsp;|&nbsp;&nbsp;
        <asp:LinkButton ID="lbShowExternal" OnClick="ShowExternalCampaigns" runat="server">Show Externally Hosted Campaigns</asp:LinkButton>
        <br />
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
                            Externally Hosted: <%# (bool)DataBinder.Eval(Container.DataItem, "IsExternal") == true ? "Yes" : "No" %><br />
                        </div>
                        <div class="campaign-body">
                            <a href="edit_campaigns.aspx?CampaignID=<%# DataBinder.Eval(Container.DataItem, "CampaignID") %>">Edit</a>
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
                            Externally Hosted: <%# (bool)DataBinder.Eval(Container.DataItem, "IsExternal") == true ? "Yes" : "No" %><br />
                        </div>
                        <div class="campaign-body">
                            <a href="edit_campaigns.aspx?CampaignID=<%# DataBinder.Eval(Container.DataItem, "CampaignID") %>">Edit</a>
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
    <div class="space"></div>
<cc1:If ID="If2" runat="server" Condition='<%# TrimFuel.Business.Config.Current.APPLICATION_ID == TrimFuel.Model.Enums.ApplicationEnum.TriangleCRM || TrimFuel.Business.Config.Current.APPLICATION_ID == TrimFuel.Model.Enums.ApplicationEnum.LocalhostTriangleCRM %>'>
</div>    
<div id="manager-tab-2">
    <div class="module">
        <br />
        <a href="edit_campaigns_advanced.aspx">Create New Campaign</a>
        <br />
    </div>
    <div class="module">
        <br />
        <asp:LinkButton ID="lbHideArchived2" OnClick="HideArchivedCampaigns2" runat="server">Hide Archived Campaigns</asp:LinkButton>&nbsp;&nbsp;|&nbsp;&nbsp;
        <asp:LinkButton ID="lbShowArchived2" OnClick="ShowArchivedCampaigns2" runat="server">Show Archived Campaigns</asp:LinkButton>&nbsp;&nbsp;|&nbsp;&nbsp;
        <asp:LinkButton ID="lbHideExternal2" OnClick="HideExternalCampaigns2" runat="server">Hide Externally Hosted Campaigns</asp:LinkButton>&nbsp;&nbsp;|&nbsp;&nbsp;
        <asp:LinkButton ID="lbShowExternal2" OnClick="ShowExternalCampaigns2" runat="server">Show Externally Hosted Campaigns</asp:LinkButton>
        <br />
    </div>
    <div class="module">
        <br />
        <asp:Button ID="btnDuplicate2" runat="server" Text="Create new Campaign Based on"
            OnClick="Duplicate2" />&nbsp;
            <asp:DropDownList ID="ddlCampaigns2" runat="server" DataTextField="DisplayName" DataValueField="CampaignID"
            Width="250px" />
        <br />
    </div>

    <div class="clear"></div>

    <div id="toggle" class="section">
        <a href="#"><h1>Merchant Applications</h1></a>
        <div>
            <asp:Repeater ID="rMerchantApplications2" runat="server" OnItemCommand="DoCampaignAction2">
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
                            Externally Hosted: <%# (bool)DataBinder.Eval(Container.DataItem, "IsExternal") == true ? "Yes" : "No" %><br />
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
            <asp:Repeater ID="rAvailableCampaigns2" runat="server" OnItemCommand="DoCampaignAction">
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
                            Externally Hosted: <%# (bool)DataBinder.Eval(Container.DataItem, "IsExternal") == true ? "Yes" : "No" %><br />
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
    <div class="space"></div>
</div>    
</div>    
</cc1:If>
</form>
</asp:Content>
