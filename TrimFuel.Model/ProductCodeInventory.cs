using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class ProductCodeInventory : Entity
    {
        public int? ProductCodeInventoryID { get; set; }
        public int? ProductCodeID { get; set; }
        public int? InventoryID { get; set; }
        public int? Quantity { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
        }
    }
}
