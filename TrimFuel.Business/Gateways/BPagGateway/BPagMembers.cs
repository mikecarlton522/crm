using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace TrimFuel.Business.Gateways.BPagGateway
{
    public class BPagMembers
    {
        public static string Version
        {
            get
            {
                return "1.1.0";
            }
        }
    }

    public class BPagOrderStatus
    {
        public static string Paid { get { return "0"; } }
        public static string NotPaid { get { return "1"; } }
        public static string Invalid { get { return "2"; } }
        public static string Cancelled { get { return "3"; } }
        public static string NotEffective { get { return "4"; } }
        public static string InsufficientBalance { get { return "5"; } }
        public static string AuthorizationPending { get { return "6"; } }
        public static string PaymentPending { get { return "7"; } }
        public static string NotCaptured { get { return "8"; } }
        public static string PartiallyPaid { get { return "10"; } }
        public static string InAnalyses { get { return "12"; } }
    }

    public class BPagResponceStatus
    {
        public static string Success { get { return "0"; } }
    }
}
