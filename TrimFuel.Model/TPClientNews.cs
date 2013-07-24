using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class TPClientNews : Entity
    {
        public int? TPClientNewsID { get; set; }
        public int? AdminID { get; set; }
        public string Content { get; set; }
        public DateTime? CreateDT { get; set; }
        public bool Active { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("TPClientNewsID", TPClientNewsID);
            v.AssertNotNull("AdminID", AdminID);
            v.AssertString("Content", Content, 2000);
            v.AssertNotNull("CreateDT", CreateDT);
            v.AssertNotNull("Active", Active);
        }
    }
}
