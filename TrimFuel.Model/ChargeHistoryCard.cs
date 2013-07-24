using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Containers;

namespace TrimFuel.Model
{
    public class ChargeHistoryCard : Entity, ICreditCardContainer
    {
        public long? ChargeHistoryID { get; set; }
        public string CreditCardLeft6 { get; set; }
        public string CreditCardRight4 { get; set; }
        public int? PaymentTypeID { get; set; }
        public int? ExpMonth { get; set; }
        public int? ExpYear { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
        }
    }
}
