using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class PaymentType : Entity
    {
        public int? PaymentTypeID { get; set; }
        public string DisplayName { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("PaymentTypeID", PaymentTypeID);
            v.AssertString("DisplayName", DisplayName, 50);
        }
    }
}
