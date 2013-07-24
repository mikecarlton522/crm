using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class ProductDomainRouting : Entity
    {
        public int? ProductDomainRoutingID { get; set; }
        public int? ProductDomainID { get; set; }
        public decimal? Percentage { get; set; }
        public int? CampaignID { get; set; }
        public string ExtUrl { get; set; }
        public string Affiliate { get; set; }
        public string SubAffiliate { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("ProductDomainID", ProductDomainID);
            v.AssertNotNull("Percentage", Percentage);
            v.AssertNotNull("CampaignID", CampaignID);
            v.AssertString("ExtUrl", ExtUrl, 100);
            v.AssertString("Affiliate", Affiliate, 100);
            v.AssertString("SubAffiliate", SubAffiliate, 100);
        }
    }
}
