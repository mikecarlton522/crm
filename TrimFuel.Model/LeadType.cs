using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class LeadType : Entity
    {
        public int? LeadTypeID { get; set; }
        public string DisplayName { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("LeadTypeID", LeadTypeID);
            v.AssertNotNull("DisplayName", DisplayName);
            v.AssertString("DisplayName", DisplayName, 100);
        }
    }
}
