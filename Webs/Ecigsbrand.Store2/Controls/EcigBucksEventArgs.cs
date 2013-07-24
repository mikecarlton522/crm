using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ecigsbrand.Store2.Controls
{
    public class EcigBucksEventArgs : EventArgs
    {
        public EcigBucksEventArgs(decimal ecigBucksAmount)
        {
            this.EcigBucksAmount = ecigBucksAmount;
        }

        public decimal EcigBucksAmount { get; set; }
    }
}
