using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class AggShipperSale : Entity
    {
        public long? SaleID { get; set; }
        public int? ShipperID { get; set; }
        public DateTime? CreateDT { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
        }
    }
}
