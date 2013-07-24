using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class ShipperProduct : Entity
    {
        public int? ShipperID { get; set; }
        public int? ProductID { get; set; }
        public bool? NeedConfirm { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            
        }
    }
}
