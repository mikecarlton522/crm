using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class ClosedMIDRouting : Entity
    {
        public int? ClosedMIDRoutingID  { get; set; }
        public int? ClosedMIDID { get; set; }
        public int? AssertigyMIDID { get; set; }
        public int? Percentage { get; set; }
        public int? PaymentTypeID { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("ClosedMIDID", ClosedMIDID);
            v.AssertNotNull("AssertigyMIDID", AssertigyMIDID);
        }
    }
}
