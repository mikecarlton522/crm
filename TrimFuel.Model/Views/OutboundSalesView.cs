using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class OutboundSalesView : EntityView
    {
        public int? LeadPartnerID { get; set; }
        public int? NumberOfLeads { get; set; }
        public int? NumberOfSales { get; set; }
        public decimal? CostOfSales { get; set; }
        public decimal? Refunds { get; set; }
        public int? NumberOfChargebacks { get; set; }
        public decimal? GrossRevenue { get; set; }
        public decimal? NetRevenue { get; set; }
        public double? Conversion { get; set; }
    }
}
