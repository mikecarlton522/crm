using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class CampaignAffiliate : Entity
    {
        public struct ID
        {
            public int CampaignID { get; set; }
            public int AffiliateID { get; set; }
        }

        public ID? CampaignAffiliateID { get; set; }
        public int? CampaignID { get; set; }
        public int? AffiliateID { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("CampaignID", CampaignID);
            v.AssertNotNull("Affiliate", AffiliateID);
        }
    }
}
