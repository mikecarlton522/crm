using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class PartnerClick : Entity
    {
        public int? PartnerClickID { get; set; }
        public long? BillingID { get; set; }
        public string ClickID { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertString("ClickID", ClickID, 50);
        }
    }
}
