using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class InventoryView : EntityView
    {
        public int? InventoryID { get; set; }
        public string SKU { get; set; }
        public string Product { get; set; }
        public int? Quantity { get; set; }
    }
}
