<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Menu.ascx.cs" Inherits="Securetrialoffers.admin.Controls.Menu" %>
  <center><img src='images/logo.jpg'></center>
  <br /><center>
  <div id="topmenu" class="ddsmoothmenu" style="text-align: center;<%= MenuWidth %>">
  <ul>
    <% if (!Adrel) { %>
    <li>
      <a href="#">Management</a>
      <ul>
        <li><a href="../../application_management.asp">Application Configuration</a></li>       
        <li><a href="../../tsn_shipping.asp">TSN Shipping Queue</a></li>
        <li><a href="../../control_panel.asp">Control Panel</a></li>
                <li>
                    <a href="../../childmid.asp">Edit MID Descriptors</a>
                </li>
                <li>
                    <a href="../../mid_product.asp">Product-MID Mapping</a>
                </li>     
        <li><a href="label_management.aspx">Label Management</a></li>
            </ul>
    </li>
    <% } %>
    <li>
      <a href="#">Campaign Settings</a>
      <ul>
        <li><a href="../../CampaignMgr2/Campaigns.aspx">Campaign Manager</a></li> 
        <li><a href="../../campaign_rotation.asp">Campaign Rotation</a></li> 
        <li><a href="../../product_rotation.asp">Product Rotation</a></li> 
        <li><a href="../../pixel_list.asp">Pixel Management</a></li>
        <li><a href="../../affiliate.asp">Affiliate Management</a></li>
        <li><a href="../../CampaignMgr2/DynamicEmail.aspx">Dynamic Email</a></li>
        <li><a href="referer_management.aspx">Referer Management</a></li>
      </ul>
    </li>
    <% if (!Adrel) { %>
    <li>
      <a href="#">Reporting</a>
      <ul>
        <li><a href="../../billing_report.asp">Billing Report</a></li>
                <li>
                    <a href="../../rebill_report.asp">Billing Recap Report</a>
                </li>
                <li>
                    <a href="../../rebill_success_report.asp">Rebill Success Report</a>
                </li>
                <li>
                    <a href="../../revenue_report.asp">Revenue Report</a>
                </li>                
        <li><a href="../../affiliate_report.asp">Affiliate Report</a></li> 
        <li><a href="../../conversion_report.asp">Conversion Report</a></li> 
        <li><a href="report.aspx?type=report_conversions&caption=Conversion%20Report">Summary Conversion Report</a></li>
        <li><a href="../../extra_shipment_report.asp">Manual Shipment Report</a></li>
        <li><a href="../../CampaignMgr2/Affiliatereport.aspx">Affiliate Waterfall Report</a></li> 
        <li><a href="../../apps/report.aspx?type=report_refund_auth&caption=Returns%20Report">Returns Report</a></li>         
        <li><a href="../../report_mainframe.asp?type=report_refund&caption=Pending%20Returns%20Report">Pending Returns Report</a></li>
        <li><a href="../../report_mainframe.asp?type=report_disposition&caption=Disposition%20Report">Disposition Report</a></li> 
        <li><a href="../../report_mainframe.asp?type=report_scrub&caption=Scrub%20Report">Scrub Report</a></li> 
        <li><a href="../../report_mainframe.asp?type=report_cancellation&caption=Cancellation%20Report">Cancellation Report</a></li>
            <li><a href="../../MIDRevenue_report.asp">Expected MID Revenue</a></li>
        <li><a href="../../report_mainframe.asp?type=report_lost_users&caption=Lost%20Users">Lost Users Report</a></li>
        <li><a href="../../apps/chargeback_report.aspx">Chargeback Report</a></li>
        <li><a href="sales_agr_report.aspx">Chargeback Performance Report</a></li>
      </ul>
    </li>
    <% } else { %>
    <li>
      <a href="#">Reporting</a>
      <ul>
        <li><a href="../../conversion_report.asp">Conversion Report</a></li>
        <li><a href="report.aspx?type=report_conversions&caption=Conversion%20Report">Summary Conversion Report</a></li>
        <li><a href="../../apps/chargeback_report.aspx">Chargeback Report</a></li>
      </ul>
    </li>   
    <% } %>
    <% if (!Adrel) { %>
    <li>
      <a href="#">CRM</a>
      <ul>
        <li><a href="http://www.securetrialoffers.com/acaicrm/">Agent Login</a></li>        
        <li><a href="../../CampaignMgr2/Offers.aspx">CRM Offer Administration</a></li>
        <li><a href="../../CampaignMgr2/CallCenterPage.aspx">CRM Page Content</a></li>
        <li><a href="../../extra_shipment.asp">Manual Shipment Form</a></li>
        <li><a href="../../report_mainframe.asp?type=report_notes">Customer Notes</a></li>
                <li>
                    <a href="../../../acaicrm/callcenter/queue_assertigy.asp">Abandons Queue</a>
                </li>
                <li>
                    <a href="../../../acaicrm/callcenter/queue_assertigy_orderconfirm.asp">Order Confirm Queue</a>
                </li>
      </ul>
    </li>
    <% } %>
    <li>
      <a href="logout.asp">Logout</a>     
    </li>
  </ul>
  <br style="clear: left" />
  </div>
  <div class="spacer" style="height:20px">&nbsp;</div>
