using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class ExtraTrialShipType : Entity
    {
        public int? ExtraTrialShipTypeID { get; set; }
        public string ProductCode { get; set; }
        public string DisplayName { get; set; }
        public bool? Active { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
        }
    }
}
