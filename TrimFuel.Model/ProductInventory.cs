using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class ProductInventory : Entity
    {
        public struct ID
        {
            public int? ProductID { get; set; }
            public int? InventoryID { get; set; }
        }

        public ID? ProductInventoryID { get; set; }
        public int? ProductID { get; set; }
        public int? InventoryID { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
        }
    }
}
