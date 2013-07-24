using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class RebillDiscountType : Entity
    {
        public int? DiscountTypeID { get; set; }
        public string Name { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
        }
    }
}
