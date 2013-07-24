using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class ClosedMIDQueue : Entity
    {
        public int? ClosedMIDQueueID { get; set; }
        public int? ClosedMIDID { get; set; }
        public bool? Queued { get; set; }
        public int? PaymentTypeID { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
        }
    }
}
