using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class SaleView : EntityView
    {
        public long? SaleID { get; set; }
        public DateTime? CreateDT { get; set; }
        public decimal? ChargeAmount { get; set; }
        public DateTime? ShippedDT { get; set; }
        public string TrackingNumber { get; set; }
        public DateTime? ReturnDT { get; set; }
        public Billing Billing { get; set; }
        public IList<InventoryView> InventoryList { get; set; }
    }
}
