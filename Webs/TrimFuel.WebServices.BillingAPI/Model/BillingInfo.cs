using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrimFuel.Model;

namespace TrimFuel.WebServices.BillingAPI.Model
{
    public class BillingInfo
    {
        public long BillingID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }       

        public static BillingInfo FromBilling(TrimFuel.Model.Billing billing)
        {
            if (billing == null)
                return null;

            BillingInfo res = new BillingInfo();
            res.BillingID = (long)billing.BillingID;
            res.FirstName = billing.FirstName;
            res.LastName = billing.LastName;
            res.Address1 = billing.Address1;
            res.Address2 = billing.Address2;
            res.City = billing.City;
            res.State = billing.State;
            res.Zip = billing.Zip;
            res.Phone = billing.Phone;
            res.Email = billing.Email;
           
            return res;
        }
    }
}