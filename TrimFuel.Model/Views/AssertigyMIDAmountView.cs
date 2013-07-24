using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class AssertigyMIDAmountView : EntityView
    {
        public AssertigyMIDBriefView MID { get; set; }
        public decimal? Amount { get; set; }
    }
}
