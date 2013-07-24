using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class UpsellType : Entity
    {
        public int? UpsellTypeID { get; set; }
        public string ProductCode { get; set; }
        public decimal? Price { get; set; }
        public short? Quantity { get; set; }
        public string DisplayName { get; set; }
        public bool DropDown { get; set; }
        public string Description { get; set; }
        public int? ProductID { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("Price", Price);
            v.AssertNotNull("Quantity", Quantity);
            v.AssertNotNull("DropDown", DropDown);
            v.AssertString("ProductCode", ProductCode, 50);
            v.AssertString("DisplayName", DisplayName, 50);
            v.AssertString("Description", Description, 255);
        }
    }
}
