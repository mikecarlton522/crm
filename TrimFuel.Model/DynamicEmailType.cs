using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class DynamicEmailType : Entity
    {
        public byte? DynamicEmailTypeID { get; set; }
        public string DisplayName { get; set; }
        public bool? Instant { get; set; }
        public byte? SortOrder { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("Instant", Instant);
            v.AssertString("DisplayName", DisplayName, 255);
        }
    }
}
