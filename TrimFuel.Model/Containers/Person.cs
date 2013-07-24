using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Containers
{
    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public static Person Create(Registration registration)
        {
            if (registration == null)
            {
                return null;
            }
            Person res = new Person();
            res.FirstName = registration.FirstName;
            res.LastName = registration.LastName;
            return res;
        }

        public string FullName
        {
            get
            {
                if (string.IsNullOrEmpty(FirstName))
                    return LastName;
                if (string.IsNullOrEmpty(LastName))
                    return FirstName;
                return string.Format("{0} {1}", FirstName, LastName);
            }
        }
    }
}
