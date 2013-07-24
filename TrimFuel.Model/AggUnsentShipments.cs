using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class AggUnsentShipments : Entity
    {
        public long? SaleID { get; set; }
        public long? BillingID { get; set; }
        public int? ShipperID { get; set; }
        public DateTime? Date { get; set; }
        public string Reason { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("SaleID", SaleID);
        }

        #region Logic

        public string ShortReason
        {
            get
            {
                if (Reason.Length <= 120)
                    return Reason;
                else
                    return Reason.Substring(0, 120) + "...";
            }
        }

        #endregion
    }
}
