using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class Call : Entity
    {
        public long? CallID { get; set; }
        public long? ExternalCallID { get; set; }
        public DateTime? StartDT { get; set; }
        public DateTime? EndDT { get; set; }
        public string ANI { get; set; }
        public string DNIS { get; set; }
        public int? AgentID { get; set; }
        public string AgentName { get; set; }
        public string AgentLocation { get; set; }
        public int? DispositionID { get; set; }
        public string DispositionName { get; set; }
        public string AgentNotes { get; set; }
        public long? CustomerID { get; set; }
        public string CustomerProduct { get; set; }
        public DateTime? CreateDT { get; set; }
        public string Partner { get; set; }
        public string Version { get; set; }
        public int? HoldTime { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertString("AgentName", AgentName, 50);
            v.AssertString("AgentLocation", AgentLocation, 50);
            v.AssertString("DispositionName", DispositionName, 50);
            v.AssertString("AgentNotes", AgentNotes, 1000);
            v.AssertString("CustomerProduct", CustomerProduct, 1000);
            v.AssertString("Partner", Partner, 100);
            v.AssertString("Version", Version, 100);
        }
    }
}
