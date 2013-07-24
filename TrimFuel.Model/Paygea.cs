using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class Paygea : Entity
    {
        public long? PaygeaID { get; set; }
        public long? BillingID { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public DateTime? CreateDT { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
        }
    }
}
