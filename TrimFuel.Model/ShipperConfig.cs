using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class ShipperConfig : Entity
    {
        public struct ID
        {
            public int ShipperID { get; set; }
            public string Key { get; set; }
        }

        public ID? ShipperConfigID { get; set; }
        public int? ShipperID { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            
        }
    }
}
