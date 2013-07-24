using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class SaleReturnProcessing : Entity
    {
        public int? SaleReturnProcessingID { get; set; }
        public long? SaleID { get; set; }
        public short? ReturnProcessingActionID { get; set; }
        public decimal? RefundAmount { get; set; }
        public int? NewSubscriptionID { get; set; }
        public int? ExtraTrialShipTypeID { get; set; }
        public int? UpsellTypeID { get; set; }
        public int? Quantity { get; set; }
        public int? NewRecurringPlanID { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("SaleID", SaleID);
        }
    }
}
