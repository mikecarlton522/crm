using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class NMIMerchantAccountProduct : Entity
    {
        public int? MerchantAccountProductID { get; set; }
        public int? AssertigyMIDID { get; set; }
        public int? ProductID { get; set; }
        public int? Percentage { get; set; }
        public bool? UseForRebill { get; set; }
        public bool? OnlyRefundCredit { get; set; }
        public bool? UseForTrial { get; set; }
        public bool? QueueRebills { get; set; }
        public int? RolloverAssertigyMIDID { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("AssertigyMIDID", AssertigyMIDID);
            v.AssertNotNull("ProductID", ProductID);
        }
    }
}
