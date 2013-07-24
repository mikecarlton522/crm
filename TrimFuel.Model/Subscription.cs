using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class Subscription : Entity
    {
        public int? SubscriptionID { get; set; }
        public string DisplayName { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public int? ParentSubscriptionID { get; set; }
        public int? InitialInterim { get; set; }
        public decimal? InitialShipping { get; set; }
        public decimal? SaveShipping { get; set; }
        public decimal? SaveBilling { get; set; }
        public decimal? InitialBillAmount { get; set; }
        public int? SecondInterim { get; set; }
        public decimal? SecondShipping { get; set; }
        public decimal? SecondBillAmount { get; set; }
        public int? RegularInterim { get; set; }
        public decimal? RegularShipping { get; set; }
        public decimal? RegularBillAmount { get; set; }
        public string ProductCode { get; set; }
        public bool? Selectable { get; set; }
        public int? Quantity { get; set; }
        public int? ProductID { get; set; }
        public bool? Recurring { get; set; }
        public bool? ShipFirstRebill { get; set; }
        public string SKU2 { get; set; }
        public string TrialText { get; set; }
        public string RecurText { get; set; }
        public string SaveText { get; set; }
        public string UpsellText { get; set; }
        public string ReplacementText { get; set; }
        public string ReplacementTrialText { get; set; }
        public int? QuantitySKU2 { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertString("DisplayName", DisplayName, 255);
            v.AssertString("ProductName", ProductName, 50);
            v.AssertString("Description", Description, 255);
            v.AssertString("ProductCode", ProductCode, 50);
            v.AssertString("SKU2", SKU2, 50);
            v.AssertString("TrialText", TrialText, 45);
            v.AssertString("RecurText", RecurText, 45);
            v.AssertString("SaveText", SaveText, 45);
            v.AssertString("UpsellText", UpsellText, 45);
            v.AssertString("ReplacementText", ReplacementText, 45);
            v.AssertString("ReplacementTrialText", ReplacementTrialText, 45);
        }
    }
}
