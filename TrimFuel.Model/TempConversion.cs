using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class TempConversion : Entity
    {
        public int? TempConversionID { get; set; }
        public int? PageTypeID { get; set; }
        public string Affiliate { get; set; }
        public string SubAffiliate { get; set; }
        public DateTime? CreateDT { get; set; }
        public int? CampaignID { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("PageTypeID", PageTypeID);
            v.AssertNotNull("CreateDT", CreateDT);
            v.AssertNotNull("CampaignID", CampaignID);
            v.AssertString("Affiliate", Affiliate, 20);
            v.AssertString("SubAffiliate", SubAffiliate, 20);
        }
    }
}
