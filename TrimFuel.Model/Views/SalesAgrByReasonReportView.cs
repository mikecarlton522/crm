using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class SalesAgrByReasonReportView : SalesAgrBaseReportView
    {
        public string ChargebackReasonCode { get; set; }
        public string ChargebackStatus { get; set; }
        public int? ChargebackReasonCodeID { get; set; }
        public int? ChargebackStatusTID { get; set; }
    }
}
