using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Web.Admin.Logic;
using TrimFuel.Model;
using TrimFuel.Business;
using TrimFuel.Model.Views;
using TrimFuel.Model.Enums;
using TrimFuel.Business.Utils;

namespace TrimFuel.Web.Admin
{
    public partial class outbound_sales_performance : PageX
    {
        LeadService _leadService = new LeadService();

        IList<LeadPartner> _leadPartnerList = null;
        protected IList<LeadPartner> LeadPartnerList
        {
            get
            {
                if (_leadPartnerList == null)
                    _leadPartnerList = new List<LeadPartner>();
                return _leadPartnerList;
            }
        }

        IList<OutboundSalesView> _outboundSalesViewList = null;
        protected IList<OutboundSalesView> OutboundSalesViewList
        {
            get
            {
                if (_outboundSalesViewList == null)
                    _outboundSalesViewList = new List<OutboundSalesView>();
                return _outboundSalesViewList;
            }
        }

        IList<LeadPartnerAffiliateView> _leadPartnerAffiliateList = null;
        protected IList<string> LeadPartnerAffiliateList
        {
            get
            {
                if (_leadPartnerAffiliateList == null)
                    _leadPartnerAffiliateList = _leadService.GetLeadPartnerAffiliateList();
                return _leadPartnerAffiliateList.Select(u => u.Affiliate != null ? u.Affiliate.Code : string.Empty).ToList();
            }
        }

        string AffiliateFilter
        {
            get
            {
                return AffiliateDDL.SelectedValue;
            }
        }

        public override string HeaderString
        {
            get { return "Outbound Sales Performance"; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);
        }

        protected OutboundSalesView GetOutboundSalesView(string leadPartnerID)
        {
            OutboundSalesView record = null;
            if (OutboundSalesViewList == null)
                record = new OutboundSalesView();
            else
            {
                record = OutboundSalesViewList.Where(u => u.LeadPartnerID == Utility.TryGetInt(leadPartnerID)).SingleOrDefault();
                if (record == null)
                    record = new OutboundSalesView();
            }

            //replace all nulls to 0
            if (record.Conversion == null)
            {
                record.Conversion = -1;
                record.CostOfSales = 0;
                record.GrossRevenue = 0;
                record.NetRevenue = 0;
                record.NumberOfChargebacks = 0;
                record.NumberOfLeads = 0;
                record.NumberOfSales = 0;
                record.Refunds = 0;
            }

            return record;
        }

        protected void btnGo_Click(object sender, EventArgs e)
        {
            _leadPartnerList = _leadService.GetPartnerList();
            _outboundSalesViewList = new List<OutboundSalesView>();
            foreach (var leadPartner in LeadPartnerList)
            {
                var leadPartnerRow = _leadService.GetOutboundReport(DateFilter.Date1WithTime, DateFilter.Date2WithTime, LeadTypeEnum.Abandons, leadPartner.LeadPartnerID, AffiliateFilter);
                if (leadPartnerRow != null)
                    _outboundSalesViewList.Add(leadPartnerRow);
            }
            DataBind();
        }

        protected string ShowAmount(object a)
        {
            if (a != null && !(a is DBNull))
            {
                decimal temp = Convert.ToDecimal(a);
                return temp.ToString("$0.00");
            }
            return null;
        }

        protected string ShowConversion(object a)
        {
            if (a != null && !(a is DBNull))
            {
                decimal temp = Convert.ToDecimal(a);
                if (temp != -1)
                    return temp.ToString("F") + "%";
                else
                    return "N/A";
            }
            return null;
        }
    }
}