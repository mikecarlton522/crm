using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fitdiet.Store1.Controls
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
