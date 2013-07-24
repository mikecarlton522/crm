using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class Referer : Entity
    {
        public int? RefererID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public string RefererCode { get; set; }
        public int? ParentRefererID { get; set; }
        public DateTime? CreateDT { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertString("FirstName", FirstName, 50);
            v.AssertString("LastName", LastName, 50);
            v.AssertString("Company", Company, 50);
            v.AssertString("Address1", Address1, 50);
            v.AssertString("Address2", Address2, 50);
            v.AssertString("City", City, 50);
            v.AssertString("State", State, 50);
            v.AssertString("Zip", Zip, 50);
            v.AssertString("Country", Country, 50);
            v.AssertString("RefererCode", RefererCode, 50);
            v.AssertString("Username", Username, 50);
            v.AssertString("Password", Password, 50);
        }

        #region Logic

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

        #endregion
    }
}
