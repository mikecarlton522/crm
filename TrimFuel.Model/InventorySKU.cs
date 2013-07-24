using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class InventorySKU : Entity
    {
        public struct ID
        {
            public string ProductSKU { get; set; }
            public int ShipperID { get; set; }
        }

        public ID? InventorySKUID { get; set; }
        public string ProductSKU { get; set; }
        public int? ShipperID { get; set; }
        public string InventorySKU_ { get; set; }
        public int? InStock { get; set; }
        public decimal? Cost { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
        }
    }
}
