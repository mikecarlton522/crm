using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class ShippmentNotSendedToSendView : EntityView
    {
        public int SaleID { get; set; }
        public long BillingID { get; set; }
        public string ProductCode { get; set; }
        public DateTime CreateDT { get; set; }
    }
}
