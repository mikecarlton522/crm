using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class ProductCode : Entity
    {
        public int? ProductCodeID { get; set; }
        public string ProductCode_ { get; set; }
        public string Name { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("ProductCode", ProductCode_);
            v.AssertNotNull("Name", Name);
            v.AssertString("ProductCode", ProductCode_, 50);
            v.AssertString("Name", Name, 50);
        }
    }
}
