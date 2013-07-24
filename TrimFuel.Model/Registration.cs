using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Containers;

namespace TrimFuel.Model
{
    //TODO: implemet containers: Address, Person, Contacts
    public class Registration : Entity
    {
        public long? RegistrationID { get; set; }
        public int? CampaignID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime? CreateDT { get; set; }
        public string Affiliate { get; set; }
        public string SubAffiliate { get; set; }
        public string IP { get; set; }
        public string URL { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertString("FirstName", FirstName, 50);
            v.AssertString("LastName", LastName, 50);
            v.AssertString("Address1", Address1, 100);
            v.AssertString("Address2", Address2, 50);
            v.AssertString("City", City, 50);
            v.AssertString("State", State, 50);
            v.AssertString("Email", Email, 60);
            v.AssertString("Phone", Phone, 50);
            v.AssertString("Affiliate", Affiliate, 50);
            v.AssertString("SubAffiliate", SubAffiliate, 50);
            v.AssertString("IP", IP, 50);
            v.AssertString("URL", URL, 1024);
        }

        #region Logic

        public Phone PhoneCnt
        {
            get { return new Phone(Phone); }
        }

        public Zip ZipCnt
        {
            get { return new Zip(Zip); }
        }

        #endregion

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
