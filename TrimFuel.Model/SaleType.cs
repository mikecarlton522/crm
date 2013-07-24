using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class SaleType : Entity
    {
        public short? SaleTypeID { get; set; }
        public string Name { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertString("Name", Name, 20);
        }
    }
}
