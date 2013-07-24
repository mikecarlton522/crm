using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class MerchantAccountProduct : Entity
    {
        public int? MerchantAccountProductID { get; set; }
        public int? MerchantAccountID { get; set; }
        public int? ProductID { get; set; }
        public short? Percentage { get; set; }
        public bool? UseForRebill { get; set; }
        public bool? OnlyRefundCredit { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("MerchantAccountID", MerchantAccountID);
            v.AssertNotNull("ProductID", ProductID);
            v.AssertNotNull("Percentage", Percentage);
            v.AssertNotNull("UseForRebill", UseForRebill);
            v.AssertNotNull("OnlyRefundCredit", OnlyRefundCredit);
        }
    }
}
