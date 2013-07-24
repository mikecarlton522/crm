using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class CampaignReferralDiscount : Entity
    {
        public int? CampaignID { get; set; }
        public decimal? Discount { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("Discount", Discount);
        }
    }
}
