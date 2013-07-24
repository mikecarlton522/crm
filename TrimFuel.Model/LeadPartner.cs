using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class LeadPartner : Entity
    {
        public int? LeadPartnerID { get; set; }
        public string DisplayName { get; set; }
        public bool? ServiceisActive { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("LeadPartnerID", LeadPartnerID);
            v.AssertNotNull("DisplayName", DisplayName);
            v.AssertString("DisplayName", DisplayName, 100);
        }
    }
}
