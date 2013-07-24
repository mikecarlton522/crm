using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Containers;

namespace TrimFuel.Model
{
    //TODO: implemet containers: Address, Person, Contacts
    public class Billing : Entity, ICreditCardContainer
    {
        public long? BillingID { get; set; }
        public int? CampaignID { get; set; }
        public long? RegistrationID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CreditCard { get; set; }
        public string CVV { get; set; }
        public int? PaymentTypeID { get; set; }
        public int? ExpMonth { get; set; }
        public int? ExpYear { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
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
            v.AssertString("CreditCard", CreditCard, 100);
            v.AssertString("CVV", CVV, 5);
            v.AssertString("Address1", Address1, 100);
            v.AssertString("Address2", Address2, 50);
            v.AssertString("City", City, 50);
            v.AssertString("State", State, 50);
            v.AssertString("Country", Country, 50);
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

        public CreditCard CreditCardCnt
        {
            get { return new CreditCard(CreditCard); }
        }

        public string CreditCardLeft6
        {
            get
            {
                return CreditCardCnt.DecryptedCreditCardLeft6;
            }
        }

        public string CreditCardRight4
        {
            get
            {
                return CreditCardCnt.DecryptedCreditCardRight4;
            }
        }

        public void FillFromRegistration(Registration r)
        {
            if (r != null)
            {
                CampaignID = r.CampaignID;
                RegistrationID = r.RegistrationID;

                FirstName = r.FirstName;
                LastName = r.LastName;
                Address1 = r.Address1;
                Address2 = r.Address2;
                City = r.City;
                State = r.State;
                Zip = r.Zip;
                //Country = r.Country;
                Email = r.Email;
                Phone = r.Phone;

                Affiliate = r.Affiliate;
                SubAffiliate = r.SubAffiliate;

                IP = r.IP;
                URL = r.URL;
            }
        }

        public Billing Clone(Billing b)
        {
            return new Billing()
            {
                BillingID = b.BillingID,
                CampaignID = b.CampaignID,
                RegistrationID = b.RegistrationID,
                FirstName = b.FirstName,
                LastName = b.LastName,
                CreditCard = b.CreditCard,
                CVV = b.CVV,
                PaymentTypeID = b.PaymentTypeID,
                ExpMonth = b.ExpMonth,
                ExpYear = b.ExpYear,
                Address1 = b.Address1,
                Address2 = b.Address2,
                City = b.City,
                State = b.State,
                Zip = b.Zip,
                Country = b.Country,
                Email = b.Email,
                Phone = b.Phone,
                CreateDT = b.CreateDT,
                Affiliate = b.Affiliate,
                SubAffiliate = b.SubAffiliate,
                IP = b.IP,
                URL = b.URL
            };
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

        #endregion
    }
}
