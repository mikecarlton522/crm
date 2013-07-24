using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrimFuel.Model;

namespace TrimFuel.WebServices.BillingAPI.Model
{
    public class UserInfo
    {
        public Shipping Shipping { get; set; }
        public BillingInfo Billing { get; set; }
        public ChargeHistoryList ChargeHistoryList { get; set; }
    }

    public class UserInfoList : List<UserInfo>
    {

    }
}