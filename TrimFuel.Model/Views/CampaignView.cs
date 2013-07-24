using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class CampaignView : EntityView
    {
        public Campaign Campaign { get; set; }
        public IList<CampaignAffiliate> Affiliates { get; set; }
        public IList<CampaignLeadRouting> LeadPartners { get; set; }
    }
}
