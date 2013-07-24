using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class ChargebackReasonCode : Entity
    {
        public int? ChargebackReasonCodeID { get; set; }
        public int? ReasonCode { get; set; }
        public int? PaymentTypeID { get; set; }
        public string Description { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertString("Description", Description, 255);
        }
    }
}
