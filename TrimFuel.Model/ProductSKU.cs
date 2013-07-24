using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class ProductSKU : Entity
    {
        public string ProductSKU_ { get; set; }
        public string ProductName { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
        }
    }
}
