using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class PromoGift : Entity
    {
        public long? PromoGiftID { get; set; }
        public int? PromoGiftTypeID { get; set; }
        public string GiftNumber { get; set; }
        public string Details { get; set; }
        public long? AssignedSaleID { get; set; }
        public int? StoreID { get; set; }
        public decimal? Value { get; set; }
        public decimal? RemainingValue { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("PromoGiftTypeID", PromoGiftTypeID);
            v.AssertNotNull("StoreID", StoreID);
            v.AssertString("GiftNumber", GiftNumber, 50);
            v.AssertString("Details", Details, 255);
        }
    }
}
