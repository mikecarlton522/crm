using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class LeadPartnerConfigValue : Entity
    {
        public struct ID
        {
            public int? LeadPartnerID { get; set; }
            public int? ProductID { get; set; }
            public int? LeadTypeID { get; set; }
            public string Key { get; set; }
        }

        public ID? LeadPartnerConfigValueID { get; set; }
        public int? LeadPartnerID { get; set; }
        public int? ProductID { get; set; }
        public int? LeadTypeID { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }


        protected override void ValidateFields(ValidateHelper v)
        {
        }
    }
}
