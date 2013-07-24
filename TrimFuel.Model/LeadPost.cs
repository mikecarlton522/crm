using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class LeadPost : Entity
    {
        public long? LeadPostID { get; set; }
        public long? RegistrationID { get; set; }
        public string Telephone { get; set; }
        public DateTime? CreateDT { get; set; }
        public DateTime? RegistrationDT { get; set; }
        public string PostRequest { get; set; }
        public string PostResponse { get; set; }
        public bool? Completed { get; set; }
        public int? ProductID { get; set; }
        public int? LeadTypeID { get; set; }
        public int? LeadPartnerID { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("Completed", Completed);
            v.AssertNotNull("ProductID", ProductID);
            v.AssertNotNull("LeadTypeID", LeadTypeID);
            v.AssertNotNull("LeadPartnerID", LeadPartnerID);

            v.AssertString("Telephone", Telephone, 50);
        }
    }
}
