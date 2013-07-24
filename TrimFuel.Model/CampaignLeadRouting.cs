using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class CampaignLeadRouting : Entity
    {
        public struct ID
        {
            public int CampaignID { get; set; }
            public int LeadTypeID { get; set; }
            public int LeadPartnerID { get; set; }
        }

        public ID? CampaignLeadRoutingID { get; set; }
        public int? CampaignID { get; set; }
        public int? LeadTypeID { get; set; }
        public int? LeadPartnerID { get; set; }
        public int? Percentage { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("CampaignID", CampaignID);
            v.AssertNotNull("LeadTypeID", LeadTypeID);
            v.AssertNotNull("LeadPartnerID", LeadPartnerID);
            v.AssertNotNull("Percentage", Percentage);
        }
    }
}
