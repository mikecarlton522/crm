using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class LeadPartnerAffiliateView : EntityView
    {
        public int? LeadPartnerID { get; set; }
        public int? AffiliateID { get; set; }

        public LeadPartner LeadPartner { get; set; }
        public Affiliate Affiliate { get; set; }
    }
}
