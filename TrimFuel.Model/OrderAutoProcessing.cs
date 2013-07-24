using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class OrderAutoProcessing : Entity
    {
        public struct ID
        {
            public long OrderID { get; set; }
        }

        public ID? OrderAutoProcessingID { get; set; }
        public long? OrderID { get; set; }
        public bool? Completed { get; set; }
        public DateTime? ScheduleDT { get; set; }
        public DateTime? CompleteDT { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("OrderID", OrderID);
            v.AssertNotNull("Completed", Completed);
            v.AssertNotNull("ScheduleDT", ScheduleDT);
        }
    }
}
