using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class AssertigyMIDDailyCap : Entity
    {
        public int? AssertigyMIDDailyCapID { get; set; }
        public int? AssertigyMIDID { get; set; }
        public double? TotalAmount { get; set; }
        public DateTime? CreateDT { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
        }
    }
}
