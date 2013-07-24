using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class ReturnsReportView : EntityView
    {
        public long? BillingID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? RefundCreateDT { get; set; }
        public string RefundReason { get; set; }
        public string CallRMA { get; set; }
        public string DispositionDisplayName { get; set; }
    }
}
