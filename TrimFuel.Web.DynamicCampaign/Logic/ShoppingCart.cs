using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using TrimFuel.Model;
using TrimFuel.Business.Dao;

namespace TrimFuel.Web.DynamicCampaign.Logic
{
    public static class ShoppingCart
    {
        public static IDao Dao 
        {
            get
            {
                return MySqlDaoFactory.CreateDao(DB.TrimFuel);
            }
        }

        public static Registration Registration 
        {
            get
            {
                if (RegistrationID == null)
                    return null;
                return Dao.Load<Registration>(RegistrationID);
            }
            set
            {
                if (value == null)
                {
                    RegistrationID = null;
                }
                else
                {
                    RegistrationID = value.RegistrationID;
                }
            }
        }
        public static Billing Billing
        {
            get
            {
                if (BillingID == null)
                    return null;
                return Dao.Load<Billing>(BillingID);
            }
            set
            {
                if (value == null)
                {
                    BillingID = null;
                }
                else
                {
                    BillingID = value.BillingID;
                }
            }
        }

        public static BillingSubscription BillingSubscription
        {
            get
            {
                if (BillingSubscriptionID == null)
                    return null;
                return Dao.Load<BillingSubscription>(BillingSubscriptionID);
            }
            set
            {
                if (value == null)
                {
                    BillingSubscriptionID = null;
                }
                else
                {
                    BillingSubscriptionID = value.BillingSubscriptionID;
                }
            }
        }

        public static Subscription Subscription
        {
            get
            {
                if (SubscriptionID == null)
                    return null;
                return Dao.Load<Subscription>(SubscriptionID);
            }
            set {
                SubscriptionID = value == null ? null : value.SubscriptionID;
            }
        }

        public static AssertigyMID AssertigyMID
        {
            get
            {
                if (AssertigyMIDID == null)
                    return null;
                return Dao.Load<AssertigyMID>(AssertigyMIDID);
            }
            set
            {
                if (value == null)
                {
                    AssertigyMIDID = null;
                }
                else
                {
                    AssertigyMIDID = value.AssertigyMIDID;
                }
            }
        }

        public static Coupon Coupon
        {
            get
            {
                if (CouponID == null)
                    return null;
                return Dao.Load<Coupon>(CouponID);
            }
            set {
                CouponID = value == null ? null : value.CouponID;
            }
        }


        public static int? CouponID
        {
            get
            {
                if (HttpContext.Current.Session["CouponID"] == null)
                    return null;
                return (int)HttpContext.Current.Session["CouponID"];
            }
            set
            {
                HttpContext.Current.Session["CouponID"] = value;
            }
        }

        public static long? RegistrationID 
        {
            get 
            {
                if (HttpContext.Current.Session["RegistrationID"] == null)
                    return null;
                return (long)HttpContext.Current.Session["RegistrationID"];
            }
            set
            {
                HttpContext.Current.Session["RegistrationID"] = value;
            }
        }

        public static long? BillingID
        {
            get
            {
                if (HttpContext.Current.Session["BillingID"] == null)
                    return null;
                return (long)HttpContext.Current.Session["BillingID"];
            }
            set
            {
                HttpContext.Current.Session["BillingID"] = value;
            }
        }

        public static int? SubscriptionID
        {
            get
            {
                if (HttpContext.Current.Session["SubscriptionID"] == null)
                    return null;
                return (int)HttpContext.Current.Session["SubscriptionID"];
            }
            set
            {
                HttpContext.Current.Session["SubscriptionID"] = value;
            }
        }

        public static int? BillingSubscriptionID
        {
            get
            {
                if (HttpContext.Current.Session["BillingSubscriptionID"] == null)
                    return null;
                return (int)HttpContext.Current.Session["BillingSubscriptionID"];
            }
            set
            {
                HttpContext.Current.Session["BillingSubscriptionID"] = value;
            }
        }

        public static int? AssertigyMIDID
        {
            get
            {
                if (HttpContext.Current.Session["AssertigyMIDID"] == null)
                    return null;
                return (int)HttpContext.Current.Session["AssertigyMIDID"];
            }
            set
            {
                HttpContext.Current.Session["AssertigyMIDID"] = value;
            }
        }
    }
}
