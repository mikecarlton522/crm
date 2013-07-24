using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class InventorySaleView : EntityView
    {
        public int? InventoryID { get; set; }
        public string InventoryProduct { get; set; }
        public int? InventoryQuantity { get; set; }
        public long? SaleID { get; set; }
        public DateTime? SaleCreateDT { get; set; }
        public decimal? SaleChargeAmount { get; set; }
        public DateTime? SaleShippedDT { get; set; }
        public string SaleTrackingNumber { get; set; }
        public DateTime? SaleReturnDT { get; set; }
        public long? BillingID { get; set; }
        public string BillingFirstName { get; set; }
        public string BillingLastName { get; set; }

        #region Logic

        public string BillingFullName
        {
            get
            {
                if (string.IsNullOrEmpty(BillingFirstName))
                    return BillingLastName;
                if (string.IsNullOrEmpty(BillingLastName))
                    return BillingFirstName;
                return string.Format("{0} {1}", BillingFirstName, BillingLastName);
            }
        }

        public void Fill(InventorySaleView src)
        {

        }

        #endregion
    }
}
