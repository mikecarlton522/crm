using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class SalesAgrReportFullView<T> : EntityView where T : SalesAgrBaseReportView
    {
        public int? SaleChargebackCount { get; set; }
        public decimal? SaleChargebackPercentage { get; set; }
        public int? ProjectedSaleCount { get; set; }
        public int? ProjectedSaleChargebackCount { get; set; }
        public decimal? ProjectedChargebackPercentage { get; set; }
        public T BaseReportView { get; set; }
    }
}
