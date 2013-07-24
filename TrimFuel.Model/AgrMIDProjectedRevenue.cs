using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class AgrMIDProjectedRevenue : Entity
    {
        public struct ID
        {
            public int Year { get; set; }
            public int Month { get; set; }
            public int MerchantAccountID { get; set; }
        }

        public ID? AgrMIDProjectedRevenueID { get; set; }
        public int? Year { get; set; }
        public int? Month { get; set; }
        public int? MerchantAccountID { get; set; }
        public decimal? ProjectedRevenue { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("Year", Year);
            v.AssertNotNull("Month", Month);
            v.AssertNotNull("MerchantAccountID", MerchantAccountID);
            v.AssertNotNull("ProjectedRevenue", ProjectedRevenue);
        }
    }
}
