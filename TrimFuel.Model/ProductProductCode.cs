using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class ProductProductCode : Entity
    {
        public struct ID
        {
            public int? ProductID { get; set; }
            public int? ProductCodeID { get; set; }
        }

        public ID? ProductProductCodeID { get; set; }
        public int? ProductID { get; set; }
        public int? ProductCodeID { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
        }
    }
}
