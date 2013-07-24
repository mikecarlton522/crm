using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class CustomShipperToSendView : EntityView
    {
        public decimal? BillAmount { get; set; }
        public string PartNumber { get; set; }
        public int? Quantity { get; set; }
        public decimal? UnitWeight { get; set; }
        public string Description { get; set; }
    }
}
