using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class AssertigyMIDCapView : EntityView
    {
        public long RemainingCap { get; set; }
        public AssertigyMID AssertigyMID { get; set; }
    }
}
