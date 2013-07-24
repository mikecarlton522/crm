using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class TagBillingLink : Entity
    {
        public struct ID
        {
            public int TagID { get; set; }
            public long BillingID { get; set; }
        }

        public ID? TagBillingLinkID { get; set; }
        public int? TagID { get; set; }
        public long? BillingID { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("TagID", TagID);
            v.AssertNotNull("BillingID", BillingID);
        }
    }
}
