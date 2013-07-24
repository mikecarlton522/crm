using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    //TODO: abstract
    public class Sale : Entity
    {
        public long? SaleID { get; set; }
        public short? SaleTypeID { get; set; }
        public string TrackingNumber { get; set; }
        public DateTime? CreateDT { get; set; }
        public bool? NotShip { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertString("TrackingNumber", TrackingNumber, 1024);
        }
    }
}
