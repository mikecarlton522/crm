using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ecigsbrand.Store2.Controls
{
    public class GiftCertificateEventArgs : EventArgs
    {
        public GiftCertificateEventArgs(string giftCertificateNumber)
        {
            this.GiftCertificateNumber = giftCertificateNumber;
        }

        public string GiftCertificateNumber { get; set; }
    }
}
