using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class ShipperRecord : Entity
    {
        public long? SaleID { get; set; }
        public long? RegID { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public string StatusResponse { get; set; }
        public bool? Completed { get; set; }
        public DateTime? CreateDT { get; set; }
        public DateTime? ShippedDT { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("SaleID", SaleID);
            v.AssertNotNull("Completed", Completed);
        }
    }
}
