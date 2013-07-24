using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class OrderQueueNote : Entity
    {
        public struct ID
        {
            public long OrderID { get; set; }
        }

        public ID? OrderQueueNoteID { get; set; }
        public long? OrderID { get; set; }
        public string Reason { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("OrderID", OrderID);
            v.AssertString("Reason", Reason, 255);
        }
    }
}
