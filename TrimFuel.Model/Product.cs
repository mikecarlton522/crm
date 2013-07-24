using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class Product : Entity
    {
        public int? ProductID { get; set; }
        public string ProductName { get; set; }
        public string Code { get; set; }
        public bool? ProductIsActive { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("ProductName", ProductName);
            v.AssertNotNull("ProductIsActive", ProductIsActive);
            v.AssertString("ProductName", ProductName, 100);
            v.AssertString("Code", Code, 50);
        }
    }
}
