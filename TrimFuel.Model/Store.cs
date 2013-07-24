using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class Store : Entity
    {
        public int? StoreID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("Name", Name);
            v.AssertString("Name", Name, 50);
            v.AssertString("Description", Description, 255);
        }
    }
}
