using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class InventorySKUMappingView : EntityView
    {
        public Inventory Inventory { get; set; }
        public IList<Set<Shipper, InventorySKU>> Mapping { get; set; }
    }
}
