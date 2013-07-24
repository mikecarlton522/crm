using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class Conversion : Entity
    {
        public int? ConversionID { get; set; }
        public int? PageTypeID { get; set; }
        public string Affiliate { get; set; }
        public string SubAffiliate { get; set; }
        public DateTime? CreateDT { get; set; }
        public string Flow { get; set; }
        public int? Hits { get; set; }
        public int? CampaignID { get; set; }
        public byte? Hour { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("PageTypeID", PageTypeID);
            v.AssertNotNull("CreateDT", CreateDT);
            v.AssertNotNull("Hits", Hits);
            v.AssertNotNull("CampaignID", CampaignID);
            v.AssertString("Affiliate", Affiliate, 20);
            v.AssertString("SubAffiliate", SubAffiliate, 20);
            v.AssertString("Flow", Flow, 50);
        }
    }
}
