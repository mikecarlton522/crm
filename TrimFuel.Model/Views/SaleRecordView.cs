using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class SaleRecordView : EntityView
    {
        public long? SaleID { get; set; }
        public DateTime? CreateDT { get; set; }
        public string TrackingNumber { get; set; }
        public string ShipmentMethod { get; set; }
    }
}
