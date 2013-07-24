using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using TrimFuel.Model.Containers;
using TrimFuel.Model.Views;

namespace TrimFuel.Business.Flow
{
    public class UserBuilder : BaseService
    {
        public UserView User { get; set; }

        public UserBuilder LoadByBillingID(long? billingID)
        {
            Billing b = Load<Billing>(billingID);
            if (b != null)
            {
                Registration r = Load<Registration>(b.RegistrationID);
                if (r != null)
                {
                    RegistrationService regSrv = new RegistrationService();
                    BillingExternalInfo bExt = regSrv.GetBillingExternalInfo(b.BillingID.Value);
                    RegistrationInfo rExt = regSrv.GetRegistrationInfo(b.RegistrationID.Value);
                    User = new UserView() { 
                        Billing = b,
                        Registration = r,
                        BillingExternalInfo = bExt,
                        RegistrationInfo  = rExt
                    };
                }
            }
            return this;
        }

        public UserBuilder LoadByRegistrationID(long? billingID)
        {
            throw new NotImplementedException();
        }

        public UserBuilder LoadByInternalID(long? billingID)
        {
            throw new NotImplementedException();
        }

        public UserBuilder Create(int? campaignID, string affiliate, string subAffiliate, string ip, string url)
        {
            User = new UserView();
            User.Registration = new Registration() { 
                CampaignID = campaignID,
                Affiliate = affiliate,
                SubAffiliate = subAffiliate,
                IP = ip,
                URL = url
            };
            User.RegistrationInfo = new RegistrationInfo();
            User.Billing = new Billing() {
                CampaignID = campaignID,
                Affiliate = affiliate,
                SubAffiliate = subAffiliate,
                IP = ip,
                URL = url
            };
            User.BillingExternalInfo = new BillingExternalInfo();

            return this;
        }

        public UserBuilder SetShippingAddress(string firstName, string lastName,
            string address1, string address2, string city, string state, string zip, string country,
            string email, string phone)
        {
            User.Registration.FirstName = firstName;
            User.Registration.LastName = lastName;
            User.Registration.Address1 = address1;
            User.Registration.Address2 = address2;
            User.Registration.City = city;
            User.Registration.State = state;
            User.Registration.Zip = zip;
            if (User.RegistrationInfo == null)
            {
                User.RegistrationInfo = new RegistrationInfo();
                User.RegistrationInfo.RegistrationID = User.Registration.RegistrationID;
            }
            User.RegistrationInfo.Country = country;
            User.Registration.Email = email;
            User.Registration.Phone = phone;
            return this;
        }

        public UserBuilder SetBillingAddress(string firstName, string lastName,
        string address1, string address2, string city, string state, string zip, string country,
        string email, string phone)
        {
            User.Billing.FirstName = firstName;
            User.Billing.LastName = lastName;
            User.Billing.Address1 = address1;
            User.Billing.Address2 = address2;
            User.Billing.City = city;
            User.Billing.State = state;
            User.Billing.Zip = zip;
            User.Billing.Country = country;
            User.Billing.Email = email;
            User.Billing.Phone = phone;
            return this;
        }

        public UserBuilder SetAdditionalInfo(string internalID,
            string customField1, string customField2, string customField3, string customField4, string customField5)
        {
            if (User.BillingExternalInfo == null)
            {
                User.BillingExternalInfo = new BillingExternalInfo();
                User.BillingExternalInfo.BillingID = User.Billing.BillingID;
            }
            User.BillingExternalInfo.InternalID = internalID;
            User.BillingExternalInfo.CustomField1 = customField1;
            User.BillingExternalInfo.CustomField2 = customField2;
            User.BillingExternalInfo.CustomField3 = customField3;
            User.BillingExternalInfo.CustomField4 = customField4;
            User.BillingExternalInfo.CustomField5 = customField5;
            return this;
        }

        public UserBuilder SetCreditCard(string creditCard, string cvv, int? expMonth, int? expYear)
        {
            User.Billing.CreditCard = (new CreditCard(creditCard)).EncryptedCreditCard;
            User.Billing.CVV = cvv;
            User.Billing.ExpMonth = expMonth;
            User.Billing.ExpYear = expYear;
            User.Billing.PaymentTypeID = User.Billing.CreditCardCnt.TryGetCardType();
            return this;
        }

        public virtual BusinessError<IList<string>> Validate()
        {
            BusinessError<IList<string>> res = new BusinessError<IList<string>>(null, BusinessErrorState.Success, "");
            //res.ErrorMessage = "One or more fields are invalid"
            //res.ReturnValue = new List<string>();
            //res.ReturnValue.Add("... is required");
            //...
            return res;
        }

        public BusinessError<UserView> Save()
        {
            BusinessError<UserView> res = new BusinessError<UserView>(User, BusinessErrorState.Success, string.Empty);

            BusinessError<IList<string>> isValid = Validate();
            if (isValid.State == BusinessErrorState.Error)
            {
                res.State = BusinessErrorState.Error;                
                res.ErrorMessage = isValid.ErrorMessage + string.Join("\n", isValid.ReturnValue.ToArray());
            }
            else
            {
                try
                {
                    dao.BeginTransaction();

                    if (User.Registration.RegistrationID == null)
                    {
                        User.Registration.CreateDT = DateTime.Now;
                    }
                    dao.Save(User.Registration);
                    if (User.RegistrationInfo != null)
                    {
                        User.RegistrationInfo.RegistrationID = User.Registration.RegistrationID;
                        dao.Save(User.RegistrationInfo);
                    }
                    
                    User.Billing.RegistrationID = User.Registration.RegistrationID;
                    if (User.Billing.BillingID == null)
                    {
                        User.Billing.CreateDT = DateTime.Now;
                    }
                    dao.Save(User.Billing);
                    if (User.BillingExternalInfo != null)
                    {
                        User.BillingExternalInfo.BillingID = User.Billing.BillingID;
                        dao.Save(User.BillingExternalInfo);
                    }

                    dao.CommitTransaction();
                }
                catch (Exception ex)
                {
                    dao.RollbackTransaction();
                    logger.Error(ex);
                    res.ErrorMessage = "Unknown error occurred.";
                    res.State = BusinessErrorState.Error;
                    res.ReturnValue = null;
                    User = null;
                }
            }

            return res;
        }
    }
}
