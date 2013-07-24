using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Business.Gateways.CBG
{
    public class TransactionType
    {
        public static string Sale = "sale";
        public static string Authorization = "auth";
        public static string Capture = "capture";
        public static string Void = "void";
        public static string Refund = "refund";
        public static string Credit = "credit";
        public static string Update = "update";
        public static string Validate = "validate";
    }
}
