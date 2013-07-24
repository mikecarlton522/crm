using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class Affiliate : Entity
    {
        public int? AffiliateID { get; set; }
        public int? AffiliateMasterID { get; set; }
        public string Code { get; set; }
        public string Password { get; set; }
        public int? Active { get; set; }
        public decimal? CostPerSale { get; set; }
        public bool? Deleted { get; set; }
        public string AffiliateFriendlyName { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("CostPerSale", CostPerSale);
            v.AssertNotNull("Deleted", Deleted);
            v.AssertString("Code", Code, 50);
            v.AssertString("Password", Password, 50);
            v.AssertString("AffiliateFriendlyName", AffiliateFriendlyName, 50);
        }
    }
}
