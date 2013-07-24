using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class ProductCategory : Entity
    {
        public int? ProductCategoryID { get; set; }
        public string CategoryName { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertString("CategoryName", CategoryName, 250); 
        }
    }
}
