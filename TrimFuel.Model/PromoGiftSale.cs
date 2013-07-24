using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class PromoGiftSale : Entity
    {
        public long? PromoGiftID { get; set; }
        public long? SaleID { get; set; }
        public decimal? RedeemAmount { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("PromoGiftID", PromoGiftID);
            v.AssertNotNull("SaleID", SaleID);
            v.AssertNotNull("RedeemAmount", RedeemAmount);
        }
    }
}
