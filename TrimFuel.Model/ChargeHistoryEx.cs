using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TrimFuel.Model
{
    //TODO: abstract
    [XmlInclude(typeof(ChargeDetails))]
    public class ChargeHistoryEx : Entity
    {
        public long? ChargeHistoryID { get; set; }
        public int? ChargeTypeID { get; set; }
        public int? MerchantAccountID { get; set; }
        public int? BillingSubscriptionID { get; set; }
        public DateTime? ChargeDate { get; set; }
        public decimal? Amount { get; set; }
        public string AuthorizationCode { get; set; }
        public string TransactionNumber { get; set; }
        public string Response { get; set; }
        public bool? Success { get; set; }
        public string ChildMID { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertString("AuthorizationCode", AuthorizationCode, 20);
            v.AssertString("TransactionNumber", TransactionNumber, 20);
            v.AssertString("Response", Response, 2048);
            v.AssertString("ChildMID", ChildMID, 50);
        }
    }
}
