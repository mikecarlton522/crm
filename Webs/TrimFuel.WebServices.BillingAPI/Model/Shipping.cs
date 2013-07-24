using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrimFuel.Model;

namespace TrimFuel.WebServices.BillingAPI.Model
{
    public class Shipping
    {
        public long ProspectID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public static Shipping FromRegistration(Registration reg)
        {
            if (reg == null)
            {
                return null;
            }

            Shipping res = new Shipping();
            res.ProspectID = (long)reg.RegistrationID;
            res.FirstName = reg.FirstName;
            res.LastName = reg.LastName;
            res.Address1 = reg.Address1;
            res.Address2 = reg.Address2;
            res.City = reg.City;
            res.State = reg.State;
            res.Zip = reg.Zip;
            res.Phone = reg.Phone;
            res.Email = reg.Email;

            return res;
        }
    }
}