using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class SalesAgentTPClient : Entity
    {
        public long? SalesAgentTPClientID { get; set; }
        public int? SalesAgentID { get; set; }
        public int? TPClientID { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("SalesAgentID", SalesAgentID);
            v.AssertNotNull("TPClientID", TPClientID);
        }
    }    
}
