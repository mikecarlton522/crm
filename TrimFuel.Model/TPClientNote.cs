using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class TPClientNote : Entity
    {
        public int TPClientNoteID { get; set; }
        public int TPClientID { get; set; }
        public int AdminID { get; set; }
        public string Content { get; set; }
        public DateTime? CreateDT { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("TPClientNoteID", TPClientNoteID);
            v.AssertNotNull("TPClientID", TPClientID);
            v.AssertNotNull("AdminID", AdminID);
            v.AssertString("Content", Content, 1500);
        }
    }
}
