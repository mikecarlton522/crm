using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class SaleDetails : Entity
    {
        public long? SaleID { get; set; }
        public string Description { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("SaleID", SaleID);
            v.AssertString("Description", Description, 1000);
        }
    }
}
