using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class ShippingNote : Entity
    {
        public long? ShippingNoteID { get; set; }
        public string Note { get; set; }
        public int? NoteShipmentStatus { get; set; }
        public DateTime? CreateDT { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("Note", Note);
            v.AssertNotNull("NoteShipmentStatus", NoteShipmentStatus);
            v.AssertNotNull("CreateDT", CreateDT);
        }

    }
}
