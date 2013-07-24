using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class MagentoProductCategoryView : EntityView
    {
        public int? InventoryID { get; set; }
        public string SKU { get; set; }
        public string Product { get; set; }
        public string CategoryName { get; set; }
    }
}
