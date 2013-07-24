using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class PageType : Entity
    {
        public int? PageTypeID { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("PageTypeID", PageTypeID);
            v.AssertString("Name", Name, 50);
            v.AssertNotNull("Order", Order);
        }
    }
}
