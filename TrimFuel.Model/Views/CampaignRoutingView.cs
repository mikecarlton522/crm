using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class CampaignRoutingView : EntityView
    {
        public string URL { get; set; }
        public decimal? Percentage { get; set; }
        public string ExtUrl { get; set; }
        public string Affiliate { get; set; }
        public string SubAffiliate { get; set; }
    }
}
