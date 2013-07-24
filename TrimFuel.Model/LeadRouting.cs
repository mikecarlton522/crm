using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TrimFuel.Model.Views;

namespace TrimFuel.Model
{
    public class LeadRouting : Entity
    {
        public struct ID
        {
            public int ProductID { get; set; }
            public int LeadTypeID { get; set; }
            public int LeadPartnerID { get; set; }
        }

        public ID? LeadRoutingID { get; set; }
        public int? ProductID { get; set; }
        public int? LeadTypeID { get; set; }
        public int? LeadPartnerID { get; set; }
        public int? Percentage { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("ProductID", ProductID);
            v.AssertNotNull("LeadTypeID", LeadTypeID);
            v.AssertNotNull("LeadPartnerID", LeadPartnerID);
            v.AssertNotNull("Percentage", Percentage);
        }

        
    }
}
