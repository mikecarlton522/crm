using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class ProductEvent : Entity
    {
        public int? ProductEventID { get; set; }
        public int? ProductID { get; set; }
        public int? EventTypeID { get; set; }
        public string URl { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertString("URl", URl, 250);
        }
    }
}
