using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class UnsentUnpayedView : EntityView
    {
        public int? ID { get; set; }
        public int? GroupID { get; set; }
        public int? BillingID { get; set; }
        public decimal? Amount { get; set; }
        public string BillType { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Reason { get; set; }
        public string SKU { get; set; }
        public DateTime? CreateDT { get; set; }

        #region Logic

        public string ShortReason
        {
            get
            {
                if (Reason.Length <= 120)
                    return Reason;
                else
                    return Reason.Substring(0, 120) + "...";
            }
        }
            
        #endregion
    }
}
