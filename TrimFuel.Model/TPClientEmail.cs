using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class TPClientEmail : Entity
    {
        public int TPClientEmailID { get; set; }
        public int TPClientID { get; set; }
        public int AdminID { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public string Response { get; set; }
        public DateTime? CreateDT { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("TPClientEmailID", TPClientEmailID);
            v.AssertNotNull("TPClientID", TPClientID);
            v.AssertNotNull("AdminID", AdminID);
            v.AssertString("Content", Content, 2000);
            v.AssertString("From", From, 100);
            v.AssertString("To", To, 100);
            v.AssertString("Subject", Subject, 200);
        }
    }
}
