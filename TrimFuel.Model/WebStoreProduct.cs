using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class WebStoreProduct : Entity
    {
        public int? WebStoreProductID { get; set; }
        public int? CampaignID { get; set; }
        public int? SubscriptionID { get; set; }
        public string ProductCode { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertString("ProductCode", ProductCode, 250);
        }
    }
}
