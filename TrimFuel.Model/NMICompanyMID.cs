using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class NMICompanyMID : Entity
    {
        public int? NMICompanyMID_ { get; set; }
        public int? NMICompanyID { get; set; }
        public int? AssertigyMIDID { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
        }
    }
}
