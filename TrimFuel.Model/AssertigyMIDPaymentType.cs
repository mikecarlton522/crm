using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class AssertigyMIDPaymentType : Entity
    {
        public struct ID
        {
            public int AssertigyMIDID { get; set; }
            public int PaymentTypeID { get; set; }
        }

        public ID? AssertigyMIDPaymentTypeID { get; set; }
        public int? AssertigyMIDID { get; set; }
        public int? PaymentTypeID { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("AssertigyMIDID", AssertigyMIDID);
            v.AssertNotNull("PaymentTypeID", PaymentTypeID);
        }
    }
}
