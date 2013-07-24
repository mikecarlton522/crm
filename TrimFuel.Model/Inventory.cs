using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Enums;

namespace TrimFuel.Model
{
    public class Inventory : Entity
    {
        public int? InventoryID { get; set; }
        public string SKU { get; set; }
        public string Product { get; set; }
        public int? InStock { get; set; }
        public decimal? Costs { get; set; }
        public decimal? RetailPrice { get; set; }
        public int? InventoryType { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("SKU", SKU);
            v.AssertNotNull("Product", Product);
            v.AssertNotNull("InStock", InStock);
            v.AssertNotNull("InventoryType", InventoryType);
            v.AssertString("SKU", SKU, 50);
            v.AssertString("Product", Product, 50);
        }

        public override void SetDefaultValues()
        {
            base.SetDefaultValues();

            if (InventoryType == null)
                InventoryType = InventoryTypeEnum.Inventory;
        }
    }
}
