using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrimFuel.Model;

namespace TrimFuel.WebServices.BillingAPI.Model
{
    public class Prospect
    {
        public long? ProspectID { get; set; }

        public static Prospect FromRegistration(Registration reg)
        {
            if (reg == null)
            {
                return null;
            }

            Prospect res = new Prospect();
            res.ProspectID = reg.RegistrationID;

            return res;
        }
    }
}
