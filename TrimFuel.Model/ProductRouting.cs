using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class ProductRouting : Entity
    {
        public struct ID
        {
            public int ProductID { get; set; }
        }

        public ID?  ProductRoutingID { get; set; }
        public int? ProductID { get; set; }
        public string RoutingURL { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("ProductID ", ProductID);
            v.AssertNotNull("RoutingURL", RoutingURL);
            v.AssertString("RoutingURL", RoutingURL, 500);
        }
    }
}
