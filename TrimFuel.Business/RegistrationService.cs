using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using TrimFuel.Business.Utils;
using MySql.Data.MySqlClient;
using TrimFuel.Model.Views;
using TrimFuel.Model.Containers;
using TrimFuel.Model.Enums;

namespace TrimFuel.Business
{
    public class RegistrationService : BaseService
    {
        //TODO: config
        public const string DEFAULT_COUNTRY = "US";
        public const string UK_COUNTRY = "United Kingdom";

        public Registration CreateRegistrationFromDynamicCampaign(int? campaignID, string firstName, string lastName, string address1,
            string address2, string city, string state, string zip, string country, string email, string phone,
            DateTime? createDT, string affiliate, string subAffiliate, string ip, string url)
        {
            var registration = CreateRegistration(campaignID, firstName, lastName, address1, address2, city, state, zip, country,
                email, phone, createDT, affiliate, subAffiliate, ip, url);

            if (registration != null)
            {
                new EventService().RegistrationAndConfirmation(campaignID, null, email, zip, phone, firstName, lastName, registration.RegistrationID, EventTypeEnum.Registration);
            }

            return registration;
        }

        public Registration CreateRegistration(int? campaignID, string firstName, string lastName, string address1,
            string address2, string city, string state, string zip, string country, string email, string phone,
            DateTime? createDT, string affiliate, string subAffiliate, string ip, string url)
        {
            if (string.IsNullOrEmpty(country))
            {
                country = DEFAULT_COUNTRY;
            }

            Registration res = null;
            try
            {
                dao.BeginTransaction();

                res = new Registration()
                {
                    CampaignID = campaignID,
                    FirstName = firstName,
                    LastName = lastName,
                    Address1 = address1,
                    Address2 = address2,
                    City = city,
                    State = state,
                    Zip = zip,
                    Email = email,
                    Phone = phone,
                    CreateDT = createDT,
                    Affiliate = affiliate,
                    SubAffiliate = subAffiliate,
                    IP = ip,
                    URL = url
                };
                dao.Save<Registration>(res);

                RegistrationInfo regInfo = new RegistrationInfo()
                {
                    RegistrationID = res.RegistrationID,
                    Country = country
                };
                dao.Save<RegistrationInfo>(regInfo);

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
                res = null;
            }
            return res;
        }

        public bool UpdateRegistration(long registrationID, int? campaignID, string firstName, string lastName,
            string address1, string address2, string city, string state, string zip, string country, string email, string phone,
            DateTime? createDT, string affiliate, string subAffiliate, string ip, string url)
        {
            if (string.IsNullOrEmpty(country))
            {
                country = DEFAULT_COUNTRY;
            }

            bool res = true;
            try
            {
                dao.BeginTransaction();

                Registration reg = new Registration()
                {
                    RegistrationID = registrationID,
                    CampaignID = campaignID,
                    FirstName = firstName,
                    LastName = lastName,
                    Address1 = address1,
                    Address2 = address2,
                    City = city,
                    State = state,
                    Zip = zip,
                    Email = email,
                    Phone = phone,
                    CreateDT = createDT,
                    Affiliate = affiliate,
                    SubAffiliate = subAffiliate,
                    IP = ip,
                    URL = url
                };

                RegistrationInfo regInfo = GetRegistrationInfo(registrationID);
                if (regInfo == null)
                {
                    regInfo = new RegistrationInfo()
                    {
                        RegistrationID = registrationID
                    };
                }
                regInfo.Country = country;

                dao.Save<Registration>(reg);
                dao.Save<RegistrationInfo>(regInfo);

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
                res = false;
            }
            return res;
        }

