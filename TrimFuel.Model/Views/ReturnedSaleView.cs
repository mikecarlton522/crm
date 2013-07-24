using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class ReturnedSaleView : EntityView
    {
        public byte? ReturnProcessingActionID { get; set; }
        public decimal? RefundAmount { get; set; }
        public int? NewSubscriptionID { get; set; }
        public int? ExtraTrialShipTypeID { get; set; }
        public int? UpsellTypeID { get; set; }
    }
}
