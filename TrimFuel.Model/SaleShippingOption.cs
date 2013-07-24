using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class SaleShippingOption : Entity
    {
        public struct ID
        {
            public long SaleID { get; set; }
        }

        public SaleShippingOption.ID? SaleShippingOptionID { get; set; }
        public long? SaleID { get; set; }
        public int? ShippingOptionID { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("SaleID", SaleID);
            v.AssertNotNull("ShippingOptionID", ShippingOptionID);
        }
    }
}
