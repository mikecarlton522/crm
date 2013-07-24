using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class BlockedEmailDomain: Entity
    {
        public int? BlockedEmailDomainId { get; set; }
        public string Name { get; set; }
        public DateTime? CreateDT { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
        }
    }
}
