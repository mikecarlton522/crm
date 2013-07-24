using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Containers
{
    public class Address
    {
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }

        public string FullAddress
        {
            get
            {
                if (string.IsNullOrEmpty(Address1))
                    return Address2;
                if (string.IsNullOrEmpty(Address2))
                    return Address1;
                return string.Format("{0} {1}", Address1, Address2);
            }
        }

        public static Address Create(Registration registration, RegistrationInfo registrationInfo)
        {
            if (registration == null)
            {
                return null;
            }
            Address res = new Address();
            res.Address1 = registration.Address1;
            res.Address2 = registration.Address2;
            res.City = registration.City;
            res.State = registration.State;
            res.Zip = registration.Zip;
            res.Country = (registrationInfo != null ? registrationInfo.Country : null);
            return res;
        }
    }
}
