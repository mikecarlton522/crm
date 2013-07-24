using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class MagentoProductCategory : Entity
    {
        public int? InventoryID { get; set; }
        public string CategoryName { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("InventoryID", InventoryID);
            v.AssertString("CategoryName", CategoryName, 50);
        }

    }
}