        public RegistrationInfo GetRegistrationInfo(long registrationID)
        {
            RegistrationInfo res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("select * from RegistrationInfo " +
                    "where RegistrationID = @registrationID ");
                q.Parameters.Add("@registrationID", MySqlDbType.Int64).Value = registrationID;

                res = dao.Load<RegistrationInfo>(q).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public BillingExternalInfo GetBillingExternalInfo(long billingID)
        {
            BillingExternalInfo res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("select * from BillingExternalInfo" +
                    " where BillingID = @billingID ");
                q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = billingID;

                res = dao.Load<BillingExternalInfo>(q).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public Billing CreateBilling(int? campaignID, long? registrationID, string firstName,
            string lastName, string creditCard, string cvv, int? paymentTypeID, int? expMonth,
            int? expYear, string address1, string address2, string city, string state, string zip,
            string country, string email, string phone, DateTime? createDT, string affiliate,
            string subAffiliate, string ip, string url)
        {
            if (string.IsNullOrEmpty(country))
            {
                country = DEFAULT_COUNTRY;
            }

            Billing res = null;
            try
            {
                dao.BeginTransaction();

                res = new Billing()
                {
                    CampaignID = campaignID,
                    RegistrationID = registrationID,
                    FirstName = firstName,
                    LastName = lastName,
                    CreditCard = (new CreditCard(creditCard)).EncryptedCreditCard,
                    CVV = cvv,
                    PaymentTypeID = paymentTypeID,
                    ExpMonth = expMonth,
                    ExpYear = expYear,
                    Address1 = address1,
                    Address2 = address2,
                    City = city,
                    State = state,
                    Zip = zip,
                    Country = country,
                    Email = email,
                    Phone = phone,
                    CreateDT = createDT,
                    Affiliate = affiliate,
                    SubAffiliate = subAffiliate,
                    IP = ip,
                    URL = url
                };

                dao.Save<Billing>(res);

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
                res = null;
            }
            return res;
        }

        public bool UpdateBilling(long billingID, int? campaignID, long? registrationID, string firstName,
            string lastName, string creditCard, string cvv, int? paymentTypeID, int? expMonth,
            int? expYear, string address1, string address2, string city, string state, string zip,
            string country, string email, string phone, DateTime? createDT, string affiliate,
            string subAffiliate, string ip, string url)
        {
            if (string.IsNullOrEmpty(country))
            {
                country = DEFAULT_COUNTRY;
            }

            bool res = true;
            try
            {
                dao.BeginTransaction();

                Billing bill = new Billing()
                {
                    BillingID = billingID,
                    CampaignID = campaignID,
                    RegistrationID = registrationID,
                    FirstName = firstName,
                    LastName = lastName,
                    CreditCard = (new CreditCard(creditCard)).EncryptedCreditCard,
                    CVV = cvv,
                    PaymentTypeID = paymentTypeID,
                    ExpMonth = expMonth,
                    ExpYear = expYear,
                    Address1 = address1,
                    Address2 = address2,
                    City = city,
                    State = state,
                    Zip = zip,
                    Country = country,
                    Email = email,
                    Phone = phone,
                    CreateDT = createDT,
                    Affiliate = affiliate,
                    SubAffiliate = subAffiliate,
                    IP = ip,
                    URL = url
                };

                dao.Save<Billing>(bill);

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
                res = false;
            }
            return res;
        }

        public Set<Registration, Billing> CreateOrUpdateRegistrationAndBilling(long? billingID, long? registrationID,
            string firstName, string lastName, string address1, string address2, string city, string state, string zip, string country,
            string phone, string email,
            int? campaignID, string affiliate, string subAffiliate, string ip, string url,
            int? paymentTypeID, string creditCard, string cvv, int? expMonth, int? expYear)
        {
            Set<Registration, Billing> res = new Set<Registration, Billing>();
            try
            {
                dao.BeginTransaction();

                if (registrationID == null)
                {
                    res.Value1 = CreateRegistration(campaignID, firstName, lastName, address1,
                        address2, city, state, zip, country, email, phone, DateTime.Now, affiliate, subAffiliate, ip, url);

                    registrationID = res.Value1.RegistrationID;
                }
                else
                {
                    res.Value1 = EnsureLoad<Registration>(registrationID);
                    UpdateRegistration(registrationID.Value, campaignID, firstName, lastName, address1,
                        address2, city, state, zip, country, email, phone, res.Value1.CreateDT, affiliate, subAffiliate, ip, url);
                    res.Value1 = EnsureLoad<Registration>(registrationID);
                }

                if (billingID == null)
                {
                    res.Value2 = CreateBilling(campaignID, registrationID, firstName, lastName,
                        creditCard, cvv, paymentTypeID, expMonth, expYear, address1, address2, city,
                        state, zip, country, email, phone, DateTime.Now, affiliate, subAffiliate, ip, url);
                }
                else
                {
                    res.Value2 = EnsureLoad<Billing>(billingID);
                    UpdateBilling(billingID.Value, campaignID, registrationID, firstName, lastName,
                        creditCard, cvv, paymentTypeID, expMonth, expYear, address1, address2, city,
                        state, zip, country, email, phone, res.Value2.CreateDT, affiliate, subAffiliate, ip, url);
                    res.Value2 = EnsureLoad<Billing>(billingID);
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
                res = null;
            }
            return res;
        }

        public Set<Registration, Billing> CreateOrUpdateRegistrationAndBilling(long? billingID, long? registrationID,
            string firstName, string lastName, string address1, string address2, string city, string state, string zip, string country,
            string phone, string email,
            string shippingFirstName, string shippingLastName, string shippingAddress1, string shippingAddress2, string shippingCity, string shippingState, string shippingZip, string shippingCountry,
            string shippingPhone, string shippingEmail,
            int? campaignID, string affiliate, string subAffiliate, string ip, string url,
            int? paymentTypeID, string creditCard, string cvv, int? expMonth, int? expYear)
        {
            Set<Registration, Billing> res = new Set<Registration, Billing>();
            try
            {
                dao.BeginTransaction();

                if (registrationID == null)
                {
                    res.Value1 = CreateRegistration(campaignID, shippingFirstName, shippingLastName, shippingAddress1,
                        shippingAddress2, shippingCity, shippingState, shippingZip, shippingCountry, shippingEmail, shippingPhone, DateTime.Now, affiliate, subAffiliate, ip, url);

                    registrationID = res.Value1.RegistrationID;
                }
                else
                {
                    res.Value1 = EnsureLoad<Registration>(registrationID);
                    UpdateRegistration(registrationID.Value, campaignID, shippingFirstName, shippingLastName, shippingAddress1,
                        shippingAddress2, shippingCity, shippingState, shippingZip, shippingCountry, shippingEmail, shippingPhone, res.Value1.CreateDT, affiliate, subAffiliate, ip, url);
                    res.Value1 = EnsureLoad<Registration>(registrationID);
                }

                if (billingID == null)
                {
                    res.Value2 = CreateBilling(campaignID, registrationID, firstName, lastName,
                        creditCard, cvv, paymentTypeID, expMonth, expYear, address1, address2, city,
                        state, zip, country, email, phone, DateTime.Now, affiliate, subAffiliate, ip, url);
                }
                else
                {
                    res.Value2 = EnsureLoad<Billing>(billingID);
                    UpdateBilling(billingID.Value, campaignID, registrationID, firstName, lastName,
                        creditCard, cvv, paymentTypeID, expMonth, expYear, address1, address2, city,
                        state, zip, country, email, phone, res.Value2.CreateDT, affiliate, subAffiliate, ip, url);
                    res.Value2 = EnsureLoad<Billing>(billingID);
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
                res = null;
            }
            return res;
        }

        public Coupon GetCouponForSubscription(string couponCode, int subcriptionID)
        {
            Coupon res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("select c.* from Coupon c " +
                    "inner join Subscription s on s.ProductID = c.ProductID " +
                    "where s.SubscriptionID = @subscriptionID and c.Code = @couponCode");
                q.Parameters.Add("@subscriptionID", MySqlDbType.Int32).Value = subcriptionID;
                q.Parameters.Add("@couponCode", MySqlDbType.VarChar).Value = couponCode;

                res = dao.Load<Coupon>(q).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        private ICoupon GetHardcodedDiscount(string couponCode)
        {
            if (string.IsNullOrEmpty(couponCode))
            {
                return null;
            }

            IDictionary<string, ICoupon> couponList = new Dictionary<string, ICoupon>() 
            {
                {"ec13car", new ProductCoupon("ec13car", "VG-CAR", null, 4.99M)},
                {"ec13wall", new ProductCoupon("ec13wall", "VG-WALL", null, 4.99M)},
                {"ec13bat", new ProductCoupon("ec13bat", "VG-BAT", null, 9.99M)},
                {"ecigscar", new ProductCoupon("ecigscar", "VG-CAR", null, 9.99M)},
                {"ecigswall", new ProductCoupon("ecigswall", "VG-WALL", null, 9.99M)},
                {"ecigsbattery", new ProductCoupon("ecigsbattery", "VG-BAT", null, 14.99M)}
            };

            couponCode = couponCode.ToLower();
            if (couponList.ContainsKey(couponCode))
            {
                return couponList[couponCode];
            }
            return null;
        }

        public ICoupon GetCampaignDiscount(string couponCode, int? campaignID)
        {
            if (string.IsNullOrEmpty(couponCode) || campaignID == null)
            {
                return null;
            }

            ICoupon res = GetHardcodedDiscount(couponCode);
            if (res == null)
            {
                try
                {
                    Coupon coupon = GetCouponForCampaign(couponCode, campaignID.Value);
                    long temp = 0;
                    if (coupon != null)
                    {
                        res = coupon;
                    }
                    else if (long.TryParse(couponCode, out temp))
                    {
                        CampaignReferralDiscount referralDiscount = GetReferralDiscountForCampaign(campaignID.Value);
                        if (referralDiscount != null)
                        {
                            Billing b = Load<Billing>(temp);
                            if (b != null)
                            {
                                res = new BillingReferralCoupon(temp, referralDiscount.Discount);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(GetType(), ex);
                    res = null;
                }
            }
            return res;
        }

        private Coupon GetCouponForCampaign(string couponCode, int campaignID)
        {
            Coupon res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("select c.* from CampaignCoupon cc " +
                    "inner join Coupon c on c.ID = cc.CouponID " +
                    "where cc.CampaignID = @campaignID and c.Code = @couponCode");
                q.Parameters.Add("@campaignID", MySqlDbType.Int32).Value = campaignID;
                q.Parameters.Add("@couponCode", MySqlDbType.VarChar).Value = couponCode;

                res = dao.Load<Coupon>(q).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        private CampaignReferralDiscount GetReferralDiscountForCampaign(int campaignID)
        {
            CampaignReferralDiscount res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("select crd.* from CampaignReferralDiscount crd " +
                    "where crd.CampaignID = @campaignID");
                q.Parameters.Add("@campaignID", MySqlDbType.Int32).Value = campaignID;

                res = dao.Load<CampaignReferralDiscount>(q).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public Registration GetRegistrationByPhoneNumber(string phoneNumber)
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "select * from Registration where Phone = @phoneNumber";
            cmd.Parameters.AddWithValue("@phoneNumber", phoneNumber);

            Registration res = dao.Load<Registration>(cmd).FirstOrDefault();

            return res;
        }

        public IList<Registration> GetRegistrationsByPhoneNumber(string phoneNumber)
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "select * from Registration where Phone = @phoneNumber";
            cmd.Parameters.AddWithValue("@phoneNumber", phoneNumber);

            IList<Registration> res = dao.Load<Registration>(cmd);

            return res;
        }
    }
}