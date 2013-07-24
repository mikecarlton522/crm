using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class InvoiceView2 : EntityView
    {
        public InvoiceView Invoice { get; set; }
        public ChargeHistoryView ChargeResult { get; set; }
    }
}
