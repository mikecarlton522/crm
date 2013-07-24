using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrimFuel.Web.Admin.Logic.BillingAPI
{
    interface IBillingAPI
    {
        string Void(string username, string password, string chargeHistoryID);
        string Refund(string username, string password, string amount, string chargeHistoryID);
        string Charge(string username, string password, string amount, string shipping, string firstName, string lastName,
            string address1, string address2, string city, string state, string zip, string phone, string email, string ip,
            string affiliate, string subAffiliate, string internalID,
            string paymentType, string creditCard, string cvv, string expMonth, string expYear);
    }
}
