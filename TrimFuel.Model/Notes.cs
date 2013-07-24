using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class Notes : Entity
    {
        public int? NotesID { get; set; }
        public int? BillingID { get; set; }
        public int? AdminID { get; set; }
        public string Content { get; set; }
        public DateTime? CreateDT { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
        }
    }
}
