using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class AragonLead : Entity
    {
        public long? AragonLeadID { get; set; }
        public long? BillingID { get; set; }
        public int? AragonCampaignID { get; set; }
        public DateTime? VerificationDT { get; set; }
        public int? UpsellStatus { get; set; }
        public string StatusText { get; set; }
        public string Response { get; set; }
        public bool? LeadSent { get; set; }
        public DateTime? CreateDT { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("LeadSent", LeadSent);
            v.AssertString("StatusText", StatusText, 50);
            v.AssertString("Response", Response, 2000);
        }
    }
}
