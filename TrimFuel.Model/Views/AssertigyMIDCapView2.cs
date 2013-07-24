using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class AssertigyMIDCapView2 : EntityView
    {
        public AssertigyMID MID { get; set; }
        public decimal? RemainingCap { get; set; }
    }
}
