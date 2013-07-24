using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeautyTruth.Store1.Controls
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
